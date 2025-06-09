namespace SponsorshipManagement.Application.Dtos
{
    public class SponsorDto
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
