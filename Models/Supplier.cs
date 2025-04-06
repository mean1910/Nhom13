using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class Supplier
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
        [Display(Name = "Tên nhà cung cấp")]
        public string Name { get; set; }

        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
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
        [Display(Name = "Từ khóa")]
        public string SearchString { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? IsActive { get; set; }

        [Display(Name = "Sắp xếp theo")]
        public string SortBy { get; set; }

        [Display(Name = "Thứ tự")]
        public string SortOrder { get; set; }
    }
} 