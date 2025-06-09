using System.Threading.Tasks;
using SponsorshipManagement.Application.Dtos;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface ISponsorService
    {
        Task<bool> SponsorExistsAsync(string documentNumber);
        Task<SponsorDto?> GetSponsorByDocumentAsync(string documentNumber);
    }
}
