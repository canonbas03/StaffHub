using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace StaffHub.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        [Column(TypeName ="decimal(18,2)")]
        public decimal Salary { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; } = null!;
    }
}
