using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Nhom13.Models
{
    public class Expense
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Loại chi phí")]
        public string Type { get; set; } = null!;

        [Required]
        [Display(Name = "Mô tả")]
        public string Description { get; set; } = null!;

        [Required]
        [Display(Name = "Số tiền")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Ngày chi")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        // Reference to Product if expense is for purchasing materials
        [Display(Name = "Sản phẩm liên quan")]
        public string? ProductId { get; set; }

        [Display(Name = "Người tạo")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
} 