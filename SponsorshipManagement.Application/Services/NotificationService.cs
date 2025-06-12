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
    /// Implementación del servicio de notificaciones
    /// 🎯 CU-PA-05.02.1: Envío de notificaciones de vencimiento
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly IEventRepository _eventRepository;
        
        public NotificationService(
            ISponsorRepository sponsorRepository, 
            IEventRepository eventRepository)
        {
            _sponsorRepository = sponsorRepository ?? throw new ArgumentNullException(nameof(sponsorRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
        }

        /// <summary>
        /// Envía una notificación de vencimiento de contrato
        /// </summary>
        public async Task<bool> SendContractExpirationNotificationAsync(ContractExpirationNotificationDto notification)
        {
            try
            {
                Console.WriteLine($"📧 CU-PA-05.02.1: Enviando notificación para contrato {notification.ContractId} ({notification.ContractTitle})");
                
                // Simulamos el envío de correo electrónico
                foreach (var recipient in notification.Recipients)
                {
                    await SendEmailAsync(
                        recipient,
                        GenerateEmailSubject(notification),
                        GenerateEmailBody(notification, recipient)
                    );
                }
                
                // Actualizar estado de la notificación
                notification.Status = "Sent";
                notification.SentAt = DateTime.UtcNow;
                
                Console.WriteLine($"✅ Notificación enviada con éxito a {notification.Recipients.Length} destinatarios");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando notificación: {ex.Message}");
                notification.Status = "Failed";
                notification.ErrorMessage = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Envía múltiples notificaciones de vencimiento de contrato
        /// </summary>
        public async Task<Dictionary<string, bool>> SendContractExpirationNotificationsAsync(IEnumerable<ContractExpirationNotificationDto> notifications)
        {
            var results = new Dictionary<string, bool>();
            
            foreach (var notification in notifications)
            {
                bool success = await SendContractExpirationNotificationAsync(notification);
                results[notification.Id] = success;
            }
            
            return results;
        }
        
        /// <summary>
        /// Obtiene los correos electrónicos de los contactos para un patrocinador
        /// </summary>
        public Task<IEnumerable<string>> GetSponsorContactEmailsAsync(string sponsorId)
        {
            try
            {
                // Como ISponsorRepository no tiene GetByIdAsync, 
                // usamos el método disponible o creamos correos basados en el ID
                // En este caso, generamos correos ficticios basados en el ID del patrocinador
                
                // Formato de correo ficticio basado en el ID del patrocinador
                string sponsorName = $"sponsor-{sponsorId}";
                string sponsorEmail = GenerateEmailFromName(sponsorName);
                
                // Simular múltiples contactos
                var result = new List<string> 
                { 
                    sponsorEmail,
                    $"contacto@{sponsorEmail.Split('@')[1]}",
                    $"contratos@{sponsorEmail.Split('@')[1]}"
                };
                
                return Task.FromResult<IEnumerable<string>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error obteniendo contactos del patrocinador: {ex.Message}");
                return Task.FromResult<IEnumerable<string>>(new List<string>());
            }
        }
        
        /// <summary>
        /// Obtiene los correos electrónicos de los organizadores de un evento
        /// </summary>
        public async Task<IEnumerable<string>> GetEventOrganizerEmailsAsync(string eventId)
        {
            try
            {
                // Usar el método GetEventById que está disponible en la interfaz IEventRepository
                var eventEntity = _eventRepository.GetEventById(eventId);
                
                if (eventEntity == null)
                {
                    return new List<string>();
                }
                
                // Generar correos basados en el ID del evento o en Place si está disponible
                string eventIdentifier;
                
                // Verificar si el evento tiene la propiedad Place usando reflexión
                var placeProperty = eventEntity.GetType().GetProperty("Place");
                if (placeProperty != null)
                {
                    eventIdentifier = (placeProperty.GetValue(eventEntity) as string) ?? eventId;
                }
                else
                {
                    eventIdentifier = $"event-{eventId}";
                }
                
                string eventEmail = GenerateEmailFromName(eventIdentifier);
                string domain = eventEmail.Split('@')[1];
                
                // Simular múltiples organizadores
                return new List<string> 
                { 
                    $"organizador@{domain}",
                    $"contratos@{domain}",
                    $"administracion@{domain}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error obteniendo organizadores del evento: {ex.Message}");
                return await Task.FromResult(new List<string>());
            }
        }
        
        #region Métodos auxiliares
        
        private async Task SendEmailAsync(string recipient, string subject, string body)
        {
            // Simulamos el envío de correo (en un sistema real, usaríamos SMTP, SendGrid, etc.)
            await Task.Delay(100); // Simular tiempo de envío
            
            Console.WriteLine($"📨 Enviando correo a {recipient}");
            Console.WriteLine($"📑 Asunto: {subject}");
            // Console.WriteLine($"📄 Cuerpo: {body}"); // Comentado para no llenar la consola
        }
        
        private string GenerateEmailSubject(ContractExpirationNotificationDto notification)
        {
            // Obtener plantilla (en un sistema real, estaría en la configuración)
            string template = "Contrato {ContractTitle} vence en {DaysRemaining} días";
            
            // Reemplazar variables
            return template
                .Replace("{ContractTitle}", notification.ContractTitle)
                .Replace("{DaysRemaining}", notification.DaysRemaining.ToString())
                .Replace("{ExpirationDate}", notification.ExpirationDate.ToString("dd/MM/yyyy"));
        }
        
        private string GenerateEmailBody(ContractExpirationNotificationDto notification, string recipientEmail)
        {
            // Simular nombre del destinatario basado en el correo
            string recipientName = recipientEmail.Split('@')[0];
            
            // Obtener plantilla (en un sistema real, estaría en la configuración)
            string template = 
                "Estimado {RecipientName},\n\n" +
                "Le informamos que el contrato '{ContractTitle}' con valor de {ContractValue:C} vencerá en {DaysRemaining} días " +
                "({ExpirationDate:dd/MM/yyyy}).\n\n" +
                "Por favor, contacte a la otra parte para discutir la renovación o finalización del contrato.\n\n" +
                "Saludos cordiales,\n" +
                "Sistema de Gestión de Patrocinios";
            
            // Reemplazar variables
            return template
                .Replace("{RecipientName}", char.ToUpper(recipientName[0]) + recipientName.Substring(1))
                .Replace("{ContractTitle}", notification.ContractTitle)
                .Replace("{ContractValue:C}", notification.ContractValue.ToString("C"))
                .Replace("{DaysRemaining}", notification.DaysRemaining.ToString())
                .Replace("{ExpirationDate:dd/MM/yyyy}", notification.ExpirationDate.ToString("dd/MM/yyyy"))
                .Replace("{SponsorName}", notification.SponsorName)
                .Replace("{EventName}", notification.EventName);
        }
        
        private string GenerateEmailFromName(string name)
        {
            // Simular una dirección de correo a partir de un nombre
            string normalized = name
                .ToLowerInvariant()
                .Replace(" ", ".")
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("ñ", "n");
            
            string domain = normalized.Contains("sponsor") ? "patrocinadores.com" : "eventos.com";
            
            return $"{normalized}@{domain}";
        }
        
        #endregion
    }
}