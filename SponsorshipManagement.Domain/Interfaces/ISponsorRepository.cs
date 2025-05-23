using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Domain.Interfaces
{
    public interface ISponsorRepository
    {
        Task<Sponsor?> GetByDocumentAsync(string documentNumber);
    }
}
