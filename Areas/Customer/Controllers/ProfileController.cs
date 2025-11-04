using Hotel_Management.Filters;
using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Cần thêm

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Route("Customer/[controller]/[action]")] // Giữ route này
    [CustomerAuthorize] // Bảo vệ tất cả action
    public class ProfileController : Controller
    {
        private readonly HotelManagementContext _context;

        public ProfileController(HotelManagementContext context)
        {
            _context = context;
        }

        // =======================================================
        // 1. INDEX (GET) 
        // =======================================================
        [Route("Customer/Profile/Index")] // Giữ route tường minh
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            // Không cần kiểm tra customerId == null, vì [CustomerAuthorize] đã làm

            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return NotFound("Customer profile not found.");
            }

            // Lấy Username từ Session để hiển thị
            ViewBag.Username = HttpContext.Session.GetString("Username");

            // Truyền cả model Customer sang View
            return View(customer);
        }

        // =======================================================
        // 2. EDIT (GET) 
        // =======================================================
        [HttpGet]
        public IActionResult Edit()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
            if (customer == null)
            {
                return NotFound("Customer profile not found.");
            }
            return View(customer);
        }

        // =======================================================
        // 3. EDIT (POST) 
        // =======================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Hotel_Management.Models.Customer model)
        {
            // 1. Lấy CustomerId AN TOÀN từ SESSION
            var sessionCustomerId = HttpContext.Session.GetInt32("CustomerId");

            // 2. Kiểm tra xem ID từ form có bị giả mạo không
            if (sessionCustomerId != model.CustomerId)
            {
                TempData["Error"] = "Unauthorized action detected.";
                return RedirectToAction("Index");
            }

            // 3. Xóa lỗi validation của các thuộc tính không liên quan
            ModelState.Remove("Bookings");
            ModelState.Remove("Accounts");

            if (!ModelState.IsValid)
            {
                return View(model); // Trả về form với lỗi
            }

            // 4. Lấy Customer GỐC từ DB (dùng ID session)
            var customerToUpdate = await _context.Customers
                                        .FirstOrDefaultAsync(c => c.CustomerId == sessionCustomerId);

            if (customerToUpdate == null)
            {
                return NotFound("Customer not found.");
            }

            // 5. Xử lý logic cập nhật EMAIL (Phần quan trọng)
            if (customerToUpdate.Email != model.Email)
            {
                // Email đã thay đổi -> Kiểm tra tính duy nhất
                bool emailExists = await _context.Accounts
                    .AnyAsync(a => a.Email == model.Email && a.CustomerId != sessionCustomerId);

                if (emailExists)
                {
                    TempData["Error"] = "This email address is already in use by another account.";
                    return View(model);
                }

                // Cập nhật email cho cả Customer
                customerToUpdate.Email = model.Email;

                // Cập nhật email cho cả Account
                var accountToUpdate = await _context.Accounts
                                          .FirstOrDefaultAsync(a => a.CustomerId == sessionCustomerId);
                if (accountToUpdate != null)
                {
                    accountToUpdate.Email = model.Email;
                    _context.Update(accountToUpdate);
                }
            }

            // 6. Cập nhật các trường còn lại
            customerToUpdate.FullName = model.FullName;
            customerToUpdate.Phone = model.Phone;
            customerToUpdate.Address = model.Address;
            customerToUpdate.UpdatedAt = DateTime.Now;

            _context.Update(customerToUpdate);
            await _context.SaveChangesAsync();

            // 7. Cập nhật lại FullName trong Session
            HttpContext.Session.SetString("FullName", customerToUpdate.FullName);

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }
    }
}