using System;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para filtrar contratos próximos a vencer
    /// 🎯 CU-PA-05.01.3: Filtros para exposición de contratos próximos a vencer
    /// </summary>
    public class ExpiringContractsFilterDto
    {
        /// <summary>
        /// Número de días para considerar
        /// </summary>
        public int Days { get; set; } = 30;

        /// <summary>
        /// Estados de contrato a incluir
        /// </summary>
        public string[]? IncludeStatuses { get; set; }

        /// <summary>
        /// ID del patrocinador para filtrar
        /// </summary>
        public string? SponsorId { get; set; }

        /// <summary>
        /// ID del evento para filtrar
        /// </summary>
        public string? EventId { get; set; }

        /// <summary>
        /// Valor mínimo del contrato
        /// </summary>
        public decimal? MinValue { get; set; }

        /// <summary>
        /// Estado de notificación (true=notificados, false=no notificados, null=todos)
        /// </summary>
        public bool? NotificationStatus { get; set; }
    }
}