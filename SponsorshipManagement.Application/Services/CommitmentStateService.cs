using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Application.Interfaces;  // AGREGAR ESTE USING

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Servicio para gestión de estados de compromisos
    /// 🎯 CU-PA-02.01.4: Asignación y transiciones de estado
    /// </summary>
    public class CommitmentStateService
    {
        private readonly ICommitmentRepository _commitmentRepository;
        private readonly IAuditService? _auditService; // USAR LA INTERFAZ CORRECTA

        public CommitmentStateService(
            ICommitmentRepository commitmentRepository,
            IAuditService? auditService = null)  // INTERFAZ CORRECTA
        {
            _commitmentRepository = commitmentRepository;
            _auditService = auditService;
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Inicializa el estado de un compromiso a "Pendiente"
        /// Este método se asegura de que el compromiso tenga el estado correcto
        /// </summary>
        public async Task<bool> InitializeCommitmentStateAsync(Guid commitmentId)
        {
            try
            {
                Console.WriteLine($"🎯 CU-PA-02.01.4: Inicializando estado para compromiso {commitmentId}");

                var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
                if (commitment == null)
                {
                    Console.WriteLine($"❌ Compromiso {commitmentId} no encontrado");
                    return false;
                }

                // Verificar que esté en estado Pending (debería estar por defecto)
                if (commitment.Status != CommitmentStatus.Pending)
                {
                    Console.WriteLine($"⚠️ Compromiso {commitmentId} no está en estado Pending. Estado actual: {commitment.Status}");
                    
                    // Forzar estado Pending si es necesario (caso excepcional)
                    commitment.Status = CommitmentStatus.Pending;
                    commitment.UpdatedAt = DateTime.UtcNow;
                    
                    await _commitmentRepository.UpdateAsync(commitment);
                    Console.WriteLine($"🔧 Estado corregido a Pending para compromiso {commitmentId}");
                }

                Console.WriteLine($"✅ CU-PA-02.01.4: Compromiso {commitmentId} confirmado en estado Pending");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error inicializando estado del compromiso {commitmentId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Cambia el estado de un compromiso con validaciones
        /// </summary>
        public async Task<bool> ChangeCommitmentStatusAsync(Guid commitmentId, CommitmentStatus newStatus, string reason = "")
        {
            try
            {
                var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
                if (commitment == null)
                {
                    Console.WriteLine($"❌ Compromiso {commitmentId} no encontrado");
                    return false;
                }

                var currentStatus = commitment.Status;
                
                // Validar transición
                if (!commitment.CanTransitionTo(newStatus))
                {
                    Console.WriteLine($"❌ Transición inválida: {currentStatus} → {newStatus}");
                    return false;
                }

                // Ejecutar transición según el nuevo estado
                bool success = newStatus switch
                {
                    CommitmentStatus.InProgress => commitment.StartCommitment(reason),
                    CommitmentStatus.Completed => commitment.CompleteCommitment(reason),
                    CommitmentStatus.Cancelled => commitment.CancelCommitment(reason),
                    CommitmentStatus.Overdue => commitment.MarkAsOverdueIfApplicable(),
                    _ => false
                };

                if (success)
                {
                    await _commitmentRepository.UpdateAsync(commitment);
                    Console.WriteLine($"✅ Estado cambiado: {currentStatus} → {newStatus}");
                    
                    // Log para auditoría
                    await LogStateChangeAsync(commitmentId, currentStatus, newStatus, reason);

                    // CU-PA-02.01.5: Registrar auditoría de cambio de estado
                    if (_auditService != null)
                    {
                        await _auditService.LogCommitmentStatusChangeAsync(commitmentId, currentStatus, newStatus, reason);
                    }
                    else
                    {
                        Console.WriteLine($"AuditService no disponible. Cambio: {currentStatus} -> {newStatus}");
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error cambiando estado del compromiso {commitmentId}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Procesa compromisos vencidos automáticamente
        /// </summary>
        public async Task<int> ProcessOverdueCommitmentsAsync()
        {
            try
            {
                Console.WriteLine("🔍 Procesando compromisos vencidos...");

                var allCommitments = await _commitmentRepository.GetAllAsync();
                var pendingCommitments = allCommitments.Where(c => c.Status == CommitmentStatus.Pending);
                
                int processedCount = 0;

                foreach (var commitment in pendingCommitments)
                {
                    if (commitment.IsOverdue())
                    {
                        if (commitment.MarkAsOverdueIfApplicable())
                        {
                            await _commitmentRepository.UpdateAsync(commitment);
                            processedCount++;
                            
                            Console.WriteLine($"⏰ Compromiso {commitment.Id} marcado como vencido");
                        }
                    }
                }

                Console.WriteLine($"✅ Procesados {processedCount} compromisos vencidos");
                return processedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error procesando compromisos vencidos: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtiene estadísticas de estados de compromisos
        /// </summary>
        public async Task<StateStatistics> GetStateStatisticsAsync()
        {
            try
            {
                var allCommitments = await _commitmentRepository.GetAllAsync();
                var commitmentsList = allCommitments.ToList();

                var stats = new StateStatistics
                {
                    TotalCommitments = commitmentsList.Count,
                    PendingCount = commitmentsList.Count(c => c.Status == CommitmentStatus.Pending),
                    InProgressCount = commitmentsList.Count(c => c.Status == CommitmentStatus.InProgress),
                    CompletedCount = commitmentsList.Count(c => c.Status == CommitmentStatus.Completed),
                    CancelledCount = commitmentsList.Count(c => c.Status == CommitmentStatus.Cancelled),
                    OverdueCount = commitmentsList.Count(c => c.Status == CommitmentStatus.Overdue),
                    OnHoldCount = commitmentsList.Count(c => c.Status == CommitmentStatus.OnHold)
                };

                // Calcular porcentajes
                if (stats.TotalCommitments > 0)
                {
                    stats.CompletionRate = Math.Round((double)stats.CompletedCount / stats.TotalCommitments * 100, 2);
                    stats.PendingRate = Math.Round((double)stats.PendingCount / stats.TotalCommitments * 100, 2);
                    stats.OverdueRate = Math.Round((double)stats.OverdueCount / stats.TotalCommitments * 100, 2);
                }

                return stats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo estadísticas: {ex.Message}");
                return new StateStatistics();
            }
        }

        /// <summary>
        /// Valida si una transición de estado es válida
        /// </summary>
        public async Task<bool> ValidateStateTransitionAsync(Guid commitmentId, CommitmentStatus newStatus)
        {
            try
            {
                var commitment = await _commitmentRepository.GetByIdAsync(commitmentId);
                return commitment?.CanTransitionTo(newStatus) ?? false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Registra cambio de estado para auditoría
        /// </summary>
        private static async Task LogStateChangeAsync(Guid commitmentId, CommitmentStatus fromStatus, CommitmentStatus toStatus, string reason)
        {
            try
            {
                var logEntry = new
                {
                    CommitmentId = commitmentId,
                    FromStatus = fromStatus,
                    ToStatus = toStatus,
                    Reason = reason,
                    Timestamp = DateTime.UtcNow,
                    Source = "CU-PA-02.01.4"
                };

                Console.WriteLine($"📝 STATE_CHANGE_LOG: {fromStatus} → {toStatus} for {commitmentId}");
                
                // En implementación real, esto iría a un sistema de auditoría
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en log de cambio de estado: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Estadísticas de estados de compromisos
    /// </summary>
    public class StateStatistics
    {
        public int TotalCommitments { get; set; }
        public int PendingCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
        public int OverdueCount { get; set; }
        public int OnHoldCount { get; set; }
        
        public double CompletionRate { get; set; }
        public double PendingRate { get; set; }
        public double OverdueRate { get; set; }
    }
}