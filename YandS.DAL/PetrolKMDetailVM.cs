namespace YandS.DAL
{
    public class PetrolKMDetailVM
    {
        public string PV_No { get; set; }
        public string Voucher_Date { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string Payment_Head_Remarks { get; set; }
        public string FullName { get; set; }
        public int RunningKM { get; set; }
    }
    public class PayVoucherCreatedVM
    {
        public string CaseLevel { get; set; }
        public string PaymentHeadName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentToName { get; set; }
        public string W_D_Date { get; set; }
    }
}