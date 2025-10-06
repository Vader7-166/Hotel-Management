using Microsoft.AspNetCore.Mvc;

//Trang chủ
namespace Hotel_Management.Controllers.Customer
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
