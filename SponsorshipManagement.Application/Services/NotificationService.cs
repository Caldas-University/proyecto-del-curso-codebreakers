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
        private readonly IContractRepository _contractRepository;
        
        public NotificationService(
            ISponsorRepository sponsorRepository, 
            IEventRepository eventRepository,
            IContractRepository contractRepository)
        {
            _sponsorRepository = sponsorRepository ?? throw new ArgumentNullException(nameof(sponsorRepository));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
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
        
        /// <summary>
        /// Genera un mensaje estructurado con opciones de renovación o finalización
        /// 🎯 CU-PA-05.02.2: Mensajes estructurados según rol
        /// </summary>
        public async Task<ContractActionMessageDto> GenerateActionMessageAsync(string contractId, string recipientRole, string recipientEmail)
        {
            try
            {
                Console.WriteLine($"🎯 CU-PA-05.02.2: Generando mensaje estructurado para {recipientRole} ({recipientEmail})");
                
                // Obtener datos del contrato
                var contract = await _contractRepository.GetByIdAsync(Guid.Parse(contractId));
                if (contract == null)
                    throw new ArgumentException($"Contrato con ID {contractId} no encontrado");
                
                // Crear objeto base del mensaje
                var message = new ContractActionMessageDto
                {
                    ContractId = contractId,
                    ContractTitle = contract.Title,
                    ExpirationDate = contract.EndDate,
                    DaysRemaining = (int)(contract.EndDate.Date - DateTime.UtcNow.Date).TotalDays,
                    RecipientRole = recipientRole,
                    RecipientEmail = recipientEmail
                };
                
                // Personalizar mensaje según el rol
                if (recipientRole.Equals("Patrocinador", StringComparison.OrdinalIgnoreCase))
                {
                    GenerateSponsorMessage(message);
                }
                else // Organizador
                {
                    GenerateOrganizerMessage(message);
                }
                
                Console.WriteLine($"✅ CU-PA-05.02.2: Mensaje generado con {message.RenewalOptions.Count} opciones de renovación y {message.EndingOptions.Count} opciones de finalización");
                
                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error generando mensaje estructurado: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Envía un mensaje estructurado - versión consola
        /// </summary>
        public async Task<bool> SendActionMessageAsync(ContractActionMessageDto message)
        {
            try
            {
                Console.WriteLine();
                Console.WriteLine("===========================================================");
                Console.WriteLine($"📧 MENSAJE PARA: {message.RecipientRole.ToUpper()} ({message.RecipientEmail})");
                Console.WriteLine("===========================================================");
                Console.WriteLine($"ASUNTO: {message.Subject}");
                Console.WriteLine("-----------------------------------------------------------");
                Console.WriteLine($"Contrato: {message.ContractTitle}");
                Console.WriteLine($"Vence en: {message.DaysRemaining} días ({message.ExpirationDate:dd/MM/yyyy})");
                Console.WriteLine();
                Console.WriteLine(message.Introduction);
                Console.WriteLine();
                Console.WriteLine("ANÁLISIS DE DESEMPEÑO:");
                Console.WriteLine(message.PerformanceSummary);
                Console.WriteLine();
                
                if (message.RenewalOptions.Any())
                {
                    Console.WriteLine("OPCIONES DE RENOVACIÓN:");
                    for (int i = 0; i < message.RenewalOptions.Count; i++)
                    {
                        Console.WriteLine($"[{i+1}] {message.RenewalOptions[i]}");
                    }
                    Console.WriteLine();
                }
                
                if (message.EndingOptions.Any())
                {
                    Console.WriteLine("OPCIONES DE FINALIZACIÓN:");
                    for (int i = 0; i < message.EndingOptions.Count; i++)
                    {
                        Console.WriteLine($"[{i+1}] {message.EndingOptions[i]}");
                    }
                    Console.WriteLine();
                }
                
                Console.WriteLine(message.Closing);
                Console.WriteLine("===========================================================");
                
                // Simular envío
                await Task.Delay(100);
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error enviando mensaje estructurado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Genera mensaje personalizado para patrocinadores - versión consola
        /// </summary>
        private void GenerateSponsorMessage(ContractActionMessageDto message)
        {
            // Simulamos algunos datos de análisis para demostración
            var sponsorPerformance = new
            {
                ROI = 135.2,
                Reach = 125000,
                Engagement = 15600,
                TopChannels = new[] { "Redes Sociales", "Eventos Presenciales", "Email Marketing" }
            };
            
            message.Subject = $"Opciones para su contrato de patrocinio '{message.ContractTitle}' que vence en {message.DaysRemaining} días";
            
            message.Introduction = 
                $"Estimado Patrocinador,\n\n" +
                $"Nos comunicamos con usted en relación al contrato de patrocinio '{message.ContractTitle}' " +
                $"que vence el {message.ExpirationDate:dd/MM/yyyy} (en {message.DaysRemaining} días). " +
                $"Queremos presentarle un resumen del desempeño de su inversión y las opciones disponibles.";
            
            message.PerformanceSummary = 
                $"* ROI estimado: {sponsorPerformance.ROI:F2}%\n" +
                $"* Alcance total: {sponsorPerformance.Reach:N0} impresiones\n" +
                $"* Engagement: {sponsorPerformance.Engagement:N0} interacciones\n" +
                $"* Canales con mejor desempeño: {string.Join(", ", sponsorPerformance.TopChannels)}";
            
            message.RenewalOptions = new List<string>
            {
                "Renovación Premium (24 meses): +15% valor, mayor exposición en canales digitales, presencia destacada en eventos",
                "Renovación Estándar (12 meses): Mismo valor, continuidad en beneficios actuales",
                "Renovación con Descuento (6 meses): -10% valor, período de prueba más corto con revisión trimestral"
            };
            
            message.EndingOptions = new List<string>
            {
                "Finalización Estándar: Todos los beneficios finalizan en la fecha de vencimiento",
                "Finalización con Opción de Retorno Prioritario: Derecho preferencial para futuras oportunidades (90 días)"
            };
            
            message.Closing = 
                "Agradecemos su confianza como patrocinador. Estamos a su disposición para discutir estas opciones " +
                "y encontrar la mejor solución para sus objetivos de marketing y visibilidad.\n\n" +
                "Atentamente,\nEl Equipo de Gestión de Patrocinios";
        }

        /// <summary>
        /// Genera mensaje personalizado para organizadores - versión consola
        /// </summary>
        private void GenerateOrganizerMessage(ContractActionMessageDto message)
        {
            // Simulamos algunos datos de análisis para demostración
            var organizerAnalysis = new
            {
                OverallScore = 82,
                CompliancePercentage = 95.5,
                CompletedCommitments = 18,
                TotalCommitments = 20,
                OverdueCommitments = 1,
                RiskLevel = "Bajo",
                Recommendation = "Renovar con condiciones actuales"
            };
            
            message.Subject = $"Gestión de renovación: Contrato '{message.ContractTitle}' próximo a vencer";
            
            message.Introduction = 
                $"Estimado Organizador,\n\n" +
                $"Le informamos que el contrato de patrocinio '{message.ContractTitle}' " +
                $"vencerá el {message.ExpirationDate:dd/MM/yyyy} (en {message.DaysRemaining} días). " +
                $"A continuación presentamos el análisis de cumplimiento y las acciones recomendadas.";
            
            message.PerformanceSummary = 
                $"* Score general: {organizerAnalysis.OverallScore}/100\n" +
                $"* Cumplimiento de compromisos: {organizerAnalysis.CompliancePercentage:F2}%\n" +
                $"* Compromisos completados: {organizerAnalysis.CompletedCommitments} de {organizerAnalysis.TotalCommitments}\n" +
                $"* Compromisos vencidos: {organizerAnalysis.OverdueCommitments}\n" +
                $"* Nivel de riesgo: {organizerAnalysis.RiskLevel}\n" +
                $"* Recomendación: {organizerAnalysis.Recommendation}";
            
            message.RenewalOptions = new List<string>
            {
                "Renovación Mejorada: Incremento de 15% en valor, extensión de beneficios, duración de 24 meses",
                "Renovación Estándar: Mantener condiciones actuales, duración de 12 meses",
                "Renovación Condicional: Revisión trimestral de compromisos, duración 6 meses"
            };
            
            message.EndingOptions = new List<string>
            {
                "Finalización Estándar: Cierre ordenado con informe final",
                "Finalización con Transición: Período adicional para completar compromisos pendientes"
            };
            
            message.Closing = 
                "Como organizador del evento, su decisión sobre este contrato es crucial para la planificación futura. " +
                "Estas opciones están basadas en el análisis objetivo del cumplimiento y el impacto generado.\n\n" +
                "Saludos cordiales,\nDepartamento de Gestión de Contratos";
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