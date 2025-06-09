namespace SponsorshipManagement.Application.Dtos
{
    public class EventDto
    {
        public string Id { get; set; } = string.Empty;
        public string Place { get; set; } = string.Empty;
        public decimal Fund { get; set; }
        public int Capacity { get; set; }
        public List<string> Sponsors { get; set; } = new List<string>();
    }
}
