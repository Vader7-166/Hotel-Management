using Hotel_Management.Models;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
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
                                 FloorNumber = (int)g.Key,
                                 Rooms = g.ToList()
                             })
                             .OrderBy(f => f.FloorNumber)
                             .ToList(),
                RoomTypes = db.RoomTypes.ToList()
            };

            return View(viewModel);
        }
    }
}
