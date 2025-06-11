namespace SponsorshipManagement.Application.Dtos
{
    public class ContractCommitmentDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty;
        public DateTime TargetDate { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal CompliancePercentage { get; set; }
        public bool IsOverdue { get; set; }
        public bool IsAtRisk { get; set; }
    }
}