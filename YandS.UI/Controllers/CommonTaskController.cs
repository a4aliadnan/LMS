using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using YandS.DAL;
using YandS.UI.Models;
namespace YandS.UI.Controllers
{
    public class CommonTaskController : Controller
    {
        private RBACDbContext db = new RBACDbContext();

        [HttpPost]
        public ActionResult CreateMasterTableDetail(string Mst_Desc, int MstParentId, string Remarks = null)
        {
            //string[] values = new[] { "-2", "-1", "0" };

            //var ItemToBeAddList = db.MasterSetup.Where(m => m.MstParentId == MstParentId && !values.Contains(m.Mst_Value)).ToList();

            var ItemToBeAddList = db.MasterSetup.Where(m => m.MstParentId == MstParentId).ToList();
            int Mst_Value = ItemToBeAddList.Select(s => Convert.ToInt32(s.Mst_Value)).Max() + 1;

            MasterSetups ItemToBeAdd = new MasterSetups();

            ItemToBeAdd.MstParentId = MstParentId;
            ItemToBeAdd.Mst_Desc = Mst_Desc;
            ItemToBeAdd.Mst_Value = Mst_Value.ToString();
            ItemToBeAdd.Active_Flag = true;
            ItemToBeAdd.DisplaySeq = Mst_Value;
            ItemToBeAdd.Remarks = Remarks;

            try
            {
                db.MasterSetup.Add(ItemToBeAdd);
                db.SaveChanges();

                return new JsonResult()
                {
                    Data = new { Message = "Success", id = Mst_Value.ToString(), name = Mst_Desc }
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

        [HttpPost]
        public ActionResult CreateMasterTableDetailForPayTo(string Mst_Desc, int MstParentId, string Remarks = null)
        {
            string[] values = new[] { "-2", "-1", "0" };

            var ItemToBeAddList = db.MasterSetup.Where(m => m.MstParentId == MstParentId && !values.Contains(m.Mst_Value)).ToList();
            //int Mst_Value = ItemToBeAddList.Count + 1;
            int Mst_Value = ItemToBeAddList.Select(s => Convert.ToInt32(s.Mst_Value.Replace("P",""))).Max() + 1;

            MasterSetups ItemToBeAdd = new MasterSetups();

            ItemToBeAdd.MstParentId = MstParentId;
            ItemToBeAdd.Mst_Desc = Mst_Desc;
            string ItemToBeAddValue = string.Empty;
            if (MstParentId == 214)
                ItemToBeAddValue = string.Format(@"P{0}", Mst_Value.ToString());
            else
                ItemToBeAddValue = Mst_Value.ToString();

            ItemToBeAdd.Mst_Value = ItemToBeAddValue;
            ItemToBeAdd.Active_Flag = true;
            ItemToBeAdd.DisplaySeq = Mst_Value;
            ItemToBeAdd.Remarks = Remarks;

            try
            {
                db.MasterSetup.Add(ItemToBeAdd);
                db.SaveChanges();

                return new JsonResult()
                {
                    Data = new { Message = "Success", id = ItemToBeAddValue, name = Mst_Desc }
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

        [HttpPost]
        public ActionResult GetInvoiceDocName(int InvoiceId)
        {

            try
            {
                string ReturnResult = string.Empty;

                string UploadRoot = Helper.GetStorageRoot;

                string UploadPath = Path.Combine(UploadRoot, "INVDocuments");

                DirectoryInfo d = new DirectoryInfo(UploadPath);
                var Image = d.GetFiles(InvoiceId + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

                if (Image == null)
                    ReturnResult = "#";
                else
                    ReturnResult = Image.ToString();

                return new JsonResult()
                {
                    Data = new { Message = "Success", FileName = ReturnResult }
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

        [HttpPost]
        public ActionResult GetPVDocName(int Id,string type)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = Helper.GetStorageRoot;
            string UploadPath = string.Empty;
            PayVoucher payVoucher = db.PayVouchers.Find(Id);

            try
            {
                UploadPath = Path.Combine(UploadRoot, type);

                DirectoryInfo d = new DirectoryInfo(UploadPath);
                var Image = d.GetFiles(Id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

                if (Image == null)
                    ReturnResult = "#";
                else
                    ReturnResult = Image.ToString();

            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new { Message = "Error", ErrorMessage = e.Message }
                };

            }
            return new JsonResult()
            {
                Data = new { Message = "Success", FileName = ReturnResult }
            };


        }

        [HttpPost]
        public ActionResult GetMedicalBillDetail(string EmployeeNumber)
        {

            try
            {
                string ReturnResult = string.Empty;
                List<string> MedicalBillNo = new List<string>();

                var MedicalBills = db.PayVouchers.Where(w => w.Payment_To == EmployeeNumber && w.Payment_Head == "21").Select(s => s.Payment_Head_Remarks).ToList();

                if (MedicalBills == null)
                    ReturnResult = "No Previous Record";
                else
                {
                    foreach (string medicalBillNo in MedicalBills)
                    {
                        string[] split = medicalBillNo.Split(',');

                        foreach (string item in split)
                        {
                            MedicalBillNo.Add(item);
                        }
                    }
                }

                if (MedicalBillNo.Count > 0)
                {
                    return new JsonResult()
                    {
                        Data = new { Message = "Success", MedicalBillNos = MedicalBillNo.ToArray() }
                    };
                }
                else
                {
                    return new JsonResult()
                    {
                        Data = new { Message = "Success", MedicalBillNos = ReturnResult }
                    };
                }
            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new { Message = "Error", ErrorMessage = e.Message }
                };

            }
        }

        [HttpPost]
        public ActionResult GetPetrolKMDetail()
        {
            var request = HttpContext.Request;
            string P_EmployeeNo = string.Empty;
            try
            {
                P_EmployeeNo = request.Params["P_EmployeeNo"].ToString();
            }
            catch (Exception e)
            {

            }

            var ResultList = Helper.GetPetrolKMDetailVM("5", P_EmployeeNo);

            return Json(ResultList);
        }

        [HttpPost]
        public ActionResult GetFeeDetailView()
        {
            var request = HttpContext.Request;
            string P_InvoiceId = string.Empty;
            try
            {
                P_InvoiceId = request.Params["InvoiceId"].ToString();
            }
            catch (Exception e)
            {

            }

            var ResultList = Helper.GetFeeDetailView(Convert.ToInt32(P_InvoiceId));

            return Json(ResultList);
        }

        [HttpPost]
        public ActionResult GetCaseInvoiceDetail()
        {
            var request = HttpContext.Request;
            int CaseId = 0;
            try
            {
                CaseId = Convert.ToInt32(request.Params["P_CaseId"]);
            }
            catch (Exception e)
            {

            }

            var ResultList = Helper.GetCaseInvoiceDetail(CaseId);

            return Json(ResultList);
        }

        [HttpPost]
        public ActionResult LoadClientByClassification(string ClientClassificationId)
        {
            string[] values = new[] { "-2", "-1", "0" };
            List<MasterSetups> lst = db.MasterSetup.Where(m => m.MstParentId == 241 && m.Remarks == ClientClassificationId && !values.Contains(m.Mst_Value)).ToList();

          //selecting the desired columns
            var subCategoryToReturn = lst.Select(S => new {
                Mst_Value = S.Mst_Value,
                Mst_Desc = S.Mst_Desc
            });
            //returning JSON
            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult LoadPayFor(string P_Remark)
        {
            var ResultList = Helper.LoadPayFor(P_Remark);

            //selecting the desired columns
            var subCategoryToReturn = ResultList.Select(S => new
            {
                Mst_Value = S.Mst_Value,
                Mst_Desc = S.Mst_Desc
            });
            //returning JSON
            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult LoadCaseLevelByCaseId(string OfficeFileNo)
        {
            var RetList = Helper.GetCaseLevelList("D");

            //selecting the desired columns
            var subCategoryToReturn = RetList.Select(S => new
            {
                Mst_Value = S.Mst_Value,
                Mst_Desc = S.Mst_Desc
            });
            //returning JSON
            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);

            /*
            var courtCase = db.CourtCase.Where(w => w.OfficeFileNo == OfficeFileNo).FirstOrDefault();
            if (courtCase != null)
            {
                using (var context = new RBACDbContext())
                {
                    List<MasterSetups> ResultList = new List<MasterSetups>();
                    SqlParameter pCaseId = new SqlParameter("@CaseId", courtCase.CaseId);

                    ResultList = context.Database.SqlQuery<MasterSetups>("GetCaseLevelForInvoice @CaseId", pCaseId).OrderBy(o => o.DisplaySeq).ToList();

                    //selecting the desired columns
                    var subCategoryToReturn = ResultList.Select(S => new
                    {
                        Mst_Value = S.Mst_Value,
                        Mst_Desc = S.Mst_Desc
                    });
                    //returning JSON
                    return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var Result = new MasterSetups();
                return this.Json(Result, JsonRequestBehavior.AllowGet);
            }
            */
        }

        [HttpPost]
        public ActionResult GetRefNumberCourt(string Id, string type, int CaseId)
        {
            string CourtRefNo = string.Empty;
            string LocName = string.Empty;
            try
            {
                if (type == "" && Convert.ToInt32(Id) > 2)
                {
                    if (Convert.ToInt32(Id) <= 6) //(PRIMARY / APEAL / SUPREME / ENFORCEMENT)
                    {
                        try
                        {
                            if (Convert.ToInt32(Id) == 6) //(ENFORCEMENT)
                            {
                                var CaseDetail = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).FirstOrDefault();
                                if(CaseDetail != null)
                                {
                                    LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == CaseDetail.CourtLocationid).FirstOrDefault().Mst_Desc;
                                    CourtRefNo = string.Format(@"{0}^{1}", CaseDetail.EnforcementNo, LocName);

                                }

                            }
                            else //(PRIMARY / APEAL / SUPREME )
                            {
                                var CaseDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.CaseLevelCode == Id).FirstOrDefault();
                                if (CaseDetail != null)
                                {
                                    LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == CaseDetail.CourtLocationid).FirstOrDefault().Mst_Desc;
                                    CourtRefNo = string.Format(@"{0}^{1}", CaseDetail.CourtRefNo, LocName);

                                }

                            }

                        }
                        catch (Exception e)
                        {
                            CourtRefNo = "";

                        }
                    }
                }
                else if (type == "ENF" && Convert.ToInt32(Id) > 0) //ENFORCEMENT
                {
                    try
                    {
                        var vRdoEnfInvoiceInfo = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).FirstOrDefault();

                        if (Id == "1")
                            CourtRefNo = vRdoEnfInvoiceInfo.ArrestOrderNo;
                        else if (Id == "2")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.PrimaryObjectionNo;
                        }
                        else if (Id == "3")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.ApealObjectionNo;
                        }
                        else if (Id == "4")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.SupremeObjectionNo;
                        }
                        else if (Id == "5")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.PrimaryPlaintNo;
                        }
                        else if (Id == "6")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.ApealPlaintNo;
                        }
                        else if (Id == "7")
                        {
                            CourtRefNo = vRdoEnfInvoiceInfo.SupremePlaintNo;
                        }
                    }
                    catch (Exception e)
                    {
                        CourtRefNo = "";

                    }
                }
                else //BEFORE COURT
                {
                    try
                    {
                        var vRdoBeforeCourt = db.CourtCase.Find(CaseId);

                        if (Id == "1")
                        {
                            LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == vRdoBeforeCourt.PoliceStation).FirstOrDefault().Mst_Desc;
                            CourtRefNo = string.Format(@"{0}^{1}", vRdoBeforeCourt.PoliceNo, LocName);
                        }
                        else if (Id == "2")
                        {
                            LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == vRdoBeforeCourt.PublicPlace).FirstOrDefault().Mst_Desc;
                            CourtRefNo = string.Format(@"{0}^{1}", vRdoBeforeCourt.PublicProsecutionNo, LocName);
                        }
                        else if (Id == "3")
                        {
                            LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == vRdoBeforeCourt.PAPCPlace).FirstOrDefault().Mst_Desc;
                            CourtRefNo = string.Format(@"{0}^{1}", vRdoBeforeCourt.PAPCNo, LocName);
                        }
                        else if (Id == "4")
                        {
                            LocName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == vRdoBeforeCourt.LaborConflicPlace).FirstOrDefault().Mst_Desc;
                            CourtRefNo = string.Format(@"{0}^{1}", vRdoBeforeCourt.LaborConflicNo, LocName);
                        }
                    }
                    catch (Exception e)
                    {
                        CourtRefNo = "";
                    }
                }


                return new JsonResult()
                {
                    Data = new { Message = "Success", RetResult = CourtRefNo }
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

        [HttpPost]
        public ActionResult LoadFeeTypeDetail(string Id)
        {
            List<MasterSetups> lst = Helper.GetFeeTypeCascadeDetail(Id);

            //selecting the desired columns
            var subCategoryToReturn = lst.Select(S => new {
                Mst_Value = S.Mst_Value,
                Mst_Desc = S.Mst_Desc
            });
            //returning JSON
            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult LoadFileStatusddl(string Id)
        {
            List<MasterSetups> lst = Helper.GetFileStatusList(Id, false);

            //selecting the desired columns
            var subCategoryToReturn = lst.OrderBy(o => o.DisplaySeq).Select(S => new {
                Mst_Value = S.Mst_Value,
                Mst_Desc = S.Mst_Desc
            });
            //returning JSON
            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);


        }

        [HttpPost]
        public ActionResult GetRegisterCourtDetail(string OfficeFileNo,string ActionLevel)
        {
            CaseRegistrationVM RetResult = new CaseRegistrationVM();

            var courtCase = db.CourtCase.Where(w => w.OfficeFileNo == OfficeFileNo).FirstOrDefault();

            if (courtCase != null)
            {
                var _courtCasesDetail = db.CourtCasesDetail.Where(w => w.CaseId == courtCase.CaseId && w.Courtid == ActionLevel).FirstOrDefault();

                if (_courtCasesDetail != null)
                {
                    RetResult.DetailId = _courtCasesDetail.DetailId;
                    RetResult.CaseId = _courtCasesDetail.CaseId;
                    RetResult.CourtReg_RegNo = _courtCasesDetail.CourtRefNo;
                    RetResult.CourtReg_RegCourt = _courtCasesDetail.CourtLocationid;
                    RetResult.CourtReg_RegDate = _courtCasesDetail.RegistrationDate;
                    RetResult.CourtReg_Regby = _courtCasesDetail.ApealByWho;
                    RetResult.CourtReg_ClaimAmount = courtCase.ClaimAmount;
                }
                else
                    RetResult.DetailId = 0;
            }
            return this.Json(RetResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetCaseDetailByCaseId(string OfficeFileNo)
        {
            PayVoucher payVoucher = new PayVoucher();
            string RegistrationLevel = string.Empty;

            var courtCase = db.CourtCase.Where(w => w.OfficeFileNo == OfficeFileNo).FirstOrDefault();

            if(courtCase != null)
            {
                payVoucher.CaseId = courtCase.CaseId;
                payVoucher.OfficeFileNo = courtCase.OfficeFileNo;
                payVoucher.ClientName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Client && w.Mst_Value == courtCase.ClientCode).FirstOrDefault().Mst_Desc;
                payVoucher.Defendant = courtCase.Defendant;
                payVoucher.AccountContractNo = courtCase.AccountContractNo;
                payVoucher.ClientFileNo = courtCase.ClientFileNo;
                payVoucher.ReceptionDate = courtCase.ReceptionDate?.ToString("dd/MM/yyyy");
                payVoucher.ClaimAmount = courtCase.ClaimAmount;

                var caseRegisteredExists = db.CaseRegistration.Where(w => w.CaseId == courtCase.CaseId).Count();

                if(caseRegisteredExists > 0)
                {
                    if (caseRegisteredExists > 1)
                    {
                        var caseRegistered = db.CaseRegistration.Where(w => w.CaseId == courtCase.CaseId).ToList();
                        RegistrationLevel = string.Format(@"<p>PLEASE NOTE FILE NO {0} IS ALREADY REGISTERED IN FOLLOWING ACTION LEVEL</p>", courtCase.OfficeFileNo);
                        string RegistrationLevelDetail = @"<ul>";

                        foreach (var items in caseRegistered)
                        {
                            string ActionLevelName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.ActionLevel && w.Mst_Value == items.ActionLevel).FirstOrDefault().Mst_Desc;

                            RegistrationLevelDetail += string.Format(@"<li>{0}</li>", ActionLevelName);
                        }
                        RegistrationLevelDetail += @"</ul>";
                        RegistrationLevel += RegistrationLevelDetail;
                    }
                    else
                    {
                        var caseRegistered = db.CaseRegistration.Where(w => w.CaseId == courtCase.CaseId).FirstOrDefault();
                        string ActionLevelName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.ActionLevel && w.Mst_Value == caseRegistered.ActionLevel).FirstOrDefault().Mst_Desc;
                        RegistrationLevel = string.Format(@"PLEASE NOTE FILE NO {0} IS ALREADY REGISTERED IN {1} LEVEL", courtCase.OfficeFileNo, ActionLevelName);
                    }
                }
            }
            payVoucher.RegistrationLevel = RegistrationLevel;

            //returning JSON
            return this.Json(payVoucher, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult GetExistingOfficeInTBR(string OfficeFileNo, string DataForTable)
        {
            try
            {
                string spName = @"[dbo].[REG_IN_PROG-GetExistingCase]";
                List<string> parName = new List<string>() { "@P_OfficeFileNo", "@P_DataForTable" };
                List<object> parValues = new List<object>() { OfficeFileNo, DataForTable };

                DataSet ds = Helper.getDataSet(parName.ToArray(), parValues.ToArray(), TableNames: new string[] { "data", "summary" }, procedureName: spName);
                DataTable dt = ds.Tables["data"];
                DataTable Summarydt = ds.Tables["summary"];

                var caseRegisteredExists = Summarydt.Rows.Count > 0 ? int.Parse(Summarydt.Rows[0]["caseRegisteredExists"].ToString()) : 0;

                var jsondata = dt.ToDictionary();

                return new JsonResult()
                {
                    Data = new { jsondata, caseRegisteredExists, ProcessFlag = "Y" }
                };
                //return Json(new { data = jsondata, caseRegisteredExists = caseRegisteredExists }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new { ProcessMessage = e.Message, ProcessFlag = "N" }
                };
            }

        }
        [HttpPost]
        public ActionResult GetPayVoucherCreatedList()
        {
            var request = HttpContext.Request;
            string OfficeFileNo = string.Empty;
            try
            {
                OfficeFileNo = request.Params["P_OfficeFileNo"].ToString();
            }
            catch (Exception e)
            {

            }

            var ResultList = Helper.GetPayVoucherCreatedVM(OfficeFileNo);

            return Json(ResultList);

        }

        [HttpPost]
        public ActionResult GetEnforcementDetail(int CaseId, string EnforcemetDispute)
        {
            PayVoucher payVoucher = Helper.GetEnforcementDetail(CaseId,EnforcemetDispute);
            return this.Json(payVoucher, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult AjaxIndexDataPV(string DataFor)
        {
            var request = HttpContext.Request;
            List<PVListForIndex> data = new List<PVListForIndex>();

            var start = (Convert.ToInt32(Request["start"]));
            var Length = (Convert.ToInt32(Request["length"])) == 0 ? 10 : (Convert.ToInt32(Request["length"]));
            var searchvalue = Request["search[value]"] ?? "";
            var sortcoloumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            var sortDirection = Request["order[0][dir]"] ?? "asc";
            var recordsTotal = 0;

            try
            {
                if (DataFor == "INTRA")
                {
                    data = Helper.GetPVList(sortcoloumnIndex, start, searchvalue, Length, sortDirection, User.Identity.Name, DataFor).ToList();
                    recordsTotal = data.Count > 0 ? data[0].TotalRecords : 0;
                }
                else
                {
                    DataFor = string.Empty;
                    try
                    {
                        DataFor = request.Params["DataTableName"].ToString();
                        data = Helper.GetPVList(sortcoloumnIndex, start, searchvalue, Length, sortDirection, User.Identity.Name, DataFor).ToList();
                        recordsTotal = data.Count > 0 ? data[0].TotalRecords : 0;

                    }
                    catch (Exception e)
                    {
                    }
                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { data = data, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetSessionRollId(int CaseId, string CaseLevel)
        {
            return new JsonResult()
            {
                Data = new { SessionRollId = Helper.GetSessionRollId(CaseId, CaseLevel) }
            };
        }

        [HttpPost]
        public ActionResult GetRegisterId(int CaseId)
        {
            return new JsonResult()
            {
                Data = new { CaseRegistrationId = Helper.GetRegisterId(CaseId) }
            };
        }

        public ActionResult GetDefendentTransferData()
        {
            var request = HttpContext.Request;
            List<DefendentTransferDTO> data = new List<DefendentTransferDTO>();

            int Userid  = HttpContext.User.Identity.GetUserId();
            int CaseId  = int.Parse(request.Params["CaseId"].ToString());
            string CaseLevel = request.Params["CaseLevel"].ToString();

            data = Helper.GetDefendentTransfer(CaseId, CaseLevel);

            var start = (Convert.ToInt32(Request["start"]));
            var Length = (Convert.ToInt32(Request["length"])) == 0 ? 10 : (Convert.ToInt32(Request["length"]));
            var searchvalue = Request["search[value]"] ?? "";
            var sortcoloumnIndex = Convert.ToInt32(Request["order[0][column]"]);
            var sortDirection = Request["order[0][dir]"] ?? "asc";
            var recordsTotal = 0;
                        
            return Json(new { data = data, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult GetDetailTable()
        {
            var request = HttpContext.Request;
            string DataFor = string.Empty;
            int SessionRollId = 0;

            try {DataFor = request.Params["DataFor"].ToString();} catch (Exception e) {}
            try { SessionRollId = int.Parse(request.Params["SessionRollId"].ToString());} catch (Exception e) {}

            var ResultList = Helper.GetDetailTable(DataFor, SessionRollId);

            if(DataFor == "CASEHIST")
            {
                return Json(ResultList.DataTableToList<CourtDecisionHistoryDTO>());
            }

            return Json(ResultList);
        }

        [HttpPost]
        public ActionResult ProcessDefendentTransferData(DefendentTransferDTO objDTO)
        {
            try
            {
                //if (objDTO.MoneyTrRequestDate.HasValue)
                //{
                //    var courtCasesEnforcement = db.CourtCasesEnforcement.Where(w => w.CaseId == objDTO.CaseId).OrderByDescending(O => O.EnforcementId).FirstOrDefault();
                //    db.Entry(courtCasesEnforcement).Entity.MoneyTrRequestDate = objDTO.MoneyTrRequestDate;
                //    db.Entry(courtCasesEnforcement).State = EntityState.Modified;
                //    db.SaveChanges();
                //}

                objDTO.Userid = HttpContext.User.Identity.GetUserId();
                DataTable _result = Helper.ProcessDefendentTransfer(objDTO);

                string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();


                return new JsonResult()
                {
                    Data = new { ProcessMessage, ProcessFlag }
                };
            }
            catch(Exception e)
            {
                return new JsonResult()
                {
                    Data = new { ProcessMessage = e.Message, ProcessFlag = "N" }
                };
            }
            
        }

        [HttpPost]
        public ActionResult CheckDuplicateDetail(string ValueToSearch, string TypeToSearch)
        {
            try
            {
                string strResult = Helper.checkDuplicateCaseFile(ValueToSearch, TypeToSearch);
                return Json(new { Message = strResult });
            }
            catch (Exception e)
            {
                return new JsonResult()
                {
                    Data = new { Message = "Error", ErrorMessage = e.Message }
                };

            }
        }

        [HttpPost]
        public JsonResult KeepSessionAlive()
        {

            return new JsonResult
            {
                Data = "Beat Generated"
            };
        }
    }
}