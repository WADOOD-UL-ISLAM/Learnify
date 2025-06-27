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

        public string FatherName { get; set; }
        public string CNIC {  get; set; }
        public DateTime DateofBirth { get; set; }
        public string Gender { get; set; }
        public int age { get; set; }

        public string StudentMobileNumber { get; set; }

        public string GuardianMobileNumber { get; set; }

        public string Address { get; set; }




        [NotMapped]
        public string UniqueId => $"LA-{StudentId}-{DateTime.Now.Year}";
        public List<StudentBatch> studentBatches { get; set; }
    }

}
