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
        // Hàm này để nạp lại danh sách loại phòng cho dropdown
        // (Dùng khi validation thất bại)
        private async Task PopulateRoomTypes(RoomDetailViewModel viewModel)
        {
            viewModel.AvailableRoomTypes = await db.RoomTypes
                                             .OrderBy(rt => rt.TypeName)
                                             .Select(rt => new SelectListItem
                                             {
                                                 Value = rt.RoomTypeId.ToString(),
                                                 Text = rt.TypeName
                                             })
                                             .ToListAsync();
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var availableRoomTypesList = db.RoomTypes
                                           .OrderBy(rt => rt.TypeName)
                                           .Select(rt => new SelectListItem
                                           {
                                               Value = rt.RoomTypeId.ToString(),
                                               Text = rt.TypeName
                                           })
                                           .ToList();

            RoomDetailViewModel viewModel;

            if (id > 0) // Trường hợp Cập nhật (Edit)
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
            }
            else // Trường hợp Tạo mới (Create)
            {
                viewModel = new RoomDetailViewModel();
            }
            viewModel.AvailableRoomTypes = availableRoomTypesList;

            return PartialView("_RoomDetail", viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Create(RoomDetailViewModel model)
        {
            if (ModelState.IsValid)
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

                return Json(new { success = true, message = "Thêm phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
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
                    return Json(new { success = false, message = "Không tìm thấy phòng!" });
                }

                room.RoomNumber = model.RoomNumber;
                room.RoomTypeId = model.RoomTypeId;
                room.Status = model.Status;
                room.Floor = model.Floor;
                room.Note = model.Note;
                room.UpdatedAt = DateTime.Now;

                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Cập nhật phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi server: " + ex.Message });
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
                    return Json(new { success = false, message = "Không tìm thấy phòng!" });
                }

                var isBooked = await db.BookingDetails
                    .AnyAsync(bd => bd.RoomId == id && bd.Booking.Status == "Active");
                if (isBooked)
                {
                    return Json(new { success = false, message = "Không thể xóa phòng đang có khách!" });
                }

                db.Rooms.Remove(room);
                await db.SaveChangesAsync();

                return Json(new { success = true, message = "Xóa phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa: " + ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> _GetRoomLayout()
        {
            // Query lại toàn bộ dữ liệu (giống hệt action Index)
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

