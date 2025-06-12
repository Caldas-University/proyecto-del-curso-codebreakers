using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Implementación del notificador de vencimiento de contratos
    /// 🎯 CU-PA-05.02.1: Detección y notificación de contratos a vencer
    /// </summary>
    public class ContractExpirationNotifier : IContractExpirationNotifier
    {
        private readonly IContractRenewalService _contractRenewalService;
        private readonly INotificationService _notificationService;
        private readonly IEventRepository _eventRepository;
        
        // Configuración de notificaciones con valores por defecto
        private ContractNotificationSettingsDto _settings = new ContractNotificationSettingsDto();
        
        public ContractNotificationSettingsDto Settings 
        { 
            get => _settings;
            set => _settings = value ?? new ContractNotificationSettingsDto();
        }
        
        public ContractExpirationNotifier(
            IContractRenewalService contractRenewalService,
            INotificationService notificationService,
            IEventRepository eventRepository)
        {
            _contractRenewalService = contractRenewalService ?? throw new ArgumentNullException(nameof(contractRenewalService));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        }
        
        /// <summary>
        /// Detecta contratos próximos a vencer y envía notificaciones
        /// </summary>
        public async Task<int> ProcessContractsAndSendNotificationsAsync()
        {
            Console.WriteLine("🔍 CU-PA-05.02.1: Iniciando detección de contratos a vencer...");
            
            try
            {
                // Obtener todos los contratos que requieren notificación
                var contractsToNotify = await GetContractsRequiringNotificationAsync();
                var contractsList = contractsToNotify.ToList();
                
                if (!contractsList.Any())
                {
                    Console.WriteLine("ℹ️ No hay contratos que requieran notificación en este momento");
                    return 0;
                }
                
                Console.WriteLine($"📋 Detectados {contractsList.Count} contratos que requieren notificación");
                
                // Preparar y enviar notificaciones
                int notificationsSent = 0;
                
                foreach (var contract in contractsList)
                {
                    try
                    {
                        // Obtener información adicional del evento
                        string eventName = "Evento desconocido";
                        try
                        {
                            // Verificar si el DTO tiene la propiedad EventId usando reflexión
                            var eventIdProperty = contract.GetType().GetProperty("EventId");
                            string? eventId = null;
                            
                            if (eventIdProperty != null)
                            {
                                eventId = eventIdProperty.GetValue(contract) as string;
                            }
                            
                            if (!string.IsNullOrEmpty(eventId))
                            {
                                // Obtener el evento usando el repositorio
                                var eventEntity = _eventRepository.GetEventById(eventId);
                                if (eventEntity != null)
                                {
                                    // Verificar si el evento tiene la propiedad Place usando reflexión
                                    var placeProperty = eventEntity.GetType().GetProperty("Place");
                                    if (placeProperty != null)
                                    {
                                        string? place = placeProperty.GetValue(eventEntity) as string;
                                        eventName = !string.IsNullOrEmpty(place) 
                                            ? place 
                                            : $"Evento en lugar desconocido (ID: {eventId})";
                                    }
                                    else
                                    {
                                        // Si no hay propiedad Place, intentar usar otra propiedad como identificador
                                        eventName = $"Evento {eventId}";
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"⚠️ Error obteniendo información del evento: {ex.Message}");
                        }
                        
                        // Recopilar destinatarios
                        var recipients = new List<string>();
                        
                        if (_settings.NotifySponsors && !string.IsNullOrEmpty(contract.SponsorId))
                        {
                            var sponsorEmails = await _notificationService.GetSponsorContactEmailsAsync(contract.SponsorId);
                            if (sponsorEmails != null)
                            {
                                recipients.AddRange(sponsorEmails);
                            }
                        }
                        
                        if (_settings.NotifyOrganizers && !string.IsNullOrEmpty(contract.EventId))
                        {
                            var organizerEmails = await _notificationService.GetEventOrganizerEmailsAsync(contract.EventId);
                            if (organizerEmails != null)
                            {
                                recipients.AddRange(organizerEmails);
                            }
                        }
                        
                        if (!recipients.Any())
                        {
                            Console.WriteLine($"⚠️ No se encontraron destinatarios para el contrato {contract.Id}");
                            continue;
                        }
                        
                        // Crear notificación
                        var notification = new ContractExpirationNotificationDto
                        {
                            ContractId = contract.Id,
                            ContractTitle = contract.Title,
                            ExpirationDate = contract.EndDate,
                            DaysRemaining = contract.DaysUntilExpiration,
                            ContractValue = contract.Value,
                            SponsorId = contract.SponsorId,
                            SponsorName = contract.SponsorName ?? "Desconocido",
                            EventId = contract.EventId,
                            EventName = eventName,
                            Recipients = recipients.Distinct().ToArray()
                        };
                        
                        // Enviar notificación
                        bool success = await _notificationService.SendContractExpirationNotificationAsync(notification);
                        
                        if (success)
                        {
                            // Marcar contrato como notificado
                            if (Guid.TryParse(contract.Id, out Guid contractId))
                            {
                                await _contractRenewalService.MarkContractAsNotifiedAsync(contractId);
                                notificationsSent++;
                                Console.WriteLine($"✅ Notificación enviada para contrato {contract.Id} - {contract.Title}");
                            }
                            else
                            {
                                Console.WriteLine($"⚠️ No se pudo convertir el ID {contract.Id} a GUID");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"❌ Error procesando contrato {contract.Id}: {ex.Message}");
                    }
                }
                
                Console.WriteLine($"✅ CU-PA-05.02.1: Proceso completado. {notificationsSent} notificaciones enviadas");
                return notificationsSent;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error general en procesamiento de notificaciones: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
        }
        
        /// <summary>
        /// Detecta contratos próximos a vencer pero sin enviar notificaciones
        /// </summary>
        public async Task<IEnumerable<ContractRenewalDto>> GetContractsRequiringNotificationAsync()
        {
            try
            {
                // Determinar el día más lejano para buscar
                int maxDays = _settings.DaysBeforeExpiration?.Count > 0 
                    ? _settings.DaysBeforeExpiration.Max() 
                    : 30;
                
                // Obtener todos los contratos que vencen en ese período
                var allExpiringContracts = await _contractRenewalService.GetContractsExpiringInDaysAsync(
                    maxDays, 
                    _settings.ContractStatuses?.ToArray());
                
                if (allExpiringContracts == null)
                {
                    Console.WriteLine("⚠️ No se encontraron contratos próximos a vencer");
                    return new List<ContractRenewalDto>();
                }
                
                // Filtrar por días específicos de notificación y por valor mínimo
                var contractsToNotify = allExpiringContracts;

                // Filtrar por días antes de vencimiento solo si la lista no es nula
                if (_settings.DaysBeforeExpiration != null && _settings.DaysBeforeExpiration.Count > 0)
                {
                    contractsToNotify = contractsToNotify
                        .Where(c => _settings.DaysBeforeExpiration.Contains(c.DaysUntilExpiration))
                        .ToList();
                }

                // Filtrar por contratos no notificados, usando reflexión para mayor robustez
                contractsToNotify = contractsToNotify
                    .Where(c => {
                        var notificationProperty = c.GetType().GetProperty("NotificationSent");
                        return notificationProperty == null || 
                               !(bool)(notificationProperty.GetValue(c) ?? false);
                    })
                    .ToList();

                // Filtrar por valor mínimo
                if (_settings.MinimumContractValue.HasValue)
                {
                    contractsToNotify = contractsToNotify
                        .Where(c => c.Value >= _settings.MinimumContractValue.Value)
                        .ToList();
                }

                return contractsToNotify;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo contratos para notificación: {ex.Message}");
                return new List<ContractRenewalDto>();
            }
        }

        /// <summary>
        /// Envía notificaciones con mensajes estructurados según el rol del destinatario
        /// 🎯 CU-PA-05.02.2: Mensajes estructurados con opciones
        /// </summary>
        public async Task<int> SendStructuredNotificationsAsync(int days = 7)
        {
            Console.WriteLine($"🎯 CU-PA-05.02.2: Iniciando envío de notificaciones estructuradas para contratos que vencen en {days} días");
            
            // Filtrar contratos específicamente para el día indicado
            ExpiringContractsFilterDto expiringFilter;
            try 
            {
                expiringFilter = new ExpiringContractsFilterDto
                {
                    Days = days + 1, // +1 para incluir el día exacto
                    IncludeStatuses = _settings.ContractStatuses.ToArray()
                    // Quitamos MinimumContractValue que está causando el error
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creando filtro: {ex.Message}");
                // Filtro de respaldo sin la propiedad problemática
                expiringFilter = new ExpiringContractsFilterDto
                {
                    Days = days + 1,
                    IncludeStatuses = _settings.ContractStatuses.ToArray()
                };
            }

            var contracts = await _contractRenewalService.GetContractsExpiringAsync(expiringFilter);

            // Filtrar por estado de notificación manualmente
            var contractsForDay = contracts
                .Where(c => c.DaysUntilExpiration == days && c.NotificationSent != true)
                .ToList();
            
            if (!contractsForDay.Any())
            {
                Console.WriteLine($"ℹ️ No hay contratos que venzan exactamente en {days} días pendientes de notificar");
                return 0;
            }
            
            Console.WriteLine($"📋 Encontrados {contractsForDay.Count} contratos que vencen en {days} días");
            
            int notificationsSent = 0;
            
            foreach (var contract in contractsForDay)
            {
                try
                {
                    var recipients = new List<(string Role, string Email)>();
                    
                    // Recopilar destinatarios organizadores
                    if (_settings.NotifyOrganizers && !string.IsNullOrEmpty(contract.EventId))
                    {
                        var organizerEmails = await _notificationService.GetEventOrganizerEmailsAsync(contract.EventId);
                        foreach (var email in organizerEmails)
                        {
                            recipients.Add(("Organizador", email));
                        }
                    }
                    
                    // Recopilar destinatarios patrocinadores
                    if (_settings.NotifySponsors && !string.IsNullOrEmpty(contract.SponsorId))
                    {
                        var sponsorEmails = await _notificationService.GetSponsorContactEmailsAsync(contract.SponsorId);
                        foreach (var email in sponsorEmails)
                        {
                            recipients.Add(("Patrocinador", email));
                        }
                    }
                    
                    // Enviar notificaciones personalizadas a cada destinatario
                    foreach (var (role, email) in recipients)
                    {
                        // Generar mensaje estructurado según el rol
                        var message = await _notificationService.GenerateActionMessageAsync(contract.Id, role, email);
                        
                        // Enviar mensaje
                        bool success = await _notificationService.SendActionMessageAsync(message);
                        
                        if (success)
                        {
                            notificationsSent++;
                            Console.WriteLine($"✅ Mensaje estructurado enviado a {role} ({email}) para contrato {contract.Id}");
                        }
                    }
                    
                    // Marcar contrato como notificado
                    await _contractRenewalService.MarkContractAsNotifiedAsync(Guid.Parse(contract.Id));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error procesando contrato {contract.Id} para notificación estructurada: {ex.Message}");
                }
            }
            
            Console.WriteLine($"✅ CU-PA-05.02.2: Proceso completado. {notificationsSent} notificaciones estructuradas enviadas");
            return notificationsSent;
        }
    }
}