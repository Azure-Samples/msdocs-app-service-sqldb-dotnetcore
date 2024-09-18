using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models.WagerItems
{
    public class WagerItemIndexViewModel
    {
        public int UserId { get; set; }
        public List<WagerItemViewModel>? Wagers { get; set; }
        public SelectList? UsernameList { get; set; }
    }
}
