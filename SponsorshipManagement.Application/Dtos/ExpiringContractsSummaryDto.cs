using System;
using System.Collections.Generic;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para resumen estadístico de contratos próximos a vencer
    /// 🎯 CU-PA-05.01.3: Resumen estadístico de contratos próximos a vencer
    /// </summary>
    public class ExpiringContractsSummaryDto
    {
        /// <summary>
        /// Número total de contratos próximos a vencer
        /// </summary>
        public int TotalExpiringContracts { get; set; }

        /// <summary>
        /// Valor total de contratos próximos a vencer
        /// </summary>
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Contratos agrupados por días hasta vencimiento
        /// </summary>
        public Dictionary<string, int> ExpirationRanges { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Contratos agrupados por estado
        /// </summary>
        public Dictionary<string, int> ContractsByStatus { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Top patrocinadores con contratos próximos a vencer
        /// </summary>
        public List<SponsorExpiringContractsDto> TopSponsors { get; set; } = new List<SponsorExpiringContractsDto>();

        /// <summary>
        /// Porcentaje de contratos notificados
        /// </summary>
        public double PercentageNotified { get; set; }

        /// <summary>
        /// Fecha de generación del informe
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO para información de patrocinador con contratos próximos a vencer
    /// </summary>
    public class SponsorExpiringContractsDto
    {
        /// <summary>
        /// ID del patrocinador
        /// </summary>
        public string SponsorId { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del patrocinador
        /// </summary>
        public string SponsorName { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad de contratos próximos a vencer
        /// </summary>
        public int ContractCount { get; set; }

        /// <summary>
        /// Valor total de contratos próximos a vencer
        /// </summary>
        public decimal TotalValue { get; set; }
    }
}