using System.ComponentModel.DataAnnotations;

namespace DotNetCoreSqlDb.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Category name is required")]
        public string Name { get; set; }
    }
}
