using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Hotel_Management.Filters // <-- Đổi namespace này nếu bạn đặt file ở chỗ khác
{
    // Lớp này kế thừa Attribute và IAuthorizationFilter
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        // Hàm này sẽ tự động chạy trước khi bất kỳ Action nào được gọi
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Lấy thông tin Role từ Session
            var adminRole = context.HttpContext.Session.GetString("Role");

            // Kiểm tra xem Role có phải là "Admin" không
            if (string.IsNullOrEmpty(adminRole) || adminRole != "Admin")
            {
                // Nếu không phải Admin, "đá" người dùng về trang Login
                context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "Login" },
                        { "area", "Customer" } // Chuyển về trang login của Customer
                    });
            }

            // Nếu đúng là Admin, hàm sẽ không làm gì cả và tiếp tục chạy Action
        }
    }
}