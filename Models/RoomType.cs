﻿using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class RoomType
{
    public int RoomTypeId { get; set; }

    public string TypeName { get; set; } = null!;

    public int MaxOccupancy { get; set; }

    public string? Description { get; set; }

    public decimal BasePrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
