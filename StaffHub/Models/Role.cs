using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffHub.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [ForeignKey("Department")]
        public int DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
    }
}
