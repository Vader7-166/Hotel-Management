using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class BookingController : Controller
    {
        private readonly HotelManagementContext _context;
        public BookingController(HotelManagementContext context)
        {
            _context = context;
        }
        [Route("Customer/Booking/Index")]
        public IActionResult Index()
        {
            return View();
        }
        [Route("Customer/Booking/Create")]
        public IActionResult Create()
        {
            return View();
        }




        [Route("Customer/Booking/Details")]
        public IActionResult Details(int id)
        {
            // Lấy thông tin chi tiết của Booking (bao gồm danh sách phòng)
            var booking = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền: chỉ cho phép xem booking của chính họ
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (booking.CustomerId != customerId)
            {
                return Unauthorized();
            }

            return View(booking);
        }



        [Route("Customer/Booking/MyBookings")]
        public IActionResult MyBookings()
        {
            // kiểm tra xem người dùng đã đăng nhập chưa
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            
            if (customerId == null)
            {
                // Nếu chưa đăng nhập, chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "Account", new { area = "Customer" });
            }
            // 📦 Lấy danh sách Booking thuộc Customer hiện tại
            var bookings = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room) // Lấy thêm thông tin phòng
                .Where(b => b.CustomerId == customerId)
                .OrderByDescending(b => b.BookingDate)
                .ToList();

            // 🖥️ Gửi dữ liệu sang view để hiển thị
            return View(bookings);
        }

        [HttpPost]
        [Route("Customer/Booking/Cancel/{id}")]
        public IActionResult Cancel(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
            {
                return NotFound();
            }

            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (booking.CustomerId != customerId)
            {
                return Unauthorized();
            }

            // 🚫 Chỉ được hủy nếu chưa xác nhận
            if (booking.Status == "Pending")
            {
                booking.Status = "Cancelled";
                booking.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
            }

            return RedirectToAction("MyBookings");
        }
    }
}
