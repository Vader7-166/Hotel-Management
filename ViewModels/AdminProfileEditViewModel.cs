using System;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.ViewModels
{
    public class AdminProfileEditViewModel
    {
        // === Thông tin ẩn (dùng để xác định bản ghi) ===
        [Required]
        public string Username { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        // === Thông tin Account (được phép sửa) ===
        [Required(ErrorMessage = "Email không thể bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không đúng cú pháp")]
        public string Email { get; set; }

        // === Thông tin Employee (được phép sửa) ===
        [Required(ErrorMessage = "Họ tên không thể bỏ trống")]
        public string FullName { get; set; }

        [RegularExpression(@"^(0[0-9]{9})$", ErrorMessage = "Số điện thoại phải là 10 số, bắt đầu bằng 0.")]
        public string? Phone { get; set; }

        public string? Address { get; set; }
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateOnly? BirthDate { get; set; }

        // --- Các trường ẩn, không sửa nhưng cần giữ giá trị ---
        // (Chúng ta thêm các trường này vào ViewModel để giữ logic của bạn
        // là không sửa Vị trí và Lương trong trang Profile)
        public string? Position { get; set; }
        public decimal? Salary { get; set; }
        public DateOnly? HireDate { get; set; }
    }
}