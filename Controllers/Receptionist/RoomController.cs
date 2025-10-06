using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Receptionist
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
