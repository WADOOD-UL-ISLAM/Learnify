using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class StudentBatch
    {
        public int EnrollmentId { get; set; }
        public DateTime dateTime { get; set; }

       
        
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student? student { get; set; }


        public int BatchId {  get; set; }
        [ForeignKey("BatchId")]
        public Batch? batch { get; set; }
    }
}
