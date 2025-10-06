using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Receptionist
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
