using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class EmployeeManagementController : Controller
    {
        private readonly HotelManagementContext db;

        public EmployeeManagementController(HotelManagementContext context)
        {
            db = context;
        }

        [Route("Admin/EmployeeManagement")]
        public async Task<IActionResult> EmployeeAccountList()
        {
            // Lấy tên đăng nhập của người dùng hiện tại từ Session
            var loggedInUsername = HttpContext.Session.GetString("Username");

            // Bắt đầu truy vấn
            IQueryable<Account> query = db.Accounts
                .Include(a => a.Customer)
                .Include(a => a.Employee)
                .Where(a => a.Role == "Admin" || a.Role == "Employee");

            // Nếu đã lấy được tên đăng nhập, thêm điều kiện để loại trừ chính tài khoản đó
            if (!string.IsNullOrEmpty(loggedInUsername))
            {
                query = query.Where(a => a.Username != loggedInUsername);
            }

            var accounts = await query.ToListAsync();

            return View(accounts);
        }

        [HttpGet]
        public IActionResult AddNewEmployeeAccount()
        {
            // Tạo một ViewModel rỗng và truyền nó vào View
            var model = new AddNewEmployeeViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNewEmployeeAccount(AddNewEmployeeViewModel model)
        {
            // 1. Kiểm tra validation từ ViewModel
            if (ModelState.IsValid)
            {
                // 2. Kiểm tra nghiệp vụ (Username/Email có bị trùng không)
                if (await db.Accounts.AnyAsync(a => a.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "This username is already taken.");
                }
                if (await db.Accounts.AnyAsync(a => a.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "This email is already in use.");
                }

                // Nếu vẫn còn lỗi sau khi kiểm tra, trả về View
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // 3. Sử dụng Transaction để đảm bảo an toàn
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // 3a. Tạo và lưu Employee
                        var newEmployee = new Employee
                        {
                            FullName = model.FullName,
                            Position = model.Position,
                            Email = model.Email,
                            CreatedAt = DateTime.Now
                        };
                        db.Employees.Add(newEmployee);
                        await db.SaveChangesAsync();

                        // 3b. Tạo và lưu Account, liên kết với Employee vừa tạo
                        var newAccount = new Account
                        {
                            Username = model.Username,
                            PasswordHash = HashPassword(model.Password),
                            Email = model.Email,
                            Role = model.Role,
                            EmployeeId = newEmployee.EmployeeId, // Liên kết khóa ngoại
                            IsActive = true,
                            CreatedAt = DateTime.Now
                        };
                        db.Accounts.Add(newAccount);
                        await db.SaveChangesAsync();

                        // 3c. Commit transaction
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = "Employee account created successfully!";
                        return RedirectToAction("EmployeeAccountList");
                    }
                    catch (Exception)
                    {
                        // 3d. Nếu lỗi, rollback transaction
                        await transaction.RollbackAsync();
                        ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                    }
                }
            }

            // 4. Nếu ModelState không hợp lệ ngay từ đầu, trả về View với các lỗi
            return View(model);
        }

        // Hàm HashPassword của bạn
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

        [HttpGet]
        public async Task<IActionResult> AccountTableAndPagination(int pageIndex = 1, string searchQuery = "", string role = "")
        {
            // Lấy tên đăng nhập của người dùng hiện tại từ Session
            var loggedInUsername = HttpContext.Session.GetString("Username");

            // Bắt đầu truy vấn với bộ lọc gốc
            IQueryable<Account> query = db.Accounts
                                          .Include(a => a.Customer)
                                          .Include(a => a.Employee)
                                          .Where(a => a.Role == "Admin" || a.Role == "Employee");

            // Nếu đã lấy được tên đăng nhập, thêm điều kiện để loại trừ chính tài khoản đó
            if (!string.IsNullOrEmpty(loggedInUsername))
            {
                query = query.Where(a => a.Username != loggedInUsername);
            }

            // --- Các bộ lọc khác giữ nguyên ---
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(a => a.Username.Contains(searchQuery) || a.Email.Contains(searchQuery));
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(a => a.Role == role);
            }

            // --- Logic phân trang giữ nguyên ---
            int pageSize = 10;
            var pagedResult = await query.OrderBy(a => a.Role).ThenBy(a => a.Username)
                                         .Skip((pageIndex - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            // ... (Phần ViewBag giữ nguyên) ...

            return PartialView("_AccountListPartial", pagedResult);
        }


        // Action xử lý việc xóa (sẽ được gọi bằng AJAX POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string username) // <-- Đổi tên ở đây
        {
            try
            {
                var accountToDelete = await db.Accounts.FindAsync(username); //tìm trong Account có dòng nào có giá trị username
                                                                             
                if (accountToDelete == null)
                {
                    return Json(new { success = false, message = "Account not found." }); //không tìm thấy thì sẽ trả về json false
                }
                db.Accounts.Remove(accountToDelete); //ngược lại tìm thấy thì xóa
                await db.SaveChangesAsync(); //lưu tình trạng hiện tại của db
                return Json(new { success = true, message = "Account has been deleted successfully." }); //trả về json true
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred." }); //nếu không mở được db thì báo lỗi An erro Occured
            }
        }


        //public


        // Action Details(id), Edit(id) [HttpGet], Edit(id) [HttpPost] ở đây
        public async Task<IActionResult> Details(string username) 
        {
            var account = await db.Accounts
                                  .Include(a => a.Customer)
                                  .Include(a => a.Employee)
                                  .FirstOrDefaultAsync(a => a.Username == username); //tìm các record thỏa mãn
            if (account == null) return NotFound();
            return PartialView("_AccountDetailsPartitial", account); //trả về view cùng với strongly model
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string username)
        {
            var account = await db.Accounts.FindAsync(username); 
            if (account == null) return NotFound();
            return PartialView("_AccountEditPartitial", account);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Account accountViewModel)
        {
            if (string.IsNullOrEmpty(accountViewModel.Username))
            {
                return BadRequest();
            }

            ModelState.Remove("PasswordHash");
            ModelState.Remove("CreatedAt");

            if (ModelState.IsValid)
            {
                try
                {
                    var entityToUpdate = await db.Accounts.FindAsync(accountViewModel.Username);

                    if (entityToUpdate == null)
                    {
                        return NotFound();
                    }

                    db.Entry(entityToUpdate).CurrentValues.SetValues(accountViewModel);

                    // === DÒNG QUAN TRỌNG ĐỂ SỬA LỖI ===
                    // Nói với EF Core rằng không được thay đổi thuộc tính PasswordHash
                    db.Entry(entityToUpdate).Property(x => x.PasswordHash).IsModified = false;

                    entityToUpdate.UpdatedAt = DateTime.Now;

                    await db.SaveChangesAsync();

                    return Json(new { success = true, message = "Account updated successfully!" });
                }
                catch (DbUpdateException)
                {
                    return Json(new { success = false, message = "An error occurred while saving. Please try again." });
                }
            }

            return PartialView("_AccountEditPartial", accountViewModel);
        }
    }
}