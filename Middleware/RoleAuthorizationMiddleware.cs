using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management.Middleware
{
    public class RoleAuthorizationMiddleware : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
