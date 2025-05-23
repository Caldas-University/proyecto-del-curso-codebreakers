using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    public interface ISponsorService
    {
        Task<bool> SponsorExistsAsync(string documentNumber);
    }
}
