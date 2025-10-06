using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Controllers.Admin
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
