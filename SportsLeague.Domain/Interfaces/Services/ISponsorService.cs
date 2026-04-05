using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);
        Task<IEnumerable<Sponsor>> GetSponsorsByCategoryAsync(SponsorCategory category);

        
        Task<TournamentSponsor> AddSponsorToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount);
        Task RemoveFromTournamentAsync(int sponsorId, int tournamentId);
        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId);
    }
}