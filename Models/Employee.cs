using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Position { get; set; }

    public decimal? Salary { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
