namespace SponsorshipManagement.Application.Dtos
{
    public class ContractDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Status { get; set; }
        public string SponsorId { get; set; } = string.Empty;
        public string EventId { get; set; } = string.Empty;
        public DateTime? CreatedAt { get; set; } // Puede ser null en creación
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}