using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models.WagerItems
{
    public class WagerItemEditViewModel
    {
        public WagerItem? Wager { get; set; }

        [Display(Name = "Username")]
        public string? WagerUsername { get; set; }
        public SelectList? Usernames { get; set; }
        public SelectList? SportTypeList { get; set; }
        public SelectList? BetTypesList { get; set; }
        public SelectList? NFLTeamList { get; set; }
        public SelectList? TrueFalseList { get; set; }
        public SelectList? WinLossOptionsList { get; set; }
    }
}
