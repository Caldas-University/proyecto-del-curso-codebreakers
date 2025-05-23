using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    public class SponsorRepository : ISponsorRepository
    {
        private readonly string _jsonFilePath = "D:/Academico/UNIVERSIDAD/8. octavo semestre/Ingeniería de software 2/Proyecto/Proyecto/SponsorshipManagement.Infrastructure/Data/sponsors.json";

        private async Task<List<Sponsor>> LoadSponsorsAsync()
        {
            if (!File.Exists(_jsonFilePath))
                return new List<Sponsor>();

            using FileStream fs = File.OpenRead(_jsonFilePath);
            var sponsors = await JsonSerializer.DeserializeAsync<List<Sponsor>>(fs);
            return sponsors ?? new List<Sponsor>();
        }

        public async Task<Sponsor?> GetByDocumentAsync(string documentNumber)
        {
            var sponsors = await LoadSponsorsAsync();
            return sponsors.FirstOrDefault(s => s.DocumentNumber == documentNumber);
        }
    }
}
