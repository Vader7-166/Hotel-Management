using Hotel_Management.Filters;
using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    [StaffAuthorize]
    public class BookingController : Controller
    {
        private readonly HotelManagementContext db;
        public BookingController(HotelManagementContext _db)
        {
            this.db = _db;
        }
        // Index
        public IActionResult Index(string searchString, string statusFilter, string dateFilter, int page = 1)
        {
            var bookingsQuery = db.Bookings
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails)
                    .ThenInclude(bd => bd.Room)
                .OrderByDescending(b => b.BookingId)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                bool isNumericOnly = searchString.All(char.IsDigit);

                if (isNumericOnly && int.TryParse(searchString, out int searchId))
                {
                    bookingsQuery = bookingsQuery.Where(b => b.BookingId == searchId);
                }
                else
                {
                    bookingsQuery = bookingsQuery.Where(b =>
                        b.Customer.FullName.Contains(searchString) ||
                        b.Customer.Phone.Contains(searchString)
                    );
                }
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                bookingsQuery = bookingsQuery.Where(b => b.Status == statusFilter);
            }

            if (!string.IsNullOrEmpty(dateFilter) && DateOnly.TryParse(dateFilter, out var filterDate))
            {
                bookingsQuery = bookingsQuery.Where(b => b.CheckInDate == filterDate || b.CheckOutDate == filterDate);
            }

            // Pagination 
            const int pageSize = 10;
            var totalBookingsCount = bookingsQuery.Count();
            var totalPages = (int)Math.Ceiling(totalBookingsCount / (double)pageSize);
            var paginatedBookings = bookingsQuery
                .Skip((page-1)*pageSize)    // Skip current page-1 * number of value in page
                .Take(pageSize)
                .ToList();

            var allBookings = db.Bookings.ToList();
            var today = DateOnly.FromDateTime(DateTime.Now);

            var viewModel = new BookingIndexViewModel
            {
                // Query in current page
                Bookings = paginatedBookings,       

                // Asign value to filter
                SearchString = searchString,
                StatusFilter = statusFilter,
                DateFilter = dateFilter,
                StatusFilterOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Pending", Text = "Pending" },
                    new SelectListItem { Value = "Confirmed", Text = "Confirmed" },
                    new SelectListItem { Value = "Cancelled", Text = "Cancelled" },
                    new SelectListItem { Value = "CheckedIn", Text = "CheckedIn"},
                    new SelectListItem { Value = "CheckedOut", Text = "CheckedOut"}
                },

                // Asign value for paginating
                CurrentPage = page,
                TotalPage = totalPages,

                // Asign value for statistics
                TotalBookings = allBookings.Count(),
                ConfirmedBookings = allBookings.Count(b => b.Status == "Confirmed"),
                PendingBookings = allBookings.Count(b => b.Status == "Pending"),
                TodaysCheckIns = allBookings.Count(b => b.CheckInDate == today && b.Status == "CheckedIn")
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingEditViewModel viewModel)
        {
            if (id != viewModel.BookingId) return NotFound();

            if (viewModel.CheckOutDate <= viewModel.CheckInDate)
            {
                ModelState.AddModelError("CheckOutDate", "Check-out date must be after check-in date.");
            }

            if (ModelState.IsValid)
            {
                using (var transaction = await db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Lấy các bản ghi gốc từ DB để so sánh và cập nhật
                        var bookingFromDb = await db.Bookings
                            .Include(b => b.BookingDetails)
                                .ThenInclude(bd => bd.Room)
                            .AsNoTracking() 
                            .FirstOrDefaultAsync(b => b.BookingId == viewModel.BookingId);

                        if (bookingFromDb == null) return NotFound();

                        var customerToUpdate = await db.Customers.FindAsync(viewModel.CustomerId);
                        if (customerToUpdate == null) return NotFound();

                        var bookingDetailToUpdate = await db.BookingDetails.FirstOrDefaultAsync(bd => bd.BookingId == viewModel.BookingId);
                        if (bookingDetailToUpdate == null) return NotFound();

                        // 1. Update Customer
                        customerToUpdate.FullName = viewModel.FullName;
                        customerToUpdate.Phone = viewModel.Phone;
                        customerToUpdate.Email = viewModel.Email;
                        customerToUpdate.Idnumber = viewModel.IdNumber;
                        customerToUpdate.UpdatedAt = DateTime.Now;

                        // 2. Logic change room 
                        int currentRoomId = bookingDetailToUpdate.RoomId;
                        var currentRoom = await db.Rooms.FindAsync(currentRoomId);

                        if (bookingFromDb.BookingDetails.First().Room.RoomTypeId != viewModel.RoomTypeId)
                        {
                            // Change old room status to 'Available'
                            if (currentRoom != null)
                            {
                                currentRoom.Status = "Available";
                            }

                            // Define new room
                            var newRoom = await db.Rooms
                                .Include(r => r.RoomType)
                                .FirstOrDefaultAsync(r => r.RoomTypeId == viewModel.RoomTypeId && r.Status == "Available");

                            // If out of room
                            if (newRoom == null)
                            {
                                // Alerting error and roll back
                                ModelState.AddModelError("RoomTypeId", "The selected room type is no longer available. Please choose another.");
                                await transaction.RollbackAsync();

                                // Update the dropdown value
                                viewModel.RoomTypes = await db.RoomTypes.Select(rt => new SelectListItem 
                                            { Value = rt.RoomTypeId.ToString(), Text = rt.TypeName }).ToListAsync();
                                viewModel.BookingStatus = GetStatusOptions();
                                return View(viewModel);
                            }

                            // Update new room
                            bookingDetailToUpdate.RoomId = newRoom.RoomId;
                            newRoom.Status = "Occupied";
                            bookingDetailToUpdate.UnitPrice = newRoom.RoomType.BasePrice;
                            currentRoom = newRoom;
                        }

                        // 3. Update room status base on booking status
                        string oldStatus = bookingFromDb.Status;
                        string newStatus = viewModel.Status;

                        if (oldStatus != newStatus && currentRoom != null)
                        {
                            switch (newStatus)
                            {
                                case "CheckedOut":
                                case "Cancelled":
                                    currentRoom.Status = "Available";
                                    break;
                                case "Confirmed":
                                    currentRoom.Status = "Reserved";
                                    break;
                                case "CheckedIn":
                                    currentRoom.Status = "Occupied";
                                    break;
                            }
                        }

                        // 4. Update number of nights and SubTotal
                        var nights = viewModel.CheckOutDate.DayNumber - viewModel.CheckInDate.DayNumber;
                        bookingDetailToUpdate.Nights = nights;
                        bookingDetailToUpdate.SubTotal = nights * bookingDetailToUpdate.UnitPrice;

                        // 5. Update main booking
                        var bookingEntityToUpdate = await db.Bookings.FindAsync(viewModel.BookingId);
                        if (bookingEntityToUpdate != null)
                        {
                            bookingEntityToUpdate.CheckInDate = viewModel.CheckInDate;
                            bookingEntityToUpdate.CheckOutDate = viewModel.CheckOutDate;
                            bookingEntityToUpdate.Status = viewModel.Status;
                            bookingEntityToUpdate.Notes = viewModel.Notes;
                            bookingEntityToUpdate.UpdatedAt = DateTime.Now;
                        }

                        // 6. Save all
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync();
                        ModelState.AddModelError("", "An error occurred while updating the booking.");
                    }
                }
            }

            viewModel.RoomTypes = await db.RoomTypes.Select(rt => new SelectListItem { Value = rt.RoomTypeId.ToString(), Text = rt.TypeName }).ToListAsync();
            viewModel.BookingStatus = GetStatusOptions();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            using (var transaction = await db.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Tìm booking cần xóa, TẢI KÈM TẤT CẢ DỮ LIỆU CON
                    var bookingToDelete = await db.Bookings
                        .Include(b => b.BookingDetails) // Tải kèm chi tiết booking
                        .Include(b => b.ServiceUsages)  // Tải kèm các dịch vụ đã sử dụng
                        .FirstOrDefaultAsync(b => b.BookingId == id);

                    if (bookingToDelete == null)
                    {
                        return NotFound();
                    }

                    // 2. Cập nhật lại trạng thái phòng
                    var bookingDetail = bookingToDelete.BookingDetails.FirstOrDefault();
                    if (bookingDetail != null)
                    {
                        var room = await db.Rooms.FindAsync(bookingDetail.RoomId);
                        if (room != null) room.Status = "Available";
                    }

                    // 3. XÓA CÁC BẢN GHI CON TRƯỚC
                    // Xóa tất cả ServiceUsage liên quan
                    if (bookingToDelete.ServiceUsages.Any())
                    {
                        db.ServiceUsages.RemoveRange(bookingToDelete.ServiceUsages);
                    }
                    // Xóa tất cả BookingDetails liên quan
                    if (bookingToDelete.BookingDetails.Any())
                    {
                        db.BookingDetails.RemoveRange(bookingToDelete.BookingDetails);
                    }

                    // (Nếu có Invoice liên quan, bạn cũng phải xóa nó trước)
                    var invoice = await db.Invoices.FirstOrDefaultAsync(i => i.BookingId == id);
                    if (invoice != null)
                    {
                        db.Invoices.Remove(invoice);
                    }

                    // 4. XÓA BẢN GHI CHA (BOOKING) SAU CÙNG
                    db.Bookings.Remove(bookingToDelete);

                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["SuccessMessage"] = $"Booking ID {id} has been deleted successfully.";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"An error occurred while deleting booking ID {id}.";
                }
            }

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

        public IActionResult GetBooking(int year, int month)
        {
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var bookingDetails = db.BookingDetails
                .Include(b => b.Booking)
                    .ThenInclude(c => c.Customer)
                .Include(r => r.Room)
                .Where(bd => (bd.Booking.CheckInDate < endDate && bd.Booking.CheckOutDate >= startDate))
                .Select(bd => new CalendarViewModel
                {
                    BookingDetailId = bd.BookingDetailId,
                    BookingId = bd.BookingId,
                    CustomerName = bd.Booking.Customer.FullName,
                    Phone = bd.Booking.Customer.Phone,
                    Email = bd.Booking.Customer.Email,
                    RoomNumber = bd.Room.RoomNumber,
                    CheckInDate = bd.Booking.CheckInDate,
                    CheckOutDate = bd.Booking.CheckOutDate,
                    Status = bd.Booking.Status,
                    Price = (decimal)bd.SubTotal,
                    Notes = bd.Booking.Notes
                }).ToList();

            return Json(bookingDetails);
        }

    }
}
