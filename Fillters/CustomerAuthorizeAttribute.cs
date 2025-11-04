using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hotel_Management.Filters // Đảm bảo namespace này đúng
{
    public class CustomerAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lấy vai trò từ Session
            var role = context.HttpContext.Session.GetString("Role");

            // 1. Nếu đúng là "Customer", cho phép truy cập
            if (!string.IsNullOrEmpty(role) && role == "Customer")
            {
                // Cho phép tiếp tục
                return;
            }

            // 2. Nếu là "Admin", đá về trang Admin Dashboard
            if (!string.IsNullOrEmpty(role) && role == "Admin")
            {
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Dashboard" },
                        { "action", "Index" },
                        { "area", "Admin" }
                    });
                return; // Ngăn không cho chạy tiếp
            }

            // 3. Nếu không phải cả hai (chưa đăng nhập), đá về trang Login
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