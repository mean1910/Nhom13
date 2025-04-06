using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Nhom13.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required]
        [Display(Name = "Tên khách hàng")]
        public string CustomerName { get; set; } = null!;

        [Required]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; } = null!;

        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string CustomerEmail { get; set; } = null!;

        [Required]
        [Display(Name = "Địa chỉ")]
        public string CustomerAddress { get; set; } = null!;

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Pending";

        [Display(Name = "Chi tiết đơn hàng")]
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Người tạo")]
        public string? CreatedBy { get; set; }

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount => OrderDetails.Sum(d => d.Amount);
    }

    public class OrderDetail
    {
        [Required]
        [Display(Name = "Sản phẩm")]
        public string ProductId { get; set; } = null!;

        [Required]
        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Đơn giá")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal Amount => Quantity * UnitPrice;

        [BsonIgnore]
        public string? ProductName { get; set; }
    }
}
