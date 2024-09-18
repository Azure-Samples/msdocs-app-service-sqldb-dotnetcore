using Microsoft.AspNetCore.Mvc.Rendering;
using DotNetCoreSqlDb.Types;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models.WagerItems
{
    public class WagerItemViewModel
    {
        public SelectList? Usernames { get; set; }

        public int Id { get; set; }

        public string? Username { get; set; }

        public int MatchId { get; set; }

        [Display(Name = "Sport Type")]
        public SportTypes SportType { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Wager Amount")]
        public double WagerValue { get; set; }

        [Display(Name = "Type of Bet")]
        public BetTypes BetType { get; set; }

        public int Odds { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Bet Outcome")]
        public bool? Outcome { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Time of Bet")]
        public DateTime TimeOfBet { get; set; }
        public string? WagerTeamA { get; set; }
        public string? WagerTeamB { get; set; }
        public int WagerJuice { get; set; }
        public string? Description { get; set; }
    }
}
