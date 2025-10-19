using Hotel_Management.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Hotel_Management.ViewModels
{
    public class BookingIndexViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }

        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int TodaysCheckIns { get; set; }

        // Attributes for filter 
        public string? SearchString { get; set; }
        public string? StatusFilter { get; set; }
        public string? DateFilter { get; set; }
        public List<SelectListItem> StatusFilterOptions { get; set; }

        // Attribute for pagination
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPage;
    }
}
