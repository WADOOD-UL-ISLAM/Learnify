namespace LearnifyD1.Models.ViewModels
{
    public class UnpaidFeeViewModel
    {
        public string StudentName { get; set; }
        public string StudentMobile { get; set; }
        public string GuardianMobile { get; set; }
        public string BatchName { get; set; }
        public decimal TotalFee { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount => TotalFee - PaidAmount;
    }
}
