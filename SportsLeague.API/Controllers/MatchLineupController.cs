using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/match/{matchId}")]
    public class MatchLineupController : ControllerBase
    {
        private readonly IMatchLineupService _matchLineupService;
        private readonly IMapper _mapper;

        public MatchLineupController(
            IMatchLineupService matchLineupService,
            IMapper mapper)
        {
            _matchLineupService = matchLineupService;
            _mapper = mapper;
        }

        [HttpPost("lineups")]
        public async Task<ActionResult<MatchLineupDTO>> RegisterLineup(int matchId, CreateMatchLineupDTO dto)
        {
            try
            {
                var lineup = _mapper.Map<MatchLineup>(dto);
                var created = await _matchLineupService.RegisterLineupAsync(matchId, lineup);
                var lineups = await _matchLineupService.GetLineupByMatchAsync(matchId);
                var createdLineup = lineups.FirstOrDefault(l => l.Id == created.Id);
                return Ok(_mapper.Map<MatchLineupDTO>(createdLineup));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpGet("lineups")]
        public async Task<ActionResult<IEnumerable<MatchLineupDTO>>> GetLineups(int matchId)
        {
            try
            {
                var lineups = await _matchLineupService.GetLineupByMatchAsync(matchId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupDTO>>(lineups));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("lineups/team/{teamId}")]
        public async Task<ActionResult<IEnumerable<MatchLineupDTO>>> GetLineupsByTeam(int matchId, int teamId)
        {
            try
            {
                var lineups = await _matchLineupService.GetLineupByMatchAndTeamAsync(matchId, teamId);
                return Ok(_mapper.Map<IEnumerable<MatchLineupDTO>>(lineups));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("lineups/{lineupId}")]
        public async Task<ActionResult> DeleteLineup(int matchId, int lineupId)
        {
            try
            {
                await _matchLineupService.DeleteLineupAsync(lineupId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}