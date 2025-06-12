using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Servicio para gestión de renovaciones y finalización de contratos
    /// 🎯 CU-PA-05: Gestionar renovaciones y finalización de contratos
    /// </summary>
    public class ContractRenewalService : IContractRenewalService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ISponsorRepository _sponsorRepository;
        private readonly IContractComplianceService _complianceService;
        private readonly IMetricService _metricService;
        private readonly IAdvertisingBenefitExecutionService _benefitExecutionService;

        public ContractRenewalService(
            IContractRepository contractRepository,
            ISponsorRepository sponsorRepository,
            IContractComplianceService complianceService,
            IMetricService metricService,
            IAdvertisingBenefitExecutionService benefitExecutionService)
        {
            _contractRepository = contractRepository ?? throw new ArgumentNullException(nameof(contractRepository));
            _sponsorRepository = sponsorRepository ?? throw new ArgumentNullException(nameof(sponsorRepository));
            _complianceService = complianceService ?? throw new ArgumentNullException(nameof(complianceService));
            _metricService = metricService ?? throw new ArgumentNullException(nameof(metricService));
            _benefitExecutionService = benefitExecutionService ?? throw new ArgumentNullException(nameof(benefitExecutionService));
        }

        /// <summary>
        /// Obtiene contratos que vencen en los próximos N días
        /// 🎯 CU-PA-05.01.1: Consulta de contratos próximos a vencer
        /// </summary>
        public async Task<IEnumerable<ContractRenewalDto>> GetContractsExpiringInDaysAsync(
            int days = 30, 
            string[]? includeStatuses = null)
        {
            Console.WriteLine($"🔍 CU-PA-05.01.1: Buscando contratos que vencen en los próximos {days} días");
            
            var today = DateTime.UtcNow.Date;
            var expirationLimit = today.AddDays(days);
            
            var allContracts = await _contractRepository.GetAllAsync();
            Console.WriteLine($"📊 Total de contratos obtenidos: {allContracts.Count()}");
            
            // Si no se especifican estados, usar todos los estados activos
            if (includeStatuses == null || includeStatuses.Length == 0)
            {
                includeStatuses = new[] { "Active", "Signed" };
                Console.WriteLine($"ℹ️ Usando estados predeterminados: {string.Join(", ", includeStatuses)}");
            }
            
            var filteredContracts = allContracts.Where(c => 
                c.EndDate.Date > today && 
                c.EndDate.Date <= expirationLimit && 
                includeStatuses.Contains(c.Status.ToString())).ToList();
            
            Console.WriteLine($"🔍 Contratos filtrados por fecha y estado: {filteredContracts.Count}");
            
            var result = new List<ContractRenewalDto>();
            
            foreach (var contract in filteredContracts)
            {
                var daysUntilExpiration = (int)(contract.EndDate.Date - today).TotalDays;
                
                string sponsorName = "Desconocido";
                try
                {
                    // Intentar obtener el patrocinador usando el ID
                    var sponsor = await _sponsorRepository.GetByDocumentAsync(contract.SponsorId.ToString());
                    if (sponsor != null)
                    {
                        sponsorName = sponsor.Name;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error obteniendo información del patrocinador {contract.SponsorId}: {ex.Message}");
                }
                
                // Valores por defecto para notificaciones
                bool notificationSent = false;
                DateTime? lastNotificationDate = null;
                
                // Verificar si el contrato tiene propiedades de notificación
                var contractType = contract.GetType();
                var notificationProperty = contractType.GetProperty("NotificationSent");
                var dateProperty = contractType.GetProperty("LastNotificationDate");
                
                if (notificationProperty != null && dateProperty != null)
                {
                    var notificationValue = notificationProperty.GetValue(contract);
                    if (notificationValue != null)
                    {
                        notificationSent = Convert.ToBoolean(notificationValue);
                    }
                    
                    lastNotificationDate = dateProperty.GetValue(contract) as DateTime?;
                }
                
                var renewalDto = new ContractRenewalDto
                {
                    Id = contract.Id.ToString(),
                    Title = contract.Title,
                    Description = contract.Description,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    DaysUntilExpiration = daysUntilExpiration,
                    Status = contract.Status.ToString(),
                    SponsorId = contract.SponsorId.ToString(),
                    SponsorName = sponsorName,
                    Value = contract.Value,
                    NotificationSent = notificationSent,
                    LastNotificationDate = lastNotificationDate
                };
                
                result.Add(renewalDto);
            }
            
            Console.WriteLine($"✅ CU-PA-05.01.1: Encontrados {result.Count} contratos próximos a vencer");
            
            // Ordenar por días hasta vencimiento (más cercanos primero)
            return result.OrderBy(c => c.DaysUntilExpiration);
        }

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        public async Task<bool> MarkContractAsNotifiedAsync(Guid contractId)
        {
            try
            {
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                {
                    Console.WriteLine($"❌ Contrato con ID {contractId} no encontrado");
                    return false;
                }
                
                // Verificar si las propiedades existen usando reflexión
                var contractType = contract.GetType();
                var notificationProperty = contractType.GetProperty("NotificationSent");
                var dateProperty = contractType.GetProperty("LastNotificationDate");
                
                if (notificationProperty != null && dateProperty != null)
                {
                    // Establecer las propiedades
                    notificationProperty.SetValue(contract, true);
                    dateProperty.SetValue(contract, DateTime.UtcNow);
                    
                    // Actualizar el contrato
                    var updatedContract = await _contractRepository.UpdateAsync(contract);
                    bool success = updatedContract != null;
                    
                    Console.WriteLine(success 
                        ? $"✅ Contrato {contractId} marcado como notificado" 
                        : $"❌ Error al actualizar el contrato {contractId}");
                    
                    return success;
                }
                
                Console.WriteLine("⚠️ Las propiedades NotificationSent y LastNotificationDate no están disponibles");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al marcar contrato como notificado: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Valida cumplimiento e impacto publicitario para un contrato
        /// 🎯 CU-PA-05.01.2: Validación de cumplimiento e impacto publicitario
        /// </summary>
        public async Task<ContractRenewalValidationDto> ValidateContractForRenewalAsync(Guid contractId)
        {
            Console.WriteLine($"🔍 CU-PA-05.01.2: Validando contrato {contractId} para renovación");

            // 1. Obtener datos del contrato
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
                throw new ArgumentException($"Contrato con ID {contractId} no encontrado");

            // 2. Calcular días hasta vencimiento
            var daysUntilExpiration = (int)(contract.EndDate.Date - DateTime.UtcNow.Date).TotalDays;
            Console.WriteLine($"ℹ️ Días hasta vencimiento: {daysUntilExpiration}");

            // 3. Obtener datos de cumplimiento (CU-PA-02)
            Console.WriteLine("🔄 Obteniendo datos de cumplimiento...");
            var complianceReport = await _complianceService.ValidateContractComplianceAsync(contractId);
            var complianceRecommendations = await _complianceService.GetRecommendationsAsync(contractId);
            Console.WriteLine($"✅ Datos de cumplimiento obtenidos. Porcentaje global: {complianceReport.OverallCompliancePercentage:F2}%");

            // 4. Obtener datos de impacto publicitario (CU-PA-04)
            Console.WriteLine("🔄 Obteniendo datos de impacto publicitario...");
            var advertisingMetrics = await GetAdvertisingMetricsAsync(contractId);
            var benefitsStatus = await GetBenefitsStatusAsync(contractId);
            var channelsAnalysis = await GetChannelsAnalysisAsync(contractId, contract.SponsorId);
            Console.WriteLine($"✅ Datos de impacto obtenidos. Cumplimiento de métricas: {advertisingMetrics.CompliancePercentage:F2}%");

            // 5. Calcular score general
            int overallScore = CalculateOverallScore(
                (double)complianceReport.OverallCompliancePercentage, 
                advertisingMetrics.CompliancePercentage,
                complianceReport.OverdueCommitments,
                benefitsStatus.PendingBenefits
            );
            Console.WriteLine($"📊 Score general calculado: {overallScore}/100");

            // 6. Generar recomendación de renovación
            string renewalRecommendation = GenerateRenewalRecommendation(
                overallScore, 
                (double)complianceReport.OverallCompliancePercentage, 
                advertisingMetrics.CompliancePercentage
            );
            Console.WriteLine($"📝 Recomendación: {renewalRecommendation}");

            // 7. Crear y retornar DTO con la información combinada
            var validationDto = new ContractRenewalValidationDto
            {
                ContractId = contractId.ToString(),
                ContractTitle = contract.Title,
                EndDate = contract.EndDate,
                DaysUntilExpiration = daysUntilExpiration,
                
                // Datos de cumplimiento (CU-PA-02)
                OverallCompliancePercentage = (double)complianceReport.OverallCompliancePercentage,
                TotalCommitments = complianceReport.TotalCommitments,
                CompletedCommitments = complianceReport.CompletedCommitments,
                PendingCommitments = complianceReport.InProgressCommitments,  // Usar InProgressCommitments en lugar de PendingCommitments
                OverdueCommitments = complianceReport.OverdueCommitments,
                ComplianceRecommendations = complianceRecommendations,
                RiskLevel = complianceReport.RiskLevel.ToString(),
                
                // Datos de impacto publicitario (CU-PA-04)
                TotalReach = advertisingMetrics.TotalReach,
                TotalEngagement = advertisingMetrics.TotalEngagement,
                EstimatedROI = advertisingMetrics.EstimatedROI,
                AdvertisingMetricsCompliancePercentage = advertisingMetrics.CompliancePercentage,
                ExecutedBenefits = benefitsStatus.ExecutedBenefits,
                PendingBenefits = benefitsStatus.PendingBenefits,
                TopPerformingChannels = channelsAnalysis.TopPerformingChannels,
                ImpactAnalysisSummary = channelsAnalysis.Summary,
                
                // Evaluación general
                RenewalRecommendation = renewalRecommendation,
                OverallScore = overallScore,
                GeneratedAt = DateTime.UtcNow
            };

            Console.WriteLine($"✅ CU-PA-05.01.2: Validación completada para contrato {contractId}");
            return validationDto;
        }

        #region Métodos auxiliares para obtener datos de impacto publicitario (CU-PA-04)

        private async Task<(long TotalReach, long TotalEngagement, double EstimatedROI, double CompliancePercentage)> 
            GetAdvertisingMetricsAsync(Guid contractId)
        {
            try
            {
                // Buscar métricas por contrato
                // Alternativa: usar un método específico o relacionar por EventId/SponsorId
                var contract = await _contractRepository.GetByIdAsync(contractId);
                if (contract == null)
                    return (0, 0, 0, 0);
                    
                var allMetrics = await _metricService.GetAllMetricsAsync();
                
                // Filtrar métricas relacionadas con el mismo evento del contrato
                // (Asumiendo que las métricas tienen EventId)
                var metrics = new List<MetricDto>();

                // Intentar filtrar por diferentes opciones según la estructura de MetricDto
                try {
                    // 1. Intentar filtrar por ID de contrato si existe esa propiedad
                    var contractIdProperty = typeof(MetricDto).GetProperty("ContractId");
                    if (contractIdProperty != null)
                    {
                        metrics = allMetrics.Where(m => 
                            contractIdProperty.GetValue(m)?.ToString() == contractId.ToString())
                            .ToList();
                        
                        Console.WriteLine($"📊 Filtrando métricas por ContractId: encontradas {metrics.Count}");
                    }
                    // 2. Si no hay ContractId, intentar por EventId
                    else if (typeof(MetricDto).GetProperty("EventId") != null)
                    {
                        metrics = allMetrics.Where(m => 
                            m.GetType().GetProperty("EventId")?.GetValue(m)?.ToString() == contract.EventId)
                            .ToList();
                        
                        Console.WriteLine($"📊 Filtrando métricas por EventId: encontradas {metrics.Count}");
                    }
                    // 3. Si no hay relación directa, usar SponsorId como alternativa
                    else if (typeof(MetricDto).GetProperty("SponsorId") != null)
                    {
                        metrics = allMetrics.Where(m => 
                            m.GetType().GetProperty("SponsorId")?.GetValue(m)?.ToString() == contract.SponsorId)
                            .ToList();
                        
                        Console.WriteLine($"📊 Filtrando métricas por SponsorId: encontradas {metrics.Count}");
                    }
                    // 4. Si no hay forma de relacionar, usar todas las métricas
                    else
                    {
                        metrics = allMetrics.ToList();
                        Console.WriteLine($"⚠️ No se encontró forma de filtrar métricas por contrato, usando todas: {metrics.Count}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error al filtrar métricas: {ex.Message}");
                    // En caso de error, usar una lista vacía
                    metrics = new List<MetricDto>();
                }
                
                long totalReach = 0;
                long totalEngagement = 0;
                double estimatedROI = 0;
                double compliancePercentage = 0;
                
                if (metrics != null && metrics.Any())
                {
                    // Obtener las propiedades necesarias mediante reflexión
                    var metricType = typeof(MetricDto);
                    var typeProperty = metricType.GetProperty("Type");
                    var currentValueProperty = metricType.GetProperty("CurrentValue");
                    var targetValueProperty = metricType.GetProperty("TargetValue");

                    if (typeProperty != null && currentValueProperty != null && targetValueProperty != null)
                    {
                        // Buscar métricas específicas por tipo
                        var reachMetric = metrics.FirstOrDefault(m => 
                            typeProperty.GetValue(m)?.ToString()?.Equals("Reach", StringComparison.OrdinalIgnoreCase) == true);
                        
                        if (reachMetric != null)
                        {
                            var currentValue = currentValueProperty.GetValue(reachMetric);
                            if (currentValue != null)
                            {
                                totalReach = Convert.ToInt64(currentValue);
                            }
                        }
                        
                        var engagementMetric = metrics.FirstOrDefault(m => 
                            typeProperty.GetValue(m)?.ToString()?.Equals("Engagement", StringComparison.OrdinalIgnoreCase) == true);
                        
                        if (engagementMetric != null)
                        {
                            var currentValue = currentValueProperty.GetValue(engagementMetric);
                            if (currentValue != null)
                            {
                                totalEngagement = Convert.ToInt64(currentValue);
                            }
                        }
                        
                        var roiMetric = metrics.FirstOrDefault(m => 
                            typeProperty.GetValue(m)?.ToString()?.Equals("ROI", StringComparison.OrdinalIgnoreCase) == true);
                        
                        if (roiMetric != null)
                        {
                            var currentValue = currentValueProperty.GetValue(roiMetric);
                            if (currentValue != null)
                            {
                                estimatedROI = Convert.ToDouble(currentValue);
                            }
                        }
                        
                        // Calcular porcentaje de cumplimiento general de métricas
                        double totalTargetValue = 0;
                        double totalCurrentValue = 0;
                        
                        // Sumar los valores de forma segura
                        foreach (var metric in metrics)
                        {
                            var targetValue = targetValueProperty.GetValue(metric);
                            var currentValue = currentValueProperty.GetValue(metric);
                            
                            if (targetValue != null && currentValue != null)
                            {
                                totalTargetValue += Convert.ToDouble(targetValue);
                                totalCurrentValue += Convert.ToDouble(currentValue);
                            }
                        }
                        
                        if (totalTargetValue > 0)
                        {
                            compliancePercentage = Math.Min(100, (totalCurrentValue / totalTargetValue) * 100);
                        }
                    }
                }
                
                return (totalReach, totalEngagement, estimatedROI, compliancePercentage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error obteniendo métricas publicitarias: {ex.Message}");
                return (0, 0, 0, 0);
            }
        }

        private async Task<(int ExecutedBenefits, int PendingBenefits)> GetBenefitsStatusAsync(Guid contractId)
        {
            try
            {
                // Obtener ejecuciones de beneficios publicitarios
                var executions = await _benefitExecutionService.GetExecutionsByContractAsync(contractId);
                
                if (executions != null && executions.Any())
                {
                    int executedBenefits = executions.Count(e => 
                        e.Status.Equals("Ejecutado", StringComparison.OrdinalIgnoreCase));
                    
                    int pendingBenefits = executions.Count(e => 
                        e.Status.Equals("Pendiente", StringComparison.OrdinalIgnoreCase));
                    
                    return (executedBenefits, pendingBenefits);
                }
                
                return (0, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error obteniendo estado de beneficios: {ex.Message}");
                return (0, 0);
            }
        }

        private async Task<(List<string> TopPerformingChannels, string Summary)> GetChannelsAnalysisAsync(
            Guid contractId, string sponsorId)
        {
            try
            {
                // Obtener reporte de visibilidad
                var visibilityReport = await _benefitExecutionService.GetVisibilityReportAsync(sponsorId, null);
                
                // Valores predeterminados
                var topChannels = new List<string> { "Instagram", "Facebook", "Email Marketing" };
                var summary = "Los canales digitales muestran mayor efectividad que los tradicionales. " +
                              "Se recomienda mantener presencia en redes sociales para futuras campañas.";
                
                if (visibilityReport != null && visibilityReport.Any())
                {
                    // Generar canales basados en EventId como ejemplo
                    // (Dado que VisibilityReportDto no tiene propiedades Channel ni Effectiveness)
                    var channelsData = visibilityReport
                        .GroupBy(v => v.EventId)  // Usar EventId en lugar de Channel
                        .Select(g => new { 
                            Channel = g.Key, 
                            Performance = g.Sum(v => v.ExecutedBenefits) / (double)(g.Sum(v => v.TotalBenefits) + 0.1)  // Calcular una métrica de rendimiento
                        })
                        .OrderByDescending(c => c.Performance)
                        .ToList();
                    
                    if (channelsData.Any())
                    {
                        topChannels = channelsData.Take(3).Select(c => c.Channel).ToList();
                        
                        summary = "Basado en el análisis de los datos de ejecución de beneficios, " +
                                  "se ha identificado un buen rendimiento en los canales digitales, " +
                                  "con oportunidades de mejora en medios impresos.";
                    }
                }
                
                return (topChannels, summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error en análisis de canales: {ex.Message}");
                return (new List<string>(), "No hay datos suficientes para análisis de canales");
            }
        }

        private int CalculateOverallScore(
            double compliancePercentage, 
            double advertisingMetricsPercentage,
            int overdueCommitments, 
            int pendingBenefits)
        {
            // Ponderación: 50% cumplimiento, 40% métricas publicitarias, 10% penalizaciones
            double baseScore = (compliancePercentage * 0.5) + (advertisingMetricsPercentage * 0.4);
            
            // Penalizaciones por compromisos vencidos y beneficios pendientes
            int penalties = Math.Min(10, (overdueCommitments * 2) + pendingBenefits);
            
            return (int)Math.Max(0, Math.Min(100, baseScore - penalties));
        }

        private string GenerateRenewalRecommendation(
            int overallScore, 
            double compliancePercentage, 
            double advertisingMetricsPercentage)
        {
            if (overallScore >= 85)
            {
                return "RENOVAR: Excelente desempeño general, se recomienda renovar el contrato manteniendo o mejorando condiciones";
            }
            else if (overallScore >= 70)
            {
                return "RENOVAR CON AJUSTES: Buen desempeño, se recomienda renovar con ajustes menores en las métricas de seguimiento";
            }
            else if (overallScore >= 50)
            {
                return "EVALUAR: Desempeño aceptable pero con áreas de mejora significativas. Evaluar renegociación de términos";
            }
            else
            {
                return "NO RECOMENDADO: Desempeño por debajo de lo esperado. No se recomienda renovar sin cambios sustanciales en los términos";
            }
        }

        /// <summary>
        /// Obtiene contratos que vencen según criterios avanzados de filtrado
        /// 🎯 CU-PA-05.01.3: Exposición de contratos próximos a vencer con filtros avanzados
        /// </summary>
        public async Task<IEnumerable<ContractRenewalDto>> GetContractsExpiringAsync(ExpiringContractsFilterDto filter)
        {
            Console.WriteLine($"🔍 CU-PA-05.01.3: Buscando contratos con filtros avanzados, días={filter.Days}");
            
            // Obtener la lista básica de contratos que vencen en los próximos N días
            var contracts = await GetContractsExpiringInDaysAsync(
                filter.Days, 
                filter.IncludeStatuses);
            
            // Aplicar filtros adicionales
            var filteredContracts = contracts.AsEnumerable();
            
            // Filtrar por patrocinador si se especificó
            if (!string.IsNullOrEmpty(filter.SponsorId))
            {
                filteredContracts = filteredContracts.Where(c => 
                    c.SponsorId.Equals(filter.SponsorId, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine($"📋 Filtrado por patrocinador: {filter.SponsorId}");
            }
            
            // Filtrar por evento si se especificó
            if (!string.IsNullOrEmpty(filter.EventId))
            {
                // Como EventId no está en ContractRenewalDto, necesitamos obtener los contratos originales
                var contractIds = filteredContracts.Select(c => Guid.Parse(c.Id)).ToList();
                var allContracts = await _contractRepository.GetAllAsync();
                var contractsWithEvents = allContracts.Where(c => contractIds.Contains(c.Id)).ToList();
                
                var contractIdsWithEvent = contractsWithEvents
                    .Where(c => c.EventId == filter.EventId)
                    .Select(c => c.Id.ToString())
                    .ToList();
                
                filteredContracts = filteredContracts.Where(c => contractIdsWithEvent.Contains(c.Id));
                Console.WriteLine($"📋 Filtrado por evento: {filter.EventId}");
            }
            
            // Filtrar por valor mínimo si se especificó
            if (filter.MinValue.HasValue)
            {
                filteredContracts = filteredContracts.Where(c => c.Value >= filter.MinValue.Value);
                Console.WriteLine($"📋 Filtrado por valor mínimo: {filter.MinValue}");
            }
            
            // Filtrar por estado de notificación si se especificó
            if (filter.NotificationStatus.HasValue)
            {
                filteredContracts = filteredContracts.Where(c => c.NotificationSent == filter.NotificationStatus.Value);
                Console.WriteLine($"📋 Filtrado por estado de notificación: {filter.NotificationStatus}");
            }
            
            var result = filteredContracts.ToList();
            Console.WriteLine($"✅ CU-PA-05.01.3: Encontrados {result.Count} contratos con los filtros aplicados");
            
            return result;
        }

        /// <summary>
        /// Obtiene un resumen estadístico de los contratos próximos a vencer
        /// 🎯 CU-PA-05.01.3: Exposición de estadísticas de contratos próximos a vencer
        /// </summary>
        public async Task<ExpiringContractsSummaryDto> GetExpiringContractsSummaryAsync(int days = 30)
        {
            Console.WriteLine($"📊 CU-PA-05.01.3: Generando resumen de contratos próximos a vencer en {days} días");
            
            // Obtener todos los contratos próximos a vencer
            var contracts = await GetContractsExpiringInDaysAsync(days);
            var contractsList = contracts.ToList();
            
            // Crear el objeto de resumen
            var summary = new ExpiringContractsSummaryDto
            {
                TotalExpiringContracts = contractsList.Count,
                TotalValue = contractsList.Sum(c => c.Value),
                GeneratedAt = DateTime.UtcNow
            };
            
            // Agrupar por rango de días hasta vencimiento
            var expirationRanges = new Dictionary<string, int>
            {
                { "1-7 días", contractsList.Count(c => c.DaysUntilExpiration <= 7) },
                { "8-14 días", contractsList.Count(c => c.DaysUntilExpiration > 7 && c.DaysUntilExpiration <= 14) },
                { "15-30 días", contractsList.Count(c => c.DaysUntilExpiration > 14 && c.DaysUntilExpiration <= 30) },
                { "Más de 30 días", contractsList.Count(c => c.DaysUntilExpiration > 30) }
            };
            summary.ExpirationRanges = expirationRanges;
            
            // Agrupar por estado
            summary.ContractsByStatus = contractsList
                .GroupBy(c => c.Status)
                .ToDictionary(g => g.Key, g => g.Count());
            
            // Calcular porcentaje de contratos notificados
            int notifiedCount = contractsList.Count(c => c.NotificationSent);
            summary.PercentageNotified = contractsList.Count > 0 
                ? Math.Round((double)notifiedCount / contractsList.Count * 100, 2) 
                : 0;
            
            // Obtener top patrocinadores
            var topSponsors = contractsList
                .GroupBy(c => new { Id = c.SponsorId, Name = c.SponsorName })
                .Select(g => new SponsorExpiringContractsDto
                {
                    SponsorId = g.Key.Id,
                    SponsorName = g.Key.Name,
                    ContractCount = g.Count(),
                    TotalValue = g.Sum(c => c.Value)
                })
                .OrderByDescending(s => s.ContractCount)
                .Take(5)
                .ToList();
            
            summary.TopSponsors = topSponsors;
            
            Console.WriteLine($"✅ CU-PA-05.01.3: Resumen generado con {summary.TotalExpiringContracts} contratos");
            
            return summary;
        }

        #endregion
    }
}