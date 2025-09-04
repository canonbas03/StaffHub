using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace StaffHub.Models
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public ICollection<Role> Roles { get; set; } = new List<Role>();

    }
}
