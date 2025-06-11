namespace SponsorshipManagement.Domain.Entities
{
    public class ContractComplianceReport
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal OverallCompliancePercentage { get; set; }
        public ComplianceStatus OverallStatus { get; set; }
        public int TotalCommitments { get; set; }
        public int CompletedCommitments { get; set; }
        public int InProgressCommitments { get; set; }
        public int OverdueCommitments { get; set; }
        public List<CommitmentCompliance> CommitmentCompliances { get; set; } = new List<CommitmentCompliance>();
        public string Recommendations { get; set; } = string.Empty;
        public RiskLevel RiskLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;

        public ContractComplianceReport()
        {
            Id = Guid.NewGuid();
            ReportDate = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public class CommitmentCompliance
    {
        public Guid CommitmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal CompliancePercentage { get; set; }
        public CommitmentStatus Status { get; set; }
        public int DaysRemaining { get; set; }
        public RiskLevel RiskLevel { get; set; }
        public string ActionRequired { get; set; } = string.Empty;
    }

    public enum ComplianceStatus
    {
        Excellent = 0,    // >= 90%
        Good = 1,         // >= 75%
        Regular = 2,      // >= 50%
        Poor = 3          // < 50%
    }

    public enum RiskLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }
}