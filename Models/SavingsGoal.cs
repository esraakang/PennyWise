using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PennyWise.Models
{
    public class SavingsGoal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Display(Name = "Hedef Adı")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(10, double.MaxValue, ErrorMessage = "Hedef tutarı en az 10 ₺ olmalıdır.")]
        [Display(Name = "Hedef Tutar (₺)")]
        public decimal TargetAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        [Display(Name = "Biriken Tutar (₺)")]
        public decimal CurrentAmount { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public int ProgressPercent =>
            TargetAmount > 0 ? (int)Math.Min(100, Math.Round(CurrentAmount / TargetAmount * 100)) : 0;
    }
}
