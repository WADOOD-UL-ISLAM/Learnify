using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Student
    {
        public Student()
        {
            studentBatches = new List<StudentBatch>();
        }

        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student Name is required")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string StudentEmail { get; set; }

        [Display(Name = "Upload Image")]
        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        public string? ImagePath { get; set; }

        [NotMapped]
        public string UniqueId => $"LA-{StudentId}-{DateTime.Now.Year}";
        public List<StudentBatch> studentBatches { get; set; }
    }

}
