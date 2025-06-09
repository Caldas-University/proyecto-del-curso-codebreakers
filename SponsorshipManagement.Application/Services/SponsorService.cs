using System.Threading.Tasks;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;

        public SponsorService(ISponsorRepository sponsorRepository)
        {
            _sponsorRepository = sponsorRepository;
        }

        public async Task<bool> SponsorExistsAsync(string documentNumber)
        {
            var sponsor = await _sponsorRepository.GetByDocumentAsync(documentNumber);
            return sponsor != null;
        }

        public async Task<SponsorDto?> GetSponsorByDocumentAsync(string documentNumber)
        {
            var sponsor = await _sponsorRepository.GetByDocumentAsync(documentNumber);
            if (sponsor == null)
            {
                return null;
            }
            // Aquí no usamos el Mapper de la API directamente, 
            // devolvemos un DTO desde la capa de aplicación.
            return new SponsorDto 
            { 
                Id = sponsor.Id, 
                DocumentNumber = sponsor.DocumentNumber, 
                Name = sponsor.Name 
            };
        }
    }
}
