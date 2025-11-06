using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Hotel_Management.Filters;
namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyAuthorize]
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
                .Where(a => a.Role == "Admin" || a.Role == "Employee" || a.Role == "Receptionist");

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
                            HireDate = model.HireDate,
                            CreatedAt = DateTime.Now,
                            Salary = model.Salary,

                            Phone = model.Phone,
                            Address = model.Address,
                            Gender = model.Gender,
                            BirthDate = model.BirthDate
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

        // Hàm HashPassword
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
        public async Task<IActionResult> AccountTableAndPagination(int pageIndex = 1, string searchQuery = "", string role = "", string position = "" )
        {
            // Lấy tên đăng nhập của người dùng hiện tại từ Session
            var loggedInUsername = HttpContext.Session.GetString("Username");

            // Bắt đầu truy vấn (Giữ nguyên)
            IQueryable<Account> query = db.Accounts
                                          .Include(a => a.Customer)
                                          .Include(a => a.Employee)
                                          .Where(a => a.Role == "Admin" || a.Role == "Employee" || a.Role == "Receptionist");

            // Lọc loại trừ
            if (!string.IsNullOrEmpty(loggedInUsername))
            {
                query = query.Where(a => a.Username != loggedInUsername);
            }

            // Các bộ lọc
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(a => a.Username.Contains(searchQuery) || a.Email.Contains(searchQuery));
            }
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(a => a.Role == role);
            }
            if (!string.IsNullOrEmpty(position))
            {
                // Lọc dựa trên cột Position trong bảng Employee liên quan
                query = query.Where(a => a.Employee.Position == position);
            }

            // 1. Đếm tổng số record THỎA MÃN ĐIỀU KIỆN
            var totalItems = await query.CountAsync();

            // 2. Tính toán số trang
            int pageSize = 7; // mặc định tối đa 3 record trên 1 page
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Đảm bảo luôn là "Page 1 / 1" ngay cả khi không có record nào
            if (totalPages == 0) totalPages = 1;

            // 3. Đưa thông tin vào ViewBag để View có thể đọc
            ViewBag.currentPage = pageIndex;
            ViewBag.pageNum = totalPages;


            // Logic lấy dữ liệu phân trang
            var pagedResult = await query.OrderBy(a => a.Role).ThenBy(a => a.Username)
                                         .Skip((pageIndex - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            return PartialView("_AccountListPartial", pagedResult);
        }


        // Action xử lý việc xóa (sẽ được gọi bằng AJAX POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id) // <-- id ở đây là username
        {
            try
            {
                var accountToDelete = await db.Accounts.FindAsync(id); //tìm trong Account có dòng nào có giá trị username
                                                                             
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
            // DÙNG .Include() ĐỂ TẢI EMPLOYEE
            var account = await db.Accounts
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.Username == username);

            if (account == null) return NotFound();

            // Ánh xạ (Map) từ Entity -> ViewModel
            var viewModel = new EditEmployeeViewModel
            {
                Username = account.Username,
                Email = account.Email,
                Role = account.Role,
                IsActive = account.IsActive,
                EmployeeId = account.EmployeeId ?? 0, // Lấy EmployeeId

                // Gán giá trị Employee (nếu có)
                FullName = account.Employee?.FullName,
                Position = account.Employee?.Position,
                Salary = account.Employee?.Salary,
                Phone = account.Employee?.Phone,
                Address = account.Employee?.Address,
                Gender = account.Employee?.Gender,
                BirthDate = account.Employee?.BirthDate
            };

            return PartialView("_AccountEditPartitial", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditEmployeeViewModel model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy cả 2 bản ghi gốc
                    var accountToUpdate = await db.Accounts.FindAsync(model.Username);
                    var employeeToUpdate = await db.Employees.FindAsync(model.EmployeeId);

                    if (accountToUpdate == null || employeeToUpdate == null)
                    {
                        return NotFound();
                    }

                    // 1. Cập nhật Account
                    accountToUpdate.Email = model.Email;
                    accountToUpdate.Role = model.Role;
                    accountToUpdate.IsActive = model.IsActive;
                    accountToUpdate.UpdatedAt = DateTime.Now;

                    // 2. Cập nhật Employee
                    employeeToUpdate.FullName = model.FullName;
                    employeeToUpdate.Position = model.Position;
                    employeeToUpdate.Salary = model.Salary;
                    employeeToUpdate.Phone = model.Phone;
                    employeeToUpdate.Address = model.Address;
                    employeeToUpdate.Gender = model.Gender;
                    employeeToUpdate.BirthDate = model.BirthDate;
                    employeeToUpdate.UpdatedAt = DateTime.Now;

                    await db.SaveChangesAsync();
                    return Json(new { success = true, message = "Account updated successfully!" });
                }
                catch (DbUpdateException)
                {
                    return Json(new { success = false, message = "Error saving data." });
                }
            }

            // Nếu model không hợp lệ
            return PartialView("_AccountEditPartitial", model);
        }

       
        [HttpGet]
        public IActionResult GetPositionsByRole(string role)
        {
            var adminPositions = new List<string>
            {
                "Quản lý Khách sạn",
                "Quản lý Vận hành",
                "Quản trị viên IT"
            };

            var employeePositions = new List<string>
            {
                "Lao công",
                "Hỗ trợ",
                "Phục vụ",
                "Bảo vệ"
            };

            var receptionistPositions = new List<string>
            {
                "Nhân viên tiền sảnh",
                "Nhân viên đặt phòng"
            };

            List<string> positionsToSend;

            // 2. Quyết định danh sách nào sẽ gửi đi
            if (role == "Admin")
            {
                positionsToSend = adminPositions;
            }
            else if (role == "Employee")
            {
                positionsToSend = employeePositions;
            }
            else if (role == "Receptionist")
            {
                positionsToSend = receptionistPositions;
            }
            else // Trường hợp role == "" (Tức là chọn "All Roles" trong bộ lọc)
            {
                // Gộp cả 2 danh sách lại
                positionsToSend = adminPositions.Concat(employeePositions).ToList();
            }

            // 3. Sắp xếp và trả về JSON
            positionsToSend.Sort(); // Sắp xếp theo thứ tự ABC
            return Json(positionsToSend);
        }
    }
}