using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Supplier
    {
        public Supplier()
        {
            Ingredients = new HashSet<Ingredient>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [Display(Name = "Tên nhà cung cấp")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Địa chỉ không được để trống")]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Trạng thái")]
        public bool IsActive { get; set; } = true;

        // Navigation property
        public virtual ICollection<Ingredient> Ingredients { get; set; }
    }

    public class SupplierSearchViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string SearchString { get; set; }
    }
} 