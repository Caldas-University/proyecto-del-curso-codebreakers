namespace SponsorshipManagement.Application.Dtos
{
    public class ContractDto
    {
        // No enviar Id ni CreatedAt en la creación
        public string? Id { get; set; } // Puede ser null en creación
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
    }
}