namespace SponsorshipManagement.Application.Dtos
{
    public class MetricRecordDto
    {
        public Guid Id { get; set; }
        public Guid MetricId { get; set; }
        public Guid AdvertisingBenefitExecutionId { get; set; }
        public string SponsorDocumentNumber { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime MeasurementDate { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}