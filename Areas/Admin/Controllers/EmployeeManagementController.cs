using Microsoft.AspNetCore.Mvc;
using Hotel_Management.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

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

        // Action chính để hiển thị trang quản lý
        [Route("Admin/EmployeeManagement")]
        public async Task<IActionResult> EmployeeAccountList()
        {
            // Lấy danh sách các tài khoản
            var accountsQuery = db.Accounts
                .Include(a => a.Customer) //Nạp cả dữ liệu Customer có liên quan đến Account
                .Include(a => a.Employee) //Nạp cả dữ liệu Employee có liên quan đến Account
                // DÒNG QUAN TRỌNG: Lọc để chỉ lấy các tài khoản có vai trò Admin hoặc Employee
                .Where(a => a.Role == "Admin" || a.Role == "Employee"); //chú ý chỉ nạp những 
            //kết quả sau cùng là một truy vấn SQL

            var accounts = await accountsQuery.ToListAsync(); //thực thi truy vấn trong SQL và nạp vào bộ nhớ dưới dạng list

            // Bạn có thể thêm logic phân trang ở đây nếu muốn, hoặc trả về toàn bộ
            return View(accounts);
        }

        [HttpGet]
        public IActionResult AddnewEmployeesAccount()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AccountTableAndPagination(int pageIndex = 1, string searchQuery = "", string role = "")
        {
            // Bắt đầu truy vấn với bộ lọc gốc
            IQueryable<Account> query = db.Accounts
                                          .Include(a => a.Customer)
                                          .Include(a => a.Employee)
                                          // DÒNG QUAN TRỌNG: Luôn bắt đầu bằng việc chỉ lấy Admin và Employee
                                          .Where(a => a.Role == "Admin" || a.Role == "Employee");

            // 1. ÁP DỤNG BỘ LỌC TÌM KIẾM
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(a => a.Username.Contains(searchQuery) || a.Email.Contains(searchQuery));
            }

            // 2. ÁP DỤNG BỘ LỌC VAI TRÒ (nếu người dùng chọn)
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(a => a.Role == role);
            }

            // 3. PHÂN TRANG (logic giữ nguyên)
            int pageSize = 10;
            var pagedResult = await query.OrderBy(a => a.Role).ThenBy(a => a.Username)
                                         .Skip((pageIndex - 1) * pageSize)
                                         .Take(pageSize)
                                         .ToListAsync();

            int totalFilteredAccounts = await query.CountAsync();
            ViewBag.PageNum = (int)Math.Ceiling((double)totalFilteredAccounts / pageSize);
            ViewBag.CurrentPage = pageIndex;
            ViewBag.TotalEntries = totalFilteredAccounts;

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