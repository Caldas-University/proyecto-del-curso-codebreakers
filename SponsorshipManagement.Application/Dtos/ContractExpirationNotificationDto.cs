using System;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para notificación de vencimiento de contrato
    /// 🎯 CU-PA-05.02.1: Notificación de vencimiento de contrato
    /// </summary>
    public class ContractExpirationNotificationDto
    {
        /// <summary>
        /// ID de la notificación
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// ID del contrato que vence
        /// </summary>
        public string ContractId { get; set; } = string.Empty;

        /// <summary>
        /// Título del contrato
        /// </summary>
        public string ContractTitle { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de vencimiento del contrato
        /// </summary>
        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(30);

        /// <summary>
        /// Días restantes hasta el vencimiento
        /// </summary>
        public int DaysRemaining { get; set; }

        /// <summary>
        /// Valor del contrato
        /// </summary>
        public decimal ContractValue { get; set; }

        /// <summary>
        /// ID del patrocinador
        /// </summary>
        public string SponsorId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del patrocinador
        /// </summary>
        public string SponsorName { get; set; } = string.Empty;

        /// <summary>
        /// ID del evento
        /// </summary>
        public string EventId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del evento
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Destinatarios de la notificación
        /// </summary>
        public string[] Recipients { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Fecha de envío de la notificación
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Estado de la notificación
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Mensaje de error en caso de fallo
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}