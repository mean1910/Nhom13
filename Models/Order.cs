using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Mã đơn hàng không được để trống")]
        [Display(Name = "Mã đơn hàng")]
        public string Code { get; set; } = null!;

        [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
        [Display(Name = "Nhà cung cấp")]
        public string SupplierId { get; set; } = null!;

        [BsonIgnore]
        public string? SupplierName { get; set; }

        [Display(Name = "Ngày đặt hàng")]
        public DateTime OrderDate { get; set; }

        [Display(Name = "Ngày giao hàng dự kiến")]
        public DateTime? ExpectedDeliveryDate { get; set; }

        [Display(Name = "Ngày giao hàng thực tế")]
        public DateTime? ActualDeliveryDate { get; set; }

        [Display(Name = "Trạng thái")]
        public string Status { get; set; } = "Chờ duyệt";

        [Display(Name = "Tổng tiền")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Ghi chú")]
        public string? Notes { get; set; }

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

    public class OrderDetail
    {
        [Required(ErrorMessage = "Sản phẩm không được để trống")]
        [Display(Name = "Sản phẩm")]
        public string ProductId { get; set; } = null!;

        [BsonIgnore]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Đơn giá không được để trống")]
        [Display(Name = "Đơn giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal Amount => Quantity * UnitPrice;
    }
}
