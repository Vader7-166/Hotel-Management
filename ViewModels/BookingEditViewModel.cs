using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.ViewModels
{
    public class BookingEditViewModel
    {
        // IDs to identify the records to update
        public int BookingId { get; set; }
        public int CustomerId { get; set; }

        // --- Customer Information ---
        [Required(ErrorMessage = "Full Name is required.")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [Phone(ErrorMessage = "Invalid Phone Number format.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string? Email { get; set; }

        [Display(Name = "ID Card/Passport")]
        public string? IdNumber { get; set; }

        // --- Booking Information ---
        [Required(ErrorMessage = "Please select a room type.")]
        [Display(Name = "Room Type")]
        public int RoomTypeId { get; set; }

        [Required]
        [Display(Name = "Check-in Date")]
        public DateOnly CheckInDate { get; set; }

        [Required]
        [Display(Name = "Check-out Date")]
        public DateOnly CheckOutDate { get; set; }

        [Required(ErrorMessage = "Please select a status.")]
        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        // --- Data for dropdown lists ---
        public List<SelectListItem> RoomTypes { get; set; } = new();
        public List<SelectListItem> BookingStatus { get; set; } = new();
    }
}
