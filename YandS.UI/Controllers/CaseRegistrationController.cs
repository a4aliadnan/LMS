using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using YandS.DAL;
using YandS.UI.Models;

namespace YandS.UI.Controllers
{
    [RBAC]
    public class CaseRegistrationController : Controller
    {
        private RBACDbContext db = new RBACDbContext();
        private string OfficeFileFilterTBR = Helper.getOfficeFileFilterTBR();
        private string OfficeFileFilterSR = Helper.getOfficeFileFilterSR();
        private string OfficeFileFilterENF = Helper.getOfficeFileFilterENF();
        private string OfficeFileFilterSUP = Helper.getOfficeFileFilterSUP();
        private string[] FileStatusCodesSR = Helper.getFileStatusCodesSR();
        private string[] FileStatusCodesTBR = Helper.getFileStatusCodesTBR();

        public ActionResult Index()
        {
            if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
            {
                ViewBag.LocationId = "A";
                ViewBag.UserRole = "VoucherApproval";
                ViewBag.VoucherApproval = "checked";
            }
            else
                ViewBag.LocationId = Helper.GetEmployeeLocation(User.Identity.Name).Split('-')[1];

            return View();
        }

        public ActionResult IndexMain(int? id)
        {
            if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
            {
                ViewBag.LocationId = "M";
                ViewBag.UserRole = "VoucherApproval";
                ViewBag.VoucherApproval = "checked";
            }
            else
                ViewBag.LocationId = Helper.GetEmployeeLocation(User.Identity.Name).Split('-')[1];

            ViewBag.OfficeFileNo = "";
            ViewBag.IsEdit = "";

            ViewBag.CaseRegisterActive = "";
            ViewBag.CasePaymentActive = "";
            ViewBag.CaseRegNewCaseActive = "";
            ViewBag.CaseRegAppealActive = "";
            ViewBag.CaseRegSupremeActive = "";

            ViewBag.CaseRegisterId = "0";
            ViewBag.HFCaseId = "0";
            ViewBag.HFClaimAmount = "0";
            ViewBag.Div_OfficeFileNoMain = "";
            ViewBag.Div_OfficeFileNo = "AppHidden";

            if (id == null)
            {
                ViewBag.FormMode = "CREATE";
                ViewBag.StartTAB = "btn_CasePayment";
                ViewBag.CasePaymentActive = "CasePaymentActive";
                ViewBag.LoadTable = "tablePayment";
            }
            else if (id == -1)
            {
                ViewBag.StartTAB = "btn_CaseRegNewCase";
                ViewBag.CaseRegNewCaseActive = "CaseRegNewCaseActive";
                ViewBag.LoadTable = "tableNewCases";
            }
            else
            {
                var caseRegistration = db.CaseRegistration.Find(id);
                var courtCases = db.CourtCase.Find(caseRegistration.CaseId);

                ViewBag.FormMode = "EDIT";
                ViewBag.IsEdit = "disabled";
                ViewBag.HFCaseId = caseRegistration.CaseId;
                ViewBag.HFClaimAmount = courtCases.ClaimAmount;
                ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
                ViewBag.CaseRegisterId = id;
                ViewBag.StartTAB = "btn_CaseRegister";

                ViewBag.CaseRegisterActive = "CaseRegisterActive";
                ViewBag.LoadTable = "btn_CaseRegister";
                ViewBag.Div_OfficeFileNoMain = "AppHidden";
                ViewBag.Div_OfficeFileNo = "";

            }
            return View();
        }

        [HttpPost]
        public ActionResult IndexMain(CaseRegistrationVM modal, HttpPostedFileBase upload, HttpPostedFileBase uploadPVSupDocs, HttpPostedFileBase uploadPVBTDocs)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            string UploadRoot = Helper.GetStorageRoot;

            MessageVM ProcessMessage = new MessageVM { Category = "Success", Message = "Data Save Successfully" };

            try
            {
                #region VALID MODAL PROCESSING

                if (ModelState.IsValid)
                {
                    CaseRegistration ModelToSave = new CaseRegistration();
                    if (!string.IsNullOrEmpty(modal.PartialViewName))
                    {
                        if (modal.CaseRegistrationId > 0) //EDIT
                        {
                            ModelToSave = db.CaseRegistration.Find(modal.CaseRegistrationId);
                            UpdateCaseRegister(modal, ref ModelToSave);

                            UpdateCourtCaseUpdate(modal);
                        }
                    }
                    
                    #region UPLOADED DOCUMENT SAVING

                    if (modal.FileStatus == OfficeFileStatus.LegalNotice.ToString())
                    {
                        //Process Attachment Legal Notice (OMAN POST)
                        if (upload != null && upload.ContentLength > 0)
                        {
                            string FileExtension = Path.GetExtension(upload.FileName);

                            string FileName = ModelToSave.CaseRegistrationId + FileExtension;

                            string UploadPath = Path.Combine(UploadRoot, "OmanPost", FileName);

                            upload.SaveAs(UploadPath);
                        }
                    }
                    else if (modal.FileStatus == OfficeFileStatus.ForPayment.ToString())
                    {
                        //Process Attachment Payment Vouchers
                        if (uploadPVSupDocs != null && uploadPVSupDocs.ContentLength > 0)
                        {
                            string FileExtension = Path.GetExtension(uploadPVSupDocs.FileName);

                            string FileName = ModelToSave.Voucher_No + FileExtension;

                            string UploadPath = Path.Combine(UploadRoot, "PVDocuments", FileName);

                            uploadPVSupDocs.SaveAs(UploadPath);
                        }

                        if (uploadPVBTDocs != null && uploadPVBTDocs.ContentLength > 0)
                        {
                            string FileExtension = Path.GetExtension(uploadPVBTDocs.FileName);

                            string FileName = ModelToSave.Voucher_No + FileExtension;

                            string UploadPath = Path.Combine(UploadRoot, "PVTransfers", FileName);

                            uploadPVBTDocs.SaveAs(UploadPath);
                        }
                    }
                    
                    #endregion

                    #region RETURN
                    if (!string.IsNullOrEmpty(modal.PartialViewName))
                    {

                        if (!string.IsNullOrEmpty(modal.redirectTo))
                        {
                            return Json(new { redirectTo = modal.redirectTo });
                        }
                        else
                        {
                            if (modal.PartialViewName == "TBR-REGISTERED" || modal.PartialViewName == "TBR-STOP_REGISTRATION")
                                return Json(new { sameDTLoad = "Y" });

                            if (modal.PartialViewName == "TBR-PAYMENT" && modal.IsShowWithLawyer == "Y")
                                return Json(new { ManualTrigger = "Y", OfficeFileNo = modal.OfficeFileNo });


                            CaseRegistrationVM ViewModal = new CaseRegistrationVM();
                            Helper.GetCaseRegistrationVM(modal.CaseRegistrationId, ref ViewModal);

                            ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);
                            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);
                            ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", ViewModal.DepartmentType);
                            ViewBag.DisputeLevel = new SelectList(Helper.GetDisputeLevel(), "Mst_Value", "Mst_Desc", ViewModal.DisputeLevel);
                            ViewBag.DisputeType = new SelectList(Helper.GetDisputeType(), "Mst_Value", "Mst_Desc", ViewModal.DisputeType);
                            ViewBag.Session_CaseType = new SelectList(Helper.GetSessionCaseType(), "Mst_Value", "Mst_Desc");
                            ViewBag.Session_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc");

                            ViewBag.CaseRegistrationId = modal.CaseRegistrationId;
                            ViewBag.hid_DetailId = ViewModal.DetailId;

                            return PartialView("_TBR_Modify", ViewModal);
                        }
                    }
                    else
                    {
                        ProcessMessage.IsShowForm = "Y";
                        ProcessMessage.CaseId = ModelToSave.CaseId.ToString();
                        Session["Message"] = ProcessMessage;

                        if (ProcessMessage.id == -1)
                            return RedirectToAction("IndexMain", new RouteValueDictionary(new { ProcessMessage.id }));
                        else
                            return RedirectToAction("IndexMain", new RouteValueDictionary(new { id = ModelToSave.CaseRegistrationId }));

                    }
                    #endregion
                }

                #endregion

                #region INVALID MODAL RETURN

                if (!string.IsNullOrEmpty(modal.PartialViewName))
                {
                    ViewBag.ConsultantId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", modal.ConsultantId);
                    ViewBag.OnHoldReasonDDL = new SelectList(Helper.GetOnHoldReason(), "Mst_Value", "Mst_Desc", modal.FileStatus == "4" ? modal.FileStatusRemarks : "0");
                    ViewBag.OnHoldDone = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.OnHoldDone);
                    ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.CourtFollow);
                    ViewBag.LawyerId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", modal.LawyerId);
                    ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", modal.GovernorateId);
                    ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(modal.ActionLevel), "Mst_Value", "Mst_Desc", modal.CourtLocationid);

                    ViewBag.RealEstateYesNo = new SelectList(Helper.GetYNForSelect(), "Mst_Value", "Mst_Desc", modal.RealEstateYesNo);
                    ViewBag.StopEnfRequest = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", modal.StopEnfRequest);

                    ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR), "Mst_Value", "Mst_Desc", modal.FileStatus);
                    ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.ClientReply);
                    ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", modal.TransportationSource);
                    ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", modal.DepartmentType);

                    ViewBag.DisputeLevel = new SelectList(Helper.GetDisputeLevel(), "Mst_Value", "Mst_Desc", modal.DisputeLevel);
                    ViewBag.DisputeType = new SelectList(Helper.GetDisputeType(), "Mst_Value", "Mst_Desc", modal.DisputeType);
                    ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", modal.CourtDepartment);

                    ViewBag.CaseRegistrationId = modal.CaseRegistrationId;
                    ViewBag.hid_DetailId = modal.DetailId;



                    string OmanPostDoc = Helper.GetOmanPostDoc(modal.CaseRegistrationId);

                    if (OmanPostDoc == "#")
                    {
                        ViewBag.View_OmanPostDoc = "AppHidden";
                    }
                    else
                    {
                        ViewBag.View_OmanPostDoc = "";
                        ViewBag.OmanPostDoc = OmanPostDoc;
                    }

                    string StopRegEmailsDoc = Helper.GetStopRegEmails_Doc(modal.CaseRegistrationId);

                    if (StopRegEmailsDoc == "#")
                    {
                        ViewBag.View_StopRegEmailsDoc = "AppHidden";
                    }
                    else
                    {
                        ViewBag.View_StopRegEmailsDoc = "";
                        ViewBag.StopRegEmailsDoc = StopRegEmailsDoc;
                    }

                    return Json(new {
                        Category = "Error",
                        Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
                    });
                }
                else
                {
                    ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");
                    ViewBag.OnHoldReasonDDL = new SelectList(Helper.GetOnHoldReason(), "Mst_Value", "Mst_Desc");
                    ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc");

                    ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc");
                    ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                    ViewBag.Payment_Head = new SelectList(Helper.LoadPayFor("R"), "Mst_Value", "Mst_Desc");
                    ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");

                    Session["Message"] = new MessageVM
                    {
                        Category = "Error",
                        Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
                    };

                }

                return View(modal);

                #endregion
            }
            catch (Exception ex)
            {
                Session["Message"] = new MessageVM
                {
                    Category = "Error",
                    Message = ex.Message
                };
                return View(modal);
            }

        }

        private void SaveCaseRegister(CaseRegistrationVM modal, ref CaseRegistration ModelToSave)
        {
            ModelToSave.CaseId = modal.CaseId;
            ModelToSave.ActionDate = modal.ActionDate;
            ModelToSave.ActionLevel = modal.ActionLevel;
            ModelToSave.JudgementDate = modal.JudgementDate;
            ModelToSave.UrgentCaseDays = modal.UrgentCaseDays;
            ModelToSave.EnforcementDispute = modal.EnforcementDispute;
            ModelToSave.CourtRegistration = modal.CourtRegistration;
            ModelToSave.FileStatus = modal.FileStatus;

            if (int.Parse(modal.FileStatus) == 4)
                ModelToSave.FileStatusRemarks = modal.OnHoldReasonDDL;
            else if(int.Parse(modal.FileStatus) == 6)
                ModelToSave.ElectronicNo = modal.FileStatusRemarks;
            else if(int.Parse(modal.FileStatus) == 8 && !string.IsNullOrEmpty(modal.FileStatusRemarks))
                ModelToSave.CourtFeeAmount = decimal.Parse(modal.FileStatusRemarks);
            else
                ModelToSave.FileStatusRemarks = modal.FileStatusRemarks;

            ModelToSave.CourtMessage = modal.CourtMessage;
            ModelToSave.SendDate = modal.SendDate;
            ModelToSave.FirstReminderDate = modal.FirstReminderDate;
            ModelToSave.ReminderNo = modal.ReminderNo;
            ModelToSave.CourtRequestDate = modal.CourtRequestDate;
            ModelToSave.OfficeProcedure = modal.OfficeProcedure;
            ModelToSave.PaymentDate = modal.PaymentDate;
            ModelToSave.CourtDetailRegistered = modal.CourtDetailRegistered; //THIS IS URGENT NOW BOOLEAN
            ModelToSave.AdminFile = modal.AdminFile;
            ModelToSave.OmanPostNo = modal.OmanPostNo;
            ModelToSave.DepartmentType = modal.DepartmentType;
            ModelToSave.Voucher_No = modal.Voucher_No;
            ModelToSave.FormPrintDefendant = modal.FormPrintDefendant;
            ModelToSave.FormPrintLastDate = modal.FormPrintLastDate;
            ModelToSave.FormPrintWorkRequired = modal.FormPrintWorkRequired;
            ModelToSave.Remarks = modal.MainRemarks;
            ModelToSave.RealEstateYesNo = modal.RealEstateYesNo;
            ModelToSave.RealEstateDetail = modal.RealEstateDetail;
            ModelToSave.ClaimSummary = modal.ClaimSummary;

            db.CaseRegistration.Add(ModelToSave);
            db.SaveChanges();
        }
        private void UpdateCaseRegister(CaseRegistrationVM modal, ref CaseRegistration ModelToSave)
        {
            if (!string.IsNullOrEmpty(modal.PartialViewName))
            {
                DateTime? CommissioningDate = null;

                db.Entry(ModelToSave).Entity.FormPrintWorkRequired = modal.FormPrintWorkRequired;
                db.Entry(ModelToSave).Entity.OfficeProcedure = modal.OfficeProcedure;
                db.Entry(ModelToSave).Entity.FormPrintLastDate = modal.FormPrintLastDate;
                if (modal.IsShowWithLawyer == "Y")
                    db.Entry(ModelToSave).Entity.FileStatus = OfficeFileStatus.WithLawyer.ToString();
                else
                    db.Entry(ModelToSave).Entity.FileStatus = modal.FileStatus;

                db.Entry(ModelToSave).Entity.ActionDate = modal.ActionDate;
                db.Entry(ModelToSave).Entity.Remarks = modal.MainRemarks;

                CourtCases courtCases = db.CourtCase.Find(modal.CaseId);

                if (modal.FileStatus == ModelToSave.FileStatus)
                {
                    if(modal.CourtFollow == "1")
                    {
                        db.Entry(ModelToSave).Entity.LawyerId = modal.LawyerId;

                        db.Entry(courtCases).Entity.CourtFollow = modal.CourtFollow;
                        db.Entry(courtCases).Entity.CourtFollowRequirement = modal.CourtFollowRequirement;
                        db.Entry(courtCases).Entity.CommissioningDate = modal.CommissioningDate;
                    }
                    else
                    {
                        db.Entry(ModelToSave).Entity.LawyerId = "0";
                        db.Entry(courtCases).Entity.CourtFollow = "0";
                        db.Entry(courtCases).Entity.CourtFollowRequirement = "";
                        db.Entry(courtCases).Entity.CommissioningDate = CommissioningDate;
                    }
                }
                else
                {
                    if (modal.IsShowWithLawyer == "Y")
                    {
                        if (modal.CourtFollow == "1")
                        {
                            db.Entry(ModelToSave).Entity.LawyerId = modal.LawyerId;

                            db.Entry(courtCases).Entity.CourtFollow = modal.CourtFollow;
                            db.Entry(courtCases).Entity.CourtFollowRequirement = modal.CourtFollowRequirement;
                            db.Entry(courtCases).Entity.CommissioningDate = modal.CommissioningDate;
                        }
                        else
                        {
                            db.Entry(ModelToSave).Entity.LawyerId = "0";
                            db.Entry(courtCases).Entity.CourtFollow = "0";
                            db.Entry(courtCases).Entity.CourtFollowRequirement = "";
                            db.Entry(courtCases).Entity.CommissioningDate = CommissioningDate;
                        }
                    }
                    else
                    {
                        db.Entry(ModelToSave).Entity.LawyerId = "0";

                        db.Entry(courtCases).Entity.CourtFollow = "0";
                        db.Entry(courtCases).Entity.CourtFollowRequirement = "";
                        db.Entry(courtCases).Entity.CommissioningDate = CommissioningDate;
                    }
                }

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();


                if (int.Parse(modal.ActionLevel) == 2)
                {
                    db.Entry(ModelToSave).Entity.DepartmentType = modal.DepartmentType;
                }

                if (modal.FileStatus == OfficeFileStatus.WritingSubmission.ToString() || modal.FileStatus == OfficeFileStatus.Scanned.ToString())
                {
                    db.Entry(ModelToSave).Entity.ActionDate = DateTime.UtcNow.AddHours(4);
                }

                if (int.Parse(modal.ActionLevel) == 4)
                {
                    db.Entry(ModelToSave).Entity.DisputeLevel = modal.DisputeLevel;
                    db.Entry(ModelToSave).Entity.DisputeType = modal.DisputeType;
                }

                switch (modal.PartialViewName)
                {
                    case "TBR-TRANSFER":
                        db.Entry(ModelToSave).Entity.ConsultantId = modal.ConsultantId;
                        break;
                    case "TBR-LEGAL_NOTICE":
                        db.Entry(ModelToSave).Entity.SendDate = modal.SendDate;
                        db.Entry(ModelToSave).Entity.OmanPostNo = modal.OmanPostNo;
                        db.Entry(ModelToSave).Entity.FirstReminderDate = modal.FirstReminderDate;
                        break;
                    case "TBR-ON_HOLD":
                        db.Entry(ModelToSave).Entity.FileStatusRemarks = modal.OnHoldReasonDDL;
                        db.Entry(ModelToSave).Entity.OnHoldDone = modal.OnHoldDone;
                        break;
                    case "TBR-ONLINE_SUBMISSION":
                        db.Entry(ModelToSave).Entity.CourtLocationid = modal.CourtLocationid;
                        db.Entry(ModelToSave).Entity.ElectronicNo = modal.ElectronicNo;
                        db.Entry(ModelToSave).Entity.ClaimSummary = modal.ClaimSummary;
                        db.Entry(ModelToSave).Entity.RegistrationDate = modal.RegistrationDate;

                        courtCases = db.CourtCase.Find(modal.CaseId);

                        if (int.Parse(modal.ActionLevel) == 1)
                        {
                            db.Entry(ModelToSave).Entity.DepartmentType = modal.DepartmentType;
                            db.Entry(ModelToSave).Entity.RealEstateYesNo = modal.RealEstateYesNo;
                            db.Entry(ModelToSave).Entity.RealEstateDetail = modal.RealEstateDetail;

                            
                            db.Entry(courtCases).Entity.AgainstCode = modal.AgainstCode;
                            db.Entry(courtCases).Entity.OmaniExp = modal.OmaniExp;
                            db.Entry(courtCases).Entity.GovernorateId = modal.GovernorateId;
                            db.Entry(courtCases).Entity.ClaimAmount = modal.CourtReg_ClaimAmount;
                            db.Entry(courtCases).Entity.RealEstateYesNo = modal.RealEstateYesNo;
                            db.Entry(courtCases).Entity.RealEstateDetail = modal.RealEstateDetail;
                        }
                        

                        db.Entry(courtCases).Entity.ClaimSummary = modal.ClaimSummary;

                        if (modal.AgainstCode == "3")
                            db.Entry(courtCases).Entity.StopEnfRequest = modal.StopEnfRequest;

                        db.Entry(courtCases).State = EntityState.Modified;
                        db.SaveChanges();

                        Process_TBR_REGISTERED(modal);
                        break;
                    case "TBR-COURT_NOTES":
                        db.Entry(ModelToSave).Entity.CourtMessage = modal.CourtMessage;
                        break;
                    case "TBR-PAYMENT":
                        db.Entry(ModelToSave).Entity.CourtFeeAmount = modal.CourtFeeAmount;
                        db.Entry(ModelToSave).Entity.PaymentDate = modal.PaymentDate;

                        if (modal.Voucher_No > 0)
                            UpdateVoucher(modal);
                        else
                            CreatePayVoucher(modal, ref ModelToSave);
                        break;
                    case "TBR-WITH_LAWYER":
                        break;
                    case "TBR-REGISTERED":
                        Process_TBR_REGISTERED(modal);
                        courtCases = db.CourtCase.Find(modal.CaseId);

                        db.Entry(courtCases).Entity.CaseLevelCode = modal.CaseLevelCode;
                        db.Entry(courtCases).Entity.SessionRemarks = modal.SessionRemarks;

                        if (modal.AgainstCode == "3")
                            courtCases.StopEnfRequest = modal.StopEnfRequest;

                        if (modal.CaseLevelCode != "5")
                        {
                            //CREATE OR UPDATE SESSION ROLL
                            SessionsRoll sessionRoll = db.SessionsRoll.Where(w => w.CaseId == modal.CaseId && w.DeletedOn == null).OrderByDescending(o => o.SessionRollId).FirstOrDefault();
                            if (sessionRoll == null)
                            {
                                sessionRoll = new SessionsRoll();
                                sessionRoll.CaseId = modal.CaseId;
                                sessionRoll.CountLocationId = modal.CourtLocationid;
                                sessionRoll.SessionLevel = modal.CaseLevelCode;
                                sessionRoll.CaseRegistrationId = modal.CaseRegistrationId;
                                sessionRoll.CaseType = modal.Session_CaseType;
                                sessionRoll.LawyerId = modal.Session_LawyerId;

                                db.SessionsRoll.Add(sessionRoll);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(sessionRoll).Entity.CountLocationId = modal.CourtLocationid;
                                db.Entry(sessionRoll).Entity.SessionLevel = modal.CaseLevelCode;
                                db.Entry(sessionRoll).Entity.CaseRegistrationId = modal.CaseRegistrationId;
                                db.Entry(sessionRoll).Entity.CaseType = modal.Session_CaseType;
                                db.Entry(sessionRoll).Entity.LawyerId = modal.Session_LawyerId;
                                db.Entry(sessionRoll).State = EntityState.Modified;
                                db.SaveChanges();
                            }

                            db.Entry(ModelToSave).Entity.SessionRollId = sessionRoll.SessionRollId;
                        }

                        db.Entry(ModelToSave).Entity.IsDeleted = true;
                        db.Entry(ModelToSave).Entity.CourtLocationid = modal.CourtLocationid;
                        db.Entry(ModelToSave).Entity.RegistrationDate = modal.RegistrationDate;
                        db.Entry(ModelToSave).Entity.FileStatusRemarks = modal.CourtRefNo;

                        db.Entry(courtCases).State = EntityState.Modified;
                        db.SaveChanges();

                        break;
                    case "TBR-STOP_REGISTRATION":
                        db.Entry(ModelToSave).Entity.ActionDate = modal.ActionDate;
                        db.Entry(ModelToSave).Entity.StopRegEmailDate = modal.StopRegEmailDate;
                        db.Entry(ModelToSave).Entity.StopRegUserName = modal.StopRegUserName ?? User.Identity.Name;
                        db.Entry(ModelToSave).Entity.Remarks = modal.Remarks;
                        break;
                    default:
                        db.Entry(ModelToSave).Entity.ActionDate = modal.ActionDate;
                        break;
                }
            }
            else
            {
                db.Entry(ModelToSave).Entity.CaseId = modal.CaseId;
                db.Entry(ModelToSave).Entity.ActionDate = modal.ActionDate;
                db.Entry(ModelToSave).Entity.ActionLevel = modal.ActionLevel;
                db.Entry(ModelToSave).Entity.JudgementDate = modal.JudgementDate;
                db.Entry(ModelToSave).Entity.UrgentCaseDays = modal.UrgentCaseDays;
                db.Entry(ModelToSave).Entity.EnforcementDispute = modal.EnforcementDispute;
                db.Entry(ModelToSave).Entity.CourtRegistration = modal.CourtRegistration;
                db.Entry(ModelToSave).Entity.FileStatus = modal.FileStatus;

                if (int.Parse(modal.FileStatus) == 4)
                    db.Entry(ModelToSave).Entity.FileStatusRemarks = modal.OnHoldReasonDDL;
                else if (int.Parse(modal.FileStatus) == 6)
                    db.Entry(ModelToSave).Entity.ElectronicNo = modal.FileStatusRemarks;
                else if (int.Parse(modal.FileStatus) == 8 && !string.IsNullOrEmpty(modal.FileStatusRemarks))
                    db.Entry(ModelToSave).Entity.CourtFeeAmount = decimal.Parse(modal.FileStatusRemarks);
                else
                    db.Entry(ModelToSave).Entity.FileStatusRemarks = modal.FileStatusRemarks;

                db.Entry(ModelToSave).Entity.CourtMessage = modal.CourtMessage;

                db.Entry(ModelToSave).Entity.SendDate = modal.SendDate;
                db.Entry(ModelToSave).Entity.FirstReminderDate = modal.FirstReminderDate;
                db.Entry(ModelToSave).Entity.ReminderNo = modal.ReminderNo;
                db.Entry(ModelToSave).Entity.CourtRequestDate = modal.CourtRequestDate;
                db.Entry(ModelToSave).Entity.OfficeProcedure = modal.OfficeProcedure;
                db.Entry(ModelToSave).Entity.PaymentDate = modal.PaymentDate;
                db.Entry(ModelToSave).Entity.CourtDetailRegistered = modal.CourtDetailRegistered; //THIS IS URGENT NOW BOOLEAN
                db.Entry(ModelToSave).Entity.AdminFile = modal.AdminFile;
                db.Entry(ModelToSave).Entity.OmanPostNo = modal.OmanPostNo;
                db.Entry(ModelToSave).Entity.DepartmentType = modal.DepartmentType;
                db.Entry(ModelToSave).Entity.Voucher_No = modal.Voucher_No;
                db.Entry(ModelToSave).Entity.FormPrintDefendant = modal.FormPrintDefendant;
                db.Entry(ModelToSave).Entity.FormPrintLastDate = modal.FormPrintLastDate;
                db.Entry(ModelToSave).Entity.FormPrintWorkRequired = modal.FormPrintWorkRequired;
                db.Entry(ModelToSave).Entity.Remarks = modal.MainRemarks;
                db.Entry(ModelToSave).Entity.RealEstateYesNo = modal.RealEstateYesNo;
                db.Entry(ModelToSave).Entity.RealEstateDetail = modal.RealEstateDetail;
                db.Entry(ModelToSave).Entity.ClaimSummary = modal.ClaimSummary;

            }

            db.Entry(ModelToSave).State = EntityState.Modified;
            db.SaveChanges();
        }
        private void UpdateCaseManagementInfo(CaseRegistrationVM modal)
        {
            var CourtCase = db.CourtCase.Find(modal.CaseId);

            db.Entry(CourtCase).Entity.ClaimAmount = modal.CourtReg_ClaimAmount;
            db.Entry(CourtCase).Entity.IdRegistrationNo = modal.IdRegistrationNo;
            db.Entry(CourtCase).Entity.IdRegistrationNo = modal.IdRegistrationNo;
            db.Entry(CourtCase).Entity.AgainstCode = modal.AgainstCode;
            db.Entry(CourtCase).Entity.CaseSubject = modal.CaseSubject;
            db.Entry(CourtCase).Entity.CaseTypeCode = modal.CaseTypeCode;
            db.Entry(CourtCase).Entity.ClientCaseType = modal.ClientCaseType;
            db.Entry(CourtCase).Entity.OmaniExp = modal.OmaniExp;

            db.Entry(CourtCase).State = EntityState.Modified;
            db.SaveChanges();

        }
        private MessageVM UpdateBeforeCourtInfo(CaseRegistrationVM modal)
        {
            var CourtCase = db.CourtCase.Find(modal.CaseId);
            if (CourtCase != null)
            {
                db.Entry(CourtCase).Entity.PoliceNo = modal.PoliceNo;
                db.Entry(CourtCase).Entity.PublicProsecutionNo = modal.PublicProsecutionNo;
                db.Entry(CourtCase).Entity.PAPCNo = modal.PAPCNo;
                db.Entry(CourtCase).Entity.LaborConflicNo = modal.LaborConflicNo;
                db.Entry(CourtCase).Entity.PoliceStation = modal.PoliceStation;
                db.Entry(CourtCase).Entity.PublicPlace = modal.PublicPlace;
                db.Entry(CourtCase).Entity.PAPCPlace = modal.PAPCPlace;
                db.Entry(CourtCase).Entity.LaborConflicPlace = modal.LaborConflicPlace;

                db.Entry(CourtCase).State = EntityState.Modified;
                db.SaveChanges();

                return new MessageVM
                {
                    id = -1,
                    Category = "Success",
                    Message = "BEFORE COURT REGISTER SUCCESSFULLY"
                };
            }
            else
            {
                return new MessageVM
                {
                    Category = "Error",
                    Message = "CASE DETAIL NOT CREATED FIRST CREATE DETAIL THEN UPDATE"
                };
            }
        }
        private MessageVM UpdateCourtDetailInfo(CaseRegistrationVM modal)
        {
            var ObjToUpdate = db.CourtCasesDetail.Where(w => w.CaseId == modal.CaseId && w.Courtid == modal.ActionLevel).FirstOrDefault();

            if (ObjToUpdate != null)
            {
                string SelectCaseLevel = (int.Parse(modal.ActionLevel) + 2).ToString();

                db.Entry(ObjToUpdate).Entity.RegistrationDate = modal.RegistrationDate;
                db.Entry(ObjToUpdate).Entity.CourtRefNo = modal.CourtRefNo;
                db.Entry(ObjToUpdate).Entity.CourtLocationid = modal.CourtLocationid;
                db.Entry(ObjToUpdate).Entity.ApealByWho = modal.ApealByWho;
                db.Entry(ObjToUpdate).Entity.CaseLevelCode = SelectCaseLevel;

                db.Entry(ObjToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                CourtCases courtCases = db.CourtCase.Find(ObjToUpdate.CaseId);
                courtCases.CaseLevelCode = SelectCaseLevel;
                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                return new MessageVM
                {
                    id = -1,
                    Category = "Success",
                    Message = "COURT DETAIL SAVE SUCCESSFULLY"
                };
            }
            else
            {
                //CREATE NEW DETAIL
                CourtCasesDetail _ModalToSave = new CourtCasesDetail();
                string SelectCaseLevel = (int.Parse(modal.ActionLevel) + 2).ToString();

                _ModalToSave.CaseId = modal.CaseId;
                _ModalToSave.Courtid = modal.ActionLevel;
                _ModalToSave.CourtRefNo = modal.CourtRefNo;
                _ModalToSave.CourtLocationid = modal.CourtLocationid;
                _ModalToSave.RegistrationDate = modal.RegistrationDate;
                _ModalToSave.CaseLevelCode = SelectCaseLevel;
                _ModalToSave.ApealByWho = modal.ApealByWho;

                db.CourtCasesDetail.Add(_ModalToSave);
                db.SaveChanges();

                CourtCases courtCases = db.CourtCase.Find(_ModalToSave.CaseId);
                courtCases.CaseLevelCode = SelectCaseLevel;
                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                return new MessageVM
                {
                    id = -1,
                    Category = "Success",
                    Message = "COURT DETAIL SAVE SUCCESSFULLY"
                };
                //return new MessageVM
                //{
                //    Category = "Error",
                //    Message = "COURT DETAIL NOT CREATED FIRST CREATE DETAIL THEN UPDATE"
                //};
            }
        }
        private void Process_TBR_REGISTERED(CaseRegistrationVM modal)
        {
            if(modal.PartialViewName == "TBR-ONLINE_SUBMISSION")
            {
                if (modal.ActionLevel == "4")
                {
                    var ObjToUpdate = db.CourtCasesEnforcement.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

                    if (ObjToUpdate != null)
                    {
                        db.Entry(ObjToUpdate).Entity.EnforcementNo = modal.ElectronicNo;
                        db.Entry(ObjToUpdate).Entity.RegistrationDate = modal.ActionDate;
                        db.Entry(ObjToUpdate).Entity.CourtLocationid = modal.CourtLocationid;

                        db.Entry(ObjToUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //CREATE NEW DETAIL
                        CourtCasesEnforcement _ModalToSave = new CourtCasesEnforcement();

                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = "4";
                        _ModalToSave.EnforcementBy = "0";
                        _ModalToSave.CourtLocationid = modal.CourtLocationid;
                        _ModalToSave.EnforcementNo = modal.ElectronicNo;
                        _ModalToSave.RegistrationDate = modal.ActionDate;

                        db.CourtCasesEnforcement.Add(_ModalToSave);
                        db.SaveChanges();
                    }
                }
                else
                {
                    var ObjToUpdate = db.CourtCasesDetail.Where(w => w.CaseId == modal.CaseId && w.Courtid == modal.ActionLevel).FirstOrDefault();

                    if (ObjToUpdate != null)
                    {

                        db.Entry(ObjToUpdate).Entity.CourtLocationid = modal.CourtLocationid;
                        db.Entry(ObjToUpdate).Entity.CourtRefNo = modal.ElectronicNo;
                        db.Entry(ObjToUpdate).Entity.RegistrationDate = modal.RegistrationDate;

                        db.Entry(ObjToUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //CREATE NEW DETAIL
                        CourtCasesDetail _ModalToSave = new CourtCasesDetail();
                        string SelectCaseLevel = (int.Parse(modal.ActionLevel) + 2).ToString();

                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = modal.ActionLevel;
                        _ModalToSave.CourtLocationid = modal.CourtLocationid;
                        _ModalToSave.CourtRefNo = modal.ElectronicNo;
                        _ModalToSave.RegistrationDate = modal.RegistrationDate;
                        _ModalToSave.CaseLevelCode = SelectCaseLevel;

                        db.CourtCasesDetail.Add(_ModalToSave);
                        db.SaveChanges();
                    }

                }
            }
            else
            {
                if (modal.ActionLevel == "4")
                {
                    var ObjToUpdate = db.CourtCasesEnforcement.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

                    if (ObjToUpdate != null)
                    {
                        if(modal.DisputeType == "1")
                        {
                            if (modal.DisputeLevel == "1")
                            {
                                db.Entry(ObjToUpdate).Entity.PrimaryObjectionNo = modal.PrimaryObjectionNo;
                                db.Entry(ObjToUpdate).Entity.PrimaryObjectionCourt = modal.PrimaryObjectionCourt;

                            }
                            else if (modal.DisputeLevel == "2")
                            {
                                db.Entry(ObjToUpdate).Entity.ApealObjectionNo = modal.ApealObjectionNo;
                                db.Entry(ObjToUpdate).Entity.ApealObjectionCourt = modal.ApealObjectionCourt;

                            }
                            else if (modal.DisputeLevel == "3")
                            {
                                db.Entry(ObjToUpdate).Entity.SupremeObjectionNo = modal.SupremeObjectionNo;
                                db.Entry(ObjToUpdate).Entity.SupremeObjectionCourt = modal.SupremeObjectionCourt;

                            }
                        }
                        else if (modal.DisputeType == "2")
                        {
                            if (modal.DisputeLevel == "1")
                            {
                                db.Entry(ObjToUpdate).Entity.PrimaryPlaintNo = modal.PrimaryPlaintNo;
                                db.Entry(ObjToUpdate).Entity.PrimaryPlaintCourt = modal.PrimaryPlaintCourt;

                            }
                            else if (modal.DisputeLevel == "2")
                            {
                                db.Entry(ObjToUpdate).Entity.ApealPlaintNo = modal.ApealPlaintNo;
                                db.Entry(ObjToUpdate).Entity.ApealPlaintCourt = modal.ApealPlaintCourt;

                            }
                            else if (modal.DisputeLevel == "3")
                            {
                                db.Entry(ObjToUpdate).Entity.SupremePlaintNo = modal.SupremePlaintNo;
                                db.Entry(ObjToUpdate).Entity.SupremePlaintCourt = modal.SupremePlaintCourt;

                            }
                        }

                        if (modal.DisputeLevel == "3")
                        {
                            db.Entry(ObjToUpdate).Entity.CourtDepartment = modal.CourtDepartment;

                        }

                        db.Entry(ObjToUpdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //CREATE NEW DETAIL
                        CourtCasesEnforcement _ModalToSave = new CourtCasesEnforcement();
                        string SelectCaseLevel = (int.Parse(modal.ActionLevel) + 2).ToString();

                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = modal.ActionLevel;
                        _ModalToSave.EnforcementBy = modal.ApealByWho;
                        _ModalToSave.CourtLocationid = modal.CourtLocationid;
                        _ModalToSave.EnforcementNo = modal.CourtRefNo;
                        _ModalToSave.RegistrationDate = modal.RegistrationDate;
                        _ModalToSave.CaseLevelCode = SelectCaseLevel;

                        db.CourtCasesEnforcement.Add(_ModalToSave);
                        db.SaveChanges();

                        //CourtCases courtCases = db.CourtCase.Find(_ModalToSave.CaseId);
                        //courtCases.CaseLevelCode = SelectCaseLevel;
                        //db.Entry(courtCases).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                }
                else
                {
                    var ObjToUpdate = db.CourtCasesDetail.Where(w => w.CaseId == modal.CaseId && w.CaseLevelCode == modal.CaseLevelCode).FirstOrDefault();

                    if (ObjToUpdate != null)
                    {
                        string SelectCaseLevel = modal.CaseLevelCode; // (int.Parse(modal.ActionLevel) + 2).ToString();
                        string Courtid = (int.Parse(modal.CaseLevelCode) - 2).ToString();
                        db.Entry(ObjToUpdate).Entity.ApealByWho = modal.ApealByWho;
                        db.Entry(ObjToUpdate).Entity.CourtDepartment = modal.CourtDepartment;
                        db.Entry(ObjToUpdate).Entity.CourtLocationid = modal.CourtLocationid;
                        db.Entry(ObjToUpdate).Entity.CourtRefNo = modal.CourtRefNo;
                        db.Entry(ObjToUpdate).Entity.RegistrationDate = modal.RegistrationDate;
                        db.Entry(ObjToUpdate).Entity.CaseLevelCode = SelectCaseLevel;

                        db.Entry(ObjToUpdate).State = EntityState.Modified;
                        db.SaveChanges();

                        //CourtCases courtCases = db.CourtCase.Find(ObjToUpdate.CaseId);
                        //courtCases.CaseLevelCode = SelectCaseLevel;
                        //db.Entry(courtCases).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                    else
                    {
                        //CREATE NEW DETAIL
                        CourtCasesDetail _ModalToSave = new CourtCasesDetail();
                        string SelectCaseLevel = modal.CaseLevelCode; // (int.Parse(modal.ActionLevel) + 2).ToString();
                        string Courtid = (int.Parse(modal.CaseLevelCode) - 2).ToString();

                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = Courtid;
                        _ModalToSave.ApealByWho = modal.ApealByWho;
                        _ModalToSave.CourtDepartment = modal.CourtDepartment;
                        _ModalToSave.CourtLocationid = modal.CourtLocationid;
                        _ModalToSave.CourtRefNo = modal.CourtRefNo;
                        _ModalToSave.RegistrationDate = modal.RegistrationDate;
                        _ModalToSave.CaseLevelCode = SelectCaseLevel;

                        db.CourtCasesDetail.Add(_ModalToSave);
                        db.SaveChanges();

                        //CourtCases courtCases = db.CourtCase.Find(_ModalToSave.CaseId);
                        //courtCases.CaseLevelCode = SelectCaseLevel;
                        //db.Entry(courtCases).State = EntityState.Modified;
                        //db.SaveChanges();
                    }

                }

            }
        }
        private MessageVM UpdateEnforcementCourtInfo(CaseRegistrationVM modal)
        {
            var ObjToUpdate = db.CourtCasesEnforcement.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

            if (ObjToUpdate != null)
            {
                db.Entry(ObjToUpdate).Entity.PrimaryObjectionNo = modal.PrimaryObjectionNo;
                db.Entry(ObjToUpdate).Entity.PrimaryObjectionCourt = modal.PrimaryObjectionCourt;
                db.Entry(ObjToUpdate).Entity.ApealObjectionNo = modal.ApealObjectionNo;
                db.Entry(ObjToUpdate).Entity.ApealObjectionCourt = modal.ApealObjectionCourt;
                db.Entry(ObjToUpdate).Entity.SupremeObjectionNo = modal.SupremeObjectionNo;
                db.Entry(ObjToUpdate).Entity.SupremeObjectionCourt = modal.SupremeObjectionCourt;
                db.Entry(ObjToUpdate).Entity.PrimaryPlaintNo = modal.PrimaryPlaintNo;
                db.Entry(ObjToUpdate).Entity.PrimaryPlaintCourt = modal.PrimaryPlaintCourt;
                db.Entry(ObjToUpdate).Entity.ApealPlaintNo = modal.ApealPlaintNo;
                db.Entry(ObjToUpdate).Entity.ApealPlaintCourt = modal.ApealPlaintCourt;
                db.Entry(ObjToUpdate).Entity.SupremePlaintNo = modal.SupremePlaintNo;
                db.Entry(ObjToUpdate).Entity.SupremePlaintCourt = modal.SupremePlaintCourt;

                db.Entry(ObjToUpdate).State = EntityState.Modified;
                db.SaveChanges();

                return new MessageVM
                {
                    id = -1,
                    Category = "Success",
                    Message = "ENFORCEMENT REGISTER SUCCESSFULLY"
                };
            }
            else
            {
                return new MessageVM
                {
                    Category = "Error",
                    Message = "ENFORCEMENT DETAIL NOT CREATED FIRST CREATE ENFRCEMENT DETAIL THEN UPDATE"
                };
            }
        }
        private void CreatePayVoucher(CaseRegistrationVM modal, ref CaseRegistration ModelToSave)
        {
            if (User.IsInRole("VoucherApproval") || User.Identity.Name.Equals("6"))
            {
                if (modal.PaymentDate != null && modal.Amount > 0)
                {
                    PayVoucher payVoucher = new PayVoucher();
                    payVoucher.Voucher_Date = DateTime.UtcNow.AddHours(4);
                    payVoucher.Payment_Type = "1";
                    payVoucher.Payment_Mode = "3";//3 BANK TRANSFER
                    payVoucher.VoucherType = "1"; //1 REFUNDABLE
                    payVoucher.Status = "1";
                    payVoucher.VoucherStatus = "1";
                    payVoucher.LocationCode = "0";
                    payVoucher.Credit_Account = 0;
                    payVoucher.TransTypeCode = "-2";
                    payVoucher.TransReasonCode = "0";
                    payVoucher.CaseId = modal.CaseId;

                    payVoucher.Cheque_Date = modal.Cheque_Date;
                    payVoucher.CourtType = modal.CourtType;
                    payVoucher.Payment_Head = modal.Payment_Head;
                    payVoucher.Remarks = modal.Remarks;
                    payVoucher.Payment_To = modal.Payment_To;
                    payVoucher.Amount = modal.Amount;
                    payVoucher.VatAmount = modal.VatAmount;
                    payVoucher.BillNumber = modal.BillNumber;
                    payVoucher.Debit_Account = modal.Debit_Account;
                    payVoucher.Cheque_Number = modal.Cheque_Number;
                    payVoucher.Cheque_Date = modal.Cheque_Date;

                    db.PayVouchers.Add(payVoucher);
                    db.SaveChanges();


                    payVoucher.AuthorizeBy = User.Identity.GetUserId();
                    payVoucher.AuthorizeDate = DateTime.Now;

                    string PV_Vooucher = Helper.GeneratePVNumber(payVoucher.Voucher_No);
                    payVoucher.PV_No = PV_Vooucher;
                    ModelToSave.Voucher_No = payVoucher.Voucher_No;

                    db.Entry(payVoucher).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private void UpdateVoucher(CaseRegistrationVM modal)
        {
            if (User.IsInRole("VoucherApproval") || User.Identity.Name.Equals("6"))
            {
                if (modal.PaymentDate != null && modal.Amount > 0)
                {
                    var payVoucher = db.PayVouchers.Find(modal.Voucher_No);

                    db.Entry(payVoucher).Entity.CaseId = modal.CaseId;

                    db.Entry(payVoucher).Entity.Cheque_Date = modal.Cheque_Date;
                    db.Entry(payVoucher).Entity.CourtType = modal.CourtType;
                    db.Entry(payVoucher).Entity.Payment_Head = modal.Payment_Head;
                    db.Entry(payVoucher).Entity.Remarks = modal.Remarks;
                    db.Entry(payVoucher).Entity.Payment_To = modal.Payment_To;
                    db.Entry(payVoucher).Entity.Amount = modal.Amount;
                    db.Entry(payVoucher).Entity.VatAmount = modal.VatAmount;
                    db.Entry(payVoucher).Entity.BillNumber = modal.BillNumber;
                    db.Entry(payVoucher).Entity.Debit_Account = modal.Debit_Account;
                    db.Entry(payVoucher).Entity.Cheque_Number = modal.Cheque_Number;
                    db.Entry(payVoucher).Entity.Cheque_Date = modal.Cheque_Date;

                    db.Entry(payVoucher).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private void SoftDeleteCaseRegister(ref CaseRegistration ModelToSave)
        {
            db.Entry(ModelToSave).Entity.IsDeleted = true;
            db.Entry(ModelToSave).State = EntityState.Modified;
            db.SaveChanges();

        }
        private void UpdatecourtcasesDetail(CaseRegistrationVM modal)
        {
            var courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();
            var courtcasesDetail = db.CourtCasesDetail.Where(w => w.CaseId == modal.CaseId && w.Courtid == modal.ActionLevel).FirstOrDefault();
            if (courtcasesDetail != null)
            {
                db.Entry(courtCases).Entity.RealEstateYesNo = modal.RealEstateYesNo;
                db.Entry(courtCases).Entity.RealEstateDetail = modal.RealEstateDetail;
                db.Entry(courtCases).Entity.ClaimSummary = modal.ClaimSummary;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                db.Entry(courtCases).Entity.RealEstateYesNo = modal.RealEstateYesNo;
                db.Entry(courtCases).Entity.RealEstateDetail = modal.RealEstateDetail;
                db.Entry(courtCases).Entity.ClaimSummary = modal.ClaimSummary;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                CourtCasesDetail _ModalToSave = new CourtCasesDetail();

                _ModalToSave.CaseId = modal.CaseId;
                _ModalToSave.Courtid = modal.ActionLevel;
                _ModalToSave.CourtRefNo = modal.CourtRefNo;
                _ModalToSave.CourtLocationid = modal.CourtLocationid == "" ? "0" : modal.CourtLocationid;
                _ModalToSave.RegistrationDate = modal.RegistrationDate;
                _ModalToSave.CaseLevelCode = "3";

                db.CourtCasesDetail.Add(_ModalToSave);
                db.SaveChanges();

            }
        }
        private void UpdateCourtCaseUpdate(CaseRegistrationVM modal)
        {
            Helper.ProcessCourtDecisionHistory(modal, HttpContext.User.Identity.GetUserId(), "CaseRegistrationVM");

            var CourtCase = db.CourtCase.Find(modal.CaseId);

            db.Entry(CourtCase).Entity.CurrentHearingDate = modal.CurrentHearingDate;
            db.Entry(CourtCase).Entity.CourtDecision = modal.CourtDecision;
            db.Entry(CourtCase).Entity.Requirements = modal.Requirements;
            db.Entry(CourtCase).Entity.ClientReply = modal.ClientReply;
            db.Entry(CourtCase).Entity.TransportationSource = modal.TransportationSource;
            db.Entry(CourtCase).Entity.FirstEmailDate = modal.FirstEmailDate;
            db.Entry(CourtCase).Entity.NextHearingDate = modal.NextHearingDate;
            db.Entry(CourtCase).Entity.OfficeFileStatus = modal.FileStatus;
            db.Entry(CourtCase).State = EntityState.Modified;
            db.SaveChanges();

        }

        public ActionResult AjaxIndexData(string DataFor)
        {
            var request = HttpContext.Request;
            List<CaseRegistrationListForIndex> data = new List<CaseRegistrationListForIndex>();

            var start = (Convert.ToInt32(Request["start"]));
            var Length = (Convert.ToInt32(Request["length"])) == 0 ? 10 : (Convert.ToInt32(Request["length"]));
            var searchvalue = Request["search[value]"] ?? "";
            var sortcoloumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            var sortDirection = Request["order[0][dir]"] ?? "asc";
            var recordsTotal = 0;

            try
            {
                string LocationId = string.Empty;
                string FileStatus = string.Empty;
                DataFor = string.Empty;
                try
                {
                    DataFor = request.Params["DataTableName"].ToString();
                    LocationId = request.Params["LocationId"].ToString();
                    FileStatus = request.Params["FileStatus"].ToString();
                    data = Helper.GetCaseRegistrationList(sortcoloumnIndex, start, searchvalue, Length, sortDirection, User.Identity.Name, DataFor, LocationId, FileStatus).ToList();
                    recordsTotal = data.Count > 0 ? data[0].TotalRecords : 0;

                }
                catch (Exception e)
                {
                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { data = data, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create()
        {
            CaseRegistrationVM modal = new CaseRegistrationVM();
            ViewBag.MstParentId = (int)MASTER_S.Location;

            ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");

            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CaseRegistrationVM modal)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                CaseRegistration ModelToSave = new CaseRegistration();

                ModelToSave.CaseId = modal.CaseId;
                ModelToSave.ActionDate = modal.ActionDate;
                ModelToSave.ActionLevel = modal.ActionLevel;
                ModelToSave.JudgementDate = modal.JudgementDate;
                ModelToSave.UrgentCaseDays = modal.UrgentCaseDays;
                ModelToSave.EnforcementDispute = modal.EnforcementDispute;
                ModelToSave.CourtRegistration = modal.CourtRegistration;
                ModelToSave.FileStatus = modal.FileStatus;
                ModelToSave.FileStatusRemarks = modal.FileStatusRemarks;
                ModelToSave.CourtMessage = modal.CourtMessage;

                ModelToSave.SendDate = modal.SendDate;
                ModelToSave.FirstReminderDate = modal.FirstReminderDate;
                ModelToSave.ReminderNo = modal.ReminderNo;
                ModelToSave.CourtRequestDate = modal.CourtRequestDate;
                ModelToSave.OfficeProcedure = modal.OfficeProcedure;
                ModelToSave.PaymentDate = modal.PaymentDate;
                ModelToSave.AssignedTo = modal.AssignedTo;
                ModelToSave.AssignedDate = modal.AssignedDate;
                ModelToSave.CourtDetailRegistered = modal.CourtDetailRegistered;
                ModelToSave.AdminFile = modal.AdminFile;
                ModelToSave.OmanPostNo = modal.OmanPostNo;

                if (modal.CourtDetailRegistered)
                {
                    CourtCasesDetail _ModalToSave = new CourtCasesDetail();
                    string SelectCaseLevel = string.Empty;

                    if (modal.ActionLevel == "1") { SelectCaseLevel = "3"; }
                    else if (modal.ActionLevel == "2") { SelectCaseLevel = "4"; }
                    else if (modal.ActionLevel == "3") { SelectCaseLevel = "5"; }


                    if (modal.DetailId > 0)
                    {
                        _ModalToSave = db.CourtCasesDetail.Find(modal.DetailId);

                        db.Entry(_ModalToSave).Entity.CaseId = modal.CaseId;
                        db.Entry(_ModalToSave).Entity.Courtid = modal.ActionLevel;
                        db.Entry(_ModalToSave).Entity.CourtRefNo = modal.CourtReg_RegNo;
                        db.Entry(_ModalToSave).Entity.CourtLocationid = modal.CourtReg_RegCourt;
                        db.Entry(_ModalToSave).Entity.RegistrationDate = modal.CourtReg_RegDate;
                        db.Entry(_ModalToSave).Entity.CaseLevelCode = SelectCaseLevel;
                        db.Entry(_ModalToSave).Entity.JudgementDate = modal.NextHearingDate;
                        db.Entry(_ModalToSave).Entity.JudgementDetails = modal.NextHearingNotes;
                        db.Entry(_ModalToSave).Entity.ApealByWho = modal.CourtReg_Regby;

                        db.Entry(_ModalToSave).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else
                    {
                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = modal.ActionLevel;
                        _ModalToSave.CourtRefNo = modal.CourtReg_RegNo;
                        _ModalToSave.CourtLocationid = modal.CourtReg_RegCourt;
                        _ModalToSave.RegistrationDate = modal.CourtReg_RegDate;
                        _ModalToSave.CaseLevelCode = SelectCaseLevel;
                        _ModalToSave.JudgementDate = modal.NextHearingDate;
                        _ModalToSave.JudgementDetails = modal.NextHearingNotes;
                        _ModalToSave.ApealByWho = modal.CourtReg_Regby;

                        db.CourtCasesDetail.Add(_ModalToSave);
                        db.SaveChanges();
                    }

                    if (modal.CourtReg_ClaimAmount > 0)
                    {
                        CourtCases courtCases = db.CourtCase.Find(modal.CaseId);
                        courtCases.ClaimAmount = modal.CourtReg_ClaimAmount;
                        db.Entry(courtCases).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                db.CaseRegistration.Add(ModelToSave);
                db.SaveChanges();

                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };
                return RedirectToAction("Edit", new RouteValueDictionary(new { id = ModelToSave.CaseRegistrationId }));

            }


            ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");

            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return View(modal);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var caseRegistration = db.CaseRegistration.Find(id);
            CaseRegistrationVM modal = new CaseRegistrationVM();

            if (caseRegistration == null)
                return HttpNotFound();
            else
            {
                modal.CaseRegistrationId = caseRegistration.CaseRegistrationId;
                modal.CaseId = caseRegistration.CaseId;
                //modal.ActionDate = caseRegistration.ActionDate;
                modal.ActionLevel = caseRegistration.ActionLevel;
                modal.JudgementDate = caseRegistration.JudgementDate;
                modal.UrgentCaseDays = caseRegistration.UrgentCaseDays;
                modal.EnforcementDispute = caseRegistration.EnforcementDispute;
                modal.CourtRegistration = caseRegistration.CourtRegistration;
                modal.FileStatus = caseRegistration.FileStatus;
                modal.FileStatusRemarks = caseRegistration.FileStatusRemarks;
                modal.CourtMessage = caseRegistration.CourtMessage;

                modal.SendDate = caseRegistration.SendDate;
                modal.FirstReminderDate = caseRegistration.FirstReminderDate;
                modal.ReminderNo = caseRegistration.ReminderNo;
                modal.CourtRequestDate = caseRegistration.CourtRequestDate;
                modal.OfficeProcedure = caseRegistration.OfficeProcedure;
                modal.PaymentDate = caseRegistration.PaymentDate;
                modal.AssignedTo = caseRegistration.AssignedTo;
                modal.AssignedDate = caseRegistration.AssignedDate;
                modal.CourtDetailRegistered = caseRegistration.CourtDetailRegistered;
                modal.AdminFile = caseRegistration.AdminFile;
                modal.OmanPostNo = caseRegistration.OmanPostNo;

                var courtCases = db.CourtCase.Find(modal.CaseId);

                modal.ReceptionDate = courtCases.ReceptionDate?.ToString("dd/MM/yyyy"); ;

                if (modal.CourtDetailRegistered)
                {
                    var _CaseDetail = db.CourtCasesDetail.Find(modal.DetailId);

                    modal.CourtReg_RegNo = _CaseDetail.CourtRefNo;
                    modal.CourtReg_RegCourt = _CaseDetail.CourtLocationid;
                    modal.CourtReg_RegDate = _CaseDetail.RegistrationDate;
                    modal.NextHearingDate = _CaseDetail.JudgementDate;
                    modal.NextHearingNotes = _CaseDetail.JudgementDetails;
                    modal.CourtReg_Regby = _CaseDetail.ApealByWho;

                    modal.CourtReg_ClaimAmount = courtCases.ClaimAmount;


                    ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", modal.CourtReg_RegCourt);
                    ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", modal.CourtReg_Regby);

                }
                else
                {
                    ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");

                }


                ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel), "Mst_Value", "Mst_Desc", caseRegistration.ActionLevel);
                ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute), "Mst_Value", "Mst_Desc", caseRegistration.EnforcementDispute);
                ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", caseRegistration.CourtRegistration);
                ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus), "Mst_Value", "Mst_Desc", caseRegistration.FileStatus);


                var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
                ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
                ViewBag.ClientName = ClientName;
                ViewBag.Defendant = courtCases.Defendant;
                ViewBag.dispACCNO = courtCases.AccountContractNo;
                ViewBag.dispCFNO = courtCases.ClientFileNo;

            }

            return View(modal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CaseRegistrationVM modal)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            CaseRegistration ModelToSave = db.CaseRegistration.Where(w => w.CaseRegistrationId == modal.CaseRegistrationId).FirstOrDefault();
            var courtCases = db.CourtCase.Find(modal.CaseId);
            if (ModelState.IsValid)
            {
                db.Entry(ModelToSave).Entity.ActionDate = modal.ActionDate;
                db.Entry(ModelToSave).Entity.ActionLevel = modal.ActionLevel;
                db.Entry(ModelToSave).Entity.JudgementDate = modal.JudgementDate;
                db.Entry(ModelToSave).Entity.UrgentCaseDays = modal.UrgentCaseDays;
                db.Entry(ModelToSave).Entity.EnforcementDispute = modal.EnforcementDispute;
                db.Entry(ModelToSave).Entity.CourtRegistration = modal.CourtRegistration;
                db.Entry(ModelToSave).Entity.FileStatus = modal.FileStatus;
                db.Entry(ModelToSave).Entity.FileStatusRemarks = modal.FileStatusRemarks;
                db.Entry(ModelToSave).Entity.CourtMessage = modal.CourtMessage;

                db.Entry(ModelToSave).Entity.SendDate = modal.SendDate;
                db.Entry(ModelToSave).Entity.FirstReminderDate = modal.FirstReminderDate;
                db.Entry(ModelToSave).Entity.ReminderNo = modal.ReminderNo;
                db.Entry(ModelToSave).Entity.CourtRequestDate = modal.CourtRequestDate;
                db.Entry(ModelToSave).Entity.OfficeProcedure = modal.OfficeProcedure;
                db.Entry(ModelToSave).Entity.PaymentDate = modal.PaymentDate;
                db.Entry(ModelToSave).Entity.AssignedTo = modal.AssignedTo;
                db.Entry(ModelToSave).Entity.AssignedDate = modal.AssignedDate;
                db.Entry(ModelToSave).Entity.CourtDetailRegistered = modal.CourtDetailRegistered;
                db.Entry(ModelToSave).Entity.AdminFile = modal.AdminFile;
                db.Entry(ModelToSave).Entity.OmanPostNo = modal.OmanPostNo;

                if (modal.CourtDetailRegistered)
                {
                    CourtCasesDetail _ModalToSave = new CourtCasesDetail();
                    string SelectCaseLevel = string.Empty;

                    if (modal.ActionLevel == "1") { SelectCaseLevel = "3"; }
                    else if (modal.ActionLevel == "2") { SelectCaseLevel = "4"; }
                    else if (modal.ActionLevel == "3") { SelectCaseLevel = "5"; }


                    if (modal.DetailId > 0)
                    {
                        _ModalToSave = db.CourtCasesDetail.Find(modal.DetailId);

                        db.Entry(_ModalToSave).Entity.CaseId = modal.CaseId;
                        db.Entry(_ModalToSave).Entity.Courtid = modal.ActionLevel;
                        db.Entry(_ModalToSave).Entity.CourtRefNo = modal.CourtReg_RegNo;
                        db.Entry(_ModalToSave).Entity.CourtLocationid = modal.CourtReg_RegCourt;
                        db.Entry(_ModalToSave).Entity.RegistrationDate = modal.CourtReg_RegDate;
                        db.Entry(_ModalToSave).Entity.CaseLevelCode = SelectCaseLevel;
                        db.Entry(_ModalToSave).Entity.JudgementDate = modal.NextHearingDate;
                        db.Entry(_ModalToSave).Entity.JudgementDetails = modal.NextHearingNotes;
                        db.Entry(_ModalToSave).Entity.ApealByWho = modal.CourtReg_Regby;

                        db.Entry(_ModalToSave).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else
                    {
                        _ModalToSave.CaseId = modal.CaseId;
                        _ModalToSave.Courtid = modal.ActionLevel;
                        _ModalToSave.CourtRefNo = modal.CourtReg_RegNo;
                        _ModalToSave.CourtLocationid = modal.CourtReg_RegCourt;
                        _ModalToSave.RegistrationDate = modal.CourtReg_RegDate;
                        _ModalToSave.CaseLevelCode = SelectCaseLevel;
                        _ModalToSave.JudgementDate = modal.NextHearingDate;
                        _ModalToSave.JudgementDetails = modal.NextHearingNotes;
                        _ModalToSave.ApealByWho = modal.CourtReg_Regby;

                        db.CourtCasesDetail.Add(_ModalToSave);
                        db.SaveChanges();
                    }

                    if (modal.CourtReg_ClaimAmount > 0)
                    {
                        courtCases.ClaimAmount = modal.CourtReg_ClaimAmount;
                        db.Entry(courtCases).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                db.Entry(ModelToSave).State = EntityState.Modified;
                db.SaveChanges();

                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };

                return RedirectToAction("Edit", new RouteValueDictionary(new { ModelToSave.CaseRegistrationId }));

                //return RedirectToAction("Index");
            }

            ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel), "Mst_Value", "Mst_Desc", ModelToSave.ActionLevel);
            ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute), "Mst_Value", "Mst_Desc", ModelToSave.EnforcementDispute);
            ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ModelToSave.CourtRegistration);
            ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus), "Mst_Value", "Mst_Desc", ModelToSave.FileStatus);

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;
            ViewBag.dispACCNO = courtCases.AccountContractNo;
            ViewBag.dispCFNO = courtCases.ClientFileNo;

            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return View(modal);
        }

        [HttpGet]
        public ActionResult GetTab(string Mode, string PartialViewName, int CaseId, int CaseRegisterId)
        {
            CaseRegistrationVM modal = new CaseRegistrationVM();
            modal.CaseId = CaseId;
            modal.CaseRegistrationId = CaseRegisterId;

            ViewBag.CaseRegisterId = CaseRegisterId;
            ViewBag.HFCaseId = CaseId;

            ViewBag.ActionLevel = "";
            ViewBag.FileStatus = "";
            ViewBag.FrmMode = "";

            string HiddenClassName = "AppHidden";
            string UnHiddenClass = "";

            string ClientName = string.Empty;
            string SessionRollClientName = string.Empty;

            if (CaseRegisterId > 0)
            {
                var caseRegistration = db.CaseRegistration.Find(CaseRegisterId);

                modal.CaseRegistrationId = caseRegistration.CaseRegistrationId;
                modal.CaseId = caseRegistration.CaseId;
                modal.ActionDate = caseRegistration.ActionDate;
                modal.ActionLevel = caseRegistration.ActionLevel;
                modal.JudgementDate = caseRegistration.JudgementDate;
                modal.UrgentCaseDays = caseRegistration.UrgentCaseDays;
                modal.EnforcementDispute = caseRegistration.EnforcementDispute;
                modal.CourtRegistration = caseRegistration.CourtRegistration;
                modal.FileStatus = caseRegistration.FileStatus;

                if (int.Parse(caseRegistration.FileStatus) == 6)
                    modal.FileStatusRemarks = caseRegistration.ElectronicNo;
                else if (int.Parse(caseRegistration.FileStatus) == 8)
                    modal.FileStatusRemarks = caseRegistration.CourtFeeAmount.ToString();
                else
                    modal.FileStatusRemarks = caseRegistration.FileStatusRemarks;

                modal.CourtMessage = caseRegistration.CourtMessage;

                modal.SendDate = caseRegistration.SendDate;
                modal.FirstReminderDate = caseRegistration.FirstReminderDate;
                modal.ReminderNo = caseRegistration.ReminderNo;
                modal.CourtRequestDate = caseRegistration.CourtRequestDate;
                modal.OfficeProcedure = caseRegistration.OfficeProcedure;
                modal.PaymentDate = caseRegistration.PaymentDate;
                modal.AssignedTo = caseRegistration.AssignedTo;
                modal.AssignedDate = caseRegistration.AssignedDate;
                modal.CourtDetailRegistered = caseRegistration.CourtDetailRegistered;
                modal.AdminFile = caseRegistration.AdminFile;
                modal.OmanPostNo = caseRegistration.OmanPostNo;
                modal.DepartmentType = caseRegistration.DepartmentType;
                modal.Voucher_No = caseRegistration.Voucher_No;
                modal.FormPrintJugDate = caseRegistration.JudgementDate?.ToString("dd/MM/yyyy");
                modal.FormPrintDefendant = caseRegistration.FormPrintDefendant;
                modal.FormPrintLastDate = caseRegistration.FormPrintLastDate;
                modal.FormPrintWorkRequired = caseRegistration.FormPrintWorkRequired;
                modal.MainRemarks = caseRegistration.Remarks;
                modal.RealEstateYesNo = caseRegistration.RealEstateYesNo;
                modal.RealEstateDetail = caseRegistration.RealEstateDetail;
                modal.ClaimSummary = caseRegistration.ClaimSummary;
                string strActionLevel = modal.ActionLevel.ToString();
                string strFileStatus = modal.FileStatus.ToString();
                ViewBag.ActionLevelValue = strActionLevel;
                ViewBag.FileStatusValue = strFileStatus;
                ViewBag.FrmMode = "E";

            }


            if (PartialViewName == "_CaseRegistrationVM")
            {

                ViewBag.CaseDetailClass = HiddenClassName;
                ViewBag.IsEdit = "";
                ViewBag.DivActionLevelDetail = HiddenClassName;
                ViewBag.DivFileStatusDetail = HiddenClassName;
                ViewBag.divReceptionDate = HiddenClassName;
                ViewBag.divJudgementDate = HiddenClassName;
                ViewBag.divEnforcementDispute = HiddenClassName;
                ViewBag.DivAppealDeptt = HiddenClassName;
                ViewBag.Div2 = HiddenClassName;
                ViewBag.Div4 = HiddenClassName;
                ViewBag.Div7 = HiddenClassName;
                ViewBag.Div9 = HiddenClassName;
                ViewBag.Div10 = HiddenClassName;
                ViewBag.clsPA = HiddenClassName;
                ViewBag.cls8cls6 = HiddenClassName;
                ViewBag.clsP = HiddenClassName;
                ViewBag.cls8 = HiddenClassName;
                ViewBag.clsPAS = HiddenClassName;
                ViewBag.clsRealEstate = HiddenClassName;
                ViewBag.RealEstateYES = "false";
                ViewBag.RealEstateNO = "false";
                ViewBag.PaymentVoucherContainer = HiddenClassName;
                ViewBag.PrintFormButton = HiddenClassName;
                ViewBag.DIV_UPDATE = HiddenClassName;

                ViewBag.SpanActionLevelTitle = "";
                ViewBag.lblJudgementDate = "";
                ViewBag.lblCourtRegistration = "";
                ViewBag.SpanFileStatusTitle = "";
                ViewBag.lblFileStatusRemarks = "";
                ViewBag.h3ActionLevelTitle = "";
                ViewBag.MstParentId = (int)MASTER_S.Location;

                if (Mode == "C")
                {
                    var courtCases = db.CourtCase.Find(modal.CaseId);

                    if (!string.IsNullOrEmpty(courtCases.ClientCode) && courtCases.ClientCode != "0")
                        ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

                    if (!string.IsNullOrEmpty(courtCases.SessionClientId) && courtCases.SessionClientId != "0")
                        SessionRollClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.SessionClients && w.Mst_Value == courtCases.SessionClientId).FirstOrDefault().Mst_Desc;

                    modal.ReceptionDate = courtCases.ReceptionDate?.ToString("dd/MM/yyyy");
                    modal.CourtReg_ClaimAmount = courtCases.ClaimAmount;
                    modal.ClientReply = courtCases.ClientReply;
                    modal.TransportationFee = courtCases.TransportationFee;
                    modal.TransportationSource = courtCases.TransportationSource;
                    modal.FirstEmailDate = courtCases.FirstEmailDate;
                    modal.CurrentHearingDate = courtCases.CurrentHearingDate;
                    modal.CourtDecision = courtCases.CourtDecision;
                    modal.Requirements = courtCases.Requirements;
                    modal.ClientName = ClientName;
                    modal.SessionRollClientName = SessionRollClientName;
                    modal.Defendant = courtCases.Defendant;
                    modal.SessionRollDefendentName = courtCases.SessionRollDefendentName;
                    modal.OfficeFileNo = courtCases.OfficeFileNo;

                    ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
                    ViewBag.ClientName = ClientName;
                    ViewBag.Defendant = courtCases.Defendant;
                    ViewBag.dispACCNO = courtCases.AccountContractNo;
                    ViewBag.dispCFNO = courtCases.ClientFileNo;

                    #region SETTING PARTAIL VIEW VARIABLES

                    ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtReg_RegCourt = new SelectList(Helper.GetCourtLocationList("1").OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.CourtReg_Regby = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");
                    ViewBag.OnHoldReasonDDL = new SelectList(Helper.GetOnHoldReason(), "Mst_Value", "Mst_Desc");
                    ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc");
                    ViewBag.OmanPostDoc = "#";
                    ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", courtCases.ClientReply);
                    ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", courtCases.TransportationSource);


                    #endregion
                    ViewBag.CaseDetailClass = UnHiddenClass;
                }
                else
                {
                    int id = CaseRegisterId;

                    if (id == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    var caseRegistration = db.CaseRegistration.Find(id);

                    if (caseRegistration == null)
                        return HttpNotFound();
                    else
                    {

                        #region SETTING PARTAIL VIEW VARIABLES


                        var courtCases = db.CourtCase.Find(modal.CaseId);

                        if (!string.IsNullOrEmpty(courtCases.ClientCode) && courtCases.ClientCode != "0")
                            ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

                        if (!string.IsNullOrEmpty(courtCases.SessionClientId) && courtCases.SessionClientId != "0")
                            SessionRollClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.SessionClients && w.Mst_Value == courtCases.SessionClientId).FirstOrDefault().Mst_Desc;


                        modal.ReceptionDate = courtCases.ReceptionDate?.ToString("dd/MM/yyyy");
                        modal.CourtReg_ClaimAmount = courtCases.ClaimAmount;
                        modal.CurrentHearingDate = courtCases.CurrentHearingDate;
                        modal.CourtDecision = courtCases.CourtDecision;
                        modal.Requirements = courtCases.Requirements;
                        modal.ClientReply = courtCases.ClientReply;
                        modal.TransportationFee = courtCases.TransportationFee;
                        modal.TransportationSource = courtCases.TransportationSource;
                        modal.FirstEmailDate = courtCases.FirstEmailDate;
                        modal.ClientName = ClientName;
                        modal.SessionRollClientName = SessionRollClientName;
                        modal.Defendant = courtCases.Defendant;
                        modal.SessionRollDefendentName = courtCases.SessionRollDefendentName;
                        modal.OfficeFileNo = courtCases.OfficeFileNo;

                        ViewBag.ActionLevel = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ActionLevel), "Mst_Value", "Mst_Desc", caseRegistration.ActionLevel);
                        ViewBag.EnforcementDispute = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementDispute), "Mst_Value", "Mst_Desc", caseRegistration.EnforcementDispute);
                        ViewBag.CourtRegistration = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", caseRegistration.CourtRegistration);
                        ViewBag.FileStatus = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", caseRegistration.FileStatus);

                        ViewBag.OnHoldReasonDDL = new SelectList(Helper.GetOnHoldReason(), "Mst_Value", "Mst_Desc", caseRegistration.FileStatus == "4" ? caseRegistration.FileStatusRemarks : "0");
                        ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", caseRegistration.ActionLevel == "2" ? caseRegistration.DepartmentType : "0");

                        ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", courtCases.ClientReply);
                        ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", courtCases.TransportationSource);


                        #endregion

                        #region SETTING VIEWBAG

                        string strSpanActionLevelTitle = "";
                        string strlblJudgementDate = "";
                        string strlblCourtRegistration = "";
                        string strSpanFileStatusTitle = "";
                        string strlblFileStatusRemarks = "";

                        //action level
                        if (int.Parse(caseRegistration.ActionLevel) > 0)
                        {
                            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
                            ViewBag.ClientName = ClientName;
                            ViewBag.Defendant = courtCases.Defendant;
                            ViewBag.dispACCNO = courtCases.AccountContractNo;
                            ViewBag.dispCFNO = courtCases.ClientFileNo;
                            ViewBag.FormMode = "EDIT";

                            ViewBag.CaseDetailClass = UnHiddenClass;
                            ViewBag.IsEdit = "disabled";
                            ViewBag.DivActionLevelDetail = UnHiddenClass;
                            ViewBag.PrintFormButton = UnHiddenClass;

                            if (caseRegistration.ActionLevel == "1")
                            {
                                strSpanActionLevelTitle = "NEW CASES";
                                ViewBag.divReceptionDate = UnHiddenClass;
                                strlblCourtRegistration = "PRIMARY COURT";
                                ViewBag.h3ActionLevelTitle = "CaseRegNewCaseActive";
                            }
                            else if (caseRegistration.ActionLevel == "2")
                            {
                                strSpanActionLevelTitle = "APPEAL";
                                ViewBag.divJudgementDate = UnHiddenClass;
                                ViewBag.DivAppealDeptt = UnHiddenClass;
                                strlblJudgementDate = "PRIMARY JUDGEMENT DATE";
                                strlblCourtRegistration = "APPEAL COURT";
                                ViewBag.h3ActionLevelTitle = "CaseRegAppealActive";
                            }
                            else if (caseRegistration.ActionLevel == "3")
                            {
                                strSpanActionLevelTitle = "SUPREME";
                                ViewBag.divJudgementDate = UnHiddenClass;
                                strlblJudgementDate = "APPEAL JUDGEMENT DATE";
                                strlblCourtRegistration = "SUPREME COURT";
                                ViewBag.h3ActionLevelTitle = "CaseRegSupremeActive";
                            }
                            else if (caseRegistration.ActionLevel == "4")
                            {
                                var EnfDetail = Helper.GetEnforcementDetail(caseRegistration.CaseId, caseRegistration.EnforcementDispute);
                                strSpanActionLevelTitle = "DISPUTE";

                                ViewBag.divEnforcementDispute = UnHiddenClass;
                                ViewBag.divJudgementDate = UnHiddenClass;
                                ViewBag.dispENFNO = EnfDetail.EnforcementNo;
                                ViewBag.dispENFCOURT = EnfDetail.EnforcementCourt;
                                ViewBag.h3ActionLevelTitle = "";
                                if (caseRegistration.EnforcementDispute == "1" || caseRegistration.EnforcementDispute == "3")
                                {
                                    strlblJudgementDate = "DATE OF RECEIPT DECISION";
                                }
                                else if (caseRegistration.EnforcementDispute == "2" || caseRegistration.EnforcementDispute == "4")
                                {
                                    strlblJudgementDate = "PRIMARY JUDGEMENT DATE";
                                }

                                strlblCourtRegistration = "COURT REGISTRATION";

                            }

                        }

                        //file status
                        if (caseRegistration.FileStatus == "2" || caseRegistration.FileStatus == "4" || caseRegistration.FileStatus == "6" || caseRegistration.FileStatus == "7" || caseRegistration.FileStatus == "8" || caseRegistration.FileStatus == "9" || caseRegistration.FileStatus == "10")
                        {
                            ViewBag.DivFileStatusDetail = UnHiddenClass;
                            strSpanFileStatusTitle = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.FileStatus && w.Mst_Value == caseRegistration.FileStatus).FirstOrDefault().Mst_Desc;

                            if (caseRegistration.FileStatus == "2")
                            {
                                ViewBag.Div2 = UnHiddenClass;
                                ViewBag.OmanPostDoc = Helper.GetOmanPostDoc(caseRegistration.CaseRegistrationId);
                            }
                            else if (caseRegistration.FileStatus == "4")
                            {
                                ViewBag.Div4 = UnHiddenClass;
                                strlblFileStatusRemarks = "ON HOLD REASON";
                            }
                            else if (caseRegistration.FileStatus == "6")
                            {
                                ViewBag.cls8cls6 = UnHiddenClass;
                                if (int.Parse(caseRegistration.ActionLevel) < 4)
                                    ViewBag.clsPAS = UnHiddenClass;


                                if (caseRegistration.ActionLevel == "1")
                                {
                                    ViewBag.clsP = UnHiddenClass;
                                }

                                if (caseRegistration.RealEstateYesNo == "Y")
                                {
                                    ViewBag.clsRealEstate = UnHiddenClass;
                                }

                                if (caseRegistration.ActionLevel == "1" || caseRegistration.ActionLevel == "2")
                                    ViewBag.clsPA = UnHiddenClass;

                                strlblFileStatusRemarks = "ELECTRONIC NO.";
                            }
                            else if (caseRegistration.FileStatus == "7")
                            {
                                ViewBag.Div7 = UnHiddenClass;
                            }
                            else if (caseRegistration.FileStatus == "8")
                            {
                                strlblFileStatusRemarks = "FEE";
                                ViewBag.cls8cls6 = UnHiddenClass;
                                ViewBag.cls8 = UnHiddenClass;
                                ViewBag.PaymentVoucherContainer = UnHiddenClass;
                            }
                            else if (caseRegistration.FileStatus == "9")
                            {
                                ViewBag.Div9 = UnHiddenClass;
                            }
                            else if (caseRegistration.FileStatus == "10")
                            {
                                ViewBag.Div10 = UnHiddenClass;
                            }
                        }


                        ViewBag.SpanActionLevelTitle = strSpanActionLevelTitle;
                        ViewBag.lblJudgementDate = strlblJudgementDate;
                        ViewBag.lblCourtRegistration = strlblCourtRegistration;
                        ViewBag.SpanFileStatusTitle = strSpanFileStatusTitle;
                        ViewBag.lblFileStatusRemarks = strlblFileStatusRemarks;
                        ViewBag.DIV_UPDATE = UnHiddenClass;
                        #endregion
                    }
                }

            }
            else if (PartialViewName == "_CaseRegPayVoucher")
            {
                if (Mode == "C")
                {
                    ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc");
                    ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                    List<MasterSetups> PaymentHeadList = new List<MasterSetups>();
                    MasterSetups PleaseSelect = new MasterSetups();
                    PleaseSelect.Mst_Value = "0";
                    PleaseSelect.Mst_Desc = "PLEASE SELECT";
                    PaymentHeadList.Add(PleaseSelect);

                    var ObjList = Helper.LoadPayFor("R");
                    var ObjList2 = (from Obj in ObjList select new MasterSetups { Mst_Value = Obj.Mst_Value, Mst_Desc = Obj.Mst_Desc }).ToList();

                    PaymentHeadList.AddRange(ObjList2);

                    ViewBag.Payment_Head = new SelectList(PaymentHeadList, "Mst_Value", "Mst_Desc");
                    ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");
                    ViewBag.VoucherDoc = "#";
                    ViewBag.PVTransferDoc = "#";

                }
                else
                {
                    int id = CaseRegisterId;

                    if (id == 0)
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                    var caseRegistration = db.CaseRegistration.Find(id);

                    if (caseRegistration == null)
                        return HttpNotFound();

                    var PayVoucher = db.PayVouchers.Find(caseRegistration.Voucher_No);
                    if (PayVoucher != null)
                    {
                        modal.CourtType = PayVoucher.CourtType;
                        modal.Payment_Head = PayVoucher.Payment_Head;
                        modal.Remarks = PayVoucher.Remarks;
                        modal.Payment_To = PayVoucher.Payment_To;
                        modal.Amount = PayVoucher.Amount;
                        modal.VatAmount = PayVoucher.VatAmount;
                        modal.BillNumber = PayVoucher.BillNumber;
                        modal.Debit_Account = PayVoucher.Debit_Account;
                        modal.Cheque_Number = PayVoucher.Cheque_Number;
                        modal.Cheque_Date = PayVoucher.Cheque_Date;


                        ViewBag.VoucherDoc = Helper.GetVoucherDoc((int)caseRegistration.Voucher_No);
                        ViewBag.PVTransferDoc = Helper.GetPVTranferDoc((int)caseRegistration.Voucher_No);


                        ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc", modal.CourtType);
                        ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc", modal.Debit_Account);
                        ViewBag.Payment_Head = new SelectList(Helper.LoadPayFor("R"), "Mst_Value", "Mst_Desc", modal.Payment_Head);
                        ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc", modal.Payment_To);

                    }
                    else
                    {
                        ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc");
                        ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                        ViewBag.Payment_Head = new SelectList(Helper.LoadPayFor("R"), "Mst_Value", "Mst_Desc");
                        ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");

                    }
                }
            }
            else if (PartialViewName == "_CaseRegBeforeCourt")
            {
                ViewBag.PoliceStation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                ViewBag.PublicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                ViewBag.PAPCPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                ViewBag.LaborConflicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
            }
            else if (PartialViewName == "_CaseRegEnforcement")
            {
                ViewBag.PrimaryObjectionCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc");
                ViewBag.ApealObjectionCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc");
                ViewBag.SupremeObjectionCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc");

                ViewBag.PrimaryPlaintCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc");
                ViewBag.ApealPlaintCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc");
                ViewBag.SupremePlaintCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc");

            }
            else if (PartialViewName == "_CaseRegCaseManagementInfo")
            {
                var CourtCase = db.CourtCase.Find(modal.CaseId);
                modal.IdRegistrationNo = CourtCase.IdRegistrationNo;
                modal.CRRegistrationNo = CourtCase.CRRegistrationNo;
                modal.CourtReg_ClaimAmount = CourtCase.ClaimAmount;

                ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", CourtCase.AgainstCode);
                ViewBag.CaseSubject = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseSubject), "Mst_Value", "Mst_Desc", CourtCase.CaseSubject);
                ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", CourtCase.CaseTypeCode);
                ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", CourtCase.ClientCaseType); // Blank
                ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", CourtCase.OmaniExp);
            }
            else if (PartialViewName == "_CaseRegCourtInfo")
            {
                int id = CaseRegisterId;

                if (id == 0)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var caseRegistration = db.CaseRegistration.Find(id);

                if (caseRegistration == null)
                    return HttpNotFound();

                string strCaseLevel = string.Empty;

                ViewBag.Courtid = caseRegistration.ActionLevel;

                if (caseRegistration.ActionLevel == "1")
                {
                    strCaseLevel = "PRIMARY NO";
                }
                else if (caseRegistration.ActionLevel == "2")
                {
                    strCaseLevel = "APPEAL NO";
                }
                else if (caseRegistration.ActionLevel == "3")
                {
                    strCaseLevel = "SUPREME NO";
                }

                ViewBag.CourtRefNo = strCaseLevel;
                ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(ViewBag.Courtid), "Mst_Value", "Mst_Desc", caseRegistration.CourtRegistration);
                if (caseRegistration.ActionLevel == "3")
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc");
                else
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");
            }


            return PartialView(PartialViewName, modal);

        }
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string UserLocation = string.Empty;

            var caseRegistration = db.CaseRegistration.Find(id);
            if (caseRegistration == null)
            {
                return HttpNotFound();
            }

            var courtCases = db.CourtCase.Find(caseRegistration.CaseId);

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;

            return View(caseRegistration);
        }

        // POST: CaseRegistration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var caseRegistration = db.CaseRegistration.Find(id);
            SoftDeleteCaseRegister(ref caseRegistration);
            MessageVM ProcessMessage = new MessageVM { id = -1, Category = "Success", Message = "RECORD DELETED SUCCESSFULLY" };

            return Json(new { redirectTo = "#btn_PayVoucherPDC" });

        }
        public ActionResult PrintForm(int id)
        {
            if (id <= 0)
                return HttpNotFound();

            var caseRegistration = db.CaseRegistration.Find(id);
            CaseRegistrationVM modal = new CaseRegistrationVM();
            Helper.GetCaseRegistrationVM(id, ref modal);
            if (!caseRegistration.IsDeleted)
            {
                modal.FormPrintJugDate = caseRegistration.JudgementDate?.ToString("dd/MM/yyyy");

                ViewBag.UserId = User.Identity.Name;
                ViewBag.PrintDate = DateTime.Now.ToString("dd/MM/yyyy");
            }

            return View(modal);
        }

        [HttpPost]
        public ActionResult GetTableTotal(string DataFor, string LocationId)
        {
            try
            {
                int TableTotal = Helper.GetTableTotal(DataFor, LocationId);

                return new JsonResult()
                {
                    Data = new { Message = "Success", TableTotal }
                };
            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new { Message = "Error", ErrorMessage = e.Message }
                };
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
