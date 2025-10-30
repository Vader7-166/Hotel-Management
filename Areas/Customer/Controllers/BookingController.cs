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
            ViewBag.RoomTypes=_context.RoomTypes.ToList();
            return View();
        }




        [Route("Customer/Booking/Details")]
        public IActionResult Details(int id)
        {
            // Lấy thông tin chi tiết Booking (gồm phòng và dịch vụ)
            var booking = _context.Bookings
                .Include(b => b.BookingDetails)
                    .ThenInclude(d => d.Room)
                        .ThenInclude(r => r.RoomType)
                .Include(b => b.ServiceUsages)
                    .ThenInclude(su => su.Service)
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
        // ===========================
        // 🔍 AJAX: Filter available rooms
        // ===========================
        [HttpGet]
        public IActionResult FilterRooms(DateTime checkIn, DateTime checkOut, decimal? minPrice, decimal? maxPrice, int? guests)
        {
            if (checkIn.Date < DateTime.Today)
                return BadRequest(new { success = false, message = "Check-in date cannot be in the past." });

            if (checkOut <= checkIn)
                return BadRequest(new { success = false, message = "Check-out date must be after check-in date." });

            // 🧠 Tách toàn bộ dữ liệu BookingDetails ra trước, vì EF không dịch được ToDateTime
            var bookingDetails = _context.BookingDetails
                .Include(bd => bd.Booking)
                .AsEnumerable() // ⚠️ chuyển sang xử lý bằng LINQ to Objects
                .Where(bd => bd.Booking != null &&
                    checkIn.Date < bd.Booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) &&
                    checkOut.Date > bd.Booking.CheckInDate.ToDateTime(TimeOnly.MinValue))
                .Select(bd => bd.RoomId)
                .Distinct()
                .ToList();

            // Lọc phòng chưa được đặt
            var availableRooms = _context.Rooms
                .Include(r => r.RoomType)
                .Where(r => !bookingDetails.Contains(r.RoomId) && r.Status == "Available");

            if (minPrice.HasValue)
                availableRooms = availableRooms.Where(r => r.RoomType.BasePrice >= minPrice);
            if (maxPrice.HasValue)
                availableRooms = availableRooms.Where(r => r.RoomType.BasePrice <= maxPrice);
            if (guests.HasValue && guests > 0)
                availableRooms = availableRooms.Where(r => r.RoomType.MaxOccupancy >= guests);

            var result = availableRooms.Select(r => new
            {
                roomId = r.RoomId,
                roomNumber = r.RoomNumber,
                typeName = r.RoomType.TypeName,
                basePrice = r.RoomType.BasePrice,
                maxOccupancy = r.RoomType.MaxOccupancy,
                description = r.RoomType.Description,
                status = r.Status
            }).ToList();

            Console.WriteLine($"✅ Found {result.Count} available rooms");
            return Json(new { success = true, data = result });
        }

        [HttpGet]
        [Route("Customer/Booking/Create")]
        public IActionResult Create(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var room = _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomId == roomId);

            if (room == null)
                return NotFound();
            var services=_context.Services.ToList();

            ViewBag.Room = room;
            ViewBag.Services = services;
            ViewBag.CheckIn = checkIn.ToString("yyyy-MM-dd");
            ViewBag.CheckOut = checkOut.ToString("yyyy-MM-dd");
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Customer/Booking/Create")]
        public IActionResult Create(Booking booking, int roomId, int[] selectedServices )
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login", "Account", new { area = "Customer" });

            // Gán thông tin Booking
            booking.CustomerId = customerId.Value;
            booking.BookingDate = DateTime.Now;
            booking.Status = "Pending";
            booking.CreatedAt = DateTime.Now;

            _context.Bookings.Add(booking);
            _context.SaveChanges();

            // 🧮 Tính giá phòng và số đêm
            var room = _context.Rooms.Include(r => r.RoomType).FirstOrDefault(r => r.RoomId == roomId);
            if (room == null)
                return NotFound("Room not found");

            var nights = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) -
                          booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
            var roomPrice = room.RoomType.BasePrice;

            // 💾 Lưu chi tiết Booking
            var detail = new BookingDetail
            {
                BookingId = booking.BookingId,
                RoomId = roomId,
                UnitPrice = roomPrice,
                Nights = nights,
                SubTotal = roomPrice * nights,
                CreatedAt = DateTime.Now
            };

            _context.BookingDetails.Add(detail);
            

            // 🧩 Thêm các dịch vụ khách chọn
            foreach (var serviceId in selectedServices)
            {
                var service = _context.Services.FirstOrDefault(s => s.ServiceId == serviceId);
                if (service != null)
                {
                    _context.ServiceUsages.Add(new ServiceUsage
                    {
                        BookingId = booking.BookingId,
                        ServiceId = serviceId,
                        Quantity = 1,
                        UnitPrice = service.UnitPrice,
                        TotalAmount = service.UnitPrice,
                        UsageDate = DateTime.Now
                    });
                }
            }
            // Cập nhật trạng thái phòng
            room.Status = "Booked";

            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }



    }
}
