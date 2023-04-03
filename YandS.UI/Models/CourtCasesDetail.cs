namespace YandS.UI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class CourtCasesDetail : Base
    {
        [Key]
        public int DetailId { get; set; }
        public int CaseId { get; set; }
        public CourtCases CourtCases { get; set; }

        [Display(Name = "Court")]
        [StringLength(3)]
        public string Courtid { get; set; } //Dropdown Court

        [Display(Name = "Court Registration No")]
        [StringLength(100)]
        public string CourtRefNo { get; set; }

        [Display(Name = "Court Location")]
        [StringLength(5)]
        public string CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Registration Date")]
        [Column(TypeName = "datetime2")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        [StringLength(10)]
        public string CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "Case Level")]
        [StringLength(2)]
        public string CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        [StringLength(5)]
        public string CourtStatus { get; set; }
        [Display(Name = "Apeal By Who")]
        [StringLength(2)]
        public string ApealByWho { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Judgement Date")]
        public DateTime? JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Judgement Receiving Date")]
        [Column(TypeName = "datetime2")]
        public DateTime? JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string JudgementDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [RestrictFutureDate(ErrorMessage = "Future date not allowed for Final Closure Date")]
        [Display(Name = "Closure Date")]
        [Column(TypeName = "datetime2")]
        public DateTime? ClosureDate { get; set; }

        [Display(Name = "Closed By")]
        [StringLength(50)]
        public string ClosedbyStaff { get; set; }
        [StringLength(50)]
        public string NextCaseLevel { get; set; } //Dropdown 
        [StringLength(100)]
        public string NextCaseLevelCode { get; set; }
        public ICollection<CourtCasesFollowup> FollowupId { get; set; }

        public CourtCasesDetail()
        {
            FollowupId = new HashSet<CourtCasesFollowup>();
        }

        #region NotMapped Names

        [NotMapped]
        public string CourtName { get; set; }
        [NotMapped]
        public string CourtLocationName { get; set; }
        [NotMapped]
        public string CourtDepartmentName { get; set; }
        [NotMapped]
        public string CaseLevelName { get; set; }

        #endregion
    }

    #region Primary/Apeal/Supreme/Enforcement Court View Modal

    public class CourtCasesDetailVM
    {
        public int DetailId { get; set; }
        public int CaseId { get; set; }

        [Display(Name = "COURT")]
        public string Courtid { get; set; } //Dropdown Court

        [Required(ErrorMessage = "Court Registration Number is Required")]
        [Display(Name = "COURT REGISTRATION NO")]
        public string CourtRefNo { get; set; }

        [Required(ErrorMessage = "Court Location is Required")]
        [Display(Name = "COURT")]
        public string CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Court Registration Date is Required")]
        [RestrictFutureDate(ErrorMessage = "Future Date not allowed for Registration Date")]
        [Display(Name = "REGISTRATION DATE")]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        public string CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "CASE LEVEL")]
        public string CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        public string CourtStatus { get; set; }

        [Required(ErrorMessage = "Please Select Appeal by Who")]
        //[Required(ErrorMessage = "This is Required")]
        [Display(Name = "Apeal By Who")]
        public string ApealByWho { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [RestrictFutureDate(ErrorMessage = "Future Date not allowed for Judgement Date")]
        [DateGreaterThan(otherPropertyName = "RegistrationDate", ErrorMessage = "Judgement Date must be greater than Registration Date")]
        [Display(Name = "Judgement Date")]
        public DateTime? JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [RestrictFutureDate(ErrorMessage = "Future Date not allowed for Judgement Receiving Date")]
        [DateGreaterThan(otherPropertyName = "JudgementDate", ErrorMessage = "Judgement Receiving Date must be greater than Judgement Date")]
        [Display(Name = "Judgement Receiving Date")]
        public DateTime? JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string JudgementDetails { get; set; }

        public string CourtName { get; set; }
        public string CourtLocationName { get; set; }
        public string CourtDepartmentName { get; set; }
        public string CaseLevelName { get; set; }
        [Display(Name = "Y & S Updates")]
        public string YandSNotesUpdate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [RestrictFutureDate(ErrorMessage = "Future date not allowed for Final Closure Date")]
        [Display(Name = "Closure Date")]
        public DateTime? ClosureDate { get; set; }

        [Display(Name = "Closed By")]
        public string ClosedbyStaff { get; set; }

        //[Required(ErrorMessage = "This is Required")]
        [Display(Name = "Next Case Level")]
        public string NextCaseLevel { get; set; } //Dropdown 

        //[Required(ErrorMessage = "This is Required")]
        [Display(Name = "Next Case Level Code")]
        public string NextCaseLevelCode { get; set; }
        public string RealEstateYesNo { get; set; }
        public string RealEstateDetail { get; set; }
        public string ClaimSummary { get; set; }

        #region SESSION ROLL VM

        public int SessionRollId { get; set; }

        #region SESSION ROLL VM - UPDATE

        //[Required(ErrorMessage = "This is Required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? CurrentHearingDate { get; set; }
        //[Required(ErrorMessage = "This is Required")]
        public string CourtDecision { get; set; }
        public string SessionRemarks { get; set; }
        public string Requirements { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "FIRST EMAIL DATE تاريخ الايميل الأول")]
        public DateTime? FirstEmailDate { get; set; }
        [Display(Name = "CLIENT REPLY رد الموكل")]
        public string ClientReply { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? NextHearingDate { get; set; }
        [Display(Name = "SOURCE المصدر")]
        public string TransportationSource { get; set; }
        public string TransportationFee { get; set; }
        #endregion

        #region SESSION ROLL VM - FOLLOW

        //[Required(ErrorMessage = "This is Required")]
        public string SessionClientId { get; set; }
        //[Required(ErrorMessage = "This is Required")]
        public string ClientName { get; set; }
        public string Defendant { get; set; }
        public string OfficeFileNo { get; set; }
        public string SessionRollClientName { get; set; }
        public string SessionRollDefendentName { get; set; }
        public bool ShowFollowup { get; set; }
        public bool ShowSuspend { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastDate { get; set; }
        public string WorkRequired { get; set; }
        public string SessionNotes { get; set; }
        public string FollowerId { get; set; }
        public string SuspendedWorkRequired { get; set; }
        public string SuspendedSessionNotes { get; set; }
        public string UpdatedOn { get; set; }

        #endregion


        #endregion
        public CourtCasesDetailVM()
        {
            this.NextCaseLevel = "";
            this.SessionClientId = "0";
            this.FollowerId = "0";
        }
    }
    public class PrimaryCourtViewModal
    {
        public int Primary_DetailId { get; set; }
        public int Primary_CaseId { get; set; }

        [Display(Name = "Court")]
        public string Primary_Courtid { get; set; } //Dropdown Court

        [Display(Name = "Court RefNo")]
        public string Primary_CourtRefNo { get; set; }

        [Display(Name = "Court Location")]
        public string Primary_CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        //[CheckDateRange]
        [Display(Name = "Registration Date")]
        public DateTime? Primary_RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        public string Primary_CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "Case Level")]
        public string Primary_CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        public string Primary_CourtStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        //[CheckDateRange]
        [Display(Name = "Judgement Date")]
        public DateTime? Primary_JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        //[CheckDateRange]
        [Display(Name = "Judgement Receiving Date")]
        public DateTime? Primary_JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string Primary_JudgementDetails { get; set; }

        public string Primary_CourtName { get; set; }
        public string Primary_CourtLocationName { get; set; }
        public string Primary_CourtDepartmentName { get; set; }
        public string Primary_CaseLevelName { get; set; }

    }
    public class ApealCourtViewModal
    {
        public int Apeal_DetailId { get; set; }
        public int Apeal_CaseId { get; set; }

        [Display(Name = "Court")]
        public string Apeal_Courtid { get; set; } //Dropdown Court

        [Required(ErrorMessage = "Apeal Court RefNo is required")]
        [Display(Name = "Court RefNo")]
        public string Apeal_CourtRefNo { get; set; }

        [Display(Name = "Court Location")]
        public string Apeal_CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Registration Date")]
        public DateTime? Apeal_RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        public string Apeal_CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "Case Level")]
        public string Apeal_CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        public string Apeal_CourtStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Judgement Date")]
        public DateTime? Apeal_JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Judgement Receiving Date")]
        public DateTime? Apeal_JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string Apeal_JudgementDetails { get; set; }

        public string Apeal_CourtName { get; set; }
        public string Apeal_CourtLocationName { get; set; }
        public string Apeal_CourtDepartmentName { get; set; }
        public string Apeal_CaseLevelName { get; set; }

    }
    public class SupremeCourtViewModal
    {
        public int Supreme_DetailId { get; set; }
        public int Supreme_CaseId { get; set; }

        [Display(Name = "Court")]
        public string Supreme_Courtid { get; set; } //Dropdown Court

        [Required(ErrorMessage = "Supreme Court RefNo is required")]
        [Display(Name = "Court RefNo")]
        public string Supreme_CourtRefNo { get; set; }

        [Display(Name = "Court Location")]
        public string Supreme_CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Registration Date")]
        public DateTime? Supreme_RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        public string Supreme_CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "Case Level")]
        public string Supreme_CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        public string Supreme_CourtStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Judgement Date")]
        public DateTime? Supreme_JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Judgement Receiving Date")]
        public DateTime? Supreme_JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string Supreme_JudgementDetails { get; set; }

        public string Supreme_CourtName { get; set; }
        public string Supreme_CourtLocationName { get; set; }
        public string Supreme_CourtDepartmentName { get; set; }
        public string Supreme_CaseLevelName { get; set; }

    }
    public class EnforcementCourtViewModal
    {
        public int Enforcement_DetailId { get; set; }
        public int Enforcement_CaseId { get; set; }

        [Display(Name = "Court")]
        public string Enforcement_Courtid { get; set; } //Dropdown Court

        [Required(ErrorMessage = "Enforcement Court RefNo is required")]
        [Display(Name = "Court RefNo")]
        public string Enforcement_CourtRefNo { get; set; }

        [Display(Name = "Court Location")]
        public string Enforcement_CourtLocationid { get; set; } //Dropdown Location

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Registration Date")]
        public DateTime? Enforcement_RegistrationDate { get; set; }

        [Display(Name = "Court Department")]
        public string Enforcement_CourtDepartment { get; set; } //Dropdown 

        [Display(Name = "Case Level")]
        public string Enforcement_CaseLevelCode { get; set; } //Dropdown 

        [Display(Name = "Court Status")]
        public string Enforcement_CourtStatus { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        [Display(Name = "Judgement Date")]
        public DateTime? Enforcement_JudgementDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Judgement Receiving Date")]
        //[System.Web.Mvc.Remote("ValidateDateEqualOrGreater", "CourtCases", HttpMethod = "Post", ErrorMessage = "Date greater than current date is not allowed.")]
        [CheckDateRange]
        public DateTime? Enforcement_JudgementReceivingDate { get; set; }

        [Display(Name = "Judgement Details")]
        public string Enforcement_JudgementDetails { get; set; }

        public string Enforcement_CourtName { get; set; }
        public string Enforcement_CourtLocationName { get; set; }
        public string Enforcement_CourtDepartmentName { get; set; }
        public string Enforcement_CaseLevelName { get; set; }

    }


    #endregion
}