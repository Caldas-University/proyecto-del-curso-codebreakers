using System;
using System.Collections.Generic;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// DTO para mensajes estructurados con opciones de renovación o finalización
    /// 🎯 CU-PA-05.02.2: Mensajes estructurados según rol
    /// </summary>
    public class ContractActionMessageDto
    {
        public string ContractId { get; set; } = string.Empty;
        public string ContractTitle { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public int DaysRemaining { get; set; }
        public string RecipientRole { get; set; } = string.Empty;
        public string RecipientEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string PerformanceSummary { get; set; } = string.Empty;
        public List<string> RenewalOptions { get; set; } = new List<string>();
        public List<string> EndingOptions { get; set; } = new List<string>();
        public string Closing { get; set; } = string.Empty;
    }
}