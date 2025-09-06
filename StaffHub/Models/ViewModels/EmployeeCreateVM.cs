using System.ComponentModel.DataAnnotations;

namespace StaffHub.Models.ViewModels
{
    public class EmployeeCreateVM
    {
        // Employee fields
        public int EmployeeId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public decimal Salary { get; set; }
        public int RoleId { get; set; }

        // Identity fields
        public string Email { get; set; } = null!;
        [Required]
        public string? Password { get; set; } = null!;
        public string PermissionRole { get; set; } = null!;
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}
