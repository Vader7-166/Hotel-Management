using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Areas.Receptionist.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
