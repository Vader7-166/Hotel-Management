using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RoomController : Controller
    {
        private readonly HotelManagementContext db;
        public RoomController(HotelManagementContext _db)
        {
            this.db = _db;
        }

        public IActionResult Index()
        {
            var rooms = db.Rooms
                .Include("RoomType")
                .OrderBy(r => r.Floor)
                .ThenBy(r => r.RoomNumber)
                .ToList();

            var viewModel = new RoomManagementViewModel
            {
                Floors = rooms.GroupBy(r => r.Floor)
                             .Select(g => new FloorGroup
                             {
                                 FloorNumber = g.Key ?? 0,
                                 Rooms = g.ToList()
                             })
                             .OrderBy(f => f.FloorNumber)
                             .ToList(),
                RoomTypes = db.RoomTypes.ToList()
            };

            return View(viewModel);
        }

        // This method loads room type list for dropdown
        private async Task PopulateRoomTypes(RoomDetailViewModel viewModel)
        {
            viewModel.AvailableRoomTypes = await db.RoomTypes
                .Select(rt => new SelectListItem
                {
                    Value = rt.RoomTypeId.ToString(),
                    Text = $"{rt.TypeName} ({rt.Rooms.Count(r => r.Status == "Available")} available)"
                }).ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var availableRoomTypesList = db.RoomTypes
                                           .Select(rt => new SelectListItem
                                           {
                                               Value = rt.RoomTypeId.ToString(),
                                               Text = $"{rt.TypeName} ({rt.Rooms.Count(r => r.Status == "Available")} available)"
                                           }).ToList();

            RoomDetailViewModel viewModel;

            if (id > 0) // Edit mode
            {
                var room = db.Rooms.FirstOrDefault(r => r.RoomId == id);

                if (room == null)
                {
                    return NotFound();
                }

                viewModel = new RoomDetailViewModel
                {
                    RoomId = room.RoomId,
                    RoomNumber = room.RoomNumber,
                    RoomTypeId = room.RoomTypeId,
                    Status = room.Status,
                    Floor = room.Floor ?? 1,
                    Note = room.Note ?? ""
                };

                // Get booking info if room is not available
                if (room.Status == "Occupied" || room.Status == "CheckedIn" || room.Status == "Reserved")
                {
                    var today = DateOnly.FromDateTime(DateTime.Now);

                    // Find current or upcoming booking for this room
                    var activeBooking = await db.BookingDetails
                        .Include(bd => bd.Booking.Customer)
                        .Where(bd => bd.RoomId == id &&
                                     (bd.Booking.Status == "CheckedIn" || bd.Booking.Status == "Confirmed") &&
                                     bd.Booking.CheckOutDate >= today)
                        .OrderBy(bd => bd.Booking.CheckInDate)
                        .FirstOrDefaultAsync();

                    // If found, add to ViewModel
                    if (activeBooking != null)
                    {
                        viewModel.CustomerName = activeBooking.Booking.Customer.FullName;
                        viewModel.CheckInDate = activeBooking.Booking.CheckInDate;
                        viewModel.CheckOutDate = activeBooking.Booking.CheckOutDate;
                    }
                }
            }
            else // Create mode
            {
                viewModel = new RoomDetailViewModel();
            }

            viewModel.AvailableRoomTypes = availableRoomTypesList;
            return PartialView("_RoomDetail", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoomDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRoomTypes(model);
                return PartialView("_RoomDetail", model);
            }

            try
            {
                var room = new Room
                {
                    RoomNumber = model.RoomNumber,
                    RoomTypeId = model.RoomTypeId,
                    Status = model.Status,
                    Floor = model.Floor,
                    Note = model.Note,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                db.Rooms.Add(room);
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Room added successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(RoomDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRoomTypes(model);
                return PartialView("_RoomDetail", model);
            }

            try
            {
                var room = await db.Rooms.FindAsync(model.RoomId);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found!" });
                }

                room.RoomNumber = model.RoomNumber;
                room.RoomTypeId = model.RoomTypeId;
                room.Status = model.Status;
                room.Floor = model.Floor;
                room.Note = model.Note;
                room.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Room updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Server error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var room = await db.Rooms.FindAsync(id);
                if (room == null)
                {
                    return Json(new { success = false, message = "Room not found!" });
                }

                var isBooked = await db.BookingDetails
                    .AnyAsync(bd => bd.RoomId == id && bd.Booking.Status == "CheckedIn");

                if (isBooked)
                {
                    return Json(new { success = false, message = "Cannot delete a room currently occupied!" });
                }

                db.Rooms.Remove(room);
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Room deleted successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting room: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> _GetRoomLayout()
        {
            // Reload all room data (same as Index)
            var floorsData = await db.Rooms
                .Include(r => r.RoomType)
                .GroupBy(r => r.Floor ?? 0)
                .Select(g => new FloorGroup
                {
                    FloorNumber = g.Key,
                    Rooms = g.ToList()
                })
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();

            var viewModel = new RoomManagementViewModel
            {
                Floors = floorsData
            };

            return PartialView("_RoomLayout", viewModel);
        }
    }
}
