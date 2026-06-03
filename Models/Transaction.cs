using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PennyWise.Models
{
    public enum TransactionType
    {
        Gelir,
        Gider
    }

    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        [Display(Name = "Tutar (₺)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "İşlem Türü")]
        public TransactionType Type { get; set; }

        [Required]
        [Display(Name = "Tarih")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; }
    }
}
