namespace SponsorshipManagement.Domain.Entities
{
    public class ContractCommitment
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CommitmentType Type { get; set; }
        public decimal TargetValue { get; set; }
        public decimal CurrentValue { get; set; }
        public string Unit { get; set; } = string.Empty; // Personas, Clicks, Impresiones, etc.
        public DateTime TargetDate { get; set; }
        public CommitmentPriority Priority { get; set; }
        public CommitmentStatus Status { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ContractCommitment()
        {
            Id = Guid.NewGuid();
            Status = CommitmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public decimal GetCompliancePercentage()
        {
            if (TargetValue == 0) return 0;
            return Math.Min((CurrentValue / TargetValue) * 100, 100);
        }

        public bool IsOverdue()
        {
            return DateTime.UtcNow > TargetDate && Status != CommitmentStatus.Completed;
        }

        public bool IsAtRisk()
        {
            var daysRemaining = (TargetDate - DateTime.UtcNow).Days;
            var compliancePercentage = GetCompliancePercentage();
            
            return daysRemaining < 7 && compliancePercentage < 80;
        }
    }

    public enum CommitmentType
    {
        Reach = 0,          // Alcance
        Engagement = 1,     // Interacciones
        Attendance = 2,     // Asistencia
        Sales = 3,          // Ventas
        BrandAwareness = 4, // Conocimiento de marca
        WebTraffic = 5      // Tráfico web
    }

    public enum CommitmentPriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Critical = 3
    }

}