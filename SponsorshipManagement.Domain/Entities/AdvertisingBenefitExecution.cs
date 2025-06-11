namespace SponsorshipManagement.Domain.Entities
{
    public class AdvertisingBenefitExecution
    {
        public Guid Id { get; set; }
        public Guid SponsorshipContractId { get; set; }
        public Guid AdvertisingBenefitId { get; set; }
        public string SponsorDocumentNumber { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public DateTime ExecutionDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pendiente, Ejecutado, Cancelado
        public string ExecutionDetails { get; set; } = string.Empty;
        public string MediaFiles { get; set; } = string.Empty; // URLs o paths de archivos de evidencia
        public decimal ActualCost { get; set; }
        public string ExecutedBy { get; set; } = string.Empty; // Usuario o responsable
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}