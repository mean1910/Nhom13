using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class WarehouseIngredient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IngredientId { get; set; }

        [ForeignKey("IngredientId")]
        public virtual Ingredient Ingredient { get; set; }

        [Required]
        [Display(Name = "Số lượng")]
        [Range(0, double.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public double Quantity { get; set; }

        [Display(Name = "Mức tồn kho tối thiểu")]
        [Range(0, double.MaxValue, ErrorMessage = "Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0")]
        public double MinimumQuantity { get; set; }
    }
} 