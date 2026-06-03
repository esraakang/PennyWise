using System.ComponentModel.DataAnnotations;

namespace PennyWise.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; } = string.Empty;

        public string Icon { get; set; } = "fa-solid fa-tag";

        [MaxLength(20)]
        public string Color { get; set; } = "#10b981";

        // navigation
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
