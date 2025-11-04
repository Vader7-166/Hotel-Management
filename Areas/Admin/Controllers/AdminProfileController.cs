using Hotel_Management.Filters;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")] // Đảm bảo nó nằm trong Area Admin
    [AdminAuthorize]
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
            // 3. Lấy username của admin đang đăng nhập từ Session
            var loggedInUsername = HttpContext.Session.GetString("Username");

            // 4. Kiểm tra xem admin đã đăng nhập chưa
            if (string.IsNullOrEmpty(loggedInUsername))
            {
                // Nếu chưa, đá về trang đăng nhập
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            // 5. Truy vấn CSDL để lấy thông tin tài khoản
            var adminAccount = await db.Accounts
                .Include(a => a.Employee) // Lấy kèm thông tin Employee (nơi chứa FullName, Position, v.v.)
                .FirstOrDefaultAsync(a => a.Username == loggedInUsername);

            // 6. Kiểm tra xem có tìm thấy tài khoản không
            if (adminAccount == null)
            {
                // Nếu username trong session không có trong CSDL (lỗi lạ)
                return NotFound("Account not found.");
            }

            // 7. Trả về View, truyền đối tượng 'adminAccount' cho View
            return View(adminAccount);
        }

        [HttpGet]
        public async Task<IActionResult> EditAdminProfile()
        {
            // 1. Lấy username từ session
            var username = HttpContext.Session.GetString("Username");

            // 2. THÊM BẢO VỆ: Kiểm tra xem session có tồn tại không
            // (Đây là logic bạn đã thêm cho ShowAdminProfile)
            if (string.IsNullOrEmpty(username))
            {
                // Nếu không, chuyển hướng người dùng về trang đăng nhập
                // (Hãy đổi "SignIn" và "Account" cho đúng)
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            // 3. Code cũ của bạn: Tìm tài khoản
            var account = await db.Accounts.Include(a => a.Employee)
                                         .FirstOrDefaultAsync(a => a.Username == username);

            // 4. Kiểm tra (Bảo vệ thêm)
            if (account == null)
            {
                return NotFound("Account not found.");
            }

            // 5. Trả về View "Edit.cshtml" với Model hợp lệ
            return View(account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdminProfile(Account model)
        {
            // 1. Lấy username từ session để bảo mật
            var loggedInUsername = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(loggedInUsername))
            {
                // Nếu mất session, đá về trang đăng nhập
                return RedirectToAction("Login", "Account", new { Area = "Customer" });
            }

            // 2. Gỡ các validation không cần thiết (Giữ nguyên)
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Role");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("Employee.Position");
            ModelState.Remove("Employee.Salary");
            ModelState.Remove("Employee.HireDate");

            // 3. Kiểm tra các trường còn lại
            if (!ModelState.IsValid)
            {
                // SỬA Ở ĐÂY:
                // Nếu validation thất bại, trả về View ĐỂ HIỂN THỊ LỖI
                // thay vì trả về BadRequest
                return View(model);
            }

            try
            {
                // 4. Lấy bản ghi GỐC từ DB (Giữ nguyên)
                var accountToUpdate = await db.Accounts
                    .Include(a => a.Employee)
                    .FirstOrDefaultAsync(a => a.Username == loggedInUsername);

                if (accountToUpdate == null)
                {
                    return NotFound("Account not found.");
                }

                // 5. Cập nhật thông tin (Giữ nguyên)
                accountToUpdate.Email = model.Email;
                accountToUpdate.UpdatedAt = DateTime.Now;

                if (accountToUpdate.Employee != null && model.Employee != null)
                {
                    accountToUpdate.Employee.FullName = model.Employee.FullName;
                    accountToUpdate.Employee.Phone = model.Employee.Phone;
                    accountToUpdate.Employee.Address = model.Employee.Address;
                    accountToUpdate.Employee.Gender = model.Employee.Gender;
                    accountToUpdate.Employee.BirthDate = model.Employee.BirthDate;
                    accountToUpdate.Employee.UpdatedAt = DateTime.Now;
                }

                // 7. Lưu tất cả thay đổi (Giữ nguyên)
                await db.SaveChangesAsync();

                // 8. SỬA Ở ĐÂY:
                // Trả về lệnh CHUYỂN HƯỚNG thay vì JSON
                return RedirectToAction("ShowAdminProfile");
            }
            catch (Exception ex)
            {
                // SỬA Ở ĐÂY:
                // Ghi log và trả về View với thông báo lỗi
                // Ghi log lỗi (ex.Message)
                ModelState.AddModelError(string.Empty, "An internal server error occurred.");
                return View(model);
            }
        }
    }
}