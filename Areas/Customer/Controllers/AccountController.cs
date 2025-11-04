using Hotel_Management.Filters;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;


//đăng ký đăng nhập
namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    
    public class AccountController : Controller
    {
        private readonly HotelManagementContext _context;
        public AccountController(HotelManagementContext context)
        {
            _context = context;
        }
        [Route("Customer/Account/Index")]
        public IActionResult Index()
        {
            return View();
        }


        [Route("Customer/Account/Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("Customer/Account/Login")]
        public IActionResult Login(string Username, string Password)
        {
            // 🧩 1. Kiểm tra dữ liệu nhập vào
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ViewBag.Error = "Please enter all required information!";
                return View();
            }

            // 🔒 2. Hash mật khẩu để so sánh
            string hashedPassword = HashPassword(Password);

            // 🧠 3. Tìm tài khoản hợp lệ
            var account = _context.Accounts
                .FirstOrDefault(a => a.Username == Username && a.PasswordHash == hashedPassword && a.IsActive);

            if (account == null)
            {
                ViewBag.Error = "Invalid username or password!";
                return View();
            }
            //Luu thong tin tai khoan vao Session
            HttpContext.Session.SetString("Username", account.Username);
            HttpContext.Session.SetString("Role", account.Role);

            //Neu la khach hang thi luu thong tin khach hang
            if (account.Role == "Customer")
            {
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == account.CustomerId);
                if (customer != null)
                {
                    HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                    HttpContext.Session.SetString("FullName", customer.FullName);
                }
            }
            if (account.Role == "Admin" )
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            else if (account.Role == "Customer")
            {
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }
            else if (account.Role == "Receptionist")
            {
                return RedirectToAction("Index", "Booking", new { area = "Admin" });
            }
            else 
            {
                ViewBag.Error = "Invalid role! . Please contact support!";
                return View();
            }
 
        }




        [Route("Customer/Account/Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("Customer/Account/Register")]
        public IActionResult Register(string FullName, String UserName, string Email, string Password, string ConfirmPassword)
        {
            if (string.IsNullOrEmpty(FullName) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(ConfirmPassword))
            {
                ViewBag.Error = "Please enter all the required information !.";
                return View();
            }
            if (Password != ConfirmPassword)
            {
                ViewBag.Error = "Password and Confirm Password do not match !.";
                return View();
            }
            var existingAccount = _context.Accounts.FirstOrDefault(a => a.Username == UserName || a.Email == Email);
            if (existingAccount != null)
            {
                ViewBag.Error = "Username or Email already exists !.";
                return View();
            }
            var customer = new Hotel_Management.Models.Customer
            {
                FullName = FullName,
                Email = Email,
                CreatedAt = DateTime.Now,

            };
            _context.Customers.Add(customer);
            _context.SaveChanges();
            var account = new Account
            {
                Username = UserName,
                PasswordHash = HashPassword(Password),
                Email = Email,
                CustomerId = customer.CustomerId,
                Role = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();
            return RedirectToAction("Login", "Account", new { area = "Customer" });
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
        [Route("Customer/Account/Logout")]
        public IActionResult Logout()
        {
            //xóa session
            HttpContext.Session.Clear();
            //quay về trang chủ
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }
    }
}
