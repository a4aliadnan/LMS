using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using YandS.DAL;
using YandS.UI.Models;
using YandS.UI.Models.ViewModels;

namespace YandS.UI.Controllers
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<Error> AllErrors(this ModelStateDictionary modelState)
        {
            var result = new List<Error>();
            var erroneousFields = modelState.Where(ms => ms.Value.Errors.Any())
                                            .Select(x => new { x.Key, x.Value.Errors });

            foreach (var erroneousField in erroneousFields)
            {
                var fieldKey = erroneousField.Key;
                var fieldErrors = erroneousField.Errors
                                   .Select(error => new Error(fieldKey, error.ErrorMessage));
                result.AddRange(fieldErrors);
            }

            return result;
        }
    }
    [RBAC]
    public class CourtCasesController : Controller
    {
        private RBACDbContext db = new RBACDbContext();
        private string OfficeFileFilterSR = Helper.getOfficeFileFilterSR();
        private string OfficeFileFilterENF = Helper.getOfficeFileFilterENF();
        private string OfficeFileFilterTBR = Helper.getOfficeFileFilterTBR();
        private string[] FileStatusCodesSR = Helper.getFileStatusCodesSR();
        private string[] FileStatusCodesTBR = Helper.getFileStatusCodesTBR();

        #region Private Method

        private void UpdateSessionRoll(string CastObjName, object obj)
        {
            SessionsRoll _sessionRoll = new SessionsRoll();

            if (CastObjName == "ENFORCEMENT")
            {
                var objDTO = (CourtCasesEnforcement)obj;

                _sessionRoll = db.SessionsRoll.Find(objDTO.SessionRollId);

                db.Entry(_sessionRoll).Entity.CountLocationId = objDTO.CourtLocationid;

                db.Entry(_sessionRoll).Entity.ShowFollowup = objDTO.ShowFollowup;
                db.Entry(_sessionRoll).Entity.ShowSuspend = objDTO.ShowSuspend;
                db.Entry(_sessionRoll).Entity.LastDate = objDTO.LastDate;
                db.Entry(_sessionRoll).Entity.WorkRequired = objDTO.WorkRequired;
                db.Entry(_sessionRoll).Entity.SessionNotes = objDTO.SessionNotes;
                db.Entry(_sessionRoll).Entity.SuspendedWorkRequired = objDTO.SuspendedWorkRequired;
                db.Entry(_sessionRoll).Entity.SuspendedSessionNotes = objDTO.SuspendedSessionNotes;
                db.Entry(_sessionRoll).Entity.FollowerId = objDTO.FollowerId;

                db.Entry(_sessionRoll).State = EntityState.Modified;
                db.SaveChanges();

            }
            else if (CastObjName == "COURTDETAIL")
            {
                var objDTO = (CourtCasesDetailVM)obj;

                _sessionRoll = db.SessionsRoll.Find(objDTO.SessionRollId);

                if (_sessionRoll != null)
                {
                    db.Entry(_sessionRoll).Entity.CountLocationId = objDTO.CourtLocationid;

                    db.Entry(_sessionRoll).Entity.ShowFollowup = objDTO.ShowFollowup;
                    db.Entry(_sessionRoll).Entity.ShowSuspend = objDTO.ShowSuspend;
                    db.Entry(_sessionRoll).Entity.LastDate = objDTO.LastDate;
                    db.Entry(_sessionRoll).Entity.WorkRequired = objDTO.WorkRequired;
                    db.Entry(_sessionRoll).Entity.SessionNotes = objDTO.SessionNotes;
                    db.Entry(_sessionRoll).Entity.SuspendedWorkRequired = objDTO.SuspendedWorkRequired;
                    db.Entry(_sessionRoll).Entity.SuspendedSessionNotes = objDTO.SuspendedSessionNotes;
                    db.Entry(_sessionRoll).Entity.FollowerId = objDTO.FollowerId;

                    db.Entry(_sessionRoll).State = EntityState.Modified;
                    db.SaveChanges();

                }
                else
                {
                    SessionsRoll ModelToSave = new SessionsRoll();

                    ModelToSave.CaseId = objDTO.CaseId;
                    ModelToSave.CountLocationId = objDTO.CourtLocationid;

                    ModelToSave.CaseType = "0";
                    ModelToSave.LawyerId = "0";
                    ModelToSave.SessionLevel = objDTO.CaseLevelCode;

                    ModelToSave.ShowFollowup = objDTO.ShowFollowup;
                    ModelToSave.ShowSuspend = objDTO.ShowSuspend;
                    ModelToSave.LastDate = objDTO.LastDate;
                    ModelToSave.WorkRequired = objDTO.WorkRequired;
                    ModelToSave.SessionNotes = objDTO.SessionNotes;
                    ModelToSave.SuspendedWorkRequired = objDTO.SuspendedWorkRequired;
                    ModelToSave.SuspendedSessionNotes = objDTO.SuspendedSessionNotes;
                    ModelToSave.FollowerId = objDTO.FollowerId;


                    db.SessionsRoll.Add(ModelToSave);
                    db.SaveChanges();

                }

            }
            else if (CastObjName == "BEFORECOURT")
            {
                var objDTO = (BeforeCourtVM)obj;

                _sessionRoll = db.SessionsRoll.Find(objDTO.SessionRollId);

                if (_sessionRoll != null)
                {
                    //db.Entry(_sessionRoll).Entity.CountLocationId = objDTO.CourtLocationid;
                    db.Entry(_sessionRoll).Entity.CountLocationId = "0";

                    db.Entry(_sessionRoll).Entity.ShowFollowup = objDTO.ShowFollowup;
                    db.Entry(_sessionRoll).Entity.ShowSuspend = objDTO.ShowSuspend;
                    db.Entry(_sessionRoll).Entity.LastDate = objDTO.LastDate;
                    db.Entry(_sessionRoll).Entity.WorkRequired = objDTO.WorkRequired;
                    db.Entry(_sessionRoll).Entity.SessionNotes = objDTO.SessionNotes;
                    db.Entry(_sessionRoll).Entity.SuspendedWorkRequired = objDTO.SuspendedWorkRequired;
                    db.Entry(_sessionRoll).Entity.SuspendedSessionNotes = objDTO.SuspendedSessionNotes;
                    db.Entry(_sessionRoll).Entity.FollowerId = objDTO.FollowerId;

                    db.Entry(_sessionRoll).State = EntityState.Modified;
                    db.SaveChanges();

                }
                //else
                //{
                //    SessionsRoll ModelToSave = new SessionsRoll();

                //    ModelToSave.SessionClientId = objDTO.SessionClientId;
                //    ModelToSave.SessionRollDefendentName = objDTO.SessionRollDefendentName;
                //    ModelToSave.CaseId = objDTO.CaseId;
                //    ModelToSave.CountLocationId = objDTO.CourtLocationid;

                //    ModelToSave.CaseType = "0";
                //    ModelToSave.LawyerId = "0";
                //    ModelToSave.SessionLevel = objDTO.CaseLevelCode;

                //    ModelToSave.ShowFollowup = objDTO.ShowFollowup;
                //    ModelToSave.ShowSuspend = objDTO.ShowSuspend;
                //    ModelToSave.LastDate = objDTO.LastDate;
                //    ModelToSave.WorkRequired = objDTO.WorkRequired;
                //    ModelToSave.SessionNotes = objDTO.SessionNotes;
                //    ModelToSave.SuspendedWorkRequired = objDTO.SuspendedWorkRequired;
                //    ModelToSave.SuspendedSessionNotes = objDTO.SuspendedSessionNotes;
                //    ModelToSave.FollowerId = objDTO.FollowerId;


                //    db.SessionsRoll.Add(ModelToSave);
                //    db.SaveChanges();

                //}
            }
        }
        private void UpdateBeforeCourt(string CastObjName, object obj)
        {
            if (CastObjName == "BEFORECOURT")
            {
                var modal = (BeforeCourtVM)obj;
                CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

                db.Entry(courtCases).Entity.PoliceNo = modal.PoliceNo;
                db.Entry(courtCases).Entity.PublicProsecutionNo = modal.PublicProsecutionNo;
                db.Entry(courtCases).Entity.PAPCNo = modal.PAPCNo;
                db.Entry(courtCases).Entity.LaborConflicNo = modal.LaborConflicNo;
                db.Entry(courtCases).Entity.PoliceStation = modal.PoliceStation;
                db.Entry(courtCases).Entity.PublicPlace = modal.PublicPlace;
                db.Entry(courtCases).Entity.PAPCPlace = modal.PAPCPlace;
                db.Entry(courtCases).Entity.LaborConflicPlace = modal.LaborConflicPlace;
                db.Entry(courtCases).Entity.ReconciliationNo = modal.ReconciliationNo;
                db.Entry(courtCases).Entity.ReconciliationDep = modal.ReconciliationDep;

                db.Entry(courtCases).Entity.CurrentHearingDate = modal.CurrentHearingDate;
                db.Entry(courtCases).Entity.CourtDecision = modal.CourtDecision;
                db.Entry(courtCases).Entity.SessionRemarks = modal.SessionRemarks;
                db.Entry(courtCases).Entity.Requirements = modal.Requirements;
                db.Entry(courtCases).Entity.SessionClientId = modal.SessionClientId;
                db.Entry(courtCases).Entity.SessionRollDefendentName = modal.SessionRollDefendentName;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                UpdateSessionRoll("BEFORECOURT", modal);

            }
            else if (CastObjName == "CREATEBEFORECOURT")
            {
                var modal = (ToBeRegisterVM)obj;
                CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

                db.Entry(courtCases).Entity.PoliceNo = modal.PoliceNo;
                db.Entry(courtCases).Entity.PublicProsecutionNo = modal.PublicProsecutionNo;
                db.Entry(courtCases).Entity.PAPCNo = modal.PAPCNo;
                db.Entry(courtCases).Entity.LaborConflicNo = modal.LaborConflicNo;
                db.Entry(courtCases).Entity.PoliceStation = modal.PoliceStation;
                db.Entry(courtCases).Entity.PublicPlace = modal.PublicPlace;
                db.Entry(courtCases).Entity.PAPCPlace = modal.PAPCPlace;
                db.Entry(courtCases).Entity.LaborConflicPlace = modal.LaborConflicPlace;
                db.Entry(courtCases).Entity.ReconciliationNo = modal.ReconciliationNo;
                db.Entry(courtCases).Entity.ReconciliationDep = modal.ReconciliationDep;

                db.Entry(courtCases).Entity.CurrentHearingDate = modal.CurrentHearingDate;
                db.Entry(courtCases).Entity.CourtDecision = modal.CourtDecision;
                db.Entry(courtCases).Entity.SessionRemarks = modal.SessionRemarks;
                db.Entry(courtCases).Entity.Requirements = modal.Requirements;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void ProcessCourtDetail(object obj, string CastObjName = null, string PartialViewName = null)
        {
            var modal = (ToBeRegisterVM)obj;

            if (string.IsNullOrEmpty(CastObjName))
            {
                CourtCasesDetail _ModalToSave = new CourtCasesDetail();

                if (modal.DetailId == 0)
                {
                    _ModalToSave.CaseId = modal.CaseId;
                    _ModalToSave.Courtid = modal.Courtid;
                    _ModalToSave.CourtRefNo = modal.CourtRefNo;
                    _ModalToSave.CourtLocationid = modal.CourtLocationid;
                    _ModalToSave.RegistrationDate = modal.RegistrationDate;
                    _ModalToSave.CaseLevelCode = modal.CaseLevelCode;
                    _ModalToSave.ApealByWho = modal.ApealByWho;

                    db.CourtCasesDetail.Add(_ModalToSave);
                    db.SaveChanges();

                    Helper.ProcessCaseRegistrationProgress(obj);
                }
                else
                {
                    _ModalToSave = db.CourtCasesDetail.Find(modal.DetailId);

                    string OldCourtRegNumber = _ModalToSave.CourtRefNo;
                    string NewCourtRegNumber = modal.CourtRefNo;

                    db.Entry(_ModalToSave).Entity.Courtid = modal.Courtid;
                    db.Entry(_ModalToSave).Entity.CourtRefNo = modal.CourtRefNo;
                    db.Entry(_ModalToSave).Entity.CourtLocationid = modal.CourtLocationid;
                    db.Entry(_ModalToSave).Entity.RegistrationDate = modal.RegistrationDate;
                    db.Entry(_ModalToSave).Entity.CaseLevelCode = modal.CaseLevelCode;
                    db.Entry(_ModalToSave).Entity.ApealByWho = modal.ApealByWho;

                    db.Entry(_ModalToSave).State = EntityState.Modified;
                    db.SaveChanges();

                    if (OldCourtRegNumber != NewCourtRegNumber)
                        Helper.ProcessCaseRegistrationProgress(obj);
                }

            }
            else
            {
                CourtCasesEnforcement _ModalToSave = new CourtCasesEnforcement();

                if (modal.DetailId == 0)
                {
                    modal.Courtid = "4";
                    modal.CaseLevelCode = "6";

                    _ModalToSave.CaseId = modal.CaseId;
                    _ModalToSave.Courtid = modal.Courtid;
                    _ModalToSave.EnforcementNo = modal.CourtRefNo;
                    _ModalToSave.CourtLocationid = modal.CourtLocationid;
                    _ModalToSave.RegistrationDate = modal.RegistrationDate;
                    _ModalToSave.CaseLevelCode = modal.CaseLevelCode;
                    _ModalToSave.EnforcementAdmin = modal.EnforcementAdmin;
                    _ModalToSave.EnforcementBy = modal.ApealByWho;
                    _ModalToSave.EnforcementlevelId = modal.EnforcementlevelId;

                    if (!string.IsNullOrEmpty(PartialViewName))
                    {
                        _ModalToSave.ActionDate = modal.EnforcementActionDate; //Common for all
                        if (modal.UpdatePV_Type == "ENF_UPDATE")
                            _ModalToSave.LawyerId = modal.LawyerId;

                        _ModalToSave.DEF_Corresponding = modal.DEF_Corresponding; //Common for all
                        _ModalToSave.DEF_DateOfContact = modal.DEF_DateOfContact; //Common for all
                        _ModalToSave.DEF_MobileNo = modal.DEF_MobileNo; //Common for all
                        _ModalToSave.DEF_MobileNo2 = modal.DEF_MobileNo2; //Common for all
                        _ModalToSave.DEF_MobileNo3 = modal.DEF_MobileNo3; //Common for all
                        _ModalToSave.DEF_MobileNo4 = modal.DEF_MobileNo4; //Common for all
                        _ModalToSave.DEF_MobileNo5 = modal.DEF_MobileNo5; //Common for all
                        _ModalToSave.DEF_CallerName = modal.DEF_CallerName; //Common for all
                        _ModalToSave.DEF_LawyerId = modal.DEF_LawyerId; //Common for all
                        _ModalToSave.DEF_VisitDate = modal.DEF_VisitDate; //Common for all

                        if (modal.EnforcementlevelId == OfficeFileStatus.Announcement.ToString()) //ANNOUNCEMENT الإعلان
                        {
                            _ModalToSave.AnnouncementTypeId = modal.AnnouncementTypeId;
                            _ModalToSave.AnnouncementCompleteDt = modal.AnnouncementCompleteDt;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ContactingAuthorities.ToString()) //CONTACTING AUTHORITIES استعلام الجهات
                        {
                            _ModalToSave.SubmissionDt = modal.SubmissionDt;
                            _ModalToSave.ApprovalDt = modal.ApprovalDt;
                            _ModalToSave.InquiryDt = modal.InquiryDt;
                            _ModalToSave.MOHResultDt = modal.MOHResultDt;
                            _ModalToSave.MOHResult = modal.MOHResult;
                            _ModalToSave.ROPResultDt = modal.ROPResultDt;
                            _ModalToSave.ROPResult = modal.ROPResult;
                            _ModalToSave.BankResultDt = modal.BankResultDt;
                            _ModalToSave.BankResult = modal.BankResult;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.JudicalSale.ToString()) //JUDICIAL SALE البيع القضائي
                        {
                            _ModalToSave.AuctionProcess = modal.AuctionProcess;
                            _ModalToSave.JUDAuctionDate = modal.JUDAuctionDate;
                            _ModalToSave.JUDAuctionRepeat = modal.JUDAuctionRepeat;
                            _ModalToSave.JUDAuctionValue = modal.JUDAuctionValue;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ArrestApplication.ToString()) //ARREST APPLICATION طلب الحبس
                        {
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ArrestOrder.ToString()) //ARREST ORDER قرار بالحبس
                        {
                            _ModalToSave.ArrestOrderNo = modal.ArrestOrderNo;
                            _ModalToSave.ArrestOrderIssueDate = modal.ArrestOrderIssueDate;
                            _ModalToSave.PoliceStationid = modal.PoliceStationid;
                            _ModalToSave.Arrested = modal.Arrested;
                            _ModalToSave.ArrestedDate = modal.ArrestedDate;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.Suspendfd.ToString()) //SUSPENDED وقف مؤقت
                        {
                            _ModalToSave.SuspensionCauseId = modal.SuspensionCauseId;
                            _ModalToSave.SuspensionPeriod = modal.SuspensionPeriod;
                            _ModalToSave.SuspensionStartDate = modal.SuspensionStartDate;
                            _ModalToSave.JUDDecisionId = modal.JUDDecisionId;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.RecoveryRedStamp_Re_Open.ToString()) //RECOVERY JUDGMENT استرداد الصيغة
                        {
                            _ModalToSave.CauseOfRecoveryId = modal.CauseOfRecoveryId;
                            _ModalToSave.DateOfInstruction = modal.DateOfInstruction;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.RecoveryRedStamp_Close.ToString()) //RECOVERY JUDGMENT استرداد الصيغة
                        {
                            _ModalToSave.CauseOfRecoveryId = modal.CauseOfRecoveryId;

                            //CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();
                            //db.Entry(courtCases).Entity.ReasonCode = modal.ReasonCode;
                            //db.Entry(courtCases).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.Dispute.ToString()) //DISPUTE منازعة تنفيذ
                        {
                            _ModalToSave.CurrentDisputeLevelandType = modal.CurrentDisputeLevelandType;
                            _ModalToSave.PrimaryObjectionNo = modal.PrimaryObjectionNo;
                            _ModalToSave.PrimaryObjectionCourt = modal.PrimaryObjectionCourt;
                            _ModalToSave.ApealObjectionNo = modal.ApealObjectionNo;
                            _ModalToSave.ApealObjectionCourt = modal.ApealObjectionCourt;
                            _ModalToSave.SupremeObjectionNo = modal.SupremeObjectionNo;
                            _ModalToSave.SupremeObjectionCourt = modal.SupremeObjectionCourt;
                            _ModalToSave.PrimaryPlaintNo = modal.PrimaryPlaintNo;
                            _ModalToSave.PrimaryPlaintCourt = modal.PrimaryPlaintCourt;
                            _ModalToSave.ApealPlaintNo = modal.ApealPlaintNo;
                            _ModalToSave.ApealPlaintCourt = modal.ApealPlaintCourt;
                            _ModalToSave.SupremePlaintNo = modal.SupremePlaintNo;
                            _ModalToSave.SupremePlaintCourt = modal.SupremePlaintCourt;
                        }
                    }

                    db.CourtCasesEnforcement.Add(_ModalToSave);
                    db.SaveChanges();
                }
                else
                {
                    _ModalToSave = db.CourtCasesEnforcement.Find(modal.DetailId);

                    db.Entry(_ModalToSave).Entity.Courtid = modal.Courtid;
                    db.Entry(_ModalToSave).Entity.EnforcementNo = modal.CourtRefNo; //BASIC INFO 3
                    db.Entry(_ModalToSave).Entity.CourtLocationid = modal.CourtLocationid; //BASIC INFO 5
                    db.Entry(_ModalToSave).Entity.RegistrationDate = modal.RegistrationDate; //BASIC INFO 2
                    db.Entry(_ModalToSave).Entity.CaseLevelCode = modal.CaseLevelCode;
                    db.Entry(_ModalToSave).Entity.EnforcementBy = modal.ApealByWho; //BASIC INFO 1
                    db.Entry(_ModalToSave).Entity.EnforcementAdmin = modal.EnforcementAdmin; //Common
                    db.Entry(_ModalToSave).Entity.EnforcementlevelId = modal.EnforcementlevelId; //Common

                    if (!string.IsNullOrEmpty(PartialViewName))
                    {
                        db.Entry(_ModalToSave).Entity.ActionDate = modal.EnforcementActionDate; //Common for all

                        if (modal.UpdatePV_Type == "ENF_UPDATE")
                            db.Entry(_ModalToSave).Entity.LawyerId = modal.LawyerId; //Common for all

                        db.Entry(_ModalToSave).Entity.DEF_Corresponding = modal.DEF_Corresponding; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_DateOfContact = modal.DEF_DateOfContact; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_MobileNo = modal.DEF_MobileNo; //Common for all

                        db.Entry(_ModalToSave).Entity.DEF_MobileNo2 = modal.DEF_MobileNo2; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_MobileNo3 = modal.DEF_MobileNo3; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_MobileNo4 = modal.DEF_MobileNo4; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_MobileNo5 = modal.DEF_MobileNo5; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_CallerName = modal.DEF_CallerName; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_LawyerId = modal.DEF_LawyerId; //Common for all
                        db.Entry(_ModalToSave).Entity.DEF_VisitDate = modal.DEF_VisitDate; //Common for all


                        if (modal.EnforcementlevelId == OfficeFileStatus.Announcement.ToString()) //ANNOUNCEMENT الإعلان
                        {
                            db.Entry(_ModalToSave).Entity.AnnouncementTypeId = modal.AnnouncementTypeId;
                            db.Entry(_ModalToSave).Entity.AnnouncementCompleteDt = modal.AnnouncementCompleteDt;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ContactingAuthorities.ToString()) //CONTACTING AUTHORITIES استعلام الجهات
                        {
                            db.Entry(_ModalToSave).Entity.SubmissionDt = modal.SubmissionDt;
                            db.Entry(_ModalToSave).Entity.ApprovalDt = modal.ApprovalDt;
                            db.Entry(_ModalToSave).Entity.InquiryDt = modal.InquiryDt;
                            db.Entry(_ModalToSave).Entity.MOHResultDt = modal.MOHResultDt;
                            db.Entry(_ModalToSave).Entity.MOHResult = modal.MOHResult;
                            db.Entry(_ModalToSave).Entity.ROPResultDt = modal.ROPResultDt;
                            db.Entry(_ModalToSave).Entity.ROPResult = modal.ROPResult;
                            db.Entry(_ModalToSave).Entity.BankResultDt = modal.BankResultDt;
                            db.Entry(_ModalToSave).Entity.BankResult = modal.BankResult;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.JudicalSale.ToString()) //JUDICIAL SALE البيع القضائي
                        {
                            db.Entry(_ModalToSave).Entity.AuctionProcess = modal.AuctionProcess;
                            db.Entry(_ModalToSave).Entity.JUDAuctionDate = modal.JUDAuctionDate;
                            db.Entry(_ModalToSave).Entity.JUDAuctionRepeat = modal.JUDAuctionRepeat;
                            db.Entry(_ModalToSave).Entity.JUDAuctionValue = modal.JUDAuctionValue;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ArrestApplication.ToString()) //ARREST APPLICATION طلب الحبس
                        {
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.ArrestOrder.ToString()) //ARREST ORDER قرار بالحبس
                        {
                            db.Entry(_ModalToSave).Entity.ArrestOrderNo = modal.ArrestOrderNo;
                            db.Entry(_ModalToSave).Entity.ArrestOrderIssueDate = modal.ArrestOrderIssueDate;
                            db.Entry(_ModalToSave).Entity.PoliceStationid = modal.PoliceStationid;
                            db.Entry(_ModalToSave).Entity.Arrested = modal.Arrested;
                            db.Entry(_ModalToSave).Entity.ArrestedDate = modal.ArrestedDate;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.Suspendfd.ToString()) //SUSPENDED وقف مؤقت
                        {
                            db.Entry(_ModalToSave).Entity.SuspensionCauseId = modal.SuspensionCauseId;
                            db.Entry(_ModalToSave).Entity.SuspensionPeriod = modal.SuspensionPeriod;
                            db.Entry(_ModalToSave).Entity.SuspensionStartDate = modal.SuspensionStartDate;
                            db.Entry(_ModalToSave).Entity.JUDDecisionId = modal.JUDDecisionId;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.RecoveryRedStamp_Re_Open.ToString()) //RECOVERY JUDGMENT استرداد الصيغة
                        {
                            _ModalToSave.CauseOfRecoveryId = modal.CauseOfRecoveryId;
                            _ModalToSave.DateOfInstruction = modal.DateOfInstruction;
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.RecoveryRedStamp_Close.ToString()) //RECOVERY JUDGMENT استرداد الصيغة
                        {
                            _ModalToSave.CauseOfRecoveryId = modal.CauseOfRecoveryId;

                            //CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();
                            //db.Entry(courtCases).Entity.ReasonCode = modal.ReasonCode;
                            //db.Entry(courtCases).State = EntityState.Modified;
                            //db.SaveChanges();
                        }
                        else if (modal.EnforcementlevelId == OfficeFileStatus.Dispute.ToString()) //DISPUTE منازعة تنفيذ
                        {
                            db.Entry(_ModalToSave).Entity.CurrentDisputeLevelandType = modal.CurrentDisputeLevelandType;
                            db.Entry(_ModalToSave).Entity.PrimaryObjectionNo = modal.PrimaryObjectionNo;
                            db.Entry(_ModalToSave).Entity.PrimaryObjectionCourt = modal.PrimaryObjectionCourt;
                            db.Entry(_ModalToSave).Entity.ApealObjectionNo = modal.ApealObjectionNo;
                            db.Entry(_ModalToSave).Entity.ApealObjectionCourt = modal.ApealObjectionCourt;
                            db.Entry(_ModalToSave).Entity.SupremeObjectionNo = modal.SupremeObjectionNo;
                            db.Entry(_ModalToSave).Entity.SupremeObjectionCourt = modal.SupremeObjectionCourt;
                            db.Entry(_ModalToSave).Entity.PrimaryPlaintNo = modal.PrimaryPlaintNo;
                            db.Entry(_ModalToSave).Entity.PrimaryPlaintCourt = modal.PrimaryPlaintCourt;
                            db.Entry(_ModalToSave).Entity.ApealPlaintNo = modal.ApealPlaintNo;
                            db.Entry(_ModalToSave).Entity.ApealPlaintCourt = modal.ApealPlaintCourt;
                            db.Entry(_ModalToSave).Entity.SupremePlaintNo = modal.SupremePlaintNo;
                            db.Entry(_ModalToSave).Entity.SupremePlaintCourt = modal.SupremePlaintCourt;
                        }
                    }

                    db.Entry(_ModalToSave).State = EntityState.Modified;
                    db.SaveChanges();
                }

                if (modal.UpdatePV_Type == "ENF_UPDATE_SESSION")
                    ProcessSessionRollDetail(modal);
            }

            if (!string.IsNullOrEmpty(PartialViewName))
                UpdateCourtCases(obj, "", PartialViewName);
            else
                UpdateCourtCases(obj);
        }
        private void UpdateCourtCases(object obj, string CastObjName = null, string PartialViewName = null)
        {
            var modal = (ToBeRegisterVM)obj;
            string SelectCaseLevel = (int.Parse(modal.Courtid) + 2).ToString();

            CourtCases courtCases = db.CourtCase.Find(modal.CaseId);

            db.Entry(courtCases).Entity.CaseLevelCode = SelectCaseLevel;
            db.Entry(courtCases).Entity.CurrentHearingDate = modal.CurrentHearingDate; //UPDATE --> UPDATE DATE
            db.Entry(courtCases).Entity.CourtDecision = modal.CourtDecision; //UPDATE --> ENFORCEMENT UPDATES
            db.Entry(courtCases).Entity.SessionRemarks = modal.SessionRemarks;
            db.Entry(courtCases).Entity.Requirements = modal.Requirements; //UPDATE --> ENFORCEMENT UPDATES
            db.Entry(courtCases).Entity.ClaimSummary = modal.ClaimSummary;
            db.Entry(courtCases).Entity.RealEstateYesNo = modal.RealEstateYesNo; //BASIC INFO 12
            db.Entry(courtCases).Entity.RealEstateDetail = modal.RealEstateDetail; //BASIC INFO 12
            db.Entry(courtCases).Entity.GovernorateId = modal.GovernorateId; //BASIC INFO 4
            db.Entry(courtCases).Entity.AgainstInsurance = modal.AgainstInsurance; //BASIC INFO 6
            db.Entry(courtCases).Entity.OfficeFileStatus = modal.EnforcementlevelId; //NEW

            if (!string.IsNullOrEmpty(PartialViewName))
            {
                db.Entry(courtCases).Entity.SessionClientId = modal.SessionClientId; //BASIC INFO 1
                db.Entry(courtCases).Entity.SessionRollDefendentName = modal.SessionRollDefendentName; //BASIC INFO 2

                db.Entry(courtCases).Entity.ReceiveLevelCode = modal.ReceiveLevelCode; //BASIC INFO 7
                db.Entry(courtCases).Entity.AgainstCode = modal.AgainstCode; //BASIC INFO 8
                db.Entry(courtCases).Entity.OmaniExp = modal.OmaniExp; //BASIC INFO 9
                db.Entry(courtCases).Entity.CaseTypeCode = modal.CaseTypeCode; //BASIC INFO 10
                db.Entry(courtCases).Entity.ReOpenEnforcement = modal.ReOpenEnforcement; //BASIC INFO 11
                db.Entry(courtCases).Entity.ClientReply = modal.ClientReply; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.CourtFollow = modal.CourtFollow; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.CourtFollowRequirement = modal.CourtFollowRequirement; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.FirstEmailDate = modal.FirstEmailDate; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.CommissioningDate = modal.CommissioningDate; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.NextHearingDate = modal.NextHearingDate; //UPDATE --> ENFORCEMENT UPDATES
                db.Entry(courtCases).Entity.TransportationSource = modal.TransportationSource; //UPDATE --> ENFORCEMENT UPDATES
            }

            db.Entry(courtCases).State = EntityState.Modified;
            db.SaveChanges();

        }
        private ToBeRegisterVM GetFilledPartailView(int CaseId, string PartialViewName = null)
        {
            ToBeRegisterVM ViewModal = new ToBeRegisterVM();
            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();

            if (courtCases != null)
            {
                string ClientName = string.Empty;
                string ClientClassName = string.Empty;
                string CaseStatusName = string.Empty;
                string SessionRollClientName = string.Empty;

                if (!string.IsNullOrEmpty(courtCases.ClientCode) && courtCases.ClientCode != "0")
                    ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

                if (!string.IsNullOrEmpty(courtCases.ClientClassificationCode) && courtCases.ClientClassificationCode != "0")
                    ClientClassName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.ClientClassification && w.Mst_Value == courtCases.ClientClassificationCode).FirstOrDefault().Mst_Desc;

                if (!string.IsNullOrEmpty(courtCases.CaseStatus) && courtCases.CaseStatus != "0")
                    CaseStatusName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseStatus && w.Mst_Value == courtCases.CaseStatus).FirstOrDefault().Mst_Desc;

                if (!string.IsNullOrEmpty(courtCases.SessionClientId) && courtCases.SessionClientId != "0")
                    SessionRollClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.SessionClients && w.Mst_Value == courtCases.SessionClientId).FirstOrDefault().Mst_Desc;

                ViewModal.CaseId = courtCases.CaseId;
                ViewModal.OfficeFileNo = courtCases.OfficeFileNo;
                ViewModal.ClientClassificationCode = courtCases.ClientClassificationCode;
                ViewModal.ReceptionDate = courtCases.ReceptionDate;
                ViewModal.ReceiveLevelCode = courtCases.ReceiveLevelCode;
                ViewModal.CaseTypeCode = courtCases.CaseTypeCode;
                ViewModal.AgainstCode = courtCases.AgainstCode;
                ViewModal.ClientCode = courtCases.ClientCode;
                ViewModal.Defendant = courtCases.Defendant;
                ViewModal.CaseLevelCode = courtCases.CaseLevelCode;
                ViewModal.ClientCaseType = courtCases.ClientCaseType;
                ViewModal.AccountContractNo = courtCases.AccountContractNo;
                ViewModal.ClientFileNo = courtCases.ClientFileNo;
                ViewModal.ClaimAmount = courtCases.ClaimAmount;
                ViewModal.IdRegistrationNo = courtCases.IdRegistrationNo;
                ViewModal.OmaniExp = courtCases.OmaniExp;
                ViewModal.CRRegistrationNo = courtCases.CRRegistrationNo;
                ViewModal.CaseSubject = courtCases.CaseSubject;
                ViewModal.Subject = courtCases.Subject;
                ViewModal.ODBBankBranch = courtCases.ODBBankBranch;
                ViewModal.LoanManager = courtCases.LoanManager;
                ViewModal.SpecialInstructions = courtCases.SpecialInstructions;
                ViewModal.YandSNotes = courtCases.YandSNotes;
                ViewModal.CurrentHearingDate = courtCases.CurrentHearingDate;
                ViewModal.CourtDecision = courtCases.CourtDecision;
                ViewModal.SessionRemarks = courtCases.SessionRemarks;
                ViewModal.Requirements = courtCases.Requirements;
                ViewModal.SessionClientId = courtCases.SessionClientId;
                ViewModal.SessionRollDefendentName = courtCases.SessionRollDefendentName;
                ViewModal.SessionRollClientName = SessionRollClientName;
                ViewModal.CourtFollowRequirement = courtCases.CourtFollowRequirement;
                ViewModal.UpdatedOn = courtCases.UpdatedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? courtCases.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
                ViewModal.UpdateBoxDate = courtCases.UpdateBoxDate?.ToString("dd/MM/yyyy") ?? string.Empty;
                ViewModal.UpdatedBy = Helper.GetUserName(courtCases?.UpdatedBy ?? 0);
                ViewModal.UpdateBoxBy = courtCases?.UpdateBoxBy;
                ViewModal.UpdateBoxByName = Helper.GetUserName(courtCases?.UpdateBoxBy ?? 0);


                #region BEFORE COURT

                //BEFORE COURT
                ViewModal.PoliceNo = courtCases.PoliceNo;
                ViewModal.PoliceStation = courtCases.PoliceStation;
                ViewModal.PublicProsecutionNo = courtCases.PublicProsecutionNo;
                ViewModal.PublicPlace = courtCases.PublicPlace;
                ViewModal.PAPCNo = courtCases.PAPCNo;
                ViewModal.PAPCPlace = courtCases.PAPCPlace;
                ViewModal.LaborConflicNo = courtCases.LaborConflicNo;
                ViewModal.LaborConflicPlace = courtCases.LaborConflicPlace;
                ViewModal.ReconciliationNo = courtCases.ReconciliationNo;
                ViewModal.ReconciliationDep = courtCases.ReconciliationDep;

                #endregion

                #region NEW CASES [Tobe Register]

                //NEW CASES [Tobe Register]

                ViewModal.ReceptionDateDisp = courtCases.ReceptionDate;
                var caseRegistration = db.CaseRegistration.Where(w => w.CaseId == CaseId).OrderByDescending(O => O.CaseRegistrationId).FirstOrDefault();

                if (caseRegistration != null)
                {
                    ViewModal.CaseRegistrationId = caseRegistration.CaseRegistrationId;
                    ViewModal.ActionDate = caseRegistration.ActionDate;
                    ViewModal.AdminFile = caseRegistration.AdminFile;
                    ViewModal.UrgentCaseDays = caseRegistration.UrgentCaseDays;
                    ViewModal.CourtDetailRegistered = caseRegistration.CourtDetailRegistered;
                    ViewModal.OfficeProcedure = caseRegistration.OfficeProcedure;
                    ViewModal.MainRemarks = caseRegistration.Remarks;
                    ViewModal.FormPrintLastDate = caseRegistration.FormPrintLastDate;
                    ViewModal.FormPrintWorkRequired = caseRegistration.FormPrintWorkRequired;
                    ViewModal.FileStatus = courtCases.OfficeFileStatus;
                }

                #endregion

                #region PRIMARY APPEAL SUPREME ENFORCEMENT

                //PRIMARY APPEAL SUPREME ENFORCEMENT

                if (string.IsNullOrEmpty(PartialViewName))
                {
                    if (int.Parse(courtCases.CaseLevelCode) >= 3 && int.Parse(courtCases.CaseLevelCode) <= 5)
                    {
                        var courtCaseDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.CaseLevelCode == courtCases.CaseLevelCode).OrderByDescending(O => O.DetailId).FirstOrDefault();

                        if (courtCaseDetail != null)
                        {
                            ViewModal.DetailId = courtCaseDetail.DetailId;
                            ViewModal.Courtid = courtCaseDetail.Courtid;
                            ViewModal.CourtRefNo = courtCaseDetail.CourtRefNo;
                            ViewModal.CourtLocationid = courtCaseDetail.CourtLocationid;
                            ViewModal.RegistrationDate = courtCaseDetail.RegistrationDate;
                            ViewModal.ApealByWho = courtCaseDetail.ApealByWho;

                            if (int.Parse(courtCases.CaseLevelCode) == 3)
                                ViewModal.CaseLevelName = "PRIMARY COURT";
                            else if (int.Parse(courtCases.CaseLevelCode) == 4)
                                ViewModal.CaseLevelName = "APPEAL COURT";
                            else if (int.Parse(courtCases.CaseLevelCode) == 5)
                                ViewModal.CaseLevelName = "SUPREME COURT";

                            if (!string.IsNullOrEmpty(courtCaseDetail.CourtLocationid))
                                ViewModal.COURT = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCaseDetail.CourtLocationid).FirstOrDefault().Mst_Desc;
                            else
                                ViewModal.COURT = string.Empty;

                        }
                    }
                    else if (int.Parse(courtCases.CaseLevelCode) == 6)
                    {
                        var courtCasesEnforcement = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).OrderByDescending(O => O.EnforcementId).FirstOrDefault();

                        if (courtCasesEnforcement != null)
                        {
                            ViewModal.DetailId = courtCasesEnforcement.EnforcementId;
                            ViewModal.Courtid = courtCasesEnforcement.Courtid;
                            ViewModal.CourtRefNo = courtCasesEnforcement.EnforcementNo;
                            ViewModal.CourtLocationid = courtCasesEnforcement.CourtLocationid;
                            ViewModal.RegistrationDate = courtCasesEnforcement.RegistrationDate;
                            ViewModal.ApealByWho = courtCasesEnforcement.EnforcementBy;
                            ViewModal.CaseLevelName = "ENFORCEMENT COURT";
                            ViewModal.EnforcementAdmin = courtCasesEnforcement.EnforcementAdmin;
                            ViewModal.EnforcementlevelId = courtCases.OfficeFileStatus;

                            if (!string.IsNullOrEmpty(courtCasesEnforcement.CourtLocationid))
                                ViewModal.COURT = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCasesEnforcement.CourtLocationid).FirstOrDefault().Mst_Desc;
                            else
                                ViewModal.COURT = string.Empty;
                        }
                    }
                }
                else
                {
                    string strCaseLevelCode = "0";
                    if (PartialViewName == "_PartViewPrimary")
                    {
                        strCaseLevelCode = "3";
                    }
                    else if (PartialViewName == "_PartViewAppeal")
                    {
                        strCaseLevelCode = "4";
                    }
                    else if (PartialViewName == "_PartViewSupreme")
                    {
                        strCaseLevelCode = "5";
                    }
                    else
                    {
                        strCaseLevelCode = "6";
                    }

                    if (int.Parse(strCaseLevelCode) >= 3 && int.Parse(strCaseLevelCode) <= 5)
                    {
                        var courtCaseDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.CaseLevelCode == strCaseLevelCode).OrderByDescending(O => O.DetailId).FirstOrDefault();

                        if (courtCaseDetail != null)
                        {
                            ViewModal.DetailId = courtCaseDetail.DetailId;
                            ViewModal.Courtid = courtCaseDetail.Courtid;
                            ViewModal.CourtRefNo = courtCaseDetail.CourtRefNo;
                            ViewModal.CourtLocationid = courtCaseDetail.CourtLocationid;
                            ViewModal.RegistrationDate = courtCaseDetail.RegistrationDate;
                            ViewModal.ApealByWho = courtCaseDetail.ApealByWho;

                            if (int.Parse(courtCases.CaseLevelCode) == 3)
                                ViewModal.CaseLevelName = "PRIMARY COURT";
                            else if (int.Parse(courtCases.CaseLevelCode) == 4)
                                ViewModal.CaseLevelName = "APPEAL COURT";
                            else if (int.Parse(courtCases.CaseLevelCode) == 5)
                                ViewModal.CaseLevelName = "SUPREME COURT";

                            if (!string.IsNullOrEmpty(courtCaseDetail.CourtLocationid))
                                ViewModal.COURT = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCaseDetail.CourtLocationid).FirstOrDefault().Mst_Desc;
                            else
                                ViewModal.COURT = string.Empty;

                        }
                    }
                    else if (int.Parse(strCaseLevelCode) == 6)
                    {
                        var courtCasesEnforcement = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).OrderByDescending(O => O.EnforcementId).FirstOrDefault();

                        if (courtCasesEnforcement != null)
                        {
                            if (PartialViewName == "_PartViewEnforcement")
                            {
                                ViewModal.DetailId = courtCasesEnforcement.EnforcementId;
                                ViewModal.Courtid = courtCasesEnforcement.Courtid;
                                ViewModal.CourtRefNo = courtCasesEnforcement.EnforcementNo;
                                ViewModal.CourtLocationid = courtCasesEnforcement.CourtLocationid;
                                ViewModal.RegistrationDate = courtCasesEnforcement.RegistrationDate;
                                ViewModal.ApealByWho = courtCasesEnforcement.EnforcementBy;
                                ViewModal.CaseLevelName = "ENFORCEMENT COURT";
                                ViewModal.EnforcementAdmin = courtCasesEnforcement.EnforcementAdmin;
                                ViewModal.EnforcementlevelId = courtCases.OfficeFileStatus;

                                if (!string.IsNullOrEmpty(courtCasesEnforcement.CourtLocationid))
                                    ViewModal.COURT = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCasesEnforcement.CourtLocationid).FirstOrDefault().Mst_Desc;
                                else
                                    ViewModal.COURT = string.Empty;

                            }
                            else
                            {
                                ViewModal.DetailId = courtCasesEnforcement.EnforcementId;
                                ViewModal.Courtid = courtCasesEnforcement.Courtid;
                                ViewModal.CourtRefNo = courtCasesEnforcement.EnforcementNo;
                                ViewModal.CourtLocationid = courtCasesEnforcement.CourtLocationid;
                                ViewModal.RegistrationDate = courtCasesEnforcement.RegistrationDate;
                                ViewModal.ApealByWho = courtCasesEnforcement.EnforcementBy;
                                ViewModal.CaseLevelName = "ENFORCEMENT COURT";
                                ViewModal.EnforcementAdmin = courtCasesEnforcement.EnforcementAdmin;
                                ViewModal.EnforcementlevelId = courtCases.OfficeFileStatus;

                                if (!string.IsNullOrEmpty(courtCasesEnforcement.CourtLocationid))
                                    ViewModal.COURT = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCasesEnforcement.CourtLocationid).FirstOrDefault().Mst_Desc;
                                else
                                    ViewModal.COURT = string.Empty;


                                ViewModal.EnforcementActionDate = courtCasesEnforcement.ActionDate;
                                ViewModal.LawyerId = courtCasesEnforcement.LawyerId ?? "0";
                                ViewModal.DEF_Corresponding = courtCasesEnforcement.DEF_Corresponding;
                                ViewModal.DEF_DateOfContact = courtCasesEnforcement.DEF_DateOfContact;
                                ViewModal.DEF_MobileNo = courtCasesEnforcement.DEF_MobileNo;

                                ViewModal.AnnouncementTypeId = courtCasesEnforcement.AnnouncementTypeId;
                                ViewModal.AnnouncementCompleteDt = courtCasesEnforcement.AnnouncementCompleteDt;

                                ViewModal.SubmissionDt = courtCasesEnforcement.SubmissionDt;
                                ViewModal.ApprovalDt = courtCasesEnforcement.ApprovalDt;
                                ViewModal.InquiryDt = courtCasesEnforcement.InquiryDt;
                                ViewModal.MOHResultDt = courtCasesEnforcement.MOHResultDt;
                                ViewModal.MOHResult = courtCasesEnforcement.MOHResult;
                                ViewModal.ROPResultDt = courtCasesEnforcement.ROPResultDt;
                                ViewModal.ROPResult = courtCasesEnforcement.ROPResult;
                                ViewModal.BankResultDt = courtCasesEnforcement.BankResultDt;
                                ViewModal.BankResult = courtCasesEnforcement.BankResult;

                                ViewModal.AuctionProcess = courtCasesEnforcement.AuctionProcess;
                                ViewModal.JUDAuctionDate = courtCasesEnforcement.JUDAuctionDate;
                                ViewModal.JUDAuctionRepeat = courtCasesEnforcement.JUDAuctionRepeat;
                                ViewModal.JUDAuctionValue = courtCasesEnforcement.JUDAuctionValue;

                                ViewModal.ArrestOrderNo = courtCasesEnforcement.ArrestOrderNo;
                                ViewModal.ArrestOrderIssueDate = courtCasesEnforcement.ArrestOrderIssueDate;
                                ViewModal.PoliceStationid = courtCasesEnforcement.PoliceStationid;
                                ViewModal.Arrested = courtCasesEnforcement.Arrested;
                                ViewModal.ArrestedDate = courtCasesEnforcement.ArrestedDate;

                                ViewModal.SuspensionCauseId = courtCasesEnforcement.SuspensionCauseId;
                                ViewModal.SuspensionPeriod = courtCasesEnforcement.SuspensionPeriod;
                                ViewModal.SuspensionStartDate = courtCasesEnforcement.SuspensionStartDate;
                                ViewModal.JUDDecisionId = courtCasesEnforcement.JUDDecisionId;

                                ViewModal.CauseOfRecoveryId = courtCasesEnforcement.CauseOfRecoveryId;
                                ViewModal.DateOfInstruction = courtCasesEnforcement.DateOfInstruction;

                                ViewModal.CurrentDisputeLevelandType = courtCasesEnforcement.CurrentDisputeLevelandType;
                                ViewModal.PrimaryObjectionNo = courtCasesEnforcement.PrimaryObjectionNo;
                                ViewModal.PrimaryObjectionCourt = courtCasesEnforcement.PrimaryObjectionCourt;
                                ViewModal.ApealObjectionNo = courtCasesEnforcement.ApealObjectionNo;
                                ViewModal.ApealObjectionCourt = courtCasesEnforcement.ApealObjectionCourt;
                                ViewModal.SupremeObjectionNo = courtCasesEnforcement.SupremeObjectionNo;
                                ViewModal.SupremeObjectionCourt = courtCasesEnforcement.SupremeObjectionCourt;
                                ViewModal.PrimaryPlaintNo = courtCasesEnforcement.PrimaryPlaintNo;
                                ViewModal.PrimaryPlaintCourt = courtCasesEnforcement.PrimaryPlaintCourt;
                                ViewModal.ApealPlaintNo = courtCasesEnforcement.ApealPlaintNo;
                                ViewModal.ApealPlaintCourt = courtCasesEnforcement.ApealPlaintCourt;
                                ViewModal.SupremePlaintNo = courtCasesEnforcement.SupremePlaintNo;
                                ViewModal.SupremePlaintCourt = courtCasesEnforcement.SupremePlaintCourt;

                                ViewModal.MoneyTrRequestDate = courtCasesEnforcement.MoneyTrRequestDate;

                                ViewModal.DEF_MobileNo2 = courtCasesEnforcement.DEF_MobileNo2;
                                ViewModal.DEF_MobileNo3 = courtCasesEnforcement.DEF_MobileNo3;
                                ViewModal.DEF_MobileNo4 = courtCasesEnforcement.DEF_MobileNo4;
                                ViewModal.DEF_MobileNo5 = courtCasesEnforcement.DEF_MobileNo5;
                                ViewModal.DEF_CallerName = courtCasesEnforcement.DEF_CallerName;
                                ViewModal.DEF_LawyerId = courtCasesEnforcement.DEF_LawyerId;
                                ViewModal.DEF_VisitDate = courtCasesEnforcement.DEF_VisitDate;
                            }
                        }
                    }
                }

                #endregion

                #region SESSION ROLL
                var sessionRoll = db.SessionsRoll.Where(w => w.CaseId == CaseId && w.DeletedOn == null).OrderByDescending(O => O.SessionRollId).FirstOrDefault();

                ViewModal.CountLocationName = ViewModal.COURT;
                if (!string.IsNullOrEmpty(courtCases.CaseLevelCode) && courtCases.CaseLevelCode != "0")
                    ViewModal.CurrentCaseLevel = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseLevel && w.Mst_Value == courtCases.CaseLevelCode).FirstOrDefault().Mst_Desc;

                if (sessionRoll != null)
                {
                    ViewModal.SessionRollId = sessionRoll.SessionRollId;
                    ViewModal.CaseType = sessionRoll.CaseType;
                    ViewModal.Session_LawyerId = sessionRoll.LawyerId;
                    ViewModal.CourtFollow_LawyerId = sessionRoll.CourtFollow_LawyerId;
                    ViewModal.FollowerId = sessionRoll.FollowerId;
                    ViewModal.SuspendedFollowerId = sessionRoll.SuspendedFollowerId;

                    ViewModal.ShowFollowup = sessionRoll.ShowFollowup;
                    ViewModal.Update_Follow = sessionRoll.ShowFollowup ? "Y" : "N";
                    ViewModal.ShowSuspend = sessionRoll.ShowSuspend;
                    ViewModal.Update_Suspend = sessionRoll.ShowSuspend ? "Y" : "N";

                    ViewModal.WorkRequired = sessionRoll.WorkRequired;
                    ViewModal.SessionNotes = sessionRoll.SessionNotes;
                    ViewModal.LastDate = sessionRoll.LastDate;

                    ViewModal.SuspendedWorkRequired = sessionRoll.SuspendedWorkRequired;
                    ViewModal.SuspendedSessionNotes = sessionRoll.SuspendedSessionNotes;
                    ViewModal.SuspendedLastDate = sessionRoll.SuspendedLastDate;
                    ViewModal.SessionNote_Remark = sessionRoll.SessionNote_Remark;
                }

                #endregion

                ViewModal.RealEstateYesNo = courtCases.RealEstateYesNo;
                ViewModal.RealEstateDetail = courtCases.RealEstateDetail;
                ViewModal.ClaimSummary = courtCases.ClaimSummary;
                ViewModal.GovernorateId = courtCases.GovernorateId;
                ViewModal.AgainstInsurance = courtCases.AgainstInsurance;
                ViewModal.ClientName = ClientName;
                ViewModal.ClientClassName = ClientClassName;
                ViewModal.CaseStatusName = CaseStatusName;
                ViewModal.ClientReply = courtCases.ClientReply;
                ViewModal.CourtFollow = courtCases.CourtFollow;
                ViewModal.FirstEmailDate = courtCases.FirstEmailDate;
                ViewModal.CommissioningDate = courtCases.CommissioningDate;
                ViewModal.ReasonCode = courtCases.ReasonCode;
                ViewModal.LitigationFileClosureDate = courtCases.LitigationFileClosureDate;
                ViewModal.FileAllocation = courtCases.FileAllocation;
                ViewModal.ReOpenEnforcement = courtCases.ReOpenEnforcement;
                ViewModal.TransportationSource = courtCases.TransportationSource;
                ViewModal.NextHearingDate = courtCases.NextHearingDate;
                ViewModal.TransportationFee = courtCases.TransportationFee;
            }

            return ViewModal;
        }
        private FileClosureEnteryVM GetFileClosureVM(int CaseId)
        {
            FileClosureEnteryVM ViewModal = new FileClosureEnteryVM();
            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();

            if (courtCases != null)
            {
                string ClientName = string.Empty;
                string ClientClassName = string.Empty;
                string CaseStatusName = string.Empty;

                if (!string.IsNullOrEmpty(courtCases.ClientCode))
                    ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

                if (!string.IsNullOrEmpty(courtCases.ClientClassificationCode))
                    ClientClassName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.ClientClassification && w.Mst_Value == courtCases.ClientClassificationCode).FirstOrDefault().Mst_Desc;

                if (!string.IsNullOrEmpty(courtCases.CaseStatus))
                    CaseStatusName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseStatus && w.Mst_Value == courtCases.CaseStatus).FirstOrDefault().Mst_Desc;

                ViewModal.CaseId = courtCases.CaseId;
                ViewModal.OfficeFileNo = courtCases.OfficeFileNo;
                ViewModal.ClientClassificationCode = courtCases.ClientClassificationCode;
                ViewModal.ReceptionDate = courtCases.ReceptionDate;
                ViewModal.ReceiveLevelCode = courtCases.ReceiveLevelCode;
                ViewModal.CaseTypeCode = courtCases.CaseTypeCode;
                ViewModal.AgainstCode = courtCases.AgainstCode;
                ViewModal.ClientCode = courtCases.ClientCode;
                ViewModal.Defendant = courtCases.Defendant;
                ViewModal.ClientCaseType = courtCases.ClientCaseType;
                ViewModal.AccountContractNo = courtCases.AccountContractNo;
                ViewModal.ClientFileNo = courtCases.ClientFileNo;
                ViewModal.ClaimAmount = courtCases.ClaimAmount;
                ViewModal.OmaniExp = courtCases.OmaniExp;
                ViewModal.ODBBankBranch = courtCases.ODBBankBranch;
                ViewModal.LoanManager = courtCases.LoanManager;
                ViewModal.GovernorateId = courtCases.GovernorateId;
                ViewModal.FinanceFileClosureDate = courtCases.FinanceFileClosureDate;
                ViewModal.ReOpenEnforcement = courtCases.ReOpenEnforcement;
                ViewModal.ClosingNotes = courtCases.ClosingNotes;
                ViewModal.StatusCode = courtCases.CaseStatus;
                ViewModal.ClientName = ClientName;
                ViewModal.ReasonCode = courtCases.ReasonCode;
                ViewModal.LitigationFileClosureDate = courtCases.LitigationFileClosureDate;
                ViewModal.FileAllocation = courtCases.FileAllocation;
                ViewModal.StatusDate = courtCases.ClosureDate;
                ViewModal.CurrentCaseLevel = courtCases.CaseLevelCode;
                ViewModal.JudRecRedStamp = courtCases.JudRecRedStamp;

            }

            return ViewModal;
        }
        private void ProcessCourtDecisionHistory(ToBeRegisterVM modal)
        {
            Helper.ProcessCourtDecisionHistory(modal, HttpContext.User.Identity.GetUserId(), "ToBeRegisterVM");
        }
        private void ProcessModifyCase(ToBeRegisterVM RegModal)
        {
            ProcessCourtDecisionHistory(RegModal);
            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == RegModal.CaseId).FirstOrDefault();
            //db.Entry(courtCases).Entity.OfficeFileNo = RegModal.OfficeFileNo;
            db.Entry(courtCases).Entity.ClientClassificationCode = RegModal.ClientClassificationCode;
            db.Entry(courtCases).Entity.ReceptionDate = RegModal.ReceptionDate;
            db.Entry(courtCases).Entity.ReceiveLevelCode = RegModal.ReceiveLevelCode;
            db.Entry(courtCases).Entity.CaseTypeCode = RegModal.CaseTypeCode;
            db.Entry(courtCases).Entity.AgainstCode = RegModal.AgainstCode;
            db.Entry(courtCases).Entity.ClientCode = RegModal.ClientCode;
            db.Entry(courtCases).Entity.Defendant = RegModal.Defendant;
            db.Entry(courtCases).Entity.OmaniExp = RegModal.OmaniExp;

            db.Entry(courtCases).Entity.SessionClientId = RegModal.SessionClientId;
            db.Entry(courtCases).Entity.SessionRollDefendentName = RegModal.SessionRollDefendentName;

            db.Entry(courtCases).Entity.CaseLevelCode = RegModal.CaseLevelCode;
            db.Entry(courtCases).Entity.CurrentHearingDate = RegModal.CurrentHearingDate;
            db.Entry(courtCases).Entity.CourtDecision = RegModal.CourtDecision;
            db.Entry(courtCases).Entity.SessionRemarks = RegModal.SessionRemarks;
            db.Entry(courtCases).Entity.Requirements = RegModal.Requirements;
            db.Entry(courtCases).Entity.ClientReply = RegModal.ClientReply;
            db.Entry(courtCases).Entity.TransportationSource = RegModal.TransportationSource;
            db.Entry(courtCases).Entity.NextHearingDate = RegModal.NextHearingDate;

            if (RegModal.CaseTypeCode == "6")
            {
                RegModal.ClientClassificationCode = "7";
            }

            if (RegModal.ClientClassificationCode == "1") //BANKS
            {
                db.Entry(courtCases).Entity.AccountContractNo = RegModal.AccountContractNo;
                db.Entry(courtCases).Entity.ClientFileNo = RegModal.ClientFileNo;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;

                if (RegModal.ClientCode == "10") //OMAN DEVELOPMENT BANK
                {
                    db.Entry(courtCases).Entity.ClientCaseType = RegModal.ClientCaseType;
                    db.Entry(courtCases).Entity.ODBBankBranch = RegModal.ODBBankBranch;
                    db.Entry(courtCases).Entity.LoanManager = RegModal.LoanManager;
                }
                else
                {
                    db.Entry(courtCases).Entity.ClientCaseType = "0";
                    db.Entry(courtCases).Entity.ODBBankBranch = "0";
                    db.Entry(courtCases).Entity.LoanManager = "0";
                }
            }
            else if (RegModal.ClientClassificationCode == "2") //FINANCE
            {
                db.Entry(courtCases).Entity.AccountContractNo = RegModal.AccountContractNo;
                db.Entry(courtCases).Entity.ClientFileNo = RegModal.ClientFileNo;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;

                if (RegModal.ClientCode == "170") //UNITED FINANCE COMPANY
                    db.Entry(courtCases).Entity.LoanManager = RegModal.LoanManager;
                else
                    db.Entry(courtCases).Entity.LoanManager = "0";
            }
            else if (RegModal.ClientClassificationCode == "3") //REAL ESTATE
            {
                db.Entry(courtCases).Entity.AccountContractNo = RegModal.AccountContractNo;
                db.Entry(courtCases).Entity.ClientFileNo = RegModal.ClientFileNo;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;
            }
            else if (RegModal.ClientClassificationCode == "4") //INSURANCE
            {
                db.Entry(courtCases).Entity.AccountContractNo = RegModal.AccountContractNo;
                db.Entry(courtCases).Entity.ClientFileNo = RegModal.ClientFileNo;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;
            }
            else if (RegModal.ClientClassificationCode == "5") //COMPANIES
            {
                db.Entry(courtCases).Entity.Subject = RegModal.Subject;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;
            }
            else if (RegModal.ClientClassificationCode == "6") //INDIVIDUALS
            {
                db.Entry(courtCases).Entity.Subject = RegModal.Subject;
                db.Entry(courtCases).Entity.ClaimAmount = RegModal.ClaimAmount;
            }
            else if (RegModal.ClientClassificationCode == "7")
                db.Entry(courtCases).Entity.Subject = RegModal.Subject;

            db.Entry(courtCases).State = EntityState.Modified;
            db.SaveChanges();

            /*
             TO BE REGISTER need to created in Case Registration
            */

            if (RegModal.CaseLevelCode == "1")
            {
                CaseRegistration ModelToSave = new CaseRegistration();
                ModelToSave = db.CaseRegistration.Where(w => w.CaseId == RegModal.CaseId && !w.IsDeleted).OrderByDescending(o => o.CaseRegistrationId).FirstOrDefault();

                if (ModelToSave == null)
                {
                    ModelToSave = new CaseRegistration();
                    ModelToSave.CaseId = courtCases.CaseId;
                    ModelToSave.ActionDate = RegModal.ActionDate;// DateTime.UtcNow.AddHours(4);
                    ModelToSave.AdminFile = RegModal.AdminFile;
                    ModelToSave.UrgentCaseDays = RegModal.UrgentCaseDays;
                    ModelToSave.CourtDetailRegistered = RegModal.CourtDetailRegistered;
                    ModelToSave.OfficeProcedure = RegModal.OfficeProcedure;
                    ModelToSave.Remarks = RegModal.MainRemarks;

                    ModelToSave.FormPrintWorkRequired = RegModal.FormPrintWorkRequired;
                    ModelToSave.FormPrintLastDate = RegModal.FormPrintLastDate;
                    //ModelToSave.FileStatus = RegModal.FileStatus;

                    db.CaseRegistration.Add(ModelToSave);
                    db.SaveChanges();
                    Helper.UpdateOfficeFileStatus(courtCases.CaseId, RegModal.FileStatus);
                }
                else
                {
                    db.Entry(ModelToSave).Entity.ActionDate = RegModal.ActionDate;
                    db.Entry(ModelToSave).Entity.AdminFile = RegModal.AdminFile;
                    db.Entry(ModelToSave).Entity.UrgentCaseDays = RegModal.UrgentCaseDays;
                    db.Entry(ModelToSave).Entity.CourtDetailRegistered = RegModal.CourtDetailRegistered;
                    db.Entry(ModelToSave).Entity.OfficeProcedure = RegModal.OfficeProcedure;
                    db.Entry(ModelToSave).Entity.Remarks = RegModal.MainRemarks;
                    db.Entry(ModelToSave).Entity.FormPrintWorkRequired = RegModal.FormPrintWorkRequired;
                    db.Entry(ModelToSave).Entity.FormPrintLastDate = RegModal.FormPrintLastDate;
                    //db.Entry(ModelToSave).Entity.FileStatus = RegModal.FileStatus;

                    db.Entry(ModelToSave).State = EntityState.Modified;
                    db.SaveChanges();
                    Helper.UpdateOfficeFileStatus(courtCases.CaseId, RegModal.FileStatus);
                }
            }
            else if (courtCases.CaseLevelCode == "2")
                UpdateBeforeCourt("CREATEBEFORECOURT", RegModal);
            else if (int.Parse(courtCases.CaseLevelCode) >= 3 && int.Parse(courtCases.CaseLevelCode) <= 5)
                ProcessCourtDetail(RegModal);
            else if (courtCases.CaseLevelCode == "6")
                ProcessCourtDetail(RegModal, "ENFORCEMENT");
        }
        private void ProcessModifyEnforcement(ToBeRegisterVM RegModal)
        {
            ProcessCourtDetail(RegModal, "ENFORCEMENT", RegModal.PartialViewName);
        }
        private int InitializedCaseRegistration(int CaseId,string ActionLevel,string DisputeLevel)
        {
            CaseRegistration ModelToSave = new CaseRegistration();

            ModelToSave.CaseId = CaseId;
            ModelToSave.ActionDate = DateTime.UtcNow.AddHours(4);
            ModelToSave.ActionLevel = ActionLevel;
            ModelToSave.DisputeLevel = DisputeLevel;
            //ModelToSave.FileStatus = "0";
            Helper.UpdateOfficeFileStatus(CaseId, "0");
            if (DisputeLevel == "0")
                ModelToSave.EnforcementDispute = "0";
            else
            {
                ModelToSave.EnforcementDispute = "1";
                ModelToSave.DisputrRegisterDate = DateTime.UtcNow.AddHours(4);
            }

            db.CaseRegistration.Add(ModelToSave);
            db.SaveChanges();

            return ModelToSave.CaseRegistrationId;
        }
        private void ProcessSessionRollDetail(ToBeRegisterVM modal)
        {
            Helper.ProcessSessionRollDetail(modal);
        }
        private int CreatePayVoucher(ToBeRegisterVM modal)
        {
            return Helper.CreatePaymentVoucher(modal, "ToBeRegisterVM");
        }
        #endregion

        public ActionResult Index(int? id)
        {
            if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
            {
                if (id == -3)
                {
                    ViewBag.LocationId = "A";
                    ViewBag.LocationCheckALL = "checked";
                }
                else
                {
                    ViewBag.LocationId = "M";
                    ViewBag.VoucherApproval = "checked";
                }

                ViewBag.UserRole = "VoucherApproval";
                
            }
            else
            {
                if (id == -3)
                {
                    ViewBag.LocationId = "A";
                    ViewBag.LocationCheckALL = "checked";
                }
                else
                    ViewBag.LocationId = Helper.GetEmployeeLocation(User.Identity.Name).Split('-')[1];
            }

            ViewBag.CreateCaseActive = "";
            ViewBag.GeneralActive = "";
            ViewBag.BeforeCourtActive = "";
            ViewBag.TobeRegisterActive = "";
            ViewBag.PrimaryActive = "";
            ViewBag.AppealActive = "";
            ViewBag.SupremeActive = "";
            ViewBag.EnforcementActive = "";
            ViewBag.CreateBtnText = "CREATE CASE";
            ViewBag.ViewContainer = "#PartialViewContainer";
            ViewBag.IsCourtFollow = "";
            ViewBag.hidIsCourtFollow = "N";
            ViewBag.hidIsClientReply = "N";

            if (id == null)
            {
                if(User.Identity.Name == "3")
                {
                    ViewBag.TobeRegisterActive = "TobeRegisterActive";
                    ViewBag.ViewToLoad = "_TobeRegister";
                }
                else
                {
                    if (User.IsInRole("EnforcementAdmins"))
                    {
                        ViewBag.EnforcementActive = "EnforcementActive";
                        ViewBag.ViewToLoad = "_Enforcement";
                    }
                    else
                    {
                        ViewBag.GeneralActive = "GeneralActive";
                        ViewBag.ViewToLoad = "_General";
                    }
                }
                
            }
            else
            {
                if (id == -1)
                {
                    ViewBag.ViewFollowUpTabs = "AppHidden";
                    ViewBag.StartTAB = "btn_CaseRegNewCase";
                    ViewBag.CaseRegNewCaseActive = "CaseRegNewCaseActive";
                    ViewBag.LoadTable = "tableNewCases";
                }
                else if(id == -2)
                {
                    ViewBag.ViewFollowUpTabs = "AppHidden";
                    ViewBag.IsCourtFollow = "AppHidden";
                    ViewBag.EnforcementActive = "EnforcementActive";
                    ViewBag.ViewToLoad = "_Enforcement";
                    ViewBag.hidIsCourtFollow = "Y";
                }
                else if(id == -3)
                {
                    ViewBag.ViewFollowUpTabs = "AppHidden";
                    ViewBag.IsCourtFollow = "AppHidden";
                    ViewBag.EnforcementActive = "EnforcementActive";
                    ViewBag.ViewToLoad = "_Enforcement";
                    ViewBag.hidIsClientReply = "Y";
                }
                else
                {
                    ViewBag.HFCaseId = id;
                    ViewBag.CreateCaseActive = "CreateCaseActive";
                    ViewBag.ViewToLoad = "_ModifyCase";
                    ViewBag.CreateBtnText = "MODIFY CASE";
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult AjaxIndexData()
        {
            string LocationId = string.Empty;
            string UserLocation = string.Empty;
            var request = HttpContext.Request;
            if (User.IsInRole("CourtCasesViewAll") || User.IsSysAdmin())
                UserLocation = "A";
            else
                UserLocation = Helper.GetEmployeeLocation(User.Identity.Name);

            CourtCaseDTView DtView = new CourtCaseDTView();
            List<CourtCaseListForIndex> data = new List<CourtCaseListForIndex>();

            var start = (Convert.ToInt32(Request["start"]));
            var Length = (Convert.ToInt32(Request["length"])) == 0 ? 10 : (Convert.ToInt32(Request["length"]));
            var searchvalue = Request["search[value]"] ?? "";
            var sortcoloumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            var sortDirection = Request["order[0][dir]"] ?? "asc";
            var recordsTotal = 0;
            string DataFor = string.Empty;
            string ProcedureName = string.Empty;

            string CaseLevel = string.Empty;
            DateTime DateFrom = DateTime.Now;
            DateTime DateTo = DateTime.Now;
            string CallerName = string.Empty;
            string FileStatus = string.Empty;
            int MCTRecords = 0;
            int SLLRecords = 0;

            //UserLocation.ToUpper().Substring(0, 1)

            try
            {
                LocationId = request.Params["LocationId"].ToString();
            }
            catch (Exception ex)
            {
                LocationId = UserLocation.ToUpper().Substring(0, 1);
            }

            try
            {
                CaseLevel = request.Params["CaseLevel"].ToString();
            }
            catch (Exception ex)
            {
                CaseLevel = string.Empty;
            }


            try
            {
                FileStatus = request.Params["FileStatus"].ToString();
            }
            catch (Exception ex)
            {
                FileStatus = string.Empty;
            }


            try
            {
                DataFor = request.Params["DataTableName"].ToString();

                if (DataFor.IndexOf("TBR-") >= 0)
                {
                    List<string> parName = Helper.getDefaultParNames();
                    List<object> parValues = Helper.getDefaultParValues();
                    parName.AddRange(new[] { "@UserName", "@DataFor", "@Location", "@FileStatus" });
                    parValues.AddRange(new[] { User.Identity.Name, DataFor, LocationId, FileStatus });

                    var ds = Helper.getDataSet(parName.ToArray(), parValues.ToArray(), TableNames: new string[] { "data", "summary" });
                    DataTable dt = ds.Tables["data"];
                    DataTable Summarydt = ds.Tables["summary"];

                    var jsondata = dt.ToDictionary();

                    recordsTotal = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["recordsTotal"].ToString()) : 0;
                    MCTRecords = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["MCTRecords"].ToString()) : 0;
                    SLLRecords = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["SLLRecords"].ToString()) : 0;

                    return Json(new { data = jsondata, recordsTotal = recordsTotal, recordsFiltered = recordsTotal, MuscatTotal = MCTRecords, SalalahTotal = SLLRecords }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (DataFor == "CASE_MANAGEMENT")
                    {
                        List<string> parName = Helper.getDefaultParNames();
                        List<object> parValues = Helper.getDefaultParValues();
                        ProcedureName = request.Params["ProcedureName"].ToString();

                        parName.AddRange(new[] { "@UserName", "@DataFor", "@Location", "@CaseLevelCode" });
                        parValues.AddRange(new[] { User.Identity.Name, DataFor, UserLocation, CaseLevel });

                        var ds = Helper.getDataSet(parName.ToArray(), parValues.ToArray(), TableNames: new string[] { "data", "summary" }, procedureName: ProcedureName);
                        DataTable dt = ds.Tables["data"];
                        DataTable Summarydt = ds.Tables["summary"];

                        var jsondata = dt.ToDictionary();

                        recordsTotal = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["recordsTotal"].ToString()) : 0;
                        MCTRecords = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["MCTRecords"].ToString()) : 0;
                        SLLRecords = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["SLLRecords"].ToString()) : 0;

                        return Json(new { data = jsondata, recordsTotal = recordsTotal, recordsFiltered = recordsTotal, MuscatTotal = MCTRecords, SalalahTotal = SLLRecords }, JsonRequestBehavior.AllowGet);

                    }

                }

                if (DataFor == "ENF-CONTRESULT" || DataFor == "ENF-RECFRCOURT" || DataFor == "ENF-SUSPEND")
                {
                    DateFrom = request.Params["DateFrom"].ToString() == "" ? DateTime.Now.AddYears(-100) : DateTime.ParseExact(request.Params["DateFrom"].ToString(), "dd/MM/yyyy", null);
                    DateTo = request.Params["DateTo"].ToString() == "" ? DateTime.Now.AddYears(100) : DateTime.ParseExact(request.Params["DateTo"].ToString(), "dd/MM/yyyy", null);
                    CallerName = request.Params["CallerName"].ToString();
                }

                DtView = Helper.GetCaseList(sortcoloumnIndex, start, searchvalue, Length, sortDirection, LocationId, DataFor, CaseLevel, DateFrom, DateTo, CallerName);

                recordsTotal = data.Count > 0 ? data[0].TotalRecords : 0;
                return Json(new { data = DtView.data, recordsTotal = DtView.recordsTotal, recordsFiltered = DtView.recordsFiltered, MuscatTotal = DtView.MCTRecords, SalalahTotal = DtView.SLLRecords }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
            }
            return Json(new { data = data, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Create()
        {
            if (User.IsInRole("AllowCloseCase") || User.IsSysAdmin())
                ViewBag.AllowCloseCase = "Y";
            else
                ViewBag.AllowCloseCase = "N";

            if (User.IsInRole("AllowAddClient") || User.IsSysAdmin())
                ViewBag.AllowAddClient = "Y";
            else
                ViewBag.AllowAddClient = "N";

            ViewBag.MstParentId = (int)MASTER_S.Client;

            ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc");
            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc");
            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc");
            ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client), "Mst_Value", "Mst_Desc");
            ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc");
            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ToBeRegisterVM RegModal)
        {
            try
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);

                if (ModelState.IsValid)
                {
                    CourtCases courtCases = new CourtCases();
                    var emp = db.Employees.Where(w => w.EmployeeNumber == User.Identity.Name).FirstOrDefault();
                    courtCases.OfficeFileNo = GenerateCaseNumber(emp.LocationCode);
                    courtCases.ClientClassificationCode = RegModal.ClientClassificationCode;
                    courtCases.ReceptionDate = RegModal.ReceptionDate;
                    courtCases.ReceiveLevelCode = RegModal.ReceiveLevelCode;
                    courtCases.CaseTypeCode = RegModal.CaseTypeCode;
                    courtCases.AgainstCode = RegModal.AgainstCode;
                    courtCases.ClientCode = RegModal.ClientCode;
                    courtCases.Defendant = RegModal.Defendant;
                    courtCases.CaseLevelCode = RegModal.CaseLevelCode;

                    courtCases.CurrentHearingDate = RegModal.CurrentHearingDate;
                    courtCases.CourtDecision = RegModal.CourtDecision;
                    courtCases.SessionRemarks = RegModal.SessionRemarks;
                    courtCases.Requirements = RegModal.Requirements;
                    courtCases.SessionClientId = RegModal.SessionClientId;
                    courtCases.SessionRollDefendentName = RegModal.SessionRollDefendentName;
                    courtCases.OmaniExp = RegModal.OmaniExp;
                    courtCases.ClientReply = RegModal.ClientReply;
                    courtCases.TransportationSource = RegModal.TransportationSource;

                    if (RegModal.ClientClassificationCode == "1")
                    {
                        courtCases.ClientCaseType = RegModal.ClientCaseType;
                        courtCases.AccountContractNo = RegModal.AccountContractNo;
                        courtCases.ClientFileNo = RegModal.ClientFileNo;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                        courtCases.ODBBankBranch = RegModal.ODBBankBranch;
                        courtCases.LoanManager = RegModal.LoanManager;

                    }
                    else if (RegModal.ClientClassificationCode == "2")
                    {
                        courtCases.AccountContractNo = RegModal.AccountContractNo;
                        courtCases.ClientFileNo = RegModal.ClientFileNo;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                        courtCases.LoanManager = RegModal.LoanManager;
                    }
                    else if (RegModal.ClientClassificationCode == "3")
                    {
                        courtCases.AccountContractNo = RegModal.AccountContractNo;
                        courtCases.ClientFileNo = RegModal.ClientFileNo;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                    }
                    else if (RegModal.ClientClassificationCode == "4")
                    {
                        courtCases.AccountContractNo = RegModal.AccountContractNo;
                        courtCases.ClientFileNo = RegModal.ClientFileNo;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                    }
                    else if (RegModal.ClientClassificationCode == "5")
                    {
                        courtCases.Subject = RegModal.Subject;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                    }
                    else if (RegModal.ClientClassificationCode == "6")
                    {
                        courtCases.Subject = RegModal.Subject;
                        courtCases.ClaimAmount = RegModal.ClaimAmount;
                    }
                    else if (RegModal.ClientClassificationCode == "7")
                    {
                        courtCases.Subject = RegModal.Subject;
                    }


                    db.CourtCase.Add(courtCases);
                    db.SaveChanges();

                    RegModal.CaseId = courtCases.CaseId;

                    /*
                      TO BE REGISTER need to created in Case Registration
                     */
                    if (courtCases.CaseLevelCode == "1")
                    {
                        CaseRegistration ModelToSave = new CaseRegistration();
                        ModelToSave.CaseId = courtCases.CaseId;
                        ModelToSave.ActionDate = RegModal.ActionDate;// DateTime.UtcNow.AddHours(4);
                        ModelToSave.AdminFile = RegModal.AdminFile;
                        ModelToSave.UrgentCaseDays = RegModal.UrgentCaseDays;
                        ModelToSave.CourtDetailRegistered = RegModal.CourtDetailRegistered;
                        ModelToSave.OfficeProcedure = RegModal.OfficeProcedure;
                        ModelToSave.Remarks = RegModal.MainRemarks;
                        ModelToSave.FormPrintWorkRequired = RegModal.FormPrintWorkRequired;
                        ModelToSave.FormPrintLastDate = RegModal.FormPrintLastDate;

                        //ModelToSave.FileStatus = RegModal.FileStatus ?? "0";
                        db.CaseRegistration.Add(ModelToSave);
                        db.SaveChanges();

                        Helper.UpdateOfficeFileStatus(courtCases.CaseId, RegModal.FileStatus ?? "0");
                    }
                    else if (courtCases.CaseLevelCode == "2")
                        UpdateBeforeCourt("CREATEBEFORECOURT", RegModal);
                    else if (int.Parse(courtCases.CaseLevelCode) >= 3 && int.Parse(courtCases.CaseLevelCode) <= 5)
                        ProcessCourtDetail(RegModal);
                    else if (courtCases.CaseLevelCode == "6")
                        ProcessCourtDetail(RegModal, "ENFORCEMENT");


                    Session["Message"] = new MessageVM
                    {
                        Category = "Success",
                        Message = "Data Save Successfully"
                    };

                    return RedirectToAction("Index", "CourtCases", new RouteValueDictionary(new { id = courtCases.CaseId }));
                }
                else
                {
                    ViewBag.MstParentId = (int)MASTER_S.Client;

                    ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc");
                    ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc");
                    ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                    ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc");
                    ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client), "Mst_Value", "Mst_Desc");
                    ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc");
                    ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc");
                    ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc");
                    ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc");

                    return Json(new { Category = "Error", Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray()) });
                }
            }
            catch (Exception e)
            {
                return Json(new { Category = "Error", Message = e.Message });
            }
        }

        public ActionResult FileClosure(int? CaseId)
        {
            if (CaseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();
            if (courtCases == null)
            {
                return HttpNotFound();
            }

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            var CaseLevelName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseLevel && w.Mst_Value == courtCases.CaseLevelCode).FirstOrDefault().Mst_Desc;

            FileClosureEnteryVM ViewModal = GetFileClosureVM((int)CaseId);
            string[] StatusCodes = new[] { "1", "2" };

            ViewBag.StatusCode = new SelectList(Helper.GetStatusCodeList(false, StatusCodes), "Mst_Value", "Mst_Desc", ViewModal.StatusCode);
            ViewBag.FileAllocation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileAllocation), "Mst_Value", "Mst_Desc", ViewModal.FileAllocation);
            ViewBag.ReasonCode = new SelectList(Helper.GetCaseClosingReasons(), "Mst_Value", "Mst_Desc", ViewModal.ReasonCode);
            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", ViewModal.ReceiveLevelCode);
            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.CaseTypeCode);
            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
            ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.ClientCaseType);
            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);
            ViewBag.ODBBankBranch = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ODBBankBranch), "Mst_Value", "Mst_Desc", ViewModal.ODBBankBranch);
            ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc", ViewModal.LoanManager);
            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
            ViewBag.ReOpenEnforcement = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ReOpenEnforcement);
            ViewBag.FinalOnvoiceOnHold = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.FinalOnvoiceOnHold);
            ViewBag.JudRecRedStamp = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.JudRecRedStamp);


            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;
            ViewBag.ClosureLevel = CaseLevelName;

            return View(ViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FileClosure(FileClosureEnteryVM modal)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

            if (ModelState.IsValid)
            {
                CourtCaseStatusDetail StatusDetail = new CourtCaseStatusDetail();
                StatusDetail.CaseId = modal.CaseId;
                StatusDetail.StatusCode = modal.StatusCode;
                StatusDetail.StatusDate = modal.StatusDate;
                StatusDetail.ReasonCode = modal.ReasonCode;
                StatusDetail.CurrentCaseLevel = modal.CurrentCaseLevel;
                StatusDetail.ClosureLevel = modal.ClosureLevel;
                StatusDetail.FileAllocation = modal.FileAllocation;
                StatusDetail.LitigationFileClosureDate = modal.LitigationFileClosureDate;
                StatusDetail.TempMonth = modal.TempMonth;
                StatusDetail.CreatedBy = User.Identity.GetUserId();
                StatusDetail.CreatedOn = DateTime.UtcNow.AddHours(4);

                db.CourtCaseStatusDetail.Add(StatusDetail);
                db.SaveChanges();



                if(modal.btnSave == "btnFileClosureSave")
                {
                    db.Entry(courtCases).Entity.ClosureDate = modal.StatusDate;
                    db.Entry(courtCases).Entity.ClosedbyStaff = User.Identity.Name;
                    db.Entry(courtCases).Entity.CaseStatus = modal.StatusCode;
                    db.Entry(courtCases).Entity.ClosingNotes = modal.ClosingNotes;
                    db.Entry(courtCases).Entity.FinalOnvoiceOnHold = modal.FinalOnvoiceOnHold;
                    db.Entry(courtCases).Entity.SuggestedDate = modal.SuggestedDate;
                    db.Entry(courtCases).Entity.JudRecRedStamp = modal.JudRecRedStamp;

                    db.Entry(courtCases).Entity.ReasonCode = modal.ReasonCode;
                    db.Entry(courtCases).Entity.FileAllocation = modal.FileAllocation;
                    db.Entry(courtCases).Entity.LitigationFileClosureDate = modal.LitigationFileClosureDate;

                }
                else if(modal.btnSave == "btnLitigationSave")
                {
                    db.Entry(courtCases).Entity.LitigationFileClosureDate = modal.LitigationFileClosureDate;

                }
                else if(modal.btnSave == "btnFinanceClosureSave")
                {
                    db.Entry(courtCases).Entity.FinanceFileClosureDate = modal.FinanceFileClosureDate;
                }

                db.Entry(courtCases).Entity.ReceiveLevelCode = modal.ReceiveLevelCode;
                db.Entry(courtCases).Entity.CaseTypeCode = modal.CaseTypeCode;
                db.Entry(courtCases).Entity.AgainstCode = modal.AgainstCode;
                db.Entry(courtCases).Entity.ClaimAmount = modal.ClaimAmount;
                db.Entry(courtCases).Entity.OmaniExp = modal.OmaniExp;
                db.Entry(courtCases).Entity.ClientCaseType = modal.ClientCaseType;
                db.Entry(courtCases).Entity.AccountContractNo = modal.AccountContractNo;
                db.Entry(courtCases).Entity.ClientFileNo = modal.ClientFileNo;
                db.Entry(courtCases).Entity.ODBBankBranch = modal.ODBBankBranch;
                db.Entry(courtCases).Entity.LoanManager = modal.LoanManager;
                db.Entry(courtCases).Entity.GovernorateId = modal.GovernorateId;
                db.Entry(courtCases).Entity.ReOpenEnforcement = modal.ReOpenEnforcement;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                if(courtCases.CaseStatus == "2")
                {
                    Helper.ProcessCaseRegistrationProgress(courtCases, "FileClosureEnteryVM");
                }

                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };

                return RedirectToAction("FileClosure", new RouteValueDictionary(new { courtCases.CaseId }));

                //return RedirectToAction("Index");
            }

            if (courtCases == null)
            {
                return HttpNotFound();
            }

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            var CaseLevelName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseLevel && w.Mst_Value == courtCases.CaseLevelCode).FirstOrDefault().Mst_Desc;
            string[] StatusCodes = new[] { "1", "2" };
            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;
            ViewBag.ClosureLevel = CaseLevelName;

            ViewBag.StatusCode = new SelectList(Helper.GetStatusCodeList(false, StatusCodes), "Mst_Value", "Mst_Desc", modal.StatusCode);
            ViewBag.ReasonCode = new SelectList(Helper.GetCaseClosingReasons(), "Mst_Value", "Mst_Desc", modal.ReasonCode);
            ViewBag.FileAllocation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileAllocation), "Mst_Value", "Mst_Desc", modal.FileAllocation);

            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", modal.ReceiveLevelCode);
            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", modal.CaseTypeCode);
            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", modal.AgainstCode);
            ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", modal.ClientCaseType);
            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", modal.OmaniExp);
            ViewBag.ODBBankBranch = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ODBBankBranch), "Mst_Value", "Mst_Desc", modal.ODBBankBranch);
            ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc", modal.LoanManager);
            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", modal.GovernorateId);
            ViewBag.ReOpenEnforcement = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", modal.ReOpenEnforcement);
            ViewBag.FinalOnvoiceOnHold = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.FinalOnvoiceOnHold);
            ViewBag.JudRecRedStamp = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.JudRecRedStamp);

            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return View(modal);
        }

        public ActionResult Finance(int? CaseId)
        {
            if (CaseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();
            if (courtCases == null)
            {
                return HttpNotFound();
            }

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

            FinanceVM ViewModal = new FinanceVM();

            if (courtCases != null)
            {
                ViewModal.CaseId = courtCases.CaseId;
                ViewModal.ClaimAmount = courtCases.ClaimAmount;
                ViewModal.SpecialInstructions = courtCases.SpecialInstructions;
                ViewModal.ClosingNotes = courtCases.ClosingNotes;

                var Invoices = db.CaseInvoice.Where(w => w.CaseId == CaseId && w.IsLastInvoice == true).FirstOrDefault();

                if (Invoices != null)
                {
                    ViewModal.DateOfLastInvoice = Invoices.InvoiceDate;
                }
            }

            ViewBag.SupportingDoc = GetCaseAgreementDoc((int)CaseId);

            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;
            ViewBag.TransportationFee = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", courtCases.TransportationFee);


            return View(ViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Finance(FinanceVM modal, HttpPostedFileBase upload)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            string UploadRoot = Helper.GetStorageRoot;
            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();

            if (ModelState.IsValid)
            {

                db.Entry(courtCases).Entity.SpecialInstructions = modal.SpecialInstructions;
                db.Entry(courtCases).Entity.ClosingNotes = modal.ClosingNotes;
                db.Entry(courtCases).Entity.TransportationFee = modal.TransportationFee;

                db.Entry(courtCases).State = EntityState.Modified;
                db.SaveChanges();

                if (upload != null && upload.ContentLength > 0)
                {
                    string FileExtension = Path.GetExtension(upload.FileName);

                    string FileName = courtCases.CaseId + FileExtension;

                    string UploadPath = Path.Combine(UploadRoot, "CaseAgreement", FileName);

                    upload.SaveAs(UploadPath);
                }

                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };

                return RedirectToAction("Finance", new RouteValueDictionary(new { courtCases.CaseId }));

                //return RedirectToAction("Index");
            }

            if (courtCases == null)
            {
                return HttpNotFound();
            }

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;

            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;
            ViewBag.TransportationFee = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", modal.TransportationFee);


            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return View(modal);
        }

        public ActionResult BeforeCourt(int? CaseId)
        {
            if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
                ViewBag.UserRole = "VoucherApproval";
            else
                ViewBag.UserRole = "";

            if (CaseId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();
            BeforeCourtVM ViewModal = new BeforeCourtVM();

            if (courtCases == null)
                return HttpNotFound();
            else
            {
                ViewModal.CaseId = courtCases.CaseId;
                ViewModal.PoliceNo = courtCases.PoliceNo;
                ViewModal.PublicProsecutionNo = courtCases.PublicProsecutionNo;
                ViewModal.PAPCNo = courtCases.PAPCNo;
                ViewModal.LaborConflicNo = courtCases.LaborConflicNo;
                ViewModal.ReconciliationNo = courtCases.ReconciliationNo;
                ViewBag.PoliceStation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", courtCases.PoliceStation);
                ViewBag.PublicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", courtCases.PublicPlace);
                ViewBag.PAPCPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", courtCases.PAPCPlace);
                ViewBag.LaborConflicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", courtCases.LaborConflicPlace);
                ViewBag.ReconciliationDep = new SelectList(Helper.GetReconciliationDeptt(), "Mst_Value", "Mst_Desc", courtCases.ReconciliationDep);

                var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
                ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
                ViewBag.ClientName = ClientName;
                ViewBag.Defendant = courtCases.Defendant;
                ViewBag.hid_CaseId = CaseId;

                int SessionRollId = Helper.GetSessionRollId(courtCases.CaseId, "2");
                var SessionRolls = db.SessionsRoll.Find(SessionRollId);
                ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel().OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                ViewModal.SessionClientId = courtCases.SessionClientId;
                ViewModal.SessionRollDefendentName = courtCases.SessionRollDefendentName;
                ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", courtCases.SessionClientId);


                if (SessionRolls != null)
                {
                    ViewBag.hid_SessionRollId = SessionRolls.SessionRollId;
                    ViewModal.ShowFollowup = SessionRolls.ShowFollowup;
                    ViewModal.ShowSuspend = SessionRolls.ShowSuspend;
                    ViewModal.LastDate = SessionRolls.LastDate;
                    ViewModal.WorkRequired = SessionRolls.WorkRequired;
                    ViewModal.SessionNotes = SessionRolls.SessionNotes;
                    ViewModal.SuspendedWorkRequired = SessionRolls.WorkRequired;
                    ViewModal.SuspendedSessionNotes = SessionRolls.SessionNotes;
                    ViewModal.FollowerId = SessionRolls.FollowerId;

                }
                else
                {
                    ViewBag.hid_SessionRollId = 0;
                    ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc");

                }
            }

            return View(ViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BeforeCourt(BeforeCourtVM modal)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            ViewBag.hid_CaseId = modal.CaseId;
            if (ModelState.IsValid)
            {
                UpdateBeforeCourt("BEFORECOURT", modal);


                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };

                if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
                    ViewBag.UserRole = "VoucherApproval";
                else
                    ViewBag.UserRole = "";

                return RedirectToAction("BeforeCourt", new RouteValueDictionary(new { modal.CaseId }));


                //return RedirectToAction("Index");
            }

            ViewBag.PoliceStation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", modal.PoliceStation);
            ViewBag.PublicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", modal.PublicPlace);
            ViewBag.PAPCPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", modal.PAPCPlace);
            ViewBag.LaborConflicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", modal.LaborConflicPlace);

            ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel().OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc");

            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == modal.CaseId).FirstOrDefault();
            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            ViewBag.OfficeFileNo = courtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = courtCases.Defendant;

            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return View(modal);
        }
        public ActionResult TobeReg(int? CaseId)
        {
            if (User.IsInRole("AllowCloseCase") || User.IsSysAdmin())
                ViewBag.AllowCloseCase = "Y";
            else
                ViewBag.AllowCloseCase = "N";

            if (User.IsInRole("AllowAddClient") || User.IsSysAdmin())
                ViewBag.AllowAddClient = "Y";
            else
                ViewBag.AllowAddClient = "N";

            if (CaseId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();
            if (courtCases == null)
            {
                return HttpNotFound();
            }

            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCases.ClientCode).FirstOrDefault().Mst_Desc;
            var ClientClassName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.ClientClassification && w.Mst_Value == courtCases.ClientClassificationCode).FirstOrDefault().Mst_Desc;

            ToBeRegisterVM ViewModal = new ToBeRegisterVM();

            ViewModal.CaseId = courtCases.CaseId;
            ViewModal.OfficeFileNo = courtCases.OfficeFileNo;
            ViewModal.ClientClassificationCode = courtCases.ClientClassificationCode;
            ViewModal.ReceptionDate = courtCases.ReceptionDate;
            ViewModal.ReceiveLevelCode = courtCases.ReceiveLevelCode;
            ViewModal.CaseTypeCode = courtCases.CaseTypeCode;
            ViewModal.AgainstCode = courtCases.AgainstCode;
            ViewModal.ClientCode = courtCases.ClientCode;
            ViewModal.Defendant = courtCases.Defendant;
            ViewModal.CaseLevelCode = courtCases.CaseLevelCode;


            ViewBag.ClientName = ClientName;
            ViewBag.ClientClassName = ClientClassName;

            ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc", ViewModal.ClientClassificationCode);
            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", ViewModal.ReceiveLevelCode);
            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.CaseTypeCode);
            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
            ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client && m.Remarks == ViewModal.ClientClassificationCode), "Mst_Value", "Mst_Desc", ViewModal.ClientCode);
            ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc", ViewModal.CaseLevelCode);
            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);

            return View(ViewModal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TobeReg(ToBeRegisterVM RegModal, HttpPostedFileBase upload, HttpPostedFileBase uploadAddress, HttpPostedFileBase uploadPVSupDocs)
        {
            try
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                string UploadRoot = Helper.GetStorageRoot;
                if (ModelState.IsValid)
                {
                    ToBeRegisterVM ViewModal = new ToBeRegisterVM();

                    switch (RegModal.SavePV_Data)
                    {
                        case "_ModifyCase":
                            ProcessModifyCase(RegModal);
                            ViewModal = GetFilledPartailView(RegModal.CaseId);

                            if (User.IsInRole("AllowCloseCase") || User.IsSysAdmin())
                                ViewBag.AllowCloseCase = "Y";
                            else
                                ViewBag.AllowCloseCase = "N";

                            if (User.IsInRole("AllowAddClient") || User.IsSysAdmin())
                                ViewBag.AllowAddClient = "Y";
                            else
                                ViewBag.AllowAddClient = "N";

                            ViewModal.PartialViewName = RegModal.PartialViewName;
                            
                            break;
                        case "MoneyTransfer":
                            DefendentTransferDTO objDTO = new DefendentTransferDTO();
                            objDTO.Userid = HttpContext.User.Identity.GetUserId();
                            objDTO.DataFor = RegModal.DataFor;
                            objDTO.DefendentTransferId = RegModal.DefendentTransferId;
                            objDTO.CaseId = RegModal.CaseId;
                            objDTO.CaseLevelCode = "6";
                            objDTO.TransferDate = RegModal.TransferDate;
                            objDTO.Amount = RegModal.Amount ?? 0;
                            objDTO.MoneyTrRequestDate = RegModal.MoneyTrRequestDate;
                            objDTO.MoneyTrCompleteDate = RegModal.MoneyTrCompleteDate;

                            DataTable _result = Helper.ProcessDefendentTransfer(objDTO);

                            string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                            string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();
                            ViewModal = GetFilledPartailView(RegModal.CaseId);

                            ViewModal.DefendentTransferId = Convert.ToInt32(_result.Rows[0]["DefendentTransferId"].ToString());
                            ViewModal.PartialViewName = "ENFInfoStage";

                            int DefendentTransferId = ViewModal.DefendentTransferId;

                            var defendentTransfer = Helper.ProcessDefendentTransfer(DefendentTransferId);

                            if (defendentTransfer != null)
                            {
                                ViewModal.DefendentTransferId = defendentTransfer.DefendentTransferId;
                                ViewModal.MoneyTrRequestDate = defendentTransfer.MoneyTrRequestDate;
                                ViewModal.TransferDate = defendentTransfer.TransferDate;
                                ViewModal.Amount = defendentTransfer.Amount;
                                ViewModal.MoneyTrCompleteDate = defendentTransfer.MoneyTrCompleteDate;
                            }

                            
                            break;
                        case "ENFInfoStage":
                            Helper.UpdateBoxModified(RegModal);
                            ProcessModifyEnforcement(RegModal);

                            if (upload != null && upload.ContentLength > 0)
                            {
                                string FileExtension = Path.GetExtension(upload.FileName);

                                string FileName = RegModal.CaseId + FileExtension;

                                string UploadPath = Path.Combine(UploadRoot, "DEF_Lawyer_Docs", FileName);

                                upload.SaveAs(UploadPath);
                            }

                            if (uploadAddress != null && uploadAddress.ContentLength > 0)
                            {
                                string FileExtension = Path.GetExtension(uploadAddress.FileName);

                                string FileName = RegModal.CaseId + FileExtension;

                                string UploadPath = Path.Combine(UploadRoot, "DEF_Address_Docs", FileName);

                                uploadAddress.SaveAs(UploadPath);
                            }
                            ViewModal = GetFilledPartailView(RegModal.CaseId);
                            ViewModal.PartialViewName = "ENFInfoStage";
                            break;
                        case "ENFAddressDetail":
                        case "ENFDetail":
                            ProcessModifyEnforcement(RegModal);
                            if (upload != null && upload.ContentLength > 0)
                            {
                                string FileExtension = Path.GetExtension(upload.FileName);

                                string FileName = RegModal.CaseId + FileExtension;

                                string UploadPath = Path.Combine(UploadRoot, "DEF_Lawyer_Docs", FileName);

                                upload.SaveAs(UploadPath);
                            }

                            if (uploadAddress != null && uploadAddress.ContentLength > 0)
                            {
                                string FileExtension = Path.GetExtension(uploadAddress.FileName);

                                string FileName = RegModal.CaseId + FileExtension;

                                string UploadPath = Path.Combine(UploadRoot, "DEF_Address_Docs", FileName);

                                uploadAddress.SaveAs(UploadPath);
                            }
                            ViewModal = GetFilledPartailView(RegModal.CaseId);
                            ViewModal.PartialViewName = "ENFInfoStage";
                            break;
                        case "PVCreate":
                            int Voucher_No = CreatePayVoucher(RegModal);
                            RegModal.PVDetail.Voucher_No = Voucher_No;

                            if (uploadPVSupDocs != null && uploadPVSupDocs.ContentLength > 0)
                            {
                                string FileExtension = Path.GetExtension(uploadPVSupDocs.FileName);

                                string FileName = RegModal.PVDetail.Voucher_No + FileExtension;

                                string UploadPath = Path.Combine(UploadRoot, "PVDocuments", FileName);

                                uploadPVSupDocs.SaveAs(UploadPath);
                            }

                            ViewModal = GetFilledPartailView(RegModal.CaseId);
                            ViewModal.PartialViewName = "ENFInfoStage";
                            break;
                    }


                    string LawyerDoc = Helper.GetDEF_Lawyer_Doc(RegModal.CaseId);
                    string AddressDoc = Helper.GetDEF_Address_Doc(RegModal.CaseId);

                    if (LawyerDoc == "#")
                    {
                        ViewBag.ViewDEF_LawyerDocs = "AppHidden";
                    }
                    else
                    {
                        ViewBag.ViewDEF_LawyerDocs = "";
                        ViewBag.DEF_Lawyer_Docs = LawyerDoc;
                    }

                    if (AddressDoc == "#")
                    {
                        ViewBag.ViewDEF_AddressDocs = "AppHidden";
                    }
                    else
                    {
                        ViewBag.ViewDEF_AddressDocs = "";
                        ViewBag.DEF_Address_Docs = AddressDoc;
                    }

                    #region Pay Voucher
                    List<MasterSetups> Payment_Head_List = new List<MasterSetups>();
                    MasterSetups PleaseSelect = new MasterSetups();
                    PleaseSelect.Mst_Value = "0";
                    PleaseSelect.Mst_Desc = "PLEASE SELECT";
                    Payment_Head_List.Add(PleaseSelect);
                    Payment_Head_List.AddRange(Helper.LoadPayFor("R"));


                    ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc");
                    //ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                    //ViewBag.Credit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                    ViewBag.Payment_Head = new SelectList(Payment_Head_List, "Mst_Value", "Mst_Desc");
                    ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");

                    #endregion

                    ViewBag.MstParentId = (int)MASTER_S.SessionClients;

                    ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc", ViewModal.ClientClassificationCode);
                    ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client), "Mst_Value", "Mst_Desc", ViewModal.ClientCode);
                    ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc", ViewModal.CaseLevelCode);
                    ViewBag.hid_DetailId = ViewModal.DetailId;


                    ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", ViewModal.SessionClientId);
                    ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                    ViewBag.EnforcementAdmin = new SelectList(Helper.GetEnfcAdmin(), "Mst_Value", "Mst_Desc", ViewModal.EnforcementAdmin);
                    ViewBag.EnforcementlevelId = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterENF), "Mst_Value", "Mst_Desc", ViewModal.EnforcementlevelId);
                    ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("4"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);
                    ViewBag.AgainstInsurance = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.AgainstInsurance);
                    ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", ViewModal.ReceiveLevelCode);
                    ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.CaseTypeCode);
                    ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
                    ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);
                    ViewBag.ReOpenEnforcement = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ReOpenEnforcement);
                    ViewBag.DEF_CallerName = new SelectList(Helper.GetCallerNames(), "Mst_Value", "Mst_Desc", ViewModal.DEF_CallerName);
                    ViewBag.AnnouncementTypeId = new SelectList(Helper.GetAnnouncementType(), "Mst_Value", "Mst_Desc", ViewModal.AnnouncementTypeId);

                    ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                    ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                    ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);

                    if (ViewModal.UpdatePV_Type == "ENF_UPDATE_SESSION")
                    {
                        ViewBag.CaseType = new SelectList(Helper.GetSessionCaseType(), "Mst_Value", "Mst_Desc", ViewModal.CaseType);
                        ViewBag.Session_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc", ViewModal.Session_LawyerId);

                        ViewBag.FollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", ViewModal.FollowerId);
                        ViewBag.SuspendedFollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", ViewModal.SuspendedFollowerId);

                        ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                        ViewBag.CourtFollow_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow_LawyerId);

                    }

                    ViewBag.UpdatedOn = ViewModal.UpdatedOn;

                    return PartialView(ViewModal.PartialViewName, ViewModal);
                }
                else
                    return Json(new { Category = "Error", Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray()) });
            }
            catch (Exception e)
            {
                return Json(new { Category = "Error", Message = e.Message });
            }
        }

        [HttpGet]
        public ActionResult GetTab(string ClassificationId, string Mode, string DefendentTransfer = null, string CaseRegistrationId = null, string ActionLevel = null, string DisputeLevel = null, string CaseLevel = null)
        {
            string PartialViewName = string.Empty;
            int caseRegistrationId = 0;

            if (!string.IsNullOrEmpty(CaseRegistrationId))
            {
                int caseid = Convert.ToInt32(Mode);
                caseRegistrationId = Convert.ToInt32(CaseRegistrationId);
                CaseRegistrationVM ViewModal = new CaseRegistrationVM();
                PartialViewName = ClassificationId;

                ViewModal.PartialViewName = PartialViewName;
                ViewModal.CaseLevelCode = CaseLevel ?? null;

                Helper.GetCaseRegistrationVM(caseRegistrationId, ref ViewModal);
                //ViewModal = GetCaseRegistrationVM(caseRegistrationId, ref ViewModal);
                if (ViewModal == null)
                {
                    return HttpNotFound();
                }
                

                if (PartialViewName.IndexOf("TBR-") >= 0)
                {
                    if (PartialViewName == "TBR-TRANSFER")
                    {
                        ViewBag.ConsultantId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", ViewModal.ConsultantId);
                    }
                    else if (PartialViewName == "TBR-LEGAL_NOTICE")
                    {
                        string OmanPostDoc = Helper.GetOmanPostDoc(ViewModal.CaseRegistrationId);

                        if (OmanPostDoc == "#")
                        {
                            ViewBag.View_OmanPostDoc = "AppHidden";
                        }
                        else
                        {
                            ViewBag.View_OmanPostDoc = "";
                            ViewBag.OmanPostDoc = OmanPostDoc;
                        }
                    }
                    else if (PartialViewName == "TBR-ON_HOLD")
                    {
                        ViewBag.OnHoldReasonDDL = new SelectList(Helper.GetOnHoldReason(), "Mst_Value", "Mst_Desc", ViewModal.FileStatus == "4" ? ViewModal.FileStatusRemarks : "0");
                        ViewBag.OnHoldDone = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.OnHoldDone);

                    }
                    else if (PartialViewName == "TBR-ONLINE_SUBMISSION")
                    {
                        ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                        ViewBag.LawyerId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);
                        ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                        ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(ViewModal.ActionLevel), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                        ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
                        ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);
                        ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", ViewModal.DepartmentType);
                        ViewBag.RealEstateYesNo = new SelectList(Helper.GetYNForSelect(), "Mst_Value", "Mst_Desc", ViewModal.RealEstateYesNo);
                        ViewBag.StopEnfRequest = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.StopEnfRequest);
                    }
                    else if (PartialViewName == "TBR-COURT_NOTES")
                    {
                        ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                        ViewBag.LawyerId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);
                    }
                    else if (PartialViewName == "TBR-PAYMENT")
                    {
                        ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                        ViewBag.LawyerId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);

                        if (ViewModal.Voucher_No > 0)
                        {
                            ViewBag.VoucherDoc = Helper.GetVoucherDoc((int)ViewModal.Voucher_No);
                            ViewBag.PVTransferDoc = Helper.GetPVTranferDoc((int)ViewModal.Voucher_No);

                            ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc", ViewModal.CourtType);
                            ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc", ViewModal.Debit_Account);
                            ViewBag.Payment_Head = new SelectList(Helper.LoadPayFor("R"), "Mst_Value", "Mst_Desc", ViewModal.Payment_Head);
                            ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc", ViewModal.Payment_To);

                        }
                        else
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
                    }
                    else if (PartialViewName == "TBR-WITH_LAWYER")
                    {
                        ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                        ViewBag.LawyerId = new SelectList(Helper.GetCommonNameList(), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);

                    }
                    else if (PartialViewName == "TBR-REGISTERED")
                    {
                        ViewBag.Courtid = ViewModal.ActionLevel;

                        if (int.Parse(ViewBag.Courtid) <= 3 && int.Parse(ViewBag.Courtid) >= 1)
                            ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("TBR"), "Mst_Value", "Mst_Desc");
                        else
                        {
                            ViewBag.DisputeLevelName = ViewModal.DisputeLevelName;
                            ViewBag.DisputeTypeName = ViewModal.DisputeTypeName;

                            ViewBag.DisputeLevelId = ViewModal.DisputeLevel;
                            ViewBag.DisputeTypeId = ViewModal.DisputeType;

                            if (ViewBag.DisputeTypeId == "1") //OBJ
                            {
                                if (ViewBag.DisputeLevelId == "1") //PRI
                                {
                                    ViewBag.PrimaryObjectionCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ViewModal.PrimaryObjectionCourt);

                                }
                                else if (ViewBag.DisputeLevelId == "2") //APEAL
                                {
                                    ViewBag.ApealObjectionCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc", ViewModal.ApealObjectionCourt);

                                }
                                else if (ViewBag.DisputeLevelId == "3") //SUP
                                {
                                    ViewBag.SupremeObjectionCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc", ViewModal.SupremeObjectionCourt);
                                    ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", ViewModal.CourtDepartment);

                                }
                            }

                            else if (ViewBag.DisputeTypeId == "2") //PLAINT تظلم
                            {
                                if (ViewBag.DisputeLevelId == "1") //PRI
                                {
                                    ViewBag.PrimaryPlaintCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ViewModal.PrimaryPlaintCourt);

                                }
                                else if (ViewBag.DisputeLevelId == "2") //APEAL
                                {
                                    ViewBag.ApealPlaintCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc", ViewModal.ApealPlaintCourt);

                                }
                                else if (ViewBag.DisputeLevelId == "3") //SUP
                                {
                                    ViewBag.SupremePlaintCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc", ViewModal.SupremePlaintCourt);
                                    ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", ViewModal.CourtDepartment);

                                }
                            }
                            ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", ViewModal.CourtDepartment);
                        }

                    }
                    else if (PartialViewName == "TBR-REGISTERED_PRI" || PartialViewName == "TBR-REGISTERED_APL" || PartialViewName == "TBR-REGISTERED_SUP")
                    {
                        ViewBag.Courtid = ViewModal.ActionLevel;
                        if (int.Parse(ViewBag.Courtid) <= 3 && int.Parse(ViewBag.Courtid) >= 1)
                        {
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(ViewModal.ActionLevel), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                            ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", ViewModal.DepartmentType);
                            ViewBag.StopEnfRequest = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.StopEnfRequest);
                            ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", ViewModal.CourtDepartment);
                            ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);

                        }
                    }
                    else if (PartialViewName == "TBR-STOP_REGISTRATION")
                    {
                        string StopRegEmailsDoc = Helper.GetStopRegEmails_Doc(ViewModal.CaseRegistrationId);
                        ViewBag.StopRegUserName = ViewModal.StopRegUserName ?? User.Identity.Name;
                        if (StopRegEmailsDoc == "#")
                        {
                            ViewBag.View_StopRegEmailsDoc = "AppHidden";
                        }
                        else
                        {
                            ViewBag.View_StopRegEmailsDoc = "";
                            ViewBag.StopRegEmailsDoc = StopRegEmailsDoc;
                        }
                    }
                }
                else
                {
                    if (PartialViewName == "_TBR_Modify")
                    {
                        if (ViewModal.ActionLevel == "1")
                            ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);
                        else
                            ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR, FileStatusCodesTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);

                        ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                        ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);
                        ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", ViewModal.DepartmentType);
                        ViewBag.DisputeLevel = new SelectList(Helper.GetDisputeLevel(), "Mst_Value", "Mst_Desc", ViewModal.DisputeLevel);
                        ViewBag.DisputeType = new SelectList(Helper.GetDisputeType(), "Mst_Value", "Mst_Desc", ViewModal.DisputeType);
                        ViewBag.Session_CaseType = new SelectList(Helper.GetSessionCaseType(), "Mst_Value", "Mst_Desc");
                        ViewBag.Session_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc");

                        ViewBag.CaseRegistrationId = caseRegistrationId;

                    }
                    else if (PartialViewName == "_TBR_Create")
                    {
                        caseRegistrationId = InitializedCaseRegistration(caseid, ActionLevel, DisputeLevel);

                        ViewModal = new CaseRegistrationVM();
                        Helper.GetCaseRegistrationVM(caseRegistrationId, ref ViewModal);

                        if (ViewModal.ActionLevel == "1")
                            ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);
                        else
                            ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR, FileStatusCodesTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);

                        ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                        ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);
                        ViewBag.DepartmentType = new SelectList(Helper.GetInvestmentYN(), "Mst_Value", "Mst_Desc", ViewModal.DepartmentType);
                        ViewBag.DisputeLevel = new SelectList(Helper.GetDisputeLevel(), "Mst_Value", "Mst_Desc", ViewModal.DisputeLevel);
                        ViewBag.DisputeType = new SelectList(Helper.GetDisputeType(), "Mst_Value", "Mst_Desc", ViewModal.DisputeType);
                        ViewBag.Session_CaseType = new SelectList(Helper.GetSessionCaseType(), "Mst_Value", "Mst_Desc");
                        ViewBag.Session_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc");

                        ViewBag.CaseRegistrationId = caseRegistrationId;

                        PartialViewName = "_TBR_Modify";
                    }
                }
                ViewBag.hid_DetailId = ViewModal.DetailId;
                ViewBag.UpdatedOn = ViewModal.UpdatedOn;

                return PartialView(PartialViewName, ViewModal);

            }
            else
            {

                if (Mode == "C")
                {
                    if (ClassificationId == "1")
                    {
                        ViewBag.ClasificationName = "BANKS";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", "0"); // Blank
                        ViewBag.ODBBankBranch = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ODBBankBranch), "Mst_Value", "Mst_Desc");
                        ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc");

                        PartialViewName = "_ClientClassificationBanks";
                    }
                    else if (ClassificationId == "2")
                    {
                        ViewBag.ClasificationName = "FINANCE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", "0"); // Blank
                        ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc");

                        PartialViewName = "_ClientClassificationFinance";
                    }
                    else if (ClassificationId == "3")
                    {
                        ViewBag.ClasificationName = "REAL ESTATE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", "0"); // Blank

                        PartialViewName = "_ClientClassificationRealEstate";
                    }
                    else if (ClassificationId == "4")
                    {
                        ViewBag.ClasificationName = "INSURANCE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", "0"); // Blank

                        PartialViewName = "_ClientClassificationInsurance";
                    }
                    else if (ClassificationId == "5")
                    {
                        ViewBag.ClasificationName = "COMPANIES";

                        PartialViewName = "_ClientClassificationCompanies";
                    }
                    else if (ClassificationId == "6")
                    {
                        ViewBag.ClasificationName = "INDIVIDUALS";

                        PartialViewName = "_ClientClassificationIndividuals";
                    }
                    else if (ClassificationId == "7")
                    {
                        ViewBag.ClasificationName = "CONSULTANCY";
                        PartialViewName = "_ClientClassificationConsulatancy";
                    }
                    else
                        PartialViewName = ClassificationId;

                    if (PartialViewName == "_PartViewBeforeCourt")
                    {
                        ViewBag.PoliceStation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                        ViewBag.PublicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                        ViewBag.PAPCPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                        ViewBag.LaborConflicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc");
                        ViewBag.ReconciliationDep = new SelectList(Helper.GetReconciliationDeptt(), "Mst_Value", "Mst_Desc");
                    }
                    else if (PartialViewName == "_PartViewTobeReg")
                    {
                        string[] FileCodes = new[] { "0", "OFS-1" };
                        ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR,FileCodes), "Mst_Value", "Mst_Desc");
                        return PartialView(PartialViewName, new ToBeRegisterVM());
                    }
                    else if (PartialViewName == "_PartViewPrimary")
                    {
                        ViewBag.hid_DetailId = 0;
                        ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc");
                        ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc");
                    }
                    else if (PartialViewName == "_PartViewAppeal")
                    {
                        ViewBag.hid_DetailId = 0;
                        ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc");
                        ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc");
                        ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");
                    }
                    else if (PartialViewName == "_PartViewSupreme")
                    {
                        ViewBag.hid_DetailId = 0;
                        ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc");
                        ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc");
                        ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc");
                    }
                    else if (PartialViewName == "_PartViewEnforcement")
                    {
                        ViewBag.hid_DetailId = 0;
                        ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc");
                        ViewBag.EnforcementAdmin = new SelectList(Helper.GetEnfcAdmin(), "Mst_Value", "Mst_Desc");
                        ViewBag.EnforcementlevelId = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterENF), "Mst_Value", "Mst_Desc");
                        ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("4"), "Mst_Value", "Mst_Desc");
                        ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");
                        ViewBag.AgainstInsurance = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc");

                    }
                    else if (PartialViewName == "_CreateCase")
                    {
                        if (User.IsInRole("AllowCloseCase") || User.IsSysAdmin())
                            ViewBag.AllowCloseCase = "Y";
                        else
                            ViewBag.AllowCloseCase = "N";

                        if (User.IsInRole("AllowAddClient") || User.IsSysAdmin())
                            ViewBag.AllowAddClient = "Y";
                        else
                            ViewBag.AllowAddClient = "N";

                        ViewBag.MstParentId = (int)MASTER_S.Client;

                        ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc");
                        ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc");
                        ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                        ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc");
                        ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client), "Mst_Value", "Mst_Desc");
                        ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc");
                        ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc");
                        ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc");
                        ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc");
                        ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc");

                    }
                    else if (PartialViewName == "_PayVoucherCreate")
                    {
                        PayVoucher payVoucher = new PayVoucher();
                        ViewBag.Payment_Head = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.PaymentHeads && m.Remarks == "R" && m.Active_Flag == true).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
                        ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");
                        ViewBag.VoucherDate = DateTime.Now.ToString("dd/MM/yyyy");
                        return PartialView(PartialViewName, payVoucher);

                    }
                    else if (PartialViewName == "ContactResult")
                    {
                        ViewBag.DEF_CallerName = new SelectList(Helper.GetCallerNames(), "Mst_Value", "Mst_Desc");
                    }
                    else if (PartialViewName == "RecoveryFromCourt")
                    {
                        string[] FileCodes = new[] { "0", "OFS-29", "OFS-30" };
                        ViewBag.CauseOfRecoveryId = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterENF, FileCodes), "Mst_Value", "Mst_Desc");

                    }
                    else if (PartialViewName == "TBRAppSubmission")
                    {
                        ViewBag.ActionLevel = "2";
                        ViewBag.DisputeLevel = "0";
                    }
                    else if (PartialViewName == "TBRSupSubmission")
                    {
                        ViewBag.ActionLevel = "3";
                        ViewBag.DisputeLevel = "0";
                    }
                    else if (PartialViewName == "TBRPriDispute")
                    {
                        ViewBag.ActionLevel = "4";
                        ViewBag.DisputeLevel = "1";
                    }
                    else if (PartialViewName == "TBRAplDispute")
                    {
                        ViewBag.ActionLevel = "4";
                        ViewBag.DisputeLevel = "2";
                    }
                    else if (PartialViewName == "TBRSupDispute")
                    {
                        ViewBag.ActionLevel = "4";
                        ViewBag.DisputeLevel = "3";
                    }

                    return PartialView(PartialViewName);
                }
                else
                {
                    int caseid = Convert.ToInt32(Mode);
                    ToBeRegisterVM ViewModal = new ToBeRegisterVM();
                    ViewModal = GetFilledPartailView(caseid);
                    if (ViewModal == null)
                    {
                        return HttpNotFound();
                    }

                    if (ClassificationId == "1")
                    {

                        ViewBag.ClasificationName = "BANKS";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.ClientCaseType); // Blank
                        ViewBag.ODBBankBranch = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ODBBankBranch), "Mst_Value", "Mst_Desc", ViewModal.ODBBankBranch);
                        ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc", ViewModal.LoanManager);

                        PartialViewName = "_ClientClassificationBanks";
                    }
                    else if (ClassificationId == "2")
                    {
                        ViewBag.ClasificationName = "FINANCE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.ClientCaseType); // Blank
                        ViewBag.LoanManager = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.LoanManager), "Mst_Value", "Mst_Desc", ViewModal.LoanManager);

                        PartialViewName = "_ClientClassificationFinance";
                    }
                    else if (ClassificationId == "3")
                    {
                        ViewBag.ClasificationName = "REAL ESTATE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.ClientCaseType); // Blank

                        PartialViewName = "_ClientClassificationRealEstate";
                    }
                    else if (ClassificationId == "4")
                    {
                        ViewBag.ClasificationName = "INSURANCE";
                        ViewBag.ClientCaseType = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.ClientCaseType); // Blank

                        PartialViewName = "_ClientClassificationInsurance";
                    }
                    else if (ClassificationId == "5")
                    {
                        ViewBag.ClasificationName = "COMPANIES";

                        PartialViewName = "_ClientClassificationCompanies";
                    }
                    else if (ClassificationId == "6")
                    {
                        ViewBag.ClasificationName = "INDIVIDUALS";

                        PartialViewName = "_ClientClassificationIndividuals";
                    }
                    else if (ClassificationId == "7")
                    {
                        ViewBag.ClasificationName = "CONSULTANCY";
                        PartialViewName = "_ClientClassificationConsulatancy";
                    }
                    else
                        PartialViewName = ClassificationId;

                    ViewModal.PartialViewName = PartialViewName;

                    if (PartialViewName.IndexOf("CEL-") < 0)
                    {
                        if (PartialViewName == "_PartViewBeforeCourt")
                        {
                            ViewBag.PoliceStation = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", ViewModal.PoliceStation);
                            ViewBag.PublicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", ViewModal.PublicPlace);
                            ViewBag.PAPCPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", ViewModal.PAPCPlace);
                            ViewBag.LaborConflicPlace = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location), "Mst_Value", "Mst_Desc", ViewModal.LaborConflicPlace);
                            ViewBag.ReconciliationDep = new SelectList(Helper.GetReconciliationDeptt(), "Mst_Value", "Mst_Desc", ViewModal.ReconciliationDep);
                        }
                        else if (PartialViewName == "_PartViewTobeReg")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            if (ViewModal.CaseRegistrationId > 0)
                                ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR), "Mst_Value", "Mst_Desc", ViewModal.FileStatus);
                            else
                            {
                                string[] FileCodes = new[] { "0", "OFS-1" };
                                ViewBag.FileStatus = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterTBR, FileCodes), "Mst_Value", "Mst_Desc");
                            }

                        }
                        else if (PartialViewName == "_PartViewPrimary")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                        }
                        else if (PartialViewName == "_PartViewAppeal")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                            ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);
                        }
                        else if (PartialViewName == "_PartViewSupreme")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                            ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);
                        }
                        else if (PartialViewName == "_PartViewEnforcement")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                            ViewBag.EnforcementAdmin = new SelectList(Helper.GetEnfcAdmin(), "Mst_Value", "Mst_Desc", ViewModal.EnforcementAdmin);
                            ViewBag.EnforcementlevelId = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterENF), "Mst_Value", "Mst_Desc", ViewModal.EnforcementlevelId);
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("4"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                            ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);
                            ViewBag.AgainstInsurance = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.AgainstInsurance);

                        }
                        else if (PartialViewName == "_GetRegisterId")
                        {

                        }
                        else if (PartialViewName == "_ModifyCase")
                        {
                            if (User.IsInRole("AllowCloseCase") || User.IsSysAdmin())
                                ViewBag.AllowCloseCase = "Y";
                            else
                                ViewBag.AllowCloseCase = "N";

                            if (User.IsInRole("AllowAddClient") || User.IsSysAdmin())
                                ViewBag.AllowAddClient = "Y";
                            else
                                ViewBag.AllowAddClient = "N";

                            ViewBag.MstParentId = (int)MASTER_S.Client;
                            ViewBag.ClientClassificationCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ClientClassification), "Mst_Value", "Mst_Desc", ViewModal.ClientClassificationCode);
                            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", ViewModal.ReceiveLevelCode);
                            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.CaseTypeCode);
                            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
                            ViewBag.ClientCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client), "Mst_Value", "Mst_Desc", ViewModal.ClientCode);
                            ViewBag.CaseLevelCode = new SelectList(Helper.GetCaseLevelList("A"), "Mst_Value", "Mst_Desc", ViewModal.CaseLevelCode);
                            ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", ViewModal.SessionClientId);
                            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);
                            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);

                            DataTable _result = Helper.GetDetailTable("CASEHISTTEXT", 0, ViewModal.CaseId);

                            if (_result.Rows.Count > 0)
                                ViewBag.CaseHistoryDetail = _result.Rows[0]["CaseHistory"].ToString();

                            var _sessionsRoll = db.SessionsRoll.Where(w => w.CaseId == ViewModal.CaseId).ToList();
                            string AllJudgement = string.Empty;
                            int i = 0;
                            foreach (var modal in _sessionsRoll)
                            {
                                if (i == 0)
                                    AllJudgement = string.Format(@"{0}{1}{2}{3}{4}", modal.PrimaryJudgements, string.IsNullOrEmpty(modal.PrimaryJudgements) ? "" : Environment.NewLine, modal.AppealJudgements, string.IsNullOrEmpty(modal.AppealJudgements) ? "" : Environment.NewLine, modal.SupremeJudgements);
                                else
                                    AllJudgement = AllJudgement + Environment.NewLine + string.Format(@"{0}{1}{2}{3}{4}", modal.PrimaryJudgements, string.IsNullOrEmpty(modal.PrimaryJudgements) ? "" : Environment.NewLine, modal.AppealJudgements, string.IsNullOrEmpty(modal.AppealJudgements) ? "" : Environment.NewLine, modal.SupremeJudgements);
                                i++;

                            }

                            ViewBag.AllJudgementsDetail = AllJudgement;

                        }
                        else if (PartialViewName == "ENFInfoStage")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);
                            ViewModal.PartialViewName = PartialViewName;
                            ViewModal.SavePV_Data = PartialViewName;
                            ViewBag.MstParentId = (int)MASTER_S.SessionClients;
                            ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", ViewModal.SessionClientId);
                            ViewBag.GovernorateId = new SelectList(Helper.GetGovernorate(), "Mst_Value", "Mst_Desc", ViewModal.GovernorateId);
                            ViewBag.EnforcementAdmin = new SelectList(Helper.GetEnfcAdmin(), "Mst_Value", "Mst_Desc", ViewModal.EnforcementAdmin);
                            ViewBag.EnforcementlevelId = new SelectList(Helper.GetOfficeFileStatus(OfficeFileFilterENF), "Mst_Value", "Mst_Desc", ViewModal.EnforcementlevelId);
                            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList("4"), "Mst_Value", "Mst_Desc", ViewModal.CourtLocationid);
                            ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", ViewModal.ApealByWho);
                            ViewBag.AgainstInsurance = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.AgainstInsurance);
                            ViewBag.ReceiveLevelCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel), "Mst_Value", "Mst_Desc", ViewModal.ReceiveLevelCode);
                            ViewBag.CaseTypeCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseType).OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", ViewModal.CaseTypeCode);
                            ViewBag.AgainstCode = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst), "Mst_Value", "Mst_Desc", ViewModal.AgainstCode);
                            ViewBag.OmaniExp = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OmaniExp), "Mst_Value", "Mst_Desc", ViewModal.OmaniExp);
                            ViewBag.ReOpenEnforcement = new SelectList(Helper.GetYesNoForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ReOpenEnforcement);
                            ViewBag.LawyerId = new SelectList(Helper.GetSessionLawyers(true), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);
                            ViewBag.DEF_CallerName = new SelectList(Helper.GetCallerNames(), "Mst_Value", "Mst_Desc", ViewModal.DEF_CallerName);
                            ViewBag.AnnouncementTypeId = new SelectList(Helper.GetAnnouncementType(), "Mst_Value", "Mst_Desc", ViewModal.AnnouncementTypeId);
                            string LawyerDoc = Helper.GetDEF_Lawyer_Doc(ViewModal.CaseId);
                            string AddressDoc = Helper.GetDEF_Address_Doc(ViewModal.CaseId);

                            if (LawyerDoc == "#")
                            {
                                ViewBag.ViewDEF_LawyerDocs = "AppHidden";
                            }
                            else
                            {
                                ViewBag.ViewDEF_LawyerDocs = "";
                                ViewBag.DEF_Lawyer_Docs = LawyerDoc;
                            }

                            if (AddressDoc == "#")
                            {
                                ViewBag.ViewDEF_AddressDocs = "AppHidden";
                            }
                            else
                            {
                                ViewBag.ViewDEF_AddressDocs = "";
                                ViewBag.DEF_Address_Docs = AddressDoc;
                            }

                            int DefendentTransferId = 0;

                            if (!string.IsNullOrEmpty(DefendentTransfer))
                                DefendentTransferId = Convert.ToInt32(DefendentTransfer);

                            var defendentTransfer = Helper.ProcessDefendentTransfer(DefendentTransferId);

                            if (defendentTransfer != null)
                            {

                                ViewModal.DefendentTransferId = defendentTransfer.DefendentTransferId;
                                ViewModal.MoneyTrRequestDate = defendentTransfer.MoneyTrRequestDate;
                                ViewModal.TransferDate = defendentTransfer.TransferDate;
                                ViewModal.Amount = defendentTransfer.Amount;
                                ViewModal.MoneyTrCompleteDate = defendentTransfer.MoneyTrCompleteDate;

                            }

                            #region Pay Voucher
                            List<MasterSetups> Payment_Head_List = new List<MasterSetups>();
                            MasterSetups PleaseSelect = new MasterSetups();
                            PleaseSelect.Mst_Value = "0";
                            PleaseSelect.Mst_Desc = "PLEASE SELECT";
                            Payment_Head_List.Add(PleaseSelect);
                            Payment_Head_List.AddRange(Helper.LoadPayFor("R"));


                            ViewBag.CourtType = new SelectList(Helper.GetCaseLevelList("D"), "Mst_Value", "Mst_Desc");
                            //ViewBag.Debit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                            //ViewBag.Credit_Account = new SelectList(Helper.GetBankList(), "Mst_Value", "Mst_Desc");
                            ViewBag.Payment_Head = new SelectList(Payment_Head_List, "Mst_Value", "Mst_Desc");
                            ViewBag.Payment_To = new SelectList(Helper.GetListForPayTo(), "Mst_Value", "Mst_Desc");

                            #endregion
                        }
                        else if (PartialViewName == "MoneyTransfer")
                        {
                            int DefendentTransferId = 0;

                            if (!string.IsNullOrEmpty(DefendentTransfer))
                                DefendentTransferId = Convert.ToInt32(DefendentTransfer);

                            var defendentTransfer = Helper.ProcessDefendentTransfer(DefendentTransferId);

                            ViewBag.DefendentTransferId = DefendentTransferId;
                            ViewBag.MoneyTrRequestDate = "";
                            ViewBag.TransferDate = "";
                            ViewBag.Amount = "";
                            ViewBag.MoneyTrCompleteDate = "";

                            if (defendentTransfer != null)
                            {


                                ViewBag.DefendentTransferId = defendentTransfer.DefendentTransferId;
                                ViewBag.MoneyTrRequestDate = defendentTransfer.MoneyTrRequestDate?.ToString("dd/MM/yyyy");
                                ViewBag.TransferDate = defendentTransfer.TransferDate?.ToString("dd/MM/yyyy");
                                ViewBag.Amount = defendentTransfer.Amount;
                                ViewBag.MoneyTrCompleteDate = defendentTransfer.MoneyTrCompleteDate?.ToString("dd/MM/yyyy");

                            }

                            return PartialView(PartialViewName);
                        }
                        else if (PartialViewName == "ENF_UPDATE")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                            ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);
                            ViewBag.LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc", ViewModal.LawyerId);
                            ViewModal.UpdatePV_Type = "ENF_UPDATE";

                        }
                        else if (PartialViewName == "ENF_UPDATE_SESSION")
                        {
                            ViewModal = new ToBeRegisterVM();
                            ViewModal = GetFilledPartailView(caseid, PartialViewName);

                            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.ClientReply);
                            ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", ViewModal.TransportationSource);

                            ViewBag.CaseType = new SelectList(Helper.GetSessionCaseType(), "Mst_Value", "Mst_Desc", ViewModal.CaseType);
                            ViewBag.Session_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc", ViewModal.Session_LawyerId);

                            ViewBag.FollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", ViewModal.FollowerId);
                            ViewBag.SuspendedFollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", ViewModal.SuspendedFollowerId);

                            ViewBag.CourtFollow = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow);
                            ViewBag.CourtFollow_LawyerId = new SelectList(Helper.GetSessionLawyers(), "Mst_Value", "Mst_Desc", ViewModal.CourtFollow_LawyerId);
                            ViewModal.UpdatePV_Type = "ENF_UPDATE_SESSION";

                        }

                    }
                    else
                    {
                        ViewModal = new ToBeRegisterVM();

                        PartialViewName = PartialViewName.Split('-')[1];

                        ViewModal = GetFilledPartailView(caseid, PartialViewName);
                        ViewModal.PartialViewName = PartialViewName;

                        if (PartialViewName == "ENF_LVL_ANNOUNCEMENT")
                        {
                            ViewBag.AnnouncementTypeId = new SelectList(Helper.GetAnnouncementType(), "Mst_Value", "Mst_Desc", ViewModal.AnnouncementTypeId);
                            ViewBag.EnforcementNo = ViewModal.CourtRefNo;
                        }
                        else if (PartialViewName == "ENF_LVL_INQUIRY")
                        {
                            string[] MOHFilterCode = new[] { "1", "2" };
                            string[] ROPFilterCode = new[] { "1", "3" };
                            string[] BANKFilterCode = new[] { "1", "4" };

                            ViewBag.MOHResult = new SelectList(Helper.GetInquiryResult(MOHFilterCode), "Mst_Value", "Mst_Desc", ViewModal.MOHResult);
                            ViewBag.ROPResult = new SelectList(Helper.GetInquiryResult(ROPFilterCode), "Mst_Value", "Mst_Desc", ViewModal.ROPResult);
                            ViewBag.BankResult = new SelectList(Helper.GetInquiryResult(BANKFilterCode), "Mst_Value", "Mst_Desc", ViewModal.BankResult);

                        }
                        else if (PartialViewName == "ENF_LVL_JUDICIALAUCTION")
                        {
                            ViewBag.AuctionProcess = new SelectList(Helper.GetAuctionProcess(), "Mst_Value", "Mst_Desc", ViewModal.AuctionProcess);
                            ViewBag.JUDAuctionRepeat = new SelectList(Helper.GetJUDAuctionRepeat(), "Mst_Value", "Mst_Desc", ViewModal.JUDAuctionRepeat);

                        }
                        else if (PartialViewName == "ENF_LVL_ARRESTORDER")
                        {

                            ViewBag.PoliceStationid = new SelectList(Helper.GetCourtLocationList("4"), "Mst_Value", "Mst_Desc", ViewModal.PoliceStationid);
                            ViewBag.Arrested = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", ViewModal.Arrested);


                        }
                        else if (PartialViewName == "ENF_LVL_SUSPENDED")
                        {
                            ViewBag.SuspensionCauseId = new SelectList(db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CauseOfSuspension), "Mst_Value", "Mst_Desc", ViewModal.SuspensionCauseId);
                            ViewBag.JUDDecisionId = new SelectList(Helper.GetJudicialDecision(), "Mst_Value", "Mst_Desc", ViewModal.JUDDecisionId);
                            ViewBag.SuspensionEndDate = "";
                            ViewBag.DaysRemaining = "";

                            if (ViewModal.EnforcementActionDate.HasValue && ViewModal.SuspensionPeriod.HasValue)
                            {
                                if (ViewModal.SuspensionPeriod > 0)
                                {

                                    double dblSuspendedPeriod = ViewModal.SuspensionPeriod ?? Convert.ToDouble(ViewModal.SuspensionPeriod);
                                    DateTime dtSuspensionEndDate = ViewModal.EnforcementActionDate.Value.AddDays(dblSuspendedPeriod);
                                    ViewBag.SuspensionEndDate = dtSuspensionEndDate.ToString("dd/MM/yyyy");
                                    ViewBag.DaysRemaining = (dtSuspensionEndDate - DateTime.Today).TotalDays < 0 ? (dtSuspensionEndDate - DateTime.Today).TotalDays * -1 : (dtSuspensionEndDate - DateTime.Today).TotalDays;
                                    ViewBag.lblDaysRem_Passed = (dtSuspensionEndDate - DateTime.Today).TotalDays > 0 ? "DAYS REMAINING الأيام المتبقية عن انتهاء الوقف" : "DAYS PASSED";
                                }
                            }

                        }
                        else if (PartialViewName == "ENF_LVL_RECOVERYANDREOPEN")
                        {
                            ViewBag.CauseOfRecoveryId = new SelectList(Helper.GetCauseOfRecovery(), "Mst_Value", "Mst_Desc", ViewModal.CauseOfRecoveryId);

                        }
                        else if (PartialViewName == "ENF_LVL_CLOSEFILE")
                        {
                            ViewBag.ReasonCode = new SelectList(Helper.GetCaseClosingReasons(), "Mst_Value", "Mst_Desc", ViewModal.ReasonCode);

                        }
                        else if (PartialViewName == "ENF_LVL_DISPUTE")
                        {
                            ViewBag.CurrentDisputeLevelandType = new SelectList(Helper.GetDisputeLevelandTypes(), "Mst_Value", "Mst_Desc", ViewModal.CurrentDisputeLevelandType);
                            ViewBag.PrimaryObjectionCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ViewModal.PrimaryObjectionCourt);
                            ViewBag.ApealObjectionCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc", ViewModal.ApealObjectionCourt);
                            ViewBag.SupremeObjectionCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc", ViewModal.SupremeObjectionCourt);

                            ViewBag.PrimaryPlaintCourt = new SelectList(Helper.GetCourtLocationList("1"), "Mst_Value", "Mst_Desc", ViewModal.PrimaryPlaintCourt);
                            ViewBag.ApealPlaintCourt = new SelectList(Helper.GetCourtLocationList("2"), "Mst_Value", "Mst_Desc", ViewModal.ApealPlaintCourt);
                            ViewBag.SupremePlaintCourt = new SelectList(Helper.GetCourtLocationList("3"), "Mst_Value", "Mst_Desc", ViewModal.SupremePlaintCourt);

                        }
                    }
                    ViewBag.hid_DetailId = ViewModal.DetailId;
                    ViewBag.UpdatedOn = ViewModal.UpdatedOn;
                    return PartialView(PartialViewName, ViewModal);
                }
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string UserLocation = string.Empty;

            if (User.IsInRole("CourtCasesViewAll") || User.IsSysAdmin())
                UserLocation = string.Empty;
            else
                UserLocation = Helper.GetEmployeeLocation(User.Identity.Name);

            List<CourtCases> CaseList = GetCourtCasesList(UserLocation);
            CourtCases courtCases = CaseList.Where(w => w.CaseId == id).FirstOrDefault();
            if (courtCases == null)
            {
                return HttpNotFound();
            }
            return View(courtCases);
        }

        // POST: CourtCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourtCases courtCases = db.CourtCase.Find(id);
            courtCases.CaseStatus = "-1";
            db.Entry(courtCases).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GenerateCaseNumber(string BranchCode)
        {
            string Result = string.Empty;
            using (var context = new RBACDbContext())
            {
                var fnParameter = new SqlParameter { ParameterName = "@BranchCode", Value = BranchCode };

                Result = context.Database.SqlQuery<string>("SELECT  [dbo].[FnGenerateCase_Number](@BranchCode)", fnParameter).FirstOrDefault();


            }
            return Result;
        }

        [HttpGet]
        public ActionResult ManageCourtDetail(int CaseId, string Courtid)
        {
            if (User.IsInRole("VoucherApproval") || User.IsSysAdmin())
                ViewBag.UserRole = "VoucherApproval";
            else
                ViewBag.UserRole = "";

            CourtCasesDetail _courtCasesDetail = new CourtCasesDetail();
            string HeaderClass = string.Empty;
            string strCaseLevel = string.Empty;
            var CourtCases = db.CourtCase.Find(CaseId);
            var CourtTypes = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CourtType && w.Mst_Value == Courtid).FirstOrDefault();

            _courtCasesDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.Courtid == Courtid).Include(h => h.FollowupId).FirstOrDefault();

            if (Courtid == "1") { HeaderClass = "card-success"; strCaseLevel = "3"; }
            else if (Courtid == "2") { HeaderClass = "card-warning"; strCaseLevel = "4"; }
            else if (Courtid == "3") { HeaderClass = "card-info"; strCaseLevel = "5"; }
            else if (Courtid == "4") { HeaderClass = "card-danger"; strCaseLevel = "6"; }

            ViewBag.CourtClass = HeaderClass;
            ViewBag.CourtName = CourtTypes.Mst_Desc;
            ViewBag.hid_CaseId = CaseId;
            ViewBag.hid_Courtid = Courtid;
            ViewBag.OfficeFileNo = CourtCases.OfficeFileNo;

            ViewBag.hid_SessionRollId = Helper.GetSessionRollId(CaseId, strCaseLevel);

            if (_courtCasesDetail == null)
                ViewBag.hid_DetailId = 0;
            else
                ViewBag.hid_DetailId = _courtCasesDetail.DetailId;

            return View();
        }

        [HttpGet]
        public ActionResult CourtPView(int CaseId, string Courtid, int SessionRollId = 0)
        {
            CourtCasesDetail _courtCasesDetail = new CourtCasesDetail();
            CourtCasesDetailVM _RetPartialView = new CourtCasesDetailVM();

            string HeaderClass = string.Empty;
            string SelectCaseLevel = string.Empty;
            string CaseLevelName = string.Empty;
            ViewBag.hid_CaseId = CaseId;
            ViewBag.hid_Courtid = Courtid;

            _courtCasesDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.Courtid == Courtid).Include(h => h.FollowupId).FirstOrDefault();
            var CourtCases = db.CourtCase.Find(CaseId);
            var SessionRolls = db.SessionsRoll.Find(SessionRollId);
            var CourtTypes = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CourtType && w.Mst_Value == Courtid).FirstOrDefault();
            var AgainstName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseAgainst && w.Mst_Value == CourtCases.AgainstCode).FirstOrDefault().Mst_Desc;

            string ClientName = "";
            string SessionRollClientName = "";

            if (!string.IsNullOrEmpty(CourtCases.ClientCode))
                ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == CourtCases.ClientCode).FirstOrDefault().Mst_Desc;

            if (!string.IsNullOrEmpty(CourtCases.SessionClientId))
                SessionRollClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.SessionClients && w.Mst_Value == CourtCases.SessionClientId).FirstOrDefault().Mst_Desc;


            if (Courtid == "1") { ViewBag.CourtRefNo = "PRIMARY NO"; HeaderClass = "card-success"; SelectCaseLevel = "3"; CaseLevelName = "PRIMARY COURT"; }
            else if (Courtid == "2") { ViewBag.CourtRefNo = "APPEAL NO"; HeaderClass = "card-warning"; SelectCaseLevel = "4"; CaseLevelName = "APPEAL COURT"; }
            else if (Courtid == "3") { ViewBag.CourtRefNo = "SUPREME NO"; HeaderClass = "card-info"; SelectCaseLevel = "5"; CaseLevelName = "SUPREME COURT"; }
            else if (Courtid == "4") { ViewBag.CourtRefNo = "ENFORCEMENT NO"; HeaderClass = "card-danger"; SelectCaseLevel = "6"; CaseLevelName = "ENFORCEMENT"; }

            ViewBag.CourtClass = HeaderClass;
            ViewBag.CourtName = CourtTypes.Mst_Desc;
            ViewBag.AgainstName = AgainstName;
            ViewBag.ClaimAmount = CourtCases.ClaimAmount;
            ViewBag.OfficeFileNo = CourtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = CourtCases.Defendant;


            _RetPartialView.CaseId = CaseId;
            _RetPartialView.Courtid = Courtid;
            _RetPartialView.CaseLevelCode = SelectCaseLevel;
            _RetPartialView.YandSNotesUpdate = CourtCases.YandSNotes;
            _RetPartialView.ClaimSummary = CourtCases.ClaimSummary;
            _RetPartialView.RealEstateYesNo = CourtCases.RealEstateYesNo;
            _RetPartialView.RealEstateDetail = CourtCases.RealEstateDetail;
            _RetPartialView.CurrentHearingDate = CourtCases.CurrentHearingDate;
            _RetPartialView.CourtDecision = CourtCases.CourtDecision;
            _RetPartialView.SessionRemarks = CourtCases.SessionRemarks;
            _RetPartialView.Requirements = CourtCases.Requirements;
            _RetPartialView.SessionClientId = CourtCases.SessionClientId;
            _RetPartialView.SessionRollDefendentName = CourtCases.SessionRollDefendentName;
            _RetPartialView.NextHearingDate = CourtCases.NextHearingDate;
            _RetPartialView.TransportationFee = CourtCases.TransportationFee;
            _RetPartialView.TransportationSource = CourtCases.TransportationSource;
            _RetPartialView.ClientReply = CourtCases.ClientReply;

            _RetPartialView.ClientName = ClientName;
            _RetPartialView.Defendant = CourtCases.Defendant;
            _RetPartialView.OfficeFileNo = CourtCases.OfficeFileNo;
            _RetPartialView.SessionRollClientName = SessionRollClientName;

            if (_courtCasesDetail == null)
            {
                ViewBag.hid_DetailId = 0;

                if (SessionRolls != null)
                {
                    ViewBag.hid_SessionRollId = SessionRolls.SessionRollId;
                    _RetPartialView.ShowFollowup = SessionRolls.ShowFollowup;
                    _RetPartialView.ShowSuspend = SessionRolls.ShowSuspend;
                    _RetPartialView.LastDate = SessionRolls.LastDate;
                    _RetPartialView.WorkRequired = SessionRolls.WorkRequired;
                    _RetPartialView.SessionNotes = SessionRolls.SessionNotes;
                    _RetPartialView.SuspendedWorkRequired = SessionRolls.WorkRequired;
                    _RetPartialView.SuspendedSessionNotes = SessionRolls.SessionNotes;
                    _RetPartialView.FollowerId = SessionRolls.FollowerId;
                }
                else
                {
                    ViewBag.hid_SessionRollId = 0;
                }

                ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(Courtid), "Mst_Value", "Mst_Desc");
                ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc");

                if (Courtid == "3")
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc");
                else
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc");

                ViewBag.CaseLevelCode = CaseLevelName;
                ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel().OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc");
            }
            else
            {

                _RetPartialView.DetailId = _courtCasesDetail.DetailId;
                _RetPartialView.CaseId = _courtCasesDetail.CaseId;
                _RetPartialView.Courtid = _courtCasesDetail.Courtid;
                _RetPartialView.CourtRefNo = _courtCasesDetail.CourtRefNo;
                _RetPartialView.CourtLocationid = _courtCasesDetail.CourtLocationid;
                _RetPartialView.RegistrationDate = _courtCasesDetail.RegistrationDate;
                _RetPartialView.CourtDepartment = _courtCasesDetail.CourtDepartment;
                _RetPartialView.CaseLevelCode = _courtCasesDetail.CaseLevelCode;
                _RetPartialView.CourtStatus = _courtCasesDetail.CourtStatus;
                _RetPartialView.JudgementDate = _courtCasesDetail.JudgementDate;
                _RetPartialView.JudgementReceivingDate = _courtCasesDetail.JudgementReceivingDate;
                _RetPartialView.JudgementDetails = _courtCasesDetail.JudgementDetails;
                _RetPartialView.ApealByWho = _courtCasesDetail.ApealByWho;
                _RetPartialView.ClosureDate = _courtCasesDetail.ClosureDate;
                _RetPartialView.ClosedbyStaff = _courtCasesDetail.ClosedbyStaff;
                _RetPartialView.NextCaseLevelCode = _courtCasesDetail.NextCaseLevelCode;


                if (Courtid == "1")
                {
                    var CaseRegistered = db.CaseRegistration.Where(w => w.CaseId == CaseId && w.ActionLevel == "1").FirstOrDefault();

                    if (CaseRegistered != null)
                    {
                        _RetPartialView.ClaimSummary = CaseRegistered.ClaimSummary;
                        _RetPartialView.RealEstateYesNo = CaseRegistered.RealEstateYesNo;
                        _RetPartialView.RealEstateDetail = CaseRegistered.RealEstateDetail;

                    }
                }

                if (SessionRolls != null)
                {
                    ViewBag.hid_SessionRollId = SessionRolls.SessionRollId;
                    _RetPartialView.ShowFollowup = SessionRolls.ShowFollowup;
                    _RetPartialView.ShowSuspend = SessionRolls.ShowSuspend;
                    _RetPartialView.LastDate = SessionRolls.LastDate;
                    _RetPartialView.WorkRequired = SessionRolls.WorkRequired;
                    _RetPartialView.SessionNotes = SessionRolls.SessionNotes;
                    _RetPartialView.SuspendedWorkRequired = SessionRolls.WorkRequired;
                    _RetPartialView.SuspendedSessionNotes = SessionRolls.SessionNotes;
                    _RetPartialView.FollowerId = SessionRolls.FollowerId;
                }
                else
                {
                    ViewBag.hid_SessionRollId = 0;
                }

                ViewBag.hid_DetailId = _courtCasesDetail.DetailId;
                ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(Courtid), "Mst_Value", "Mst_Desc", _RetPartialView.CourtLocationid);
                ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", _RetPartialView.CourtDepartment);
                ViewBag.CaseLevelCode = CaseLevelName;
                if (Courtid == "3")
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc", _RetPartialView.ApealByWho);
                else
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", _RetPartialView.ApealByWho);

                ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel().OrderBy(o => o.DisplaySeq), "Mst_Value", "Mst_Desc", _courtCasesDetail.NextCaseLevel);


            }


            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", _RetPartialView.ClientReply);
            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", _RetPartialView.TransportationSource);
            ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", _RetPartialView.SessionClientId);
            ViewBag.FollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", _RetPartialView.FollowerId);

            ViewBag.UpdatedOn = CourtCases.UpdatedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? CourtCases.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");
            return PartialView("_CourtPView", _RetPartialView);

        }

        [HttpPost]
        public ActionResult CourtPView(CourtCasesDetailVM courtCasesDetail)
        {
            CourtCasesDetail _ModalToSave = new CourtCasesDetail();

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            string HeaderClass = string.Empty;
            string SelectCaseLevel = string.Empty;
            string CaseLevelName = string.Empty;

            var CourtCases = db.CourtCase.Find(courtCasesDetail.CaseId);
            var CourtTypes = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CourtType && w.Mst_Value == courtCasesDetail.Courtid).FirstOrDefault().Mst_Desc;
            var AgainstName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.CaseAgainst && w.Mst_Value == CourtCases.AgainstCode).FirstOrDefault().Mst_Desc;
            var ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == CourtCases.ClientCode).FirstOrDefault().Mst_Desc;


            if (courtCasesDetail.Courtid == "1") { HeaderClass = "card-success"; SelectCaseLevel = "3"; CaseLevelName = "PRIMARY COURT"; }
            else if (courtCasesDetail.Courtid == "2") { HeaderClass = "card-warning"; SelectCaseLevel = "4"; CaseLevelName = "APPEAL COURT"; }
            else if (courtCasesDetail.Courtid == "3") { HeaderClass = "card-info"; SelectCaseLevel = "5"; CaseLevelName = "SUPREME COURT"; }
            else if (courtCasesDetail.Courtid == "4") { HeaderClass = "card-danger"; SelectCaseLevel = "6"; CaseLevelName = "ENFORCEMENT"; }


            ViewBag.CourtClass = HeaderClass;
            ViewBag.CourtName = CourtTypes;
            ViewBag.AgainstName = AgainstName;
            ViewBag.ClaimAmount = CourtCases.ClaimAmount;
            ViewBag.OfficeFileNo = CourtCases.OfficeFileNo;
            ViewBag.ClientName = ClientName;
            ViewBag.Defendant = CourtCases.Defendant;

            if (ModelState.IsValid)
            {

                if (courtCasesDetail.DetailId <= 0) // New Create
                {

                    _ModalToSave.CaseId = courtCasesDetail.CaseId;
                    _ModalToSave.Courtid = courtCasesDetail.Courtid;
                    _ModalToSave.CourtRefNo = courtCasesDetail.CourtRefNo;
                    _ModalToSave.CourtLocationid = courtCasesDetail.CourtLocationid;
                    _ModalToSave.RegistrationDate = courtCasesDetail.RegistrationDate;
                    _ModalToSave.CourtDepartment = courtCasesDetail.CourtDepartment;
                    _ModalToSave.CaseLevelCode = courtCasesDetail.CaseLevelCode;
                    _ModalToSave.CourtStatus = courtCasesDetail.CourtStatus;
                    _ModalToSave.JudgementDate = courtCasesDetail.JudgementDate;
                    _ModalToSave.JudgementReceivingDate = courtCasesDetail.JudgementReceivingDate;
                    _ModalToSave.JudgementDetails = courtCasesDetail.JudgementDetails;
                    _ModalToSave.ApealByWho = courtCasesDetail.ApealByWho;
                    _ModalToSave.ClosureDate = courtCasesDetail.ClosureDate;
                    _ModalToSave.ClosedbyStaff = courtCasesDetail.ClosedbyStaff;
                    _ModalToSave.NextCaseLevel = courtCasesDetail.NextCaseLevel;
                    _ModalToSave.NextCaseLevelCode = courtCasesDetail.NextCaseLevelCode;

                    db.CourtCasesDetail.Add(_ModalToSave);
                    db.SaveChanges();

                    CourtCases courtCases = db.CourtCase.Find(_ModalToSave.CaseId);

                    //db.Entry(courtCases).Entity.CaseLevelCode = SelectCaseLevel;
                    db.Entry(courtCases).Entity.YandSNotes = courtCasesDetail.YandSNotesUpdate;
                    db.Entry(courtCases).Entity.CurrentHearingDate = courtCasesDetail.CurrentHearingDate;
                    db.Entry(courtCases).Entity.CourtDecision = courtCasesDetail.CourtDecision;
                    db.Entry(courtCases).Entity.SessionRemarks = courtCasesDetail.SessionRemarks;
                    db.Entry(courtCases).Entity.Requirements = courtCasesDetail.Requirements;
                    db.Entry(courtCases).Entity.ClaimSummary = courtCasesDetail.ClaimSummary;
                    db.Entry(courtCases).Entity.RealEstateYesNo = courtCasesDetail.RealEstateYesNo;
                    db.Entry(courtCases).Entity.RealEstateDetail = courtCasesDetail.RealEstateDetail;
                    db.Entry(courtCases).Entity.SessionClientId = courtCasesDetail.SessionClientId;
                    db.Entry(courtCases).Entity.SessionRollDefendentName = courtCasesDetail.SessionRollDefendentName;
                    db.Entry(courtCases).Entity.ClientReply = courtCasesDetail.ClientReply;
                    db.Entry(courtCases).Entity.NextHearingDate = courtCasesDetail.NextHearingDate;
                    db.Entry(courtCases).Entity.TransportationSource = courtCasesDetail.TransportationSource;

                    db.Entry(courtCases).State = EntityState.Modified;
                    db.SaveChanges();

                    if (_ModalToSave.Courtid == "1")
                    {
                        var CaseRegistered = db.CaseRegistration.Where(w => w.CaseId == _ModalToSave.CaseId && w.ActionLevel == "1" && w.EnforcementDispute == "0" && !w.IsDeleted).FirstOrDefault();

                        if (CaseRegistered != null)
                        {
                            db.Entry(CaseRegistered).Entity.ClaimSummary = courtCasesDetail.ClaimSummary;
                            db.Entry(CaseRegistered).Entity.RealEstateYesNo = courtCasesDetail.RealEstateYesNo;
                            db.Entry(CaseRegistered).Entity.RealEstateDetail = courtCasesDetail.RealEstateDetail;

                            db.Entry(CaseRegistered).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    Helper.ProcessCaseRegistrationProgress(courtCasesDetail, "CourtCasesDetailVM", true);
                }
                else // Modify
                {
                    _ModalToSave = db.CourtCasesDetail.Find(courtCasesDetail.DetailId);
                    string OldCourtRegNumber = _ModalToSave.CourtRefNo;
                    string NewCourtRegNumber = courtCasesDetail.CourtRefNo;

                    db.Entry(_ModalToSave).Entity.DetailId = courtCasesDetail.DetailId;
                    db.Entry(_ModalToSave).Entity.CaseId = courtCasesDetail.CaseId;
                    db.Entry(_ModalToSave).Entity.Courtid = courtCasesDetail.Courtid;
                    db.Entry(_ModalToSave).Entity.CourtRefNo = courtCasesDetail.CourtRefNo;
                    db.Entry(_ModalToSave).Entity.CourtLocationid = courtCasesDetail.CourtLocationid;
                    db.Entry(_ModalToSave).Entity.RegistrationDate = courtCasesDetail.RegistrationDate;
                    db.Entry(_ModalToSave).Entity.CourtDepartment = courtCasesDetail.CourtDepartment;
                    db.Entry(_ModalToSave).Entity.CaseLevelCode = courtCasesDetail.CaseLevelCode;
                    db.Entry(_ModalToSave).Entity.CourtStatus = courtCasesDetail.CourtStatus;
                    db.Entry(_ModalToSave).Entity.JudgementDate = courtCasesDetail.JudgementDate;
                    db.Entry(_ModalToSave).Entity.JudgementReceivingDate = courtCasesDetail.JudgementReceivingDate;
                    db.Entry(_ModalToSave).Entity.JudgementDetails = courtCasesDetail.JudgementDetails;
                    db.Entry(_ModalToSave).Entity.ApealByWho = courtCasesDetail.ApealByWho;
                    db.Entry(_ModalToSave).Entity.ClosureDate = courtCasesDetail.ClosureDate;
                    db.Entry(_ModalToSave).Entity.ClosedbyStaff = courtCasesDetail.ClosedbyStaff;
                    db.Entry(_ModalToSave).Entity.NextCaseLevel = courtCasesDetail.NextCaseLevel;
                    db.Entry(_ModalToSave).Entity.NextCaseLevelCode = courtCasesDetail.NextCaseLevelCode;

                    db.Entry(_ModalToSave).State = EntityState.Modified;
                    db.SaveChanges();

                    _ModalToSave = db.CourtCasesDetail.Find(_ModalToSave.DetailId);

                    CourtCases courtCases = db.CourtCase.Find(_ModalToSave.CaseId);
                    //db.Entry(courtCases).Entity.CaseLevelCode = SelectCaseLevel;
                    db.Entry(courtCases).Entity.YandSNotes = courtCasesDetail.YandSNotesUpdate;
                    db.Entry(courtCases).Entity.CurrentHearingDate = courtCasesDetail.CurrentHearingDate;
                    db.Entry(courtCases).Entity.CourtDecision = courtCasesDetail.CourtDecision;
                    db.Entry(courtCases).Entity.SessionRemarks = courtCasesDetail.SessionRemarks;
                    db.Entry(courtCases).Entity.Requirements = courtCasesDetail.Requirements;
                    db.Entry(courtCases).Entity.ClaimSummary = courtCasesDetail.ClaimSummary;
                    db.Entry(courtCases).Entity.RealEstateYesNo = courtCasesDetail.RealEstateYesNo;
                    db.Entry(courtCases).Entity.RealEstateDetail = courtCasesDetail.RealEstateDetail;
                    db.Entry(courtCases).Entity.SessionClientId = courtCasesDetail.SessionClientId;
                    db.Entry(courtCases).Entity.SessionRollDefendentName = courtCasesDetail.SessionRollDefendentName;
                    db.Entry(courtCases).Entity.ClientReply = courtCasesDetail.ClientReply;
                    db.Entry(courtCases).Entity.NextHearingDate = courtCasesDetail.NextHearingDate;
                    db.Entry(courtCases).Entity.TransportationSource = courtCasesDetail.TransportationSource;

                    db.Entry(courtCases).State = EntityState.Modified;
                    db.SaveChanges();

                    if (_ModalToSave.Courtid == "1")
                    {
                        var CaseRegistered = db.CaseRegistration.Where(w => w.CaseId == _ModalToSave.CaseId && w.ActionLevel == "1" && w.EnforcementDispute == "0" && !w.IsDeleted).FirstOrDefault();

                        if (CaseRegistered != null)
                        {
                            db.Entry(CaseRegistered).Entity.ClaimSummary = courtCasesDetail.ClaimSummary;
                            db.Entry(CaseRegistered).Entity.RealEstateYesNo = courtCasesDetail.RealEstateYesNo;
                            db.Entry(CaseRegistered).Entity.RealEstateDetail = courtCasesDetail.RealEstateDetail;

                            db.Entry(CaseRegistered).State = EntityState.Modified;
                            db.SaveChanges();

                        }

                    }

                    if (OldCourtRegNumber != NewCourtRegNumber)
                        Helper.ProcessCaseRegistrationProgress(courtCasesDetail, "CourtCasesDetailVM");
                }

                UpdateSessionRoll("COURTDETAIL", courtCasesDetail);

                CourtCasesDetailVM _ModalToReturn = new CourtCasesDetailVM();
                CourtCases = new CourtCases();
                CourtCases = db.CourtCase.Find(courtCasesDetail.CaseId);

                _ModalToReturn.DetailId = _ModalToSave.DetailId;
                _ModalToReturn.CaseId = _ModalToSave.CaseId;
                _ModalToReturn.Courtid = _ModalToSave.Courtid;
                _ModalToReturn.CourtRefNo = _ModalToSave.CourtRefNo;
                _ModalToReturn.CourtLocationid = _ModalToSave.CourtLocationid;
                _ModalToReturn.RegistrationDate = _ModalToSave.RegistrationDate;
                _ModalToReturn.CourtDepartment = _ModalToSave.CourtDepartment;
                _ModalToReturn.CaseLevelCode = _ModalToSave.CaseLevelCode;
                _ModalToReturn.CourtStatus = _ModalToSave.CourtStatus;
                _ModalToReturn.JudgementDate = _ModalToSave.JudgementDate;
                _ModalToReturn.JudgementReceivingDate = _ModalToSave.JudgementReceivingDate;
                _ModalToReturn.JudgementDetails = _ModalToSave.JudgementDetails;
                _ModalToReturn.ApealByWho = _ModalToSave.ApealByWho;
                _ModalToReturn.NextCaseLevel = _ModalToSave.NextCaseLevel;
                _ModalToReturn.NextCaseLevelCode = _ModalToSave.NextCaseLevelCode;
                _ModalToReturn.YandSNotesUpdate = CourtCases.YandSNotes;
                _ModalToReturn.ClosureDate = CourtCases.ClosureDate;
                _ModalToReturn.ClosedbyStaff = CourtCases.ClosedbyStaff;

                _ModalToReturn.SessionRollId = courtCasesDetail.SessionRollId;
                _ModalToReturn.CurrentHearingDate = courtCasesDetail.CurrentHearingDate;
                _ModalToReturn.CourtDecision = courtCasesDetail.CourtDecision;
                _ModalToReturn.SessionRemarks = courtCasesDetail.SessionRemarks;
                _ModalToReturn.Requirements = courtCasesDetail.Requirements;

                _ModalToReturn.SessionClientId = CourtCases.SessionClientId;
                _ModalToReturn.SessionRollDefendentName = CourtCases.SessionRollDefendentName;
                _ModalToReturn.NextHearingDate = CourtCases.NextHearingDate;
                _ModalToReturn.TransportationFee = CourtCases.TransportationFee;
                _ModalToReturn.TransportationSource = CourtCases.TransportationSource;
                _ModalToReturn.ClientReply = CourtCases.ClientReply;



                ViewBag.hid_DetailId = _ModalToReturn.DetailId;
                ViewBag.hid_SessionRollId = courtCasesDetail.SessionRollId;

                ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(_ModalToSave.Courtid), "Mst_Value", "Mst_Desc", _ModalToSave.CourtLocationid);
                ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", _ModalToSave.CourtDepartment);
                ViewBag.CaseLevelCode = CaseLevelName;
                

                if (_ModalToReturn.Courtid == "3")
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc", _ModalToReturn.ApealByWho);
                else
                    ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", _ModalToReturn.ApealByWho);


                ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel(), "Mst_Value", "Mst_Desc", _ModalToReturn.NextCaseLevel);

                ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", courtCasesDetail.ClientReply);
                ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", courtCasesDetail.TransportationSource);

                ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", courtCasesDetail.SessionClientId);
                ViewBag.FollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", courtCasesDetail.FollowerId);

                Session["Message"] = new MessageVM
                {
                    Category = "Success",
                    Message = "Data Save Successfully"
                };
                return PartialView("_CourtPView", _ModalToReturn);
            }

            ViewBag.CourtLocationid = new SelectList(Helper.GetCourtLocationList(courtCasesDetail.Courtid), "Mst_Value", "Mst_Desc", courtCasesDetail.CourtLocationid);
            ViewBag.CourtDepartment = new SelectList(Helper.GetSupremeStage(), "Mst_Value", "Mst_Desc", courtCasesDetail.CourtDepartment);
            ViewBag.CaseLevelCode = CaseLevelName;
            ViewBag.hid_CaseId = courtCasesDetail.CaseId;
            ViewBag.hid_Courtid = courtCasesDetail.Courtid;
            ViewBag.hid_DetailId = courtCasesDetail.DetailId;
            ViewBag.hid_SessionRollId = courtCasesDetail.SessionRollId;

            if (courtCasesDetail.Courtid == "3")
                ViewBag.ApealByWho = new SelectList(Helper.GetByWho(true), "Mst_Value", "Mst_Desc", courtCasesDetail.ApealByWho);
            else
                ViewBag.ApealByWho = new SelectList(Helper.GetByWho(), "Mst_Value", "Mst_Desc", courtCasesDetail.ApealByWho);

            ViewBag.NextCaseLevel = new SelectList(Helper.GetNextCaseLevel(), "Mst_Value", "Mst_Desc", courtCasesDetail.NextCaseLevel);

            ViewBag.ClientReply = new SelectList(Helper.GetYesForSelect(), "Mst_Value", "Mst_Desc", courtCasesDetail.ClientReply);
            ViewBag.TransportationSource = new SelectList(Helper.GetTransSourceSelect(), "Mst_Value", "Mst_Desc", courtCasesDetail.TransportationSource);

            ViewBag.SessionClientId = new SelectList(Helper.GetSessionClients(), "Mst_Value", "Mst_Desc", courtCasesDetail.SessionClientId);
            ViewBag.FollowerId = new SelectList(Helper.GetSessionFollowers(), "Mst_Value", "Mst_Desc", courtCasesDetail.FollowerId);


            Session["Message"] = new MessageVM
            {
                Category = "Error",
                Message = string.Join("<br/>", ModelState.Values.SelectMany(v => v.Errors).Select(x => x.ErrorMessage).ToArray())
            };
            return PartialView("_CourtPView", courtCasesDetail);
        }

        [HttpGet]
        public ActionResult GetHearingDetail(int DetailId, string Courtid)
        {
            using (var context = new RBACDbContext())
            {
                List<CourtCasesFollowup> HearingList = new List<CourtCasesFollowup>();
                SqlParameter pDetailId = new SqlParameter("@DetailId", DetailId);
                SqlParameter pCourtid = new SqlParameter("@Courtid", Courtid);

                HearingList = context.Database.SqlQuery<CourtCasesFollowup>("[dbo].[GetHearingDetail] @DetailId, @Courtid", pDetailId, pCourtid).OrderByDescending(o => o.HearingDate).ToList();
                return PartialView("_CourtCaseHearingList", HearingList);
            }
        }

        [HttpGet]
        public ActionResult CreateCourtFollowup(int DetailId, string Courtid)
        {
            CourtCasesFollowup CasesFollowups = new CourtCasesFollowup();
            CasesFollowups.DetailId = DetailId;
            return PartialView("_CourtCaseHearingCreate", CasesFollowups);
        }
        [HttpPost]
        public ActionResult CreateCourtFollowup(CourtCasesFollowup courtCasesFollowup)
        {

            if (ModelState.IsValid)
            {
                db.CourtCasesFollowup.Add(courtCasesFollowup);
                db.SaveChanges();
                return new JsonResult()
                {
                    Data = new { Message = "Success" }
                };
            }

            var modelErrors = ModelState.AllErrors();

            return Json(modelErrors);
        }

        [HttpGet]
        public ActionResult DeleteCourtFollowup(int FollowupId)
        {
            CourtCasesFollowup CasesFollowups = db.CourtCasesFollowup.Find(FollowupId);
            int DetailId = CasesFollowups.DetailId;

            db.CourtCasesFollowup.Remove(CasesFollowups);
            db.SaveChanges();

            var HearingList = db.CourtCasesFollowup.Where(w => w.DetailId == DetailId).ToList();

            return PartialView("_CourtCaseHearingList", HearingList);
        }

        [HttpGet]
        public ActionResult ManageEnforcementDetail(int CaseId)
        {
            var _CasesEnforcement = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).FirstOrDefault();

            if (_CasesEnforcement == null)
                return RedirectToAction("EnforcementCreate", new RouteValueDictionary(new { id = CaseId }));
            else
                return RedirectToAction("EnforcementEdit", new RouteValueDictionary(new { id = _CasesEnforcement.EnforcementId }));
        }
               

        [HttpPost]
        public ActionResult GetDefaulterDetail()
        {
            var request = HttpContext.Request;
            string P_DataParameter = string.Empty;
            string P_DataFor = string.Empty;
            try
            {
                P_DataParameter = request.Params["P_DataParameter"].ToString();
                P_DataFor = request.Params["P_DataFor"].ToString();
            }
            catch (Exception e)
            {

            }

            List<CourtCases> DefaulterList = new List<CourtCases>();

            if (P_DataFor == "IdRegistrationNo")
                DefaulterList = db.CourtCase.Where(w => w.IdRegistrationNo == P_DataParameter).ToList();
            else if (P_DataFor == "CRRegistrationNo")
                DefaulterList = db.CourtCase.Where(w => w.CRRegistrationNo == P_DataParameter).ToList();
            else if (P_DataFor == "AccountContractNo")
                DefaulterList = db.CourtCase.Where(w => w.AccountContractNo == P_DataParameter).ToList();
            else if (P_DataFor == "ClientFileNo")
                DefaulterList = db.CourtCase.Where(w => w.ClientFileNo == P_DataParameter).ToList();

            var ResultList = (from res in DefaulterList
                              join statusname in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseLevel)
                              on res.CaseLevelCode equals statusname.Mst_Value
                              select new CaseDefaulterVM
                              {
                                  OfficeFileNo = res.OfficeFileNo,
                                  AccountContractNo = res.AccountContractNo,
                                  ClientFileNo = res.ClientFileNo,
                                  Defendant = res.Defendant,
                                  CaseStatusName = statusname.Mst_Desc
                              }).ToList();

            return Json(ResultList);
        }

        public static List<CourtCases> GetCourtCasesList(string LocationName)
        {
            List<CourtCases> ResultList = new List<CourtCases>();
            RBACDbContext db = new RBACDbContext();

            if (!string.IsNullOrEmpty(LocationName))
                ResultList = db.CourtCase.Where(w => w.OfficeFileNo.Substring(0, 1).ToUpper() == LocationName.Substring(4, 1).ToUpper()).ToList();
            else
                ResultList = db.CourtCase.ToList();

            var ReturnResult = (from res in ResultList
                                join clname in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.Client)
                                on res.ClientCode equals clname.Mst_Value
                                join Agnst in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst)
                                on res.AgainstCode equals Agnst.Mst_Value
                                join RcvLevel in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ReceiveLevel)
                                on res.ReceiveLevelCode equals RcvLevel.Mst_Value
                                join CType in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseType)
                                on res.CaseTypeCode equals CType.Mst_Value
                                join CLevel in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseLevel)
                                 on res.CaseLevelCode equals CLevel.Mst_Value
                                join ODB in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ODBLoan)
                                on res.ODBLoanCode equals ODB.Mst_Value
                                join ODBBr in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ODBBankBranch)
                                on res.ODBBankBranch equals ODBBr.Mst_Value
                                join PolSt in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.Location)
                                on res.PoliceStation equals PolSt.Mst_Value
                                join PubPlace in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.Location)
                                on res.PublicPlace equals PubPlace.Mst_Value
                                join PAPPlace in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.Location)
                                on res.PAPCPlace equals PAPPlace.Mst_Value
                                join LaborConfPlace in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.Location)
                                on res.LaborConflicPlace equals LaborConfPlace.Mst_Value
                                join caseStatus in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseStatus)
                                on res.CaseStatus equals caseStatus.Mst_Value
                                join clientCaseType in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ClientCaseType)
                                on res.ClientCaseType equals clientCaseType.Mst_Value
                                join parentCourt in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ParentCourt)
                                on res.ParentCourtId equals parentCourt.Mst_Value
                                join caseSubj in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.CaseSubject)
                                on res.CaseSubject equals caseSubj.Mst_Value
                                join caseclass in db.Set<MasterSetups>().Where(m => m.MstParentId == (int)MASTER_S.ClientClassification)
                                on res.ClientClassificationCode equals caseclass.Mst_Value
                                select new CourtCases
                                {
                                    CaseId = res.CaseId,
                                    OfficeFileNo = res.OfficeFileNo,
                                    ClientCode = res.ClientCode,
                                    Defendant = res.Defendant,
                                    AgainstCode = res.AgainstCode,
                                    ReceptionDate = res.ReceptionDate,
                                    ReceiveLevelCode = res.ReceiveLevelCode,
                                    AccountContractNo = res.AccountContractNo,
                                    ClientFileNo = res.ClientFileNo,
                                    ClaimAmount = res.ClaimAmount,
                                    CaseTypeCode = res.CaseTypeCode,
                                    CaseLevelCode = res.CaseLevelCode,
                                    LegalNoticeDate = res.LegalNoticeDate,
                                    ODBLoanCode = res.ODBLoanCode,
                                    ODBBankBranch = res.ODBBankBranch,
                                    PoliceNo = res.PoliceNo,
                                    PoliceStation = res.PoliceStation,
                                    PublicProsecutionNo = res.PublicProsecutionNo,
                                    PublicPlace = res.PublicPlace,
                                    PAPCNo = res.PAPCNo,
                                    PAPCPlace = res.PAPCPlace,
                                    LaborConflicNo = res.LaborConflicNo,
                                    LaborConflicPlace = res.LaborConflicPlace,
                                    YandSNotes = res.YandSNotes,
                                    ClientName = clname.Mst_Desc,
                                    DefClientName = res.Defendant,
                                    AgainstName = Agnst.Mst_Desc,
                                    ReceiveLevelName = RcvLevel.Mst_Desc,
                                    CaseTypeName = CType.Mst_Desc,
                                    CaseLevelName = CLevel.Mst_Desc,
                                    ODBLoanName = ODB.Mst_Desc,
                                    ODBBankBranchName = ODBBr.Mst_Desc,
                                    PoliceStationName = PolSt.Mst_Desc,
                                    PublicPlaceName = PubPlace.Mst_Desc,
                                    PAPCPlaceName = PAPPlace.Mst_Desc,
                                    LaborConflicPlaceName = LaborConfPlace.Mst_Desc,
                                    CreatedBy = res.CreatedBy,
                                    CreatedOn = res.CreatedOn,
                                    UpdatedBy = res.UpdatedBy,
                                    UpdatedOn = res.UpdatedOn,
                                    CaseStatus = res.CaseStatus,
                                    CaseStatusName = caseStatus.Mst_Desc,
                                    IsPrivateFee = res.IsPrivateFee,
                                    IsCommission = res.IsCommission,
                                    CRRegistrationNo = res.CRRegistrationNo,
                                    SpecialInstructions = res.SpecialInstructions,
                                    ClientCaseType = res.ClientCaseType,
                                    ClientCaseTypeName = clientCaseType.Mst_Desc,
                                    ParentCourtId = res.ParentCourtId,
                                    ParentCourtName = parentCourt.Mst_Desc,
                                    IdRegistrationNo = res.IdRegistrationNo,
                                    CaseSubject = res.CaseSubject,
                                    CaseSubjectName = caseSubj.Mst_Desc,
                                    ClientClassificationCode = res.ClientClassificationCode,
                                    ClientClassificationName = caseclass.Mst_Desc,
                                    Subject = res.Subject

                                }).ToList();
            return ReturnResult;
        }

        [HttpGet]
        public ActionResult ClosureFormView(int CaseId, string Courtid, DateTime? ClosureDate, string screen = null)
        {
            if (screen == "FileClosure")
            {
                string CaseLevel = "FileClosure";

                List<ClosureFormVM> data = new List<ClosureFormVM>();
                data = Helper.GetClosureFormDetail(CaseId, CaseLevel, User.Identity.Name).ToList();
                return View(data);
            }
            else
            {
                string CaseLevel = string.Empty;

                if (string.IsNullOrEmpty(Courtid))
                {
                    CourtCases courtCases = db.CourtCase.Where(w => w.CaseId == CaseId).FirstOrDefault();
                    CaseLevel = courtCases.CaseLevelCode;
                }
                else
                    CaseLevel = Courtid;

                List<ClosureFormVM> data = new List<ClosureFormVM>();
                data = Helper.GetClosureFormDetail(CaseId, CaseLevel, User.Identity.Name).ToList();
                return View(data);

            }
        }

        public string GetCaseAgreementDoc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = Helper.GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "CaseAgreement");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
    }
}
