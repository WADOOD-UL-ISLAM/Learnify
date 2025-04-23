using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Batch
    {
        public Batch() {
            studentbatches = new List<StudentBatch>();
        }
        public int BatchId { get; set; }

        public string BatchName { get; set; }

        public DateTime time { get; set; }

        public int Slots { get; set; }

        [NotMapped]
        public string SpecialId => $"{BatchId}-{BatchName}-{time:yyyy}";

       


        public int CourseId {  get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public List<StudentBatch> studentbatches { get; set; }
       



    }
}
