using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Servicio para consulta avanzada de compromisos
    /// 🎯 CU-PA-02.04.1: Servicio para consultar compromisos
    /// </summary>
    public class CommitmentQueryService : ICommitmentQueryService
    {
        private readonly ICommitmentRepository _commitmentRepository;
        private readonly IAuditService? _auditService;

        public CommitmentQueryService(
            ICommitmentRepository commitmentRepository,
            IAuditService? auditService = null)
        {
            _commitmentRepository = commitmentRepository;
            _auditService = auditService;
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos con filtros avanzados
        /// </summary>
        public async Task<IEnumerable<CommitmentQueryDto>> GetCommitmentsWithFiltersAsync(CommitmentQueryFilters filters)
        {
            try
            {
                // Console.WriteLine($"🔍 CU-PA-02.04.1: Consultando compromisos con filtros para contrato {filters.ContractId}");

                // Obtener compromisos base
                var commitments = await _commitmentRepository.GetByContractIdAsync(filters.ContractId);
                var commitmentsList = commitments.ToList();

                // Console.WriteLine($"📊 Compromisos base encontrados: {commitmentsList.Count}");

                // Aplicar filtros
                var filteredCommitments = ApplyFilters(commitmentsList, filters);

                // Enriquecer con información adicional
                var enrichedCommitments = EnrichCommitments(filteredCommitments);

                // Aplicar ordenamiento
                var sortedCommitments = ApplySorting(enrichedCommitments, filters.SortBy, filters.SortDirection);

                // Console.WriteLine($"✅ CU-PA-02.04.1: Retornando {sortedCommitments.Count()} compromisos filtrados");

                return sortedCommitments;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error en consulta de compromisos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos con información enriquecida
        /// </summary>
        public async Task<IEnumerable<CommitmentQueryDto>> GetEnrichedCommitmentsByContractAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"📊 CU-PA-02.04.1: Obteniendo compromisos enriquecidos para contrato {contractId}");

                var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var commitmentsList = commitments.ToList();

                if (!commitmentsList.Any())
                {
                    // Console.WriteLine($"⚠️ D1: El contrato {contractId} no tiene compromisos");
                    return Enumerable.Empty<CommitmentQueryDto>();
                }

                var enrichedCommitments = EnrichCommitments(commitmentsList);

                // Console.WriteLine($"✅ Compromisos enriquecidos: {enrichedCommitments.Count()}");

                return enrichedCommitments.OrderBy(c => c.DueDate);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo compromisos enriquecidos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Agrupa compromisos por estado
        /// </summary>
        public async Task<Dictionary<string, List<CommitmentQueryDto>>> GetCommitmentsGroupedByStatusAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"📊 CU-PA-02.04.1: Agrupando compromisos por estado para contrato {contractId}");

                var enrichedCommitments = await GetEnrichedCommitmentsByContractAsync(contractId);

                var groupedCommitments = enrichedCommitments
                    .GroupBy(c => c.Status)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderBy(c => c.DueDate).ToList()
                    );

                // Console.WriteLine($"✅ Compromisos agrupados en {groupedCommitments.Keys.Count} estados");

                return groupedCommitments;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error agrupando compromisos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene compromisos críticos
        /// </summary>
        public async Task<IEnumerable<CommitmentQueryDto>> GetCriticalCommitmentsAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"⚠️ CU-PA-02.04.1: Obteniendo compromisos críticos para contrato {contractId}");

                var enrichedCommitments = await GetEnrichedCommitmentsByContractAsync(contractId);

                var criticalCommitments = enrichedCommitments
                    .Where(c => c.IsOverdue || c.IsNearExpiry)
                    .OrderBy(c => c.DaysToExpiry)
                    .ToList();

                // Console.WriteLine($"🚨 Compromisos críticos encontrados: {criticalCommitments.Count}");

                return criticalCommitments;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo compromisos críticos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Busca compromisos por texto
        /// </summary>
        public async Task<IEnumerable<CommitmentQueryDto>> SearchCommitmentsAsync(Guid contractId, string searchText)
        {
            try
            {
                // Console.WriteLine($"🔍 CU-PA-02.04.1: Buscando '{searchText}' en compromisos del contrato {contractId}");

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return await GetEnrichedCommitmentsByContractAsync(contractId);
                }

                var enrichedCommitments = await GetEnrichedCommitmentsByContractAsync(contractId);

                var searchResults = enrichedCommitments
                    .Where(c => 
                        c.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Obligations.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                        c.Responsible.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(c => c.DueDate)
                    .ToList();

                // Console.WriteLine($"✅ Encontrados {searchResults.Count} compromisos que coinciden con '{searchText}'");

                return searchResults;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error en búsqueda de compromisos: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.04.1: Obtiene historial de cambios de estado
        /// </summary>
        public async Task<IEnumerable<CommitmentStatusHistoryDto>> GetStatusHistoryAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"📜 CU-PA-02.04.1: Obteniendo historial de cambios para contrato {contractId}");

                if (_auditService == null)
                {
                    // Console.WriteLine("⚠️ AuditService no disponible - No se puede obtener historial");
                    return Enumerable.Empty<CommitmentStatusHistoryDto>();
                }

                // Obtener compromisos del contrato
                var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var history = new List<CommitmentStatusHistoryDto>();

                foreach (var commitment in commitments)
                {
                    var auditLogs = await _auditService.GetCommitmentAuditHistoryAsync(commitment.Id);
                    
                    var statusChanges = auditLogs
                        .Where(log => log.Action == AuditActions.STATUS_CHANGE)
                        .Select(log => new CommitmentStatusHistoryDto
                        {
                            CommitmentId = commitment.Id.ToString(),
                            CommitmentDescription = commitment.Description,
                            FromStatus = ExtractFromStatus(log.Details),
                            ToStatus = ExtractToStatus(log.Details),
                            ChangeDate = log.Timestamp,
                            ChangedBy = log.UserName,
                            Reason = ExtractReason(log.Details)
                        });

                    history.AddRange(statusChanges);
                }

                var sortedHistory = history
                    .OrderByDescending(h => h.ChangeDate)
                    .ToList();

                // Console.WriteLine($"✅ Historial obtenido: {sortedHistory.Count} cambios de estado");

                return sortedHistory;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo historial: {ex.Message}");
                return Enumerable.Empty<CommitmentStatusHistoryDto>();
            }
        }

        #region Private Methods

        /// <summary>
        /// Aplica filtros a la lista de compromisos
        /// </summary>
        private static List<Commitment> ApplyFilters(List<Commitment> commitments, CommitmentQueryFilters filters)
        {
            var filtered = commitments.AsEnumerable();

            // Filtro por estado
            if (filters.StatusFilter.Any())
            {
                filtered = filtered.Where(c => filters.StatusFilter.Contains(c.Status));
            }

            // Filtro por responsable
            if (!string.IsNullOrWhiteSpace(filters.ResponsibleFilter))
            {
                filtered = filtered.Where(c => c.Responsible.Contains(filters.ResponsibleFilter, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por fecha de vencimiento
            if (filters.DueDateFrom.HasValue)
            {
                filtered = filtered.Where(c => c.DueDate >= filters.DueDateFrom.Value);
            }

            if (filters.DueDateTo.HasValue)
            {
                filtered = filtered.Where(c => c.DueDate <= filters.DueDateTo.Value);
            }

            // Filtro solo vencidos
            if (filters.OnlyOverdue)
            {
                filtered = filtered.Where(c => c.IsOverdue());
            }

            // Filtro próximos a vencer
            if (filters.OnlyNearExpiry)
            {
                var nearExpiryDate = DateTime.UtcNow.AddDays(7);
                filtered = filtered.Where(c => c.DueDate <= nearExpiryDate && c.DueDate >= DateTime.UtcNow);
            }

            return filtered.ToList();
        }

        /// <summary>
        /// Enriquece compromisos con información adicional
        /// </summary>
        private static List<CommitmentQueryDto> EnrichCommitments(List<Commitment> commitments)
        {
            return commitments.Select(c => new CommitmentQueryDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                StatusDescription = GetStatusDescription(c.Status),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                StartedAt = c.StartedAt,
                CompletedAt = c.CompletedAt,
                DaysToExpiry = CalculateDaysToExpiry(c.DueDate),
                IsOverdue = c.IsOverdue(),
                IsNearExpiry = IsNearExpiry(c.DueDate),
                ProgressPercentage = CalculateProgress(c.Status),
                Priority = CalculatePriority(c)
            }).ToList();
        }

        /// <summary>
        /// Aplica ordenamiento a los compromisos
        /// </summary>
        private static IEnumerable<CommitmentQueryDto> ApplySorting(
            List<CommitmentQueryDto> commitments, 
            CommitmentSortBy sortBy, 
            SortDirection direction)
        {
            IOrderedEnumerable<CommitmentQueryDto> sorted = sortBy switch
            {
                CommitmentSortBy.CreatedAt => direction == SortDirection.Ascending 
                    ? commitments.OrderBy(c => c.CreatedAt)
                    : commitments.OrderByDescending(c => c.CreatedAt),
                CommitmentSortBy.DueDate => direction == SortDirection.Ascending 
                    ? commitments.OrderBy(c => c.DueDate)
                    : commitments.OrderByDescending(c => c.DueDate),
                CommitmentSortBy.Status => direction == SortDirection.Ascending 
                    ? commitments.OrderBy(c => c.Status)
                    : commitments.OrderByDescending(c => c.Status),
                CommitmentSortBy.Responsible => direction == SortDirection.Ascending 
                    ? commitments.OrderBy(c => c.Responsible)
                    : commitments.OrderByDescending(c => c.Responsible),
                CommitmentSortBy.Priority => direction == SortDirection.Ascending 
                    ? commitments.OrderBy(c => c.Priority)
                    : commitments.OrderByDescending(c => c.Priority),
                _ => commitments.OrderBy(c => c.DueDate)
            };

            return sorted;
        }

        /// <summary>
        /// Obtiene descripción del estado
        /// </summary>
        private static string GetStatusDescription(CommitmentStatus status)
        {
            return status switch
            {
                CommitmentStatus.Pending => "Pendiente",
                CommitmentStatus.InProgress => "En Progreso",
                CommitmentStatus.Completed => "Completado",
                CommitmentStatus.Cancelled => "Cancelado",
                CommitmentStatus.Overdue => "Vencido",
                CommitmentStatus.OnHold => "En Espera",
                _ => "Desconocido"
            };
        }

        /// <summary>
        /// Calcula días hasta vencimiento
        /// </summary>
        private static int CalculateDaysToExpiry(DateTime dueDate)
        {
            return (int)(dueDate.Date - DateTime.UtcNow.Date).TotalDays;
        }

        /// <summary>
        /// Determina si está próximo a vencer
        /// </summary>
        private static bool IsNearExpiry(DateTime dueDate)
        {
            var daysToExpiry = CalculateDaysToExpiry(dueDate);
            return daysToExpiry >= 0 && daysToExpiry <= 7;
        }

        /// <summary>
        /// Calcula porcentaje de progreso
        /// </summary>
        private static double CalculateProgress(CommitmentStatus status)
        {
            return status switch
            {
                CommitmentStatus.Pending => 0,
                CommitmentStatus.InProgress => 50,
                CommitmentStatus.OnHold => 25,
                CommitmentStatus.Completed => 100,
                CommitmentStatus.Cancelled => 0,
                CommitmentStatus.Overdue => 0,
                _ => 0
            };
        }

        /// <summary>
        /// Calcula prioridad del compromiso
        /// </summary>
        private static string CalculatePriority(Commitment commitment)
        {
            if (commitment.Status == CommitmentStatus.Completed)
                return "Completado";

            if (commitment.IsOverdue())
                return "Crítica";

            var daysToExpiry = CalculateDaysToExpiry(commitment.DueDate);

            return daysToExpiry switch
            {
                <= 3 => "Urgente",
                <= 7 => "Alta",
                <= 14 => "Media",
                _ => "Baja"
            };
        }

        /// <summary>
        /// Extrae estado origen del detalle de auditoría
        /// </summary>
        private static string ExtractFromStatus(string details)
        {
            // Implementación simple - en producción sería más robusta
            if (details.Contains("→"))
            {
                var parts = details.Split("→");
                return parts[0].Trim();
            }
            return "N/A";
        }

        /// <summary>
        /// Extrae estado destino del detalle de auditoría
        /// </summary>
        private static string ExtractToStatus(string details)
        {
            if (details.Contains("→"))
            {
                var parts = details.Split("→");
                return parts.Length > 1 ? parts[1].Split('.')[0].Trim() : "N/A";
            }
            return "N/A";
        }

        /// <summary>
        /// Extrae razón del cambio del detalle de auditoría
        /// </summary>
        private static string ExtractReason(string details)
        {
            if (details.Contains("Razón:"))
            {
                var parts = details.Split("Razón:");
                return parts.Length > 1 ? parts[1].Trim() : "";
            }
            return "";
        }

        #endregion
    }
}
