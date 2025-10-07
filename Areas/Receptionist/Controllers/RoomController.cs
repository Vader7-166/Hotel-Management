using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Receptionist.Controllers
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
