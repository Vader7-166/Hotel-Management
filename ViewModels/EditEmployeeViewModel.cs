using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.ViewModels
{
    public class EditEmployeeViewModel
    {
        // Từ Account
        [Required]
        public string Username { get; set; } // Dùng làm ID

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }

        public bool IsActive { get; set; }

        // Từ Employee
        public int EmployeeId { get; set; } // Cần cho việc cập nhật

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Position { get; set; }

        public decimal? Salary { get; set; }
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^(0[0-9]{9})$", ErrorMessage = "Số điện thoại phải là 10 số, bắt đầu bằng 0 (ví dụ: 0912345678).")]
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public DateOnly? BirthDate { get; set; }
    }
}
