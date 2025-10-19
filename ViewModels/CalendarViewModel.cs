namespace Hotel_Management.ViewModels
{
    public class CalendarViewModel
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public string CustomerName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string RoomNumber { get; set; }
        public DateOnly CheckInDate { get; set; }
        public DateOnly CheckOutDate { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public string? Notes { get; set; }
    }
}
