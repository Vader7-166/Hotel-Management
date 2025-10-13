using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Hotel_Management.ViewModels;
using Hotel_Management.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly HotelManagementContext db;
        public BookingController(HotelManagementContext _db)
        {
            this.db = _db;
        }
        // Index
        public IActionResult Index()
        {
            var allBookings = db.Bookings
                .Include(b => b.Customer)                   // Tải kèm thông tin khách hàng
                .Include(b => b.BookingDetails)             // Tải kèm danh sách chi tiết booking
                    .ThenInclude(bd => bd.Room)             // Với mỗi chi tiết, tải kèm thông tin phòng
                        .ThenInclude(r => r.RoomType)       // Với mỗi phòng, tải kèm thông tin loại phòng
                .OrderByDescending(b => b.BookingDate)      // Sắp xếp cho dễ nhìn (tùy chọn)
                .ToList();

            var today = DateOnly.FromDateTime(DateTime.Now);

            var viewModel = new BookingIndexViewModel
            {
                Bookings = allBookings,
                TotalBookings = allBookings.Count,
                ConfirmedBookings = allBookings.Count(b => b.Status == "Confirmed"),
                PendingBookings = allBookings.Count(b => b.Status == "Pending"),
                TodaysCheckIns = allBookings.Count(b => b.CheckInDate == today && b.Status != "Cancelled")
            };
            return View(viewModel);
        }

        // Create (Get/Post)
        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new BookingCreateViewModel
            {
                RoomTypes = db.RoomTypes
                    .Select(rt => new SelectListItem
                    {
                        Value = rt.RoomTypeId.ToString(),
                        Text = $"{rt.TypeName} ({rt.Rooms.Count(r => r.Status == "Available")} available)"
                    }).ToList(),
                BookingStatus = GetStatusOptions()
            };
            viewModel.CheckInDate = DateOnly.FromDateTime(DateTime.Now);
            viewModel.CheckOutDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookingCreateViewModel viewModel)
        {
            if (viewModel.CheckOutDate <= viewModel.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
            }
            if (ModelState.IsValid)
            {
                // Find or create new Customer
                var customer = db.Customers.FirstOrDefault(c => c.Phone == viewModel.Phone);
                if (customer == null)
                {
                    customer = new Models.Customer
                    {
                        FullName = viewModel.FullName,
                        Phone = viewModel.Phone,
                        Email = viewModel.Email,
                        Idnumber = viewModel.IdNumber,
                        CreatedAt = DateTime.Now
                    };
                    db.Customers.Add(customer);
                    db.SaveChanges();
                }
                // Create Booking Detail
                var roomToAssign = db.Rooms
                    .Include(r => r.RoomType) // Include RoomType để lấy giá
                    .FirstOrDefault(r => r.RoomTypeId == viewModel.RoomTypeId && r.Status == "Available");

                if (roomToAssign == null)
                {
                    ModelState.AddModelError("RoomTypeId", "The selected room type is no longer available. Please choose another.");
                    viewModel.RoomTypes = db.RoomTypes.Select(rt => new SelectListItem { Value = rt.RoomTypeId.ToString(), Text = rt.TypeName }).ToList();
                    viewModel.BookingStatus = GetStatusOptions();
                    return View(viewModel);
                }

                // Create new Booking
                var booking = new Booking
                {
                    CustomerId = customer.CustomerId,
                    CheckInDate = viewModel.CheckInDate,
                    CheckOutDate = viewModel.CheckOutDate,
                    Status = viewModel.Status,
                    BookingDate = viewModel.Status == "Confirmed" ? DateTime.Now : DateTime.MinValue,
                    Notes = viewModel.Notes,
                    CreatedAt = DateTime.Now
                };
                db.Bookings.Add(booking);
                db.SaveChanges();

                var nights = (viewModel.CheckOutDate.DayNumber - viewModel.CheckInDate.DayNumber);

                var bookingDetail = new BookingDetail
                {
                    BookingId = booking.BookingId,
                    RoomId = roomToAssign.RoomId,
                    UnitPrice = db.RoomTypes.Find(roomToAssign.RoomTypeId)?.BasePrice ?? 0,
                    Nights = nights,
                    SubTotal = nights * (db.RoomTypes.Find(roomToAssign.RoomTypeId)?.BasePrice ?? 0),
                    CreatedAt = DateTime.Now
                };
                db.BookingDetails.Add(bookingDetail);

                // Update Room's Status 
                roomToAssign.Status = "Occupied";
                // Save all change
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }
        private List<SelectListItem> GetStatusOptions()
        {
            var statuses = new List<string> { "Pending", "Confirmed", "Cancelled", "CheckedIn", "CheckedOut" };
            return statuses.Select(s => new SelectListItem
            {
                Value = s,
                Text = s
            }).ToList();
        }

        // Edit (Get/Post)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Tải booking cùng các dữ liệu liên quan
            var booking = db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                    .ThenInclude(r => r.RoomType)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null || !booking.BookingDetails.Any())
            {
                return NotFound();
            }

            var bookingDetail = booking.BookingDetails.First();

            // Lấy danh sách các loại phòng và số phòng trống của mỗi loại
            var roomTypesList = db.RoomTypes
                .Select(rt => new SelectListItem
                {
                    Value = rt.RoomTypeId.ToString(),
                    Text = $"{rt.TypeName} ({rt.Rooms.Count(r => r.Status == "Available")} available)"
                }).ToList();

            // Ánh xạ (Map) dữ liệu từ Model sang ViewModel
            var viewModel = new BookingEditViewModel
            {
                BookingId = booking.BookingId,
                CustomerId = booking.CustomerId,
                FullName = booking.Customer.FullName,
                Phone = booking.Customer.Phone,
                Email = booking.Customer.Email,
                IdNumber = booking.Customer.Idnumber,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                Status = booking.Status,
                Notes = booking.Notes,

                // Gán RoomTypeId hiện tại của booking
                RoomTypeId = bookingDetail.Room.RoomTypeId,

                // Gán danh sách lựa chọn
                RoomTypes = roomTypesList,
                BookingStatus = GetStatusOptions()
            };
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(int? id, BookingEditViewModel viewModel)
        {
            if (id != viewModel.BookingId) return NotFound();

            if (viewModel.CheckOutDate <= viewModel.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
            }
            if (ModelState.IsValid)
            {
                var bookingToUpdate = db.Bookings
                    .Include(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Room)
                    .FirstOrDefault(b => b.BookingId == viewModel.BookingId);
                if (bookingToUpdate == null) return NotFound();
                var customerToUpdate = db.Customers.Find(viewModel.CustomerId);

                if (bookingToUpdate == null || customerToUpdate == null) return NotFound();

                // 1. Cập nhật thông tin khách hàng
                customerToUpdate.FullName = viewModel.FullName;
                customerToUpdate.Phone = viewModel.Phone;
                customerToUpdate.Email = viewModel.Email;
                customerToUpdate.Idnumber = viewModel.IdNumber;
                customerToUpdate.UpdatedAt = DateTime.Now;

                // 2. Cập nhật thông tin booking chính
                bookingToUpdate.CheckInDate = viewModel.CheckInDate;
                bookingToUpdate.CheckOutDate = viewModel.CheckOutDate;
                bookingToUpdate.Status = viewModel.Status;
                bookingToUpdate.Notes = viewModel.Notes;
                bookingToUpdate.UpdatedAt = DateTime.Now;

                // --- LOGIC XỬ LÝ ĐỔI PHÒNG: Cho chọn các loại phòng, hệ thống sẽ kiểm tra xem còn phòng loại đó không?
                // Nếu còn thì tự động được xếp vào 1 phòng, nếu không sẽ trả lại thông báo ---
                var bookingDetail = bookingToUpdate.BookingDetails.First();
                int oldRoomTypeId = bookingDetail.Room.RoomTypeId;
                int oldRoomId = bookingDetail.RoomId;

                if (oldRoomTypeId != viewModel.RoomTypeId)  // Nếu khách đổi phòng 
                {
                    var oldRoom = db.Rooms.Find(oldRoomId);
                    if(oldRoom == null) oldRoom.Status = "Available";

                    var newRoom = db.Rooms.FirstOrDefault(r => (r.RoomTypeId == viewModel.RoomTypeId && r.Status == "Available"));

                    if (newRoom == null)
                    {
                        // Bao loi
                        ModelState.AddModelError("RoomTypeId", "The selected room type is no longer available. Please choose another.");
                        // Nạp lại dữ liệu cho view và trả về
                        viewModel.RoomTypes = db.RoomTypes.Select(rt => new SelectListItem { Value = rt.RoomTypeId.ToString(), Text = rt.TypeName }).ToList();
                        viewModel.BookingStatus = GetStatusOptions();
                        return View(viewModel);
                    }
                    // Neu con trong
                    bookingDetail.RoomId = newRoom.RoomId;
                    newRoom.Status = "Occupied";
                    bookingDetail.UnitPrice = newRoom.RoomType.BasePrice;

                }
                var nights = viewModel.CheckOutDate.DayNumber - viewModel.CheckInDate.DayNumber;
                bookingDetail.Nights = nights;
                bookingDetail.SubTotal = nights * bookingDetail.UnitPrice;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            // 1. Tìm booking cần xóa
            var bookingToDelete = db.Bookings
                .Include(b => b.BookingDetails)
                .FirstOrDefault(b => b.BookingId == id);

            if (bookingToDelete == null)
            {
                return NotFound();
            }

            // 2. Lấy thông tin phòng để cập nhật lại trạng thái
            var bookingDetail = bookingToDelete.BookingDetails.FirstOrDefault();
            if (bookingDetail != null)
            {
                var room = db.Rooms.Find(bookingDetail.RoomId);
                if (room != null)
                {
                    room.Status = "Available";
                }
            }
            // 3. Xóa các bản ghi liên quan trước (BookingDetails, ServiceUsages...)
            db.BookingDetails.RemoveRange(bookingToDelete.BookingDetails);
            db.Bookings.Remove(bookingToDelete);
            db.SaveChanges();

            TempData["SuccessMessage"] = $"Booking ID {id} has been deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult GetBookingDetails(int id)
        {
            var booking = db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                        .ThenInclude(r => r.RoomType)
                .FirstOrDefault(b => b.BookingId == id);

            if (booking == null)
            {
                return NotFound();
            }

            var bookingDetail = booking.BookingDetails.FirstOrDefault();

            // Tạo một đối tượng vô danh (anonymous object) để chứa dữ liệu cần thiết cho client.
            var result = new
            {
                bookingId = booking.BookingId,
                status = booking.Status,
                totalPrice = booking.BookingDetails.Sum(bd => bd.SubTotal ?? 0),

                customerName = booking.Customer.FullName,
                customerPhone = booking.Customer.Phone,
                customerEmail = booking.Customer.Email ?? "N/A",

                roomNumber = bookingDetail?.Room?.RoomNumber ?? "N/A",
                roomType = bookingDetail?.Room?.RoomType?.TypeName ?? "N/A",
                checkInDate = booking.CheckInDate.ToString("dd/MM/yyyy"),
                checkOutDate = booking.CheckOutDate.ToString("dd/MM/yyyy"),
                numberOfNights = bookingDetail?.Nights ?? 0,
                notes = booking.Notes ?? ""
            };

            return Json(result);
        }

        public IActionResult Calendar()
        {
            return View();
        }
        
    }
}
