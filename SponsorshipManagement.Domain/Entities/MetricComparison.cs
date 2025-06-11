namespace SponsorshipManagement.Domain.Entities
{
    public class MetricComparison
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ComparisonType { get; set; } = string.Empty; // PeriodOverPeriod, EventComparison, SponsorComparison
        public List<Guid> MetricRecordIds { get; set; } = new List<Guid>();
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
        public string Results { get; set; } = string.Empty; // JSON con resultados calculados
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}