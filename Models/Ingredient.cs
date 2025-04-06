using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Ingredient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nguyên liệu không được để trống")]
        [Display(Name = "Tên nguyên liệu")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        [Display(Name = "Đơn vị tính")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Giá không được để trống")]
        [Display(Name = "Giá")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public double Price { get; set; }

        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Display(Name = "Hình ảnh")]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
        [Display(Name = "Nhà cung cấp")]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual Supplier Supplier { get; set; }
    }
} 