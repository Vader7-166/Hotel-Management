using Microsoft.AspNetCore.Mvc;
using Hotel_Management.Models;
// Sử dụng bí danh để tránh xung đột namespace
using GlobalCustomer = Hotel_Management.Models.Customer;
using System.Linq; // Thêm using cho LINQ
using System.Threading.Tasks; // Thêm using cho tác vụ bất đồng bộ
using Microsoft.EntityFrameworkCore; // Thêm using cho ToListAsync

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CustomerController : Controller
    {
        private readonly HotelManagementContext db; //khai báo biến db context
        private readonly int pageSize = 3; // Tăng số lượng item mỗi trang cho thực tế hơn

        public CustomerController(HotelManagementContext context)
        {
            db = context;
        }

        [Route("Admin/Customer")]
        public async Task<IActionResult> CustomerList()
        {
            // 1. Lấy TẤT CẢ các quốc tịch duy nhất từ database
            var allNationalities = await db.Customers
                                           .Select(c => c.Nationality)
                                           .Where(n => !string.IsNullOrEmpty(n))
                                           .Distinct()
                                           .OrderBy(n => n) // Sắp xếp theo alphabet
                                           .ToListAsync();

            // 2. Gửi danh sách đầy đủ này sang View qua ViewBag
            ViewBag.AllNationalities = allNationalities;
            // Lấy toàn bộ danh sách khách hàng ban đầu
            var allCustomers = db.Customers.ToList();

            // Tính toán thông tin phân trang cho lần tải đầu tiên
            int totalCustomers = allCustomers.Count(); //đem tổng số khách hàng
            ViewBag.PageNum = (int)Math.Ceiling((double)totalCustomers / pageSize); //truyền số trang lên View
            ViewBag.CurrentPage = 1;//bắt đầu từ trang 1

            // Chỉ lấy dữ liệu cho trang đầu tiên để truyền cho View chính
            var initialCustomers = allCustomers.Take(pageSize).ToList();//lấy ra đối tượng chỉ gồm pageSize khách hàng đầu tiên

            return View(initialCustomers);
        }

        // Action này sẽ xử lý cả LỌC và PHÂN TRANG thông qua AJAX
        [HttpGet]
        public async Task<IActionResult> CustomerTableAndPagination(int pageIndex = 1, string searchQuery = "", string gender = "", string nationality = "")
        {
            // Bắt đầu với một IQueryable để xây dựng truy vấn động
            IQueryable<GlobalCustomer> query = db.Customers.AsQueryable();//lấy ra một đối tương query để chuẩn bị truy vấn

            // 1. ÁP DỤNG BỘ LỌC (nếu có)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.FullName.Contains(searchQuery) || c.Email.Contains(searchQuery) || c.Phone.Contains(searchQuery));
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(c => c.Gender == gender);// gán query bằng que sau si sử dụng truy vấn where
            }

            if (!string.IsNullOrEmpty(nationality))
            {
                query = query.Where(c => c.Nationality == nationality);
            }

            // 2. TÍNH TOÁN PHÂN TRANG TRÊN KẾT QUẢ ĐÃ LỌC
            int totalFilteredCustomers = await query.CountAsync(); // đếm số lượng khách hàng thỏa mãn kết quả lọc
            int pageNum = (int)Math.Ceiling((double)totalFilteredCustomers / pageSize);// tính xem để số lượng khách hàng đã lọc đó trong bao nhiêu trang

            ViewBag.PageNum = pageNum; //truyền pagenum cho view
            ViewBag.CurrentPage = pageIndex; //truyền page index cho view
            ViewBag.TotalEntries = totalFilteredCustomers; // Thêm thông tin tổng số bản ghi

            // 3. LẤY DỮ LIỆU CỦA TRANG HIỆN TẠI
            var result = await query
                .Skip(pageSize * (pageIndex - 1))//bắt đầu từ việc lấy dữ liệu theo page trước nếu page Index = 1 -> là điểm bắt đầu, không bỏ qua dòng nào
                .Take(pageSize) //bắt đầu lấy ra pagesize từ vị trí vừa skip
                .ToListAsync(); //chuyển kết quả thu được của query về list

            // Trả về Partial View chứa cả bảng và thanh phân trang mới
            return PartialView("_CustomerListPartial", result);
        }

        // =============================================================
        // CÁC ACTION CỦA BẠN ĐÃ ĐƯỢC GIỮ LẠI BÊN DƯỚI
        // =============================================================

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddNewCustomer()
        {
            // Truyền vào một đối tượng Customer mới để form có thể binding
            return View(new GlobalCustomer());
        }

        // POST: Nhận và xử lý dữ liệu từ form
        [HttpPost]
        [ValidateAntiForgeryToken] // Thêm AntiForgeryToken để bảo mật
        public async Task<IActionResult> AddNewCustomer(GlobalCustomer customer)
        {
            // Kiểm tra xem dữ liệu gửi lên có hợp lệ theo các quy tắc trong model không
            if (ModelState.IsValid)
            {
                // Thiết lập các giá trị mặc định cho record mới
                customer.CreatedAt = DateTime.Now;

                // Thêm đối tượng customer mới vào DbContext
                db.Customers.Add(customer);

                // Lưu các thay đổi vào cơ sở dữ liệu
                await db.SaveChangesAsync();

                // (Tùy chọn) Thêm thông báo thành công để hiển thị ở trang danh sách
                TempData["SuccessMessage"] = "New customer added successfully!";

                // Chuyển hướng người dùng về trang danh sách khách hàng
                return RedirectToAction("CustomerList", "Customer", new { area = "Admin" });
            }

            // Nếu dữ liệu không hợp lệ, hiển thị lại form với các thông báo lỗi
            // và giữ lại các dữ liệu người dùng đã nhập
            return View(customer);
        }

        public IActionResult CustomerSegmentation()
        {
            return View();
        }

        public IActionResult CustomerReport()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return PartialView("_CustomerDetailsPartitial", customer);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await db.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return PartialView("_CustomerEditPartitial", customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Rất quan trọng để chống tấn công CSRF
        public async Task<IActionResult> Edit(int id, GlobalCustomer customerViewModel)
        {
            //logic thay đối dữ liệu
            //kiểm tra xem ID của URL có khớp với ID trong model không
            if (id != customerViewModel.CustomerId)
            {
                return BadRequest(); //trả về lỗi 404 Bab Request
            }

            //bước 2 kiểm tra dữ liệu được gửi lên có hợp lệ không (dựa trên các data annotation)
            if (ModelState.IsValid)
            {
                try
                {
                    //thử lấy đối tượng gốc tự DB để cập nhật
                    var customerFromDb = await db.Customers.FindAsync(id);
                    if (customerFromDb == null)
                    {
                        return NotFound();
                    }

                    //nếu tìm thấy thì tiến hành thay thế các giá trị của đối tượng trong db bằng giá trị các thuộc tính của customerViewModel
                    customerFromDb.FullName = customerViewModel.FullName;
                    customerFromDb.BirthDate = customerViewModel.BirthDate;
                    customerFromDb.Gender = customerViewModel.Gender;
                    customerFromDb.Idnumber = customerViewModel.Idnumber;
                    customerFromDb.Phone = customerViewModel.Phone;
                    customerFromDb.Email = customerViewModel.Email;
                    customerFromDb.Address = customerViewModel.Address;
                    customerFromDb.Nationality = customerViewModel.Nationality;
                    customerFromDb.UpdatedAt = DateTime.Now; // Cập nhật thời gian chỉnh sửa

                    //Lưu các thay đổi vào db
                    await db.SaveChangesAsync();

                    //trả về một kết quả JSON để js biết rằng đã thay đổi thành công
                    return Json(new { success = true, message = "Cập nhật thông tin khách hàng thành công!" });
                }
                catch (DbUpdateConcurrencyException)
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra, dữ liệu có thể đã bị thay đổi bởi người khác." });
                }
            }
            //nếu model state không valid thì sẽ trả về partitial view với dữ liệu đã nhập dở và các thông báo lỗi Validation
            return PartialView("_CustomerEditPartitial", customerViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var customer = await db.Customers.FindAsync(id);
                if (customer == null)
                {
                    return Json(new { success = false, message = "Customer not found." });
                }

                db.Customers.Remove(customer);
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Customer has been deleted successfully." });
            }
            catch (Exception ex)
            {
                // Ghi log lỗi (nếu cần)
                return Json(new { success = false, message = "An error occurred while deleting." });
            }
        }
    }
}

