namespace SponsorshipManagement.Domain.Entities
{
    public class ContractHistory
    {
        public string Id { get; set; } = string.Empty;
        public string ContractId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty; // e.g. Created, Updated, Deleted
        public DateTime Timestamp { get; set; }
        public string PreviousVersionJson { get; set; } = string.Empty; // Stores the previous contract version as JSON
    }
}