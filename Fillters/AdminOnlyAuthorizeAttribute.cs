using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hotel_Management.Filters
{
    // Filter này CHỈ cho phép Admin
    public class AdminOnlyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");

            // 1. Nếu là Admin, cho phép
            if (!string.IsNullOrEmpty(role) && role == "Admin")
            {
                return; // OK
            }

            // 2. Nếu là Lễ tân, đá về Dashboard (vì họ đã đăng nhập)
            if (!string.IsNullOrEmpty(role) && role == "Receptionist")
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Dashboard" },
                        { "action", "Index" },
                        { "area", "Admin" }
                    });
                return;
            }

            // 3. Nếu chưa đăng nhập, đá về Login
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