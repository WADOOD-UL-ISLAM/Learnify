using System.ComponentModel.DataAnnotations;

namespace LearnifyD1.Models
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
