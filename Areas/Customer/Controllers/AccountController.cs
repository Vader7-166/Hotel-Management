using Hotel_Management.Filters;
using Hotel_Management.Models;
using Hotel_Management.Models.ViewModels.Customer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


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
            // truyền một instance mới của ViewModel vào View
            return View(new LoginViewModel());
        }

        [HttpPost]
        [Route("Customer/Account/Login")]
        public IActionResult Login(LoginViewModel model)
        {
            // 🧩 1. Kiểm tra dữ liệu nhập vào
            if (!ModelState.IsValid)
            {
                //Nếu có lỗi [Required],trả về View với model để hiển thị lỗi
                return View(model);
            }

            // 🔒 2. Hash mật khẩu để so sánh
            string hashedPassword = HashPassword(model.Password);

            // 🧠 3. Tìm tài khoản hợp lệ
            var account = _context.Accounts
                .FirstOrDefault(a => a.Username == model.Username && a.PasswordHash == hashedPassword && a.IsActive);

            if (account == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.Please try again.");
                return View(model);
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
                // TƯƠNG TỰ: Dùng ModelState.AddModelError
                ModelState.AddModelError(string.Empty, "Invalid role! . Please contact support!");
                return View(model);
            }
 
        }




        [Route("Customer/Account/Register")]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [Route("Customer/Account/Register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
               
                return View(model);
            }
            
            var existingAccount = _context.Accounts.FirstOrDefault(a => a.Username == model.Username || a.Email == model.Email);
            if (existingAccount != null)
            {
                ModelState.AddModelError(string.Empty, "Username or Email already exists. Please choose another.");
               // Trả lại View với model để hiển thị lỗi qua Validation Summary
                return View(model);
            }
            // 3. Tạo Khách hàng (Customer)
            var customer = new Hotel_Management.Models.Customer
            {
                FullName = model.FullName,
                Email = model.Email,
                CreatedAt = DateTime.Now,
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();
            var account = new Account
            {
                Username = model.Username,
                PasswordHash = HashPassword(model.Password), // Băm mật khẩu từ model
                Email = model.Email,
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
