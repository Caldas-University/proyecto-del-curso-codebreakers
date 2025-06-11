namespace SponsorshipManagement.Domain.Entities
{
    public class MetricRecord
    {
        public Guid Id { get; set; }
        public Guid MetricId { get; set; }
        public Guid AdvertisingBenefitExecutionId { get; set; }
        public string SponsorDocumentNumber { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime MeasurementDate { get; set; }
        public string Source { get; set; } = string.Empty; // Facebook, Instagram, Web, Manual
        public string Period { get; set; } = string.Empty; // Diario, Semanal, Mensual, Evento
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}