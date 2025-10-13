using Hotel_Management.Models;

namespace Hotel_Management.ViewModels
{
    public class BookingIndexViewModel
    {
        public IEnumerable<Booking> Bookings { get; set; }

        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int TodaysCheckIns { get; set; }
    }
}
