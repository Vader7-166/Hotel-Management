using Microsoft.AspNetCore.Mvc;
using Hotel_Management.Models;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RoomsController : Controller
    {
        private readonly HotelManagementContext _context;
        private const int PageSize = 6; // số phòng mỗi trang

        public RoomsController(HotelManagementContext context)
        {
            _context = context;
        }

        [Route("Customer/Room")]
        public async Task<IActionResult> Index()
        {
            var roomTypes = await _context.RoomTypes.ToListAsync();
            return View(roomTypes);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.RoomId == id);

            if (room == null)
                return NotFound();

            return View(room);
        }

        // Trang chính hiển thị loại phòng
        [Route("Customer/Room/RoomType/{id}")]
        public async Task<IActionResult> RoomTypeDetails(int id)
        {
            var roomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.RoomTypeId == id);
            if (roomType == null)
                return NotFound();

            ViewBag.RoomType = roomType;
            ViewBag.RoomTypeId = id;

            return View(); // chỉ hiển thị khung, dữ liệu được load bằng AJAX
        }

        // Load phòng theo trang (AJAX)
        [HttpGet]
        public async Task<IActionResult> LoadRooms(int id, int page = 1)
        {
            var query = _context.Rooms
                .Where(r => r.RoomTypeId == id)
                .Include(r => r.RoomType)
                .OrderBy(r => r.RoomNumber);

            int totalRooms = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalRooms / PageSize);

            var rooms = await query
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.RoomTypeId = id;

            return PartialView("_RoomListPartial", rooms);
        }
    }
}
