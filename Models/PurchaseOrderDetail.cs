using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class PurchaseOrderDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PurchaseOrderId { get; set; }

        [ForeignKey("PurchaseOrderId")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public virtual Ingredient Ingredient { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
} 