using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Course
    {
        public Course() { 
            Batches = new List<Batch>();
        }
        public int CourseId { get; set; }
        public string CourseName { get; set; }

        public int Fees { get; set; }
        public List<Batch> Batches { get; set; }
    }
}
