using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hotel_Management.Filters
{
    // Đổi tên class thành StaffAuthorizeAttribute
    public class StaffAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            // Cho phép nếu là Admin HOẶC Lễ tân
            if (!string.IsNullOrEmpty(role) && (role == "Admin" || role == "Receptionist"))
            {
                return; // Cho phép truy cập
            }

            // Nếu không phải, đá về trang Login
            context.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Account" },
                    { "action", "Login" },
                    { "area", "Customer" }
                });
        }
    }
}