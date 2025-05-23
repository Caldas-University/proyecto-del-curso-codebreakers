using System.Threading.Tasks;
using SponsorshipManagement.Application.Interfaces;
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
    }
}
