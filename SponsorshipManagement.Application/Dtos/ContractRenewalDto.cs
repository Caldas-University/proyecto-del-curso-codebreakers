using System;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para contratos próximos a vencer
    /// 🎯 CU-PA-05.01.1: Contratos que requieren renovación
    /// </summary>
    public class ContractRenewalDto
    {
        /// <summary>
        /// ID del contrato
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Título del contrato
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del contrato
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio del contrato
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Fecha de finalización del contrato
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Días restantes hasta vencimiento
        /// </summary>
        public int DaysUntilExpiration { get; set; }

        /// <summary>
        /// Estado actual del contrato
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del patrocinador
        /// </summary>
        public string SponsorName { get; set; } = string.Empty;

        /// <summary>
        /// ID del patrocinador
        /// </summary>
        public string SponsorId { get; set; } = string.Empty;

        /// <summary>
        /// Valor del contrato
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// Indica si el contrato ya ha sido notificado para renovación
        /// </summary>
        public bool NotificationSent { get; set; }

        /// <summary>
        /// Fecha de última notificación de renovación
        /// </summary>
        public DateTime? LastNotificationDate { get; set; }
    }
}