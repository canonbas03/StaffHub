using System.ComponentModel.DataAnnotations;

namespace StaffHub.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;

        public virtual Department Department { get; set; } = null!;
    }
}
