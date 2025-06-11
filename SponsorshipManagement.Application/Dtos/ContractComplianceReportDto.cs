namespace SponsorshipManagement.Application.Dtos
{
    public class ContractComplianceReportDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public DateTime ReportDate { get; set; }
        public decimal OverallCompliancePercentage { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
        public int TotalCommitments { get; set; }
        public int CompletedCommitments { get; set; }
        public int InProgressCommitments { get; set; }
        public int OverdueCommitments { get; set; }
        public List<CommitmentComplianceDto> CommitmentCompliances { get; set; } = new List<CommitmentComplianceDto>();
        public string Recommendations { get; set; } = string.Empty;
        public string RiskLevel { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class CommitmentComplianceDto
    {
        public Guid CommitmentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal CompliancePercentage { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DaysRemaining { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string ActionRequired { get; set; } = string.Empty;
    }
}