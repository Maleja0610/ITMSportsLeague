using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ISponsorRepository : IGenericRepository<Sponsor>
{
    Task<Sponsor?> GetByNameAsync(string name);
    Task<IEnumerable<Sponsor>> GetByCategoryAsync(SponsorCategory category);

}