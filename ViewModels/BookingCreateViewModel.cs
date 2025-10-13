using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.ViewModels
{
    public class BookingCreateViewModel
    {
        [Required(ErrorMessage = "Please enter the guest's full name")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Please enter the phone number")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "ID/Passport Number")]
        public string? IdNumber { get; set; }

        // --- Booking Information ---
        [Required(ErrorMessage = "Please select a room type.")]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }
        public DateOnly? BookingDate {  get; set; }

        [Required(ErrorMessage = "Please select a check-in date")]
        [Display(Name = "Check-In Date")]
        public DateOnly CheckInDate { get; set; }

        [Required(ErrorMessage = "Please select a check-out date")]
        [Display(Name = "Check-Out Date")]
        public DateOnly CheckOutDate { get; set; }

        [Required(ErrorMessage = "Please select booking status")]
        [Display(Name = "Status")]
        public string? Status { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // --- Data for the View ---
        // List of available rooms for selection
        public List<SelectListItem> RoomTypes { get; set; } = new();
        public List<SelectListItem> BookingStatus { get; set; } = new List<SelectListItem>();
    }
}
