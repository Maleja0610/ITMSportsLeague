using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Enums;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ITournamentRepository tournamentRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);

            var sponsor = await _sponsorRepository.GetByIdAsync(id);

            if (sponsor == null)
            {
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            }

            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            _logger.LogInformation("Creating sponsor: {Name}", sponsor.Name);

            var existing = await _sponsorRepository.GetByNameAsync(sponsor.Name);

            if (existing != null)
            {
                throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'");
            }

            sponsor.CreatedAt = DateTime.UtcNow;

            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            var sponsorWithSameName = await _sponsorRepository.GetByNameAsync(sponsor.Name);

            if (sponsorWithSameName != null && sponsorWithSameName.Id != id)
            {
                throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);

            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);

            await _sponsorRepository.DeleteAsync(id);
        }


        public async Task<IEnumerable<Sponsor>> GetSponsorsByCategoryAsync(SponsorCategory category)
        {
            _logger.LogInformation("Retrieving sponsors by category: {Category}", category);

            if (!Enum.IsDefined(typeof(SponsorCategory), category))
                throw new ArgumentException("Categoría no existente");

            return await _sponsorRepository.GetByCategoryAsync(category);
        }

        public async Task<TournamentSponsor> AddSponsorToTournamentAsync(int sponsorId, int tournamentId, decimal contractAmount)
        {
            _logger.LogInformation("Assigning sponsor {SponsorId} to tournament {TournamentId}", sponsorId, tournamentId);

            // Sponsor exists
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);

            if (!sponsorExists)
                throw new KeyNotFoundException(
                    $"No se encontró el sponsor con ID {sponsorId}");

            // tournament exist
            var tournamentExists = await _tournamentRepository.ExistsAsync(tournamentId);

            if (!tournamentExists)
                throw new KeyNotFoundException(
                    $"No se encontró el torneo con ID {tournamentId}");

            // do not allow duplicates
            var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "Este sponsor ya está inscrito en el torneo");
            }

            // amount greater than 0
            if (contractAmount <= 0)
                throw new InvalidOperationException("El monto del contrato debe ser mayor a 0");

            var relation = new TournamentSponsor
            {
                SponsorId = sponsorId,
                TournamentId = tournamentId,
                ContractAmount = contractAmount,
                JoinedAt = DateTime.UtcNow
            };

            return await _tournamentSponsorRepository.CreateAsync(relation);
        }

        public async Task RemoveFromTournamentAsync(int sponsorId, int tournamentId)
        {
            _logger.LogInformation("Removing sponsor {SponsorId} from tournament {TournamentId}", sponsorId, tournamentId);

            // validate if the relation exists
            var tournamentSponsorExists= await _tournamentSponsorRepository
                .GetByTournamentAndSponsorAsync(tournamentId, sponsorId);

            if (tournamentSponsorExists == null)
                throw new KeyNotFoundException("Este sponsor no existe en este torneo");

            await _tournamentSponsorRepository.DeleteAsync(tournamentSponsorExists.Id);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            _logger.LogInformation("Retrieving tournaments for sponsor {SponsorId}", sponsorId);

            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

            if (sponsor == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

            return await _tournamentSponsorRepository.GetBySponsorAsync(sponsorId);
        }
    }
}