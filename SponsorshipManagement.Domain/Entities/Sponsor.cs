namespace SponsorshipManagement.Domain.Entities
{
    public class Sponsor
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
