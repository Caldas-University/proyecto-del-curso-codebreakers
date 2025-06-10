namespace SponsorshipManagement.Domain.Entities
{
    public class Sponsor
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!; // Ej: 'Empresa', 'Persona', etc.
        public bool HasDocumentation { get; set; } // True si entregó documentación válida
    }
}
