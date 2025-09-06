namespace StaffHub.Models.ViewModels
{
    public class EmployeeCreateVM
    {
        // Employee fields
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public decimal Salary { get; set; }
        public int RoleId { get; set; }

        // Identity fields
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PermissionRole { get; set; } = null!;
    }
}
