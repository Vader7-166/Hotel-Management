using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Admin.Controllers
{
    public class AcountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
