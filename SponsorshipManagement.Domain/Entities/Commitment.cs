namespace SponsorshipManagement.Domain.Entities
{
    public class Commitment
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Obligations { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string Responsible { get; set; } = string.Empty;
        public CommitmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Commitment()
        {
            Id = Guid.NewGuid();
            Status = CommitmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public Commitment(Guid contractId, string description, string obligations, DateTime dueDate, string responsible)
            : this()
        {
            ContractId = contractId;
            Description = description;
            Obligations = obligations;
            DueDate = dueDate;
            Responsible = responsible;
        }

        public void UpdateStatus(CommitmentStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCommitment(string description, string obligations, DateTime dueDate, string responsible)
        {
            Description = description;
            Obligations = obligations;
            DueDate = dueDate;
            Responsible = responsible;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum CommitmentStatus
    {
        Pending = 0,
        InProgress = 1,
        Completed = 2,
        Cancelled = 3,
        Overdue = 4
    }
}