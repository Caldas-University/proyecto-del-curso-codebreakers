namespace SponsorshipManagement.Domain.Entities
{
    public class Metric
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string MetricType { get; set; } = string.Empty; // Alcance, Interaccion, Comparativa
        public string Category { get; set; } = string.Empty; // Social, Web, Evento, General
        public string Unit { get; set; } = string.Empty; // Views, Clicks, Personas, Porcentaje
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }
    }
}