using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using PennyWise.Models;

namespace PennyWise.ViewModels
{
    public class TransactionCreateViewModel
    {
        [Required(ErrorMessage = "Açıklama zorunludur.")]
        [MaxLength(250)]
        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçiniz.")]
        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Tutar zorunludur.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Tutar 0'dan büyük olmalıdır.")]
        [Display(Name = "Tutar (₺)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "İşlem türü seçiniz.")]
        [Display(Name = "Tür")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "Tarih zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Tarih")]
        public DateTime TransactionDate { get; set; } = DateTime.Today;

        public SelectList? Categories { get; set; }
    }

    public class TransactionListViewModel
    {
        public List<Transaction> Transactions { get; set; } = new();

        // Filtreler
        public string?           SearchTerm   { get; set; }
        public TransactionType?  FilterType   { get; set; }
        public int?              FilterCatId  { get; set; }
        public SelectList?       Categories   { get; set; }
    }
}
