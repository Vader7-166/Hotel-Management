using Hotel_Management.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
// Thêm bộ nhớ tạm (in-memory) để lưu trữ session
builder.Services.AddDistributedMemoryCache();
// Cấu hình Session cho ứng dụng
builder.Services.AddSession(options =>
{
    // Thời gian hết hạn session (không hoạt động sau 30 phút sẽ bị xóa)
    options.IdleTimeout = TimeSpan.FromMinutes(30);

    // Chỉ cho phép truy cập cookie của session từ server (bảo mật hơn)
    options.Cookie.HttpOnly = true;

    // Đánh dấu cookie này là cần thiết (Essential)
    // để không bị chặn khi người dùng bật chế độ chặn cookie
    options.Cookie.IsEssential = true;
});


// 🧩 Nếu đang chạy ở môi trường Development thì load secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// 🧩 Lấy chuỗi kết nối từ User Secrets
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string 'DatabaseConnection' is missing or empty. Check appsettings.json.");
}
// Optional: Log it for dev (remove in prod)
Console.WriteLine($"Connection String: {connectionString}");

// 🧩 Cấu hình DbContext để dùng chuỗi kết nối đó
builder.Services.AddDbContext<HotelManagementContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddControllersWithViews();
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Kích hoạt session trước khi định tuyến
app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
