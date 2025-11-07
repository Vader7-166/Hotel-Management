using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm để dùng [ForeignKey]

namespace Hotel_Management.Models;

public partial class Account
{
    [Key] // Khóa chính
    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
    [Display(Name = "Username")] 
    public string Username { get; set; } = null!;

    
    [Required(ErrorMessage = "Password hash is required")]
    [StringLength(255, ErrorMessage = "Password hash cannot exceed 255 characters")]
    public string PasswordHash { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")] // Bắt buộc nhập
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")] // Giới hạn độ dài
    [EmailAddress(ErrorMessage = "Invalid email format")] // Kiểm tra định dạng email
    [Display(Name = "Email")] 
    public string Email { get; set; } = null!;

    
    [Display(Name = "Employee ID")] 
   
    public int? EmployeeId { get; set; }

    [Display(Name = "Customer ID")] // Nhãn hiển thị
    public int? CustomerId { get; set; }

    [Required(ErrorMessage = "Role is required")] // Bắt buộc nhập
    [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters")] // Giới hạn độ dài
    [Display(Name = "Role")] // Nhãn hiển thị
    public string Role { get; set; } = null!;

    [Display(Name = "Is Active?")] // Nhãn hiển thị
    public bool IsActive { get; set; }

    [Display(Name = "Created At")] // Nhãn hiển thị
    public DateTime CreatedAt { get; set; }

    [Display(Name = "Updated At")] // Nhãn hiển thị
    public DateTime? UpdatedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public virtual Employee? Employee { get; set; }
}