using System;

namespace SponsorshipManagement.Application.DTOs
{
    public class VisibilityReportDto
    {
        public string SponsorDocumentNumber { get; set; } = "";
        public string EventId { get; set; } = "";
        public int TotalBenefits { get; set; }
        public int ExecutedBenefits { get; set; }
        public int PendingBenefits { get; set; }
        public int CancelledBenefits { get; set; }
        public int TotalEvidences { get; set; }
    }
}