using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class BookingDetail
{
    public int BookingDetailId { get; set; }

    public int BookingId { get; set; }

    public int RoomId { get; set; }

    public decimal UnitPrice { get; set; }

    public int Nights { get; set; }

    public decimal? SubTotal { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
