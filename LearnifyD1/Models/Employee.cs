using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        // Foreign key
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
