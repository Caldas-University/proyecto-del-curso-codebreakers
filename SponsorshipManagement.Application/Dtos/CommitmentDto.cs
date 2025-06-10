using System.ComponentModel.DataAnnotations;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para representar un compromiso contractual
    /// </summary>
    public class CommitmentDto
    {
        public string Id { get; set; } = string.Empty;
        public string ContractId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Obligations { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Responsible { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// Request para crear un nuevo compromiso contractual
    /// </summary>
    public class CreateCommitmentRequest
    {
        /// <summary>
        /// ID del contrato asociado al compromiso
        /// </summary>
        [Required(ErrorMessage = "El ID del contrato es obligatorio")]
        public string ContractId { get; set; } = string.Empty;

        /// <summary>
        /// Descripción del compromiso
        /// </summary>
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Obligaciones específicas del compromiso
        /// </summary>
        [Required(ErrorMessage = "Las obligaciones son obligatorias")]
        [StringLength(1000, ErrorMessage = "Las obligaciones no pueden exceder 1000 caracteres")]
        public string Obligations { get; set; } = string.Empty;

        /// <summary>
        /// Fecha límite para cumplir el compromiso
        /// </summary>
        [Required(ErrorMessage = "La fecha de vencimiento es obligatoria")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Persona responsable del compromiso
        /// </summary>
        [Required(ErrorMessage = "El responsable es obligatorio")]
        [StringLength(200, ErrorMessage = "El nombre del responsable no puede exceder 200 caracteres")]
        public string Responsible { get; set; } = string.Empty;
    }

    /// <summary>
    /// Response genérico para operaciones
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}