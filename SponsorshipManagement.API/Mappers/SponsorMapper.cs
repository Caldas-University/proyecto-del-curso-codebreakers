using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.API.Mappers
{
    public static class SponsorMapper
    {
        public static SponsorDto ToDto(Sponsor sponsor)
        {
            return new SponsorDto
            {
                Id = sponsor.Id,
                DocumentNumber = sponsor.DocumentNumber,
                Name = sponsor.Name
            };
        }

        public static Sponsor ToEntity(SponsorDto sponsorDto)
        {
            return new Sponsor
            {
                Id = sponsorDto.Id,
                DocumentNumber = sponsorDto.DocumentNumber,
                Name = sponsorDto.Name
            };
        }
    }
}
