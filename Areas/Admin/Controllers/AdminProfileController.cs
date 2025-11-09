using Hotel_Management.Filters;
using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")] 
    [StaffAuthorize]
    public class AdminProfileController : Controller
    {
        private readonly HotelManagementContext db;

        public AdminProfileController(HotelManagementContext context)
        {
            db = context;
        }


        [HttpGet]
        public async Task<IActionResult> ShowAdminProfile()
        {
            var loggedInUsername = HttpContext.Session.GetString("Username");

            // Kiểm tra xem admin đã đăng nhập chưa
            if (string.IsNullOrEmpty(loggedInUsername))
            {
                // Nếu chưa, đá về trang đăng nhập
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            // Truy vấn CSDL để lấy thông tin tài khoản
            var adminAccount = await db.Accounts
                .Include(a => a.Employee) 
                .FirstOrDefaultAsync(a => a.Username == loggedInUsername);

            // Kiểm tra xem có tìm thấy tài khoản không
            if (adminAccount == null)
            {
                // Nếu username trong session không có trong CSDL (lỗi lạ)
                return NotFound("Account not found.");
            }

            // Trả về View, truyền đối tượng 'adminAccount' cho View
            return View(adminAccount);
        }

        [HttpGet]
        public async Task<IActionResult> EditAdminProfile()
        {
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            var account = await db.Accounts
                .Include(a => a.Employee) 
                .FirstOrDefaultAsync(a => a.Username == username);

            if (account == null || account.Employee == null)
            {
                return NotFound("Không tìm thấy tài khoản hoặc hồ sơ nhân viên.");
            }

            // 3. ÁNH XẠ (MAP) từ Entity (Account/Employee) sang ViewModel
            var viewModel = new AdminProfileEditViewModel
            {
                Username = account.Username,
                EmployeeId = account.EmployeeId.Value,
                Email = account.Email,
                FullName = account.Employee.FullName,
                Phone = account.Employee.Phone,
                Address = account.Employee.Address,
                Gender = account.Employee.Gender,
                BirthDate = account.Employee.BirthDate,

                // Gán các trường ẩn
                Position = account.Employee.Position,
                Salary = account.Employee.Salary,
                HireDate = account.Employee.HireDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdminProfile(AdminProfileEditViewModel model)
        {
            // Kiểm tra Email trùng (với người KHÁC)
            if (await db.Accounts.AnyAsync(a => a.Email == model.Email && a.Username != model.Username))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng bởi một tài khoản khác.");
            }

            if (!ModelState.IsValid)
            {
                // Nếu lỗi, trả về View với model (đã chứa lỗi)
                return View(model);
            }

            // Nếu hợp lệ -> Lưu vào CSDL
            try
            {

                var accountToUpdate = await db.Accounts.FindAsync(model.Username);
                var employeeToUpdate = await db.Employees.FindAsync(model.EmployeeId);

                if (accountToUpdate == null || employeeToUpdate == null)
                {
                    return NotFound();
                }

                accountToUpdate.Email = model.Email;
                accountToUpdate.UpdatedAt = DateTime.Now;

                employeeToUpdate.FullName = model.FullName;
                employeeToUpdate.Phone = model.Phone;
                employeeToUpdate.Address = model.Address;
                employeeToUpdate.Gender = model.Gender;
                employeeToUpdate.BirthDate = model.BirthDate;
                employeeToUpdate.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();

                return RedirectToAction("ShowAdminProfile");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An internal server error occurred.");
                return View(model);
            }
        }
    }
}