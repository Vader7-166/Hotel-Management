using Hotel_Management.Models;
using Hotel_Management.Models.ViewModels.Admin;
using Hotel_Management.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Hotel_Management.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly HotelManagementContext db;

        public DashboardController(HotelManagementContext _db) 
        {
            this.db = _db;
        }
       
        public IActionResult Index()
        {
            var viewModel = new DashboardIndexViewModel();
            var today = DateOnly.FromDateTime(DateTime.Today);
            var yesterday = today.AddDays(-1);

            viewModel.TotalRoom = db.Rooms.Count();
            viewModel.CurrentlyRenting = db.Rooms.Count(r => r.Status != "Available");
            viewModel.AvailableRooms = db.Rooms.Count(r => r.Status == "Available");
            viewModel.RevenueToday = db.Invoices
                .Where(i => DateOnly.FromDateTime(i.InvoiceDate) == today && i.PaymentStatus == "Paid" && i.TotalAmount != null)
                .Sum(i => i.TotalAmount ?? 0);
            // Change to milions VND
            viewModel.RevenueToday /= 1000000;
            // Calculate profit compared to yesterday
            var revenueYesterday = (decimal)db.Invoices.Where(i => DateOnly.FromDateTime(i.InvoiceDate) == yesterday && i.PaymentStatus == "Paid").Sum(i => i.TotalAmount);
            decimal percentDiff = 0;
            if (revenueYesterday != 0)
            {
                percentDiff = (viewModel.RevenueToday - revenueYesterday) / revenueYesterday * 100;
            }
            else if (viewModel.RevenueToday > 0)
            {
                percentDiff = 100;
            }
            viewModel.RevenueCompareToYesterday = percentDiff;

            // Calculate new checkin
            var newCheckInToday = db.Bookings.Count(b => b.CheckInDate == today && b.Status != "Cancelled");
            viewModel.MoreThanYesterday = newCheckInToday;

            // Data for chart
            // 7 last days
            var dailyRevenueData = new Dictionary<DateOnly, decimal>();
            for (int i = 6; i>=0; i--)
            {
                dailyRevenueData.Add(today.AddDays(-i), 0);
            }

            var last7DaysInvoices = db.Invoices
                .Where(i => i.Booking.CheckOutDate >= today.AddDays(-6) && i.Booking.CheckOutDate <= today && i.PaymentStatus == "Paid")
                .GroupBy(i => i.Booking.CheckOutDate)
                .Select(g => new { Date = g.Key, Total = g.Sum(i => i.TotalAmount) })
                .ToList();

            foreach (var item in last7DaysInvoices)
            {
                dailyRevenueData[item.Date] = (decimal)item.Total / 1000000;
            }

            var culture = new CultureInfo("vi-VN");
            viewModel.DailyRevenueChart.Labels = dailyRevenueData.Keys.Select(d => culture.DateTimeFormat.GetDayName(d.DayOfWeek)).ToList();
            viewModel.DailyRevenueChart.Data = dailyRevenueData.Values.ToList();

            // 6 last months
            var monthlyRevenueData = new Dictionary<string, decimal>();
            var firstDayOfCurrentMonth = new DateOnly(today.Year, today.Month, 1);

            for (int i = 5; i >= 0; i--)
            {
                var month = firstDayOfCurrentMonth.AddMonths(-i);
                monthlyRevenueData.Add($"Thg {month.Month}", 0);
            }

            var last6MonthsInvoices = db.Invoices
                .Where(i => i.Booking.CheckOutDate >= firstDayOfCurrentMonth.AddMonths(-5) && i.PaymentStatus == "Paid")
                .Select(i => new { CheckOutDate = i.Booking.CheckOutDate, Total = i.TotalAmount })
                .ToList();

            var monthlyGrouped = last6MonthsInvoices
                .GroupBy(i => new { i.CheckOutDate.Year, i.CheckOutDate.Month })
                .Select(g => new { MonthKey = $"Thg {g.Key.Month}", Total = g.Sum(i => i.Total) });

            foreach (var item in monthlyGrouped)
            {
                if (monthlyRevenueData.ContainsKey(item.MonthKey))
                {
                    monthlyRevenueData[item.MonthKey] = (decimal)item.Total / 1000000; // Đổi sang tỉ VND
                }
            }

            viewModel.MonthlyRevenueChart.Labels = monthlyRevenueData.Keys.ToList();
            viewModel.MonthlyRevenueChart.Data = monthlyRevenueData.Values.ToList();

            // --- Danh sách Check-in ---
            viewModel.CheckInList = db.Bookings
                .Where(b => b.CheckInDate == today &&
                            (b.Status == "Confirmed" || b.Status == "Pending" || b.Status == "CheckedIn"))
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Room)

                .Select(b => new DashboardIndexViewModel.CheckInInfo
                {
                    Room = b.BookingDetails.FirstOrDefault().Room.RoomNumber,
                    Customer = b.Customer.FullName,
                    CheckTime = "14:00",
                    Status = b.Status == "CheckedIn" ? "Checked-in" :
                             b.Status == "Confirmed" ? "Waiting for Check-in" :
                             b.Status == "Pending" ? "Pending" :
                             b.Status
                })
                .OrderBy(c => c.Room)
                .ToList();

            viewModel.TotalCheckIn = viewModel.CheckInList.Count;

            // --- Danh sách Check-out ---
            viewModel.CheckOutList = db.Bookings
                .Where(b => b.CheckOutDate == today && (b.Status == "CheckedIn" || b.Status == "CheckedOut"))
                .Include(b => b.Customer)
                .Include(b => b.BookingDetails).ThenInclude(bd => bd.Room)
                .Select(b => new DashboardIndexViewModel.CheckOutInfo
                {
                    Room = b.BookingDetails.FirstOrDefault().Room.RoomNumber,
                    Customer = b.Customer.FullName,
                    CheckTime = "12:00",
                    Status = (b.Status == "CheckedIn") ? "Waiting for Check-out" : "Checked-out"
                })
                .OrderBy(c => c.Room)
                .ToList();

            viewModel.TotalCheckOut = viewModel.CheckOutList.Count;

            return View(viewModel);
        }
    }
}
