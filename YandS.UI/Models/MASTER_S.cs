namespace YandS.UI.Models
{
    public enum MASTER_S : ushort
    {
        CourtType = 1,
        Banks = 2,
        AccoutTitle = 3,
        Location = 4,
        AccountNumber = 5,
        Gender = 6,
        PaymentHeads = 7,
        PaymentMode = 8,
        PaymentType = 9,
        ChequeStatus = 10,
        CourtLocation = 11,
        CaseAgainst = 12,
        ReceiveLevel = 13,
        CaseType = 14,
        CaseLevel = 15,
        ODBLoan = 16,
        CourtDepartment = 17,
        ODBBankBranch = 18,
        Nationality = 196,
        Department = 197,
        Designation = 206,
        PayTo = 214,
        DocumentType = 218,
        VoucherType = 224,
        VoucherStatus = 228,
        FeesType = 232,
        Branch = 236,
        Client = 241,
        InvoiceStatus = 247,
        CaseStatus = 252,
        EnforcementLevel = 265,
        CauseOfSuspension = 272,
        ArrestOrderStatus = 280,
        ClientCaseType = 285,
        FeeClassification = 289,
        ParentCourt = 298,
        ApealByWho = 391,
        FileCloseReason = 395,
        PVTransType = 401,
        PVTransReason = 402,
        LoanManager = 428,
        OmaniExp = 460,
        CurrentCaseLevel = 499,
        ClosureLevel = 500,
        FileAllocation = 501,
        ClientClassification = 523,
        CaseSubject = 532,
        FeeTypeCascadeDetail = 567,
        BeforeCourtStage = 568,
        EnforcementStage = 569,
        CounsultingFeeType = 570,
        ActionLevel = 785,
        EnforcementDispute = 786,
        FileStatus = 788,
        OnHoldReason = 821,
        DepartmentType = 822,
        SessionLevel = 858,
        SessionCaseType = 859,
        SessionLawyers = 860,
        SessionFollower = 861,
        SessionClients = 913,
        SessionFileStatus = 1091,
        SessionOnHold = 1092,
        ReconciliationDeptt = 1152,
        Governorate = 1153,
        EnfcAdmin = 1154,
        AnnouncementType = 1288,
        InquiryResult = 1289,
        AuctionProcess = 1290,
        JudicialDecision = 1291,
        CauseOfRecovery = 1292,
        TransportationSource = 1385,
        CallerName = 1408,
        DisputeLevel = 1450,
        DisputeType = 1451,
        JudgementLevel = 1465
    }
    public enum Courts : ushort
    {
        PrimaryCourt = 1,
        ApealCourt = 2,
        SupremeCourt = 3,
        EnforcementCourt = 4,
        
    }

    public class OfficeFileStatus
    {
        private OfficeFileStatus(string value) { Value = value; }

        public string Value { get; private set; }

        public static OfficeFileStatus Trace { get { return new OfficeFileStatus("Trace"); } }
        public static OfficeFileStatus Debug { get { return new OfficeFileStatus("Debug"); } }
        public static OfficeFileStatus Info { get { return new OfficeFileStatus("Info"); } }
        public static OfficeFileStatus Warning { get { return new OfficeFileStatus("Warning"); } }
        public static OfficeFileStatus Error { get { return new OfficeFileStatus("Error"); } }

        public override string ToString()
        {
            return Value;
        }
    }
}