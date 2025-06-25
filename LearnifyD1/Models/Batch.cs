using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class Batch
    {
        public Batch()
        {
            studentbatches = new List<StudentBatch>();
        }

        public int BatchId { get; set; }
        public string BatchName { get; set; }

        public DateTime time { get; set; } // Keep this if you're using for created time

        public int Slots { get; set; }

        // Step 1.1 - Add batch day category
        public string BatchDays { get; set; }

        // Step 1.2 - Store months as int (1 = Jan, 12 = Dec)
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }

        // Step 1.3 - TimeOnly requires .NET 6+. Use string if older
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        // Course navigation
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public List<StudentBatch> studentbatches { get; set; }

        [NotMapped]
        public string SpecialId => $"{BatchId}-{BatchName}-{time:yyyy}";
    }

}
