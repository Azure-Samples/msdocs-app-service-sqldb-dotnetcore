using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models
{
    public class Degenerate
    {
        // Whenever you edit the model jjust do a search all.
        //  If you're adding a new field then just do a search all for an existing field and then just include your new field.
        // These properties should be updated to reference its own private field.
        public int Id { get; set; }

        public string? Username { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Cash Wallet")]
        public double CashWallet { get; set; }

        [Display(Name = "Number of Bets Placed")]
        public int BetsPlaced { get; set; }

        [Display(Name = "Number of Bets Won")]
        public int BetsWon { get; set; }

        [Display(Name = "Number of Bets Lost")]
        public int BetsLost => this.BetsPlaced - this.BetsWon;

        [DataType(DataType.Currency)]
        [Display(Name = "Net Cash Bet")]
        public double TotalWagesPlaced { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Profit Amount")]
        public double TotalWagesWon { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Money Lost")]
        public double TotalWagesLost => this.TotalWagesPlaced - this.TotalWagesWon;
    }
}
