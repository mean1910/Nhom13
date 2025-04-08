using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Loại chi tiêu")]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Số tiền")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Ngày chi tiêu")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }
    }
} 