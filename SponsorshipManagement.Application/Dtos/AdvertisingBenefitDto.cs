namespace SponsorshipManagement.Application.Dtos
{
    public class AdvertisingBenefitDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BenefitType { get; set; } = string.Empty;
        public decimal EstimatedValue { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}