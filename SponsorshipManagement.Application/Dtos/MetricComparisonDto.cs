namespace SponsorshipManagement.Application.Dtos
{
    public class MetricComparisonDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ComparisonType { get; set; } = string.Empty;
        public List<Guid> MetricRecordIds { get; set; } = new List<Guid>();
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string Results { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}