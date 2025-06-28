using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearnifyD1.Models
{
    public class FeeRecord
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public int BatchId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        [ForeignKey("BatchId")]
        public Batch Batch { get; set; }

        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        public int AmountPaid { get; set; }

        
    }

}
