using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int BookingId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public decimal RoomAmount { get; set; }

    public decimal ServiceAmount { get; set; }

    public decimal Discount { get; set; }

    public decimal? TotalAmount { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
