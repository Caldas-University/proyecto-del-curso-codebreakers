namespace SponsorshipManagement.Domain.Entities
{
    public class Contract
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string SponsorId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Contract()
        {
            Id = Guid.NewGuid();
            Status = ContractStatus.Draft;
            CreatedAt = DateTime.UtcNow;
        }

        public Contract(string title, string description, decimal value, DateTime startDate, DateTime endDate, string sponsorId, string eventId)
            : this()
        {
            Title = title;
            Description = description;
            Value = value;
            StartDate = startDate;
            EndDate = endDate;
            SponsorId = sponsorId;
            EventId = eventId;
        }

        public bool IsActive()
        {
            return Status == ContractStatus.Active && 
                   DateTime.UtcNow >= StartDate && 
                   DateTime.UtcNow <= EndDate;
        }

        public bool CanHaveCommitments()
        {
            return Status == ContractStatus.Active || Status == ContractStatus.Signed;
        }
    }

    public enum ContractStatus
    {
        Draft = 0,
        UnderReview = 1,
        Signed = 2,
        Active = 3,
        Completed = 4,
        Cancelled = 5,
        Expired = 6
    }
}