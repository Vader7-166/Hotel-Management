using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? EmployeeId { get; set; }

    public int? CustomerId { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Employee? Employee { get; set; }
}
