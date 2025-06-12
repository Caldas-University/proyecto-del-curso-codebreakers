using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para registrar una decisión de renovación o finalización de contrato
    /// 🎯 CU-PA-05.02.3: Backend para registrar decisiones de renovación o finalización
    /// </summary>
    public class ContractDecisionDto
    {
        /// <summary>
        /// ID del contrato sobre el que se toma la decisión
        /// </summary>
        [Required]
        public string? ContractId { get; set; }

        /// <summary>
        /// Tipo de decisión: "Renovar" o "Finalizar"
        /// </summary>
        [Required]
        public string? DecisionType { get; set; }

        /// <summary>
        /// Opción específica seleccionada (índice de la opción)
        /// </summary>
        [Required]
        public int SelectedOptionIndex { get; set; }

        /// <summary>
        /// Comentarios o notas adicionales sobre la decisión
        /// </summary>
        public string? Comments { get; set; }

        /// <summary>
        /// Email del usuario que toma la decisión
        /// </summary>
        [Required]
        [EmailAddress]
        public string? UserEmail { get; set; }

        /// <summary>
        /// Rol del usuario que toma la decisión: "Patrocinador" o "Organizador"
        /// </summary>
        [Required]
        public string? UserRole { get; set; }

        /// <summary>
        /// Fecha y hora en que se tomó la decisión
        /// </summary>
        public DateTime DecisionDate { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// DTO con la respuesta tras registrar una decisión
    /// </summary>
    public class ContractDecisionResponseDto
    {
        /// <summary>
        /// Indica si la decisión se registró correctamente
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensaje informativo sobre el resultado
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// ID de la decisión registrada (si Success es true)
        /// </summary>
        public string? DecisionId { get; set; }

        /// <summary>
        /// Próximos pasos recomendados
        /// </summary>
        public List<string> NextSteps { get; set; } = new List<string>();
    }
}