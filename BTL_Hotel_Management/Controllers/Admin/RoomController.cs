using Microsoft.AspNetCore.Mvc;

namespace BTL_Hotel_Management.Controllers.Admin
{
    public class RoomController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
