using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models.ViewModels.Customer
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)] // Giúp hiển thị input dạng password (dấu chấm)
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        // Thêm nếu bạn muốn có chức năng "Remember Me"
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
