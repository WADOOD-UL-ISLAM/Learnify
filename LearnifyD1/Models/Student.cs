using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Student
    {
        public Student() {
            studentBatches = new List<StudentBatch>();
        }
        public int StudentId { get; set; }

        public string StudentName { get; set; }

        public string StudentEmail { get; set; }
        
        public List<StudentBatch> studentBatches { get; set; }

        


    }
}
