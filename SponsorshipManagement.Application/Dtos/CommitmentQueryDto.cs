using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para consulta avanzada de compromisos
    /// 🎯 CU-PA-02.04.1: Servicio para consultar compromisos
    /// </summary>
    public class CommitmentQueryDto
    {
        /// <summary>
        /// ID del compromiso
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// ID del contrato
        /// </summary>
        public string ContractId { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del compromiso
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Obligaciones del compromiso
        /// </summary>
        public string Obligations { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de vencimiento
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Responsable del compromiso
        /// </summary>
        public string Responsible { get; set; } = string.Empty;

        /// <summary>
        /// Estado actual del compromiso
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del estado
        /// </summary>
        public string StatusDescription { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de creación
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Fecha de actualización
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Fecha de inicio (si aplica)
        /// </summary>
        public DateTime? StartedAt { get; set; }

        /// <summary>
        /// Fecha de completado (si aplica)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Días hasta vencimiento (negativo si está vencido)
        /// </summary>
        public int DaysToExpiry { get; set; }

        /// <summary>
        /// Indica si el compromiso está vencido
        /// </summary>
        public bool IsOverdue { get; set; }

        /// <summary>
        /// Indica si el compromiso está próximo a vencer (menos de 7 días)
        /// </summary>
        public bool IsNearExpiry { get; set; }

        /// <summary>
        /// Progreso del compromiso (porcentaje)
        /// </summary>
        public double ProgressPercentage { get; set; }

        /// <summary>
        /// Prioridad calculada basada en fecha de vencimiento y estado
        /// </summary>
        public string Priority { get; set; } = string.Empty;
    }

    /// <summary>
    /// Filtros para consulta de compromisos
    /// 🎯 CU-PA-02.04.1: Criterios de búsqueda avanzada
    /// </summary>
    public class CommitmentQueryFilters
    {
        /// <summary>
        /// ID del contrato (obligatorio)
        /// </summary>
        public Guid ContractId { get; set; }

        /// <summary>
        /// Estados a filtrar (si está vacío, incluye todos)
        /// </summary>
        public List<CommitmentStatus> StatusFilter { get; set; } = new List<CommitmentStatus>();

        /// <summary>
        /// Responsable específico
        /// </summary>
        public string? ResponsibleFilter { get; set; }

        /// <summary>
        /// Fecha desde para filtrar por fecha de vencimiento
        /// </summary>
        public DateTime? DueDateFrom { get; set; }

        /// <summary>
        /// Fecha hasta para filtrar por fecha de vencimiento
        /// </summary>
        public DateTime? DueDateTo { get; set; }

        /// <summary>
        /// Solo compromisos vencidos
        /// </summary>
        public bool OnlyOverdue { get; set; }

        /// <summary>
        /// Solo compromisos próximos a vencer
        /// </summary>
        public bool OnlyNearExpiry { get; set; }

        /// <summary>
        /// Ordenamiento
        /// </summary>
        public CommitmentSortBy SortBy { get; set; } = CommitmentSortBy.DueDate;

        /// <summary>
        /// Dirección del ordenamiento
        /// </summary>
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
    }

    /// <summary>
    /// Opciones de ordenamiento para compromisos
    /// </summary>
    public enum CommitmentSortBy
    {
        CreatedAt,
        DueDate,
        Status,
        Responsible,
        Priority
    }

    /// <summary>
    /// Dirección del ordenamiento
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }
}
