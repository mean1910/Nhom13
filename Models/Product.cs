using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Mã sản phẩm không được để trống")]
        [Display(Name = "Mã sản phẩm")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; } = null!;

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
        [Display(Name = "Nhà cung cấp")]
        public string SupplierId { get; set; } = null!;

        [BsonIgnore]
        public string? SupplierName { get; set; }
    }
}
