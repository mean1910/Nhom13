using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public enum OrderStatus
    {
        Pending,    // Chờ duyệt
        Shipping,   // Đang giao
        Completed   // Hoàn thành
    }

    public class PurchaseOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public string? Notes { get; set; }

        // Collection of order details
        public virtual ICollection<PurchaseOrderDetail> OrderDetails { get; set; }

        // Tự động set OrderDate và khởi tạo OrderDetails khi tạo đơn hàng mới
        public PurchaseOrder()
        {
            OrderDate = DateTime.Now;
            Status = OrderStatus.Pending;
            OrderDetails = new List<PurchaseOrderDetail>();
        }
    }
} 