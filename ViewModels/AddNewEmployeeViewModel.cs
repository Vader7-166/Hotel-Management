using System.ComponentModel.DataAnnotations;
namespace Hotel_Management.ViewModels
{
    public class AddNewEmployeeViewModel
    {
        // --- Thuộc tính từ lớp Employee ---
        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Position is required.")]
        public string Position { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }

        [DataType(DataType.Date)] // Giúp hiển thị đúng kiểu date picker
        public DateOnly? BirthDate { get; set; }

        [DataType(DataType.Date)] // Giúp hiển thị đúng kiểu date picker
        public DateOnly? HireDate { get; set; }

        // --- Thuộc tính từ lớp Account ---
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }
    }
}
