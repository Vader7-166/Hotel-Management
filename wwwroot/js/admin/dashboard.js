using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.wwwroot.js.admin
{
    public class dashboard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
