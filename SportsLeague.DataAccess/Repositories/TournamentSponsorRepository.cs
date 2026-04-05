using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Repositories
{
    public class TournamentSponsorRepository : GenericRepository<TournamentSponsor>, ITournamentSponsorRepository
    {
        public TournamentSponsorRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int SponsorId)
        {
            return await _dbSet
                .Where(tt => tt.TournamentId == tournamentId && tt.SponsorId == SponsorId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(int tournamentId)
        {
            return await _dbSet
                .Where(tt => tt.TournamentId == tournamentId)
                .Include(tt => tt.Sponsor)
                .ToListAsync();
        }

        public async Task<IEnumerable<TournamentSponsor>> GetBySponsorAsync(int sponsorId)
        {
            return await _dbSet
                .Where(ts => ts.SponsorId == sponsorId)
                .Include(ts => ts.Tournament)
                .ToListAsync();
        }
    }
}