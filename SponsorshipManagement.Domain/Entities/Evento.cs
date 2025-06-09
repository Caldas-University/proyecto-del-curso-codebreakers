namespace SponsorshipManagement.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; }
        public string Place { get; set; } = string.Empty;
        public decimal Fund { get; set; }
        public int Capacity { get; set; }
        public List<string> Sponsors { get; set; } = new List<string>();
    }
}
