namespace SponsorshipManagement.Domain.Entities
{
    public class Commitment
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Obligations { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Responsible { get; set; } = string.Empty;
        public CommitmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // 🎯 CU-PA-02.01.4: Campos adicionales para gestión de estados
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? StatusReason { get; set; } = string.Empty;
        public string? CompletionNotes { get; set; } = string.Empty;

        /// <summary>
        /// Constructor por defecto
        /// 🎯 CU-PA-02.01.4: Asigna automáticamente estado "Pendiente"
        /// </summary>
        public Commitment()
        {
            Id = Guid.NewGuid();
            Status = CommitmentStatus.Pending; //  Estado inicial siempre "Pendiente"
            CreatedAt = DateTime.UtcNow;
            
            Console.WriteLine($" CU-PA-02.01.4: Compromiso {Id} creado con estado inicial 'Pending'");
        }

        /// <summary>
        /// Constructor principal para crear compromisos
        ///  CU-PA-02.01.4: Garantiza estado inicial "Pendiente"
        /// </summary>
        public Commitment(Guid contractId, string description, string obligations, DateTime dueDate, string responsible)
            : this() // Llama al constructor base que asigna estado Pending
        {
            ContractId = contractId;
            Description = description;
            Obligations = obligations;
            DueDate = dueDate;
            Responsible = responsible;
            
            Console.WriteLine($"✅ CU-PA-02.01.4: Compromiso {Id} inicializado:");
            Console.WriteLine($"   Estado: {Status} (asignado automáticamente)");
            Console.WriteLine($"   Contrato: {ContractId}");
            Console.WriteLine($"   Responsable: {Responsible}");
            Console.WriteLine($"   Vencimiento: {DueDate:yyyy-MM-dd}");
        }

        /// <summary>
        ///  CU-PA-02.01.4: Inicia el compromiso (Pending → InProgress)
        /// </summary>
        public bool StartCommitment(string reason = "")
        {
            if (Status != CommitmentStatus.Pending)
            {
                Console.WriteLine($" No se puede iniciar compromiso {Id}. Estado actual: {Status}");
                return false;
            }

            Status = CommitmentStatus.InProgress;
            StartedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            StatusReason = reason;

            Console.WriteLine($" Compromiso {Id}: Pending → InProgress");
            return true;
        }

        /// <summary>
        /// CU-PA-02.01.4: Completa el compromiso (InProgress → Completed)
        /// </summary>
        public bool CompleteCommitment(string completionNotes = "")
        {
            if (Status != CommitmentStatus.InProgress)
            {
                Console.WriteLine($" No se puede completar compromiso {Id}. Estado actual: {Status}");
                return false;
            }

            Status = CommitmentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            CompletionNotes = completionNotes;

            Console.WriteLine($" Compromiso {Id}: InProgress → Completed");
            return true;
        }

        /// <summary>
        /// CU-PA-02.01.4: Cancela el compromiso (cualquier estado → Cancelled)
        /// </summary>
        public bool CancelCommitment(string reason)
        {
            if (Status == CommitmentStatus.Cancelled)
            {
                Console.WriteLine($"⚠️ Compromiso {Id} ya está cancelado");
                return false;
            }

            if (Status == CommitmentStatus.Completed)
            {
                Console.WriteLine($" No se puede cancelar compromiso {Id} porque ya está completado");
                return false;
            }

            Status = CommitmentStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            StatusReason = reason;

            Console.WriteLine($" Compromiso {Id}: {Status} → Cancelled. Razón: {reason}");
            return true;
        }

        /// <summary>
        ///  CU-PA-02.01.4: Verifica si el compromiso está vencido
        /// </summary>
        public bool IsOverdue()
        {
            return Status == CommitmentStatus.Pending && DateTime.UtcNow > DueDate;
        }

        /// <summary>
        /// CU-PA-02.01.4: Marca automáticamente como vencido si aplica
        /// </summary>
        public bool MarkAsOverdueIfApplicable()
        {
            if (IsOverdue() && Status == CommitmentStatus.Pending)
            {
                Status = CommitmentStatus.Overdue;
                UpdatedAt = DateTime.UtcNow;
                StatusReason = $"Compromiso vencido automáticamente. Fecha límite: {DueDate:yyyy-MM-dd}";
                
                Console.WriteLine($"⏰ Compromiso {Id}: Pending → Overdue (vencido el {DueDate:yyyy-MM-dd})");
                return true;
            }
            return false;
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Validaciones de transición de estado
        /// </summary>
        public bool CanTransitionTo(CommitmentStatus newStatus)
        {
            return newStatus switch
            {
                CommitmentStatus.Pending => false, // No se puede volver a Pending
                CommitmentStatus.InProgress => Status == CommitmentStatus.Pending || Status == CommitmentStatus.OnHold,
                CommitmentStatus.Completed => Status == CommitmentStatus.InProgress,
                CommitmentStatus.Cancelled => Status != CommitmentStatus.Completed,
                CommitmentStatus.Overdue => Status == CommitmentStatus.Pending && IsOverdue(),
                CommitmentStatus.OnHold => Status == CommitmentStatus.InProgress,
                _ => false
            };
        }

        /// <summary>
        /// Actualiza los campos del compromiso manteniendo el estado actual
        /// </summary>
        public void UpdateCommitment(string description, string obligations, DateTime dueDate, string responsible)
        {
            Description = description;
            Obligations = obligations;
            DueDate = dueDate;
            Responsible = responsible;
            UpdatedAt = DateTime.UtcNow;

            Console.WriteLine($"📝 Compromiso {Id} actualizado. Estado mantenido: {Status}");
        }
    }

    /// <summary>
    /// 🎯 CU-PA-02.01.4: Estados del compromiso con flujo definido
    /// </summary>
    public enum CommitmentStatus
    {
        /// <summary>
        /// Estado inicial - Asignado automáticamente al crear
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// En progreso - El responsable ha iniciado el trabajo
        /// </summary>
        InProgress = 1,
        
        /// <summary>
        /// Completado - Compromiso cumplido exitosamente
        /// </summary>
        Completed = 2,
        
        /// <summary>
        /// Cancelado - Compromiso cancelado por alguna razón
        /// </summary>
        Cancelled = 3,
        
        /// <summary>
        /// Vencido - Pasó la fecha límite sin completarse
        /// </summary>
        Overdue = 4,
        
        /// <summary>
        /// En espera - Pausado temporalmente
        /// </summary>
        OnHold = 5
    }
}