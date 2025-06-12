using System;
using System.Collections.Generic;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para validación de renovación con información de cumplimiento e impacto publicitario
    /// 🎯 CU-PA-05.01.2: Validación de cumplimiento e impacto publicitario
    /// </summary>
    public class ContractRenewalValidationDto
    {
        /// <summary>
        /// ID del contrato
        /// </summary>
        public string ContractId { get; set; } = string.Empty;

        /// <summary>
        /// Título del contrato
        /// </summary>
        public string ContractTitle { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de finalización del contrato
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Días restantes hasta el vencimiento
        /// </summary>
        public int DaysUntilExpiration { get; set; }

        #region Datos de Cumplimiento (CU-PA-02)

        /// <summary>
        /// Porcentaje general de cumplimiento del contrato
        /// </summary>
        public double OverallCompliancePercentage { get; set; }

        /// <summary>
        /// Total de compromisos
        /// </summary>
        public int TotalCommitments { get; set; }

        /// <summary>
        /// Compromisos completados
        /// </summary>
        public int CompletedCommitments { get; set; }

        /// <summary>
        /// Compromisos pendientes
        /// </summary>
        public int PendingCommitments { get; set; }

        /// <summary>
        /// Compromisos vencidos
        /// </summary>
        public int OverdueCommitments { get; set; }

        /// <summary>
        /// Recomendaciones de mejora de cumplimiento
        /// </summary>
        public string ComplianceRecommendations { get; set; } = string.Empty;

        /// <summary>
        /// Nivel de riesgo del contrato
        /// </summary>
        public string RiskLevel { get; set; } = string.Empty;

        #endregion

        #region Datos de Impacto Publicitario (CU-PA-04)

        /// <summary>
        /// Métrica de alcance total obtenido
        /// </summary>
        public long TotalReach { get; set; }

        /// <summary>
        /// Número total de interacciones generadas
        /// </summary>
        public long TotalEngagement { get; set; }

        /// <summary>
        /// ROI (Retorno sobre la Inversión) estimado
        /// </summary>
        public double EstimatedROI { get; set; }

        /// <summary>
        /// Porcentaje de cumplimiento de métricas publicitarias
        /// </summary>
        public double AdvertisingMetricsCompliancePercentage { get; set; }

        /// <summary>
        /// Beneficios publicitarios ejecutados
        /// </summary>
        public int ExecutedBenefits { get; set; }

        /// <summary>
        /// Beneficios publicitarios pendientes
        /// </summary>
        public int PendingBenefits { get; set; }

        /// <summary>
        /// Lista de canales con mejor desempeño
        /// </summary>
        public List<string> TopPerformingChannels { get; set; } = new List<string>();

        /// <summary>
        /// Resumen del análisis de impacto
        /// </summary>
        public string ImpactAnalysisSummary { get; set; } = string.Empty;

        #endregion

        /// <summary>
        /// Recomendación general sobre renovación
        /// </summary>
        public string RenewalRecommendation { get; set; } = string.Empty;

        /// <summary>
        /// Puntaje general de evaluación (0-100)
        /// </summary>
        public int OverallScore { get; set; }

        /// <summary>
        /// Fecha de generación del informe
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}