using DotNetCoreSqlDb.Types;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models.WagerItems
{
    public class WagerItem
    {
        /*
            - Username : to identify who put in the bet(can be multiple)
            - Wager Value : how much each user is betting(separate by commas)
            - Type of bet: MoneyLine, Over/Under, Spread
            - Current odds : +/- [Value]
                - For Over/Under +/- represents which bet you are taking respectively
            - Sports Category : NFL(eventually we will add more sports)
            - HeadToHead : The Two Teams Competing
            - Date : Date of the match
        */
        public int Id { get; set; }

        public int UserId { get; set; }

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
