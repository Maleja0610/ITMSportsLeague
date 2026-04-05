using SportsLeague.Domain.Enums;

namespace SportsLeague.Domain.Entities
{
    public class Sponsor : AuditBase
    {

        public string Name { get; set; } = null!; 

        public string ContactEmail { get; set; } = null!; 

        public string? Phone { get; set; } 

        public string? WebsiteUrl { get; set; } 

        public SponsorCategory Category { get; set; }

        //Navigation Properties
        public ICollection<TournamentSponsor> TournamentSponsors { get; set; } = new List<TournamentSponsor>();

    }

}