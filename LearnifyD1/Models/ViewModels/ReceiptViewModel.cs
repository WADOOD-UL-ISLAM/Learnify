namespace LearnifyD1.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public string ReceiptNumber { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public DateTime PaymentDate { get; set; }
        public int AmountPaid { get; set; }
        public string AmountInWords { get; set; }
        public string CourseName { get; set; }
        public string PaymentMethod { get; set; } // Cash, Cheque, etc.
        public int PaidSoFar { get; set; }
        public int Due { get; set; }

        public List<FeeRecord> FeeRecords { get; set; } // 👈 Add this
    }
}
