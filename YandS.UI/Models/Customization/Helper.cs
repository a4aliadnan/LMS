using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using YandS.DAL;
using YandS.UI.Models;
using System.Xml.Linq;
using System.Data.Entity;

namespace YandS.UI
{
    public static class Helper
    {
        /// <summary>
        ///     Returns Root Upload Folder.
        /// </summary>
        /// <returns></returns>
        public static string GetRootUploadFolder
        {
            get
            {
                string strRootUploadFolder = System.Configuration.ConfigurationManager.AppSettings["UploadRoot"].ToString();
                return strRootUploadFolder;
            }
        }

        /// <summary>
        ///     Returns Storage Root Folder.
        /// </summary>
        /// <returns></returns>
        public static string GetStorageRoot
        {
            get
            {
                string strStorageRoot = System.Web.Hosting.HostingEnvironment.MapPath("~/Uploads");
                return strStorageRoot;
            }
        }

        /// <summary>
        ///     Returns Excel Template Root Folder.
        /// </summary>
        /// <returns></returns>
        public static string GetTemplateRoot
        {
            get
            {
                string strStorageRoot = System.Web.Hosting.HostingEnvironment.MapPath("~/Template");
                return strStorageRoot;
            }
        }

        public static string GetUserImageRoot
        {
            get
            {
                string strStorageRoot = System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
                return strStorageRoot;
            }
        }

        /// <summary>
        ///     Returns ConnectionString
        /// </summary>
        /// <returns></returns>
        public static string ConnectionString
        {
            get
            {
                string strConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                return strConnectionString;
            }
        }
        public static string NumberToWords(double doubleNumber)
        {
            var beforeFloatingPoint = (int)Math.Floor(doubleNumber);
            var beforeFloatingPointWord = $"{NumberToWords(beforeFloatingPoint)} RIAL OMANI";
            var chkZeroBaisa = (doubleNumber - beforeFloatingPoint) * 1000;

            if (chkZeroBaisa > 0)
            {
                var afterFloatingPointWord = $"{NumberToWords(Convert.ToInt32((doubleNumber - beforeFloatingPoint) * 1000))} BAIZA";
                return $"{beforeFloatingPointWord.ToUpper()} and {afterFloatingPointWord.ToUpper()}";
            }
            else
            {
                return beforeFloatingPointWord.ToUpper();

            }
        }
        private static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            var words = "";

            if (number / 1000000000 > 0)
            {
                words += NumberToWords(number / 1000000000) + " billion ";
                number %= 1000000000;
            }

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            words = SmallNumberToWord(number, words);

            return words;
        }
        private static string SmallNumberToWord(int number, string words)
        {
            if (number <= 0) return words;
            if (words != "")
                words += " ";

            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
            return words;
        }
        public static string GetVoucherDoc(int Voucher_No)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "PVDocuments");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(Voucher_No + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetPVTranferDoc(int Voucher_No)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "PVTransfers");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(Voucher_No + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetInvoiceDoc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "INVDocuments");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetOmanPostDoc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "OmanPost");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetCaseAgreementDoc(int id)
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
        public static string GetDEF_Address_Doc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "DEF_Address_Docs");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetDEF_Lawyer_Doc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "DEF_Lawyer_Docs");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static string GetStopRegEmails_Doc(int id)
        {
            string ReturnResult = string.Empty;

            string UploadRoot = GetStorageRoot;

            string UploadPath = Path.Combine(UploadRoot, "StopRegEmails");

            DirectoryInfo d = new DirectoryInfo(UploadPath);
            var Image = d.GetFiles(id + ".*").OrderByDescending(f => f.FullName).FirstOrDefault();

            if (Image == null)
                ReturnResult = "#";
            else
                ReturnResult = Image.ToString();
            return ReturnResult;
        }
        public static List<MasterSetups> GetFileStatusList(string ddlfor, bool forMainIndex = true)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ResultList = new List<MasterSetups>();

            switch (ddlfor)
            {
                case "N":
                    {
                        ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Remarks.Substring(0, 1) == ddlfor && m.Active_Flag == true).ToList();
                        break;
                    }
                case "A":
                    {
                        ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Remarks.Substring(1, 1) == ddlfor && m.Active_Flag == true).ToList();
                        break;
                    }
                case "S":
                    {
                        ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Remarks.Substring(2, 1) == ddlfor && m.Active_Flag == true).ToList();
                        break;
                    }
                case "E":
                    {
                        ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Remarks.Substring(3, 1) == ddlfor && m.Active_Flag == true).ToList();
                        break;
                    }
                default:
                    {
                        ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Active_Flag == true).ToList();
                        break;
                    }
            }

            MasterSetups FileStatus = new MasterSetups();
            if (forMainIndex)
            {
                FileStatus.Mst_Desc = "ALL";
                FileStatus.Mst_Value = "A";
                FileStatus.DisplaySeq = 0;
                
            }
            else
            {
                FileStatus.Mst_Desc = "PLEASE SELECT";
                FileStatus.Mst_Value = "0";
                FileStatus.DisplaySeq = 0;
            }

            ResultList.Add(FileStatus);

            return ResultList;
        }
        public static List<MasterSetups> GetCourtLocationList(string CourtId)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ResultList = new List<MasterSetups>();

            //string[] values = null;

            //if (CourtId.Equals("1")) //Primary
            //    values = new[] { "P", "B" };
            //else if (CourtId.Equals("2")) //Apeal
            //    values = new[] { "B" };
            //                ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location && values.Contains(m.Remarks)).OrderBy(o => o.DisplaySeq).ToList();

            if (CourtId.Equals("1") || CourtId.Equals("2") || CourtId.Equals("4")) //Primary //Apeal
                ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (CourtId.Equals("3")) //Supreme
                ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Location && m.Mst_Value == "1" && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();


            return ResultList;
        }
        public static List<MasterSetups> GetNextCaseLevel()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups NextCaseLevel = new MasterSetups();
            NextCaseLevel.Mst_Desc = "PLEASE SELECT";
            NextCaseLevel.Mst_Value = "";
            NextCaseLevel.DisplaySeq = 0;
            ResultList.Add(NextCaseLevel);

            NextCaseLevel = new MasterSetups();
            NextCaseLevel.Mst_Desc = "PRIMARY COURT";
            NextCaseLevel.Mst_Value = "PRIMARY COURT";
            NextCaseLevel.DisplaySeq = 1;
            ResultList.Add(NextCaseLevel);

            NextCaseLevel = new MasterSetups();
            NextCaseLevel.Mst_Desc = "APPEAL COURT";
            NextCaseLevel.Mst_Value = "APPEAL COURT";
            NextCaseLevel.DisplaySeq = 2;
            ResultList.Add(NextCaseLevel);

            NextCaseLevel = new MasterSetups();
            NextCaseLevel.Mst_Desc = "SUPREME COURT";
            NextCaseLevel.Mst_Value = "SUPREME COURT";
            NextCaseLevel.DisplaySeq = 3;
            ResultList.Add(NextCaseLevel);

            NextCaseLevel = new MasterSetups();
            NextCaseLevel.Mst_Desc = "ENFORCEMENT";
            NextCaseLevel.Mst_Value = "ENFORCEMENT";
            NextCaseLevel.DisplaySeq = 4;
            ResultList.Add(NextCaseLevel);

            return ResultList;
        }
        public static List<MasterSetups> GetListForPayTo()
        {
            using (var context = new RBACDbContext())
            {
                List<MasterSetups> ResultList = new List<MasterSetups>();
                ResultList = context.Database.SqlQuery<MasterSetups>("GetPayToListForPV").ToList();
                return ResultList;
            }
        }
        public static List<MasterSetups> GetBankList()
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();

            var LinkBankAccoutListCR = db.LinkBankAccounts.ToList();
            ReturnResult = (from BA in LinkBankAccoutListCR
                            join bank_mas in db.Set<MasterSetups>()
                           on BA.BankId equals bank_mas.MstId
                            join account_mas in db.Set<MasterSetups>()
                                on BA.AccountId equals account_mas.MstId
                            select new MasterSetups
                            {
                                Mst_Value = BA.LinkId.ToString(),
                                Mst_Desc = string.Format(@"{0} : {1}", bank_mas.Mst_Desc, BA.AccountNumber)

                            }).ToList();
            //.Mst_Desc.ToUpper().Contains("SALALAH") ? "S" : "M"
            return ReturnResult;

        }
        public static List<MasterSetups> GetBankList(string Location)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();
            List<MasterSetups> ReturnResult1 = new List<MasterSetups>();
            MasterSetups PleaseSelect = new MasterSetups();
            PleaseSelect.Mst_Value = "0";
            PleaseSelect.Mst_Desc = "PLEASE SELECT";


            var LinkBankAccoutListCR = db.LinkBankAccounts.ToList();
            ReturnResult1 = (from BA in LinkBankAccoutListCR
                             join bank_mas in db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Banks)
                            on BA.BankId equals bank_mas.MstId
                             join account_mas in db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.AccoutTitle && m.Remarks == Location)
                                 on BA.AccountId equals account_mas.MstId
                             select new MasterSetups
                             {
                                 Mst_Value = BA.LinkId.ToString(),
                                 Mst_Desc = string.Format(@"{0} : {1}", bank_mas.Mst_Desc, BA.AccountNumber)

                             }).ToList();
            //.Mst_Desc.ToUpper().Contains("SALALAH") ? "S" : "M"
            ReturnResult.Add(PleaseSelect);
            ReturnResult.AddRange(ReturnResult1);


            return ReturnResult;

        }
        public static List<MasterSetups> GetCaseLevelList()
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();
            //string[] values = new[] { "0","1", "2", "3", "4", "5", "6" };

            //ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && values.Contains(m.Mst_Value)).OrderBy(o => o.DisplaySeq).ToList();
            ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel).OrderBy(o => o.DisplaySeq).ToList();

            return ReturnResult;

        }
        public static List<MasterSetups> LoadPayFor(string P_Remark)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();

            if (P_Remark == "R")
            {
                P_Remark = "1";
                var ResultCount = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FeeTypeCascadeDetail && m.Remarks == P_Remark).Count();

                if (ResultCount > 0)
                {
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FeeTypeCascadeDetail && m.Remarks == P_Remark && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                }
            }
            else
            {
                var ResultCount = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.PaymentHeads && m.Remarks == P_Remark).Count();

                if (ResultCount > 0)
                {
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.PaymentHeads && m.Remarks == P_Remark && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                }
            }
            return ReturnResult;
        }
        public static List<MasterSetups> GetCaseLevelList(string ItemFor)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();

            switch (ItemFor)
            {
                case "A":
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(0, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                case "B":
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(1, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                case "C":
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(2, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                case "D":
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(3, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                case "S":
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(4, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                case "TBR":
                    string[] StatusCodes = new[] { "0", "3", "4", "5" };
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && StatusCodes.Contains(m.Mst_Value) && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
                default:
                    ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Mst_Value == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
                    break;
            }

            /*
            if (ItemFor == "A")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(0, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (ItemFor == "B")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(1, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (ItemFor == "C")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(2, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (ItemFor == "D")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(3, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (ItemFor == "S")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(4, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else if (ItemFor == "TBR")
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Remarks.Substring(4, 1) == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else
                ReturnResult = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseLevel && m.Mst_Value == ItemFor && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            */

            return ReturnResult;

        }
        public static List<MasterSetups> GetPVList()
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();
            MasterSetups PleaseSelect = new MasterSetups();
            PleaseSelect.Mst_Value = "";
            PleaseSelect.Mst_Desc = "Please Select";
            ReturnResult.Add(PleaseSelect);

            var ListPV = db.PayVouchers.Where(w => w.PV_No != null && w.VoucherType == "1").ToList();
            var PVList = (from PV in ListPV
                          select new MasterSetups
                          {
                              Mst_Value = PV.PV_No,
                              Mst_Desc = PV.PV_No

                          }).ToList();

            ReturnResult.AddRange(PVList);
            return ReturnResult;

        }
        public static List<MasterSetups> GetFeeTypeCascadeDetail(string Id)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();
            MasterSetups PleaseSelect = new MasterSetups();

            if (Id == "1" || Id == "9")
            {
                PleaseSelect.Mst_Value = "";
                PleaseSelect.Mst_Desc = "PLEASE SELECT";
                ReturnResult.Add(PleaseSelect);

                var objFeeTypeCascade = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FeeTypeCascadeDetail && m.Remarks == Id).ToList();
                ReturnResult.AddRange(objFeeTypeCascade);
            }
            else
            {
                PleaseSelect.Mst_Value = "";
                PleaseSelect.Mst_Desc = "";
                ReturnResult.Add(PleaseSelect);

            }

            return ReturnResult;

        }
        public static List<MasterSetups> GetSessionLevel()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionLevel && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetSessionCaseType()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionCaseType && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetSessionLawyers(bool ForEnforcement = false)
        {
            return GetCommonNameList();
            //RBACDbContext db = new RBACDbContext();
            //string[] FilterCode = null;

            //if (ForEnforcement)
            //    FilterCode = new[] { "1", "3" };
            //else
            //    FilterCode = new[] { "1", "2" };

            //return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionLawyers && m.Active_Flag == true && FilterCode.Contains(m.Remarks)).OrderBy(o => o.DisplaySeq).ToList();

        }
        public static List<MasterSetups> GetSessionFollowers()
        {
            return GetCommonNameList();
        }
        public static List<MasterSetups> GetSessionClients()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionClients && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetStatusCodeList(bool ALL = false, string[] StatusCodes = null)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ResultList = new List<MasterSetups>();

            if (ALL)
                ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseStatus && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else
            {
                ResultList = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseStatus && m.Active_Flag == true && StatusCodes.Contains(m.Mst_Value)).OrderBy(o => o.DisplaySeq).ToList();
            }

            return ResultList;
        }
        public static List<MasterSetups> GetReconciliationDeptt()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ReconciliationDeptt && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetGovernorate()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Governorate && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetEnfcAdmin()
        {
            return GetCommonNameList();
        }
        public static List<MasterSetups> GetCurEnfcLevel()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.EnforcementLevel && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetCaseClosingReasons()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileCloseReason && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetYesNoForSelect()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "PLEASE SELECT";
            YesNo.Mst_Value = "0";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "NO لا";
            YesNo.Mst_Value = "1";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "YES نعم";
            YesNo.Mst_Value = "2";
            YesNo.DisplaySeq = 2;
            ResultList.Add(YesNo);

            return ResultList;
        }
        public static List<MasterSetups> GetJudgementIssued()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "PLEASE SELECT";
            YesNo.Mst_Value = "0";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "FINAL JUG. حكم نهائي";
            YesNo.Mst_Value = "Y";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "TRANSFER إحالة لهيئة مغايرة";
            YesNo.Mst_Value = "N";
            YesNo.DisplaySeq = 2;
            ResultList.Add(YesNo);

            return ResultList;
        }
        public static List<MasterSetups> GetByWho(bool forSupreme = false)
        {
            RBACDbContext db = new RBACDbContext();
            //string[] FilterCode = null;

            if (forSupreme)
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ApealByWho && m.Active_Flag == true && m.Remarks.Contains("5")).OrderBy(o => o.DisplaySeq).ToList();
            else
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.ApealByWho && m.Active_Flag == true && m.Remarks == "3456").OrderBy(o => o.DisplaySeq).ToList();

        }
        public static List<MasterSetups> GetCaseAgainst()
        {
            //db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst)

            /*
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> ReturnResult = new List<MasterSetups>();
            MasterSetups PleaseSelect = new MasterSetups();
            PleaseSelect.Mst_Value = "0";
            PleaseSelect.DisplaySeq = 0;
            PleaseSelect.Mst_Desc = "PLEASE SELECT";
            ReturnResult.Add(PleaseSelect);

            var ListCaseAgainst = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CaseAgainst && m.Mst_Value != "0" && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            var CaseAgainstList = (from CaseAgainst in ListCaseAgainst
                          select new MasterSetups
                          {
                              Mst_Value = CaseAgainst.Mst_Value,
                              Mst_Desc = CaseAgainst.Mst_Desc.Trim().Replace("AGAINST", "")

                          }).ToList();

            ReturnResult.AddRange(CaseAgainstList);
            return ReturnResult;
            */

            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "INDIVIDUAL &  COMPANY";
            YesNo.Mst_Value = "0";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "INDIVIDUAL ";
            YesNo.Mst_Value = "1";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "COMPANY ";
            YesNo.Mst_Value = "2";
            YesNo.DisplaySeq = 2;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "CLIENT ";
            YesNo.Mst_Value = "3";
            YesNo.DisplaySeq = 3;
            ResultList.Add(YesNo);

            return ResultList;
            
        }
        public static CourtCaseDTView GetCaseList(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserLocation, string DataFor, string CaseLevel, DateTime DateFrom, DateTime DateTo, string CallerName)
        {
            CourtCaseDTView ReturnResult = new CourtCaseDTView();

            var SortColumn = "";

            try
            {
                switch (sortcoloumnIndex)
                {
                    case 0:
                        SortColumn = "CC.OfficeFileNo";
                        break;
                    case 1:
                        SortColumn = "ClientMas.Mst_Desc";
                        break;
                    case 2:
                        SortColumn = "CC.Defendant";
                        break;
                    case 3:
                        SortColumn = "AgainstMas.Mst_Desc";
                        break;
                    case 4:
                        SortColumn = "CC.ReceptionDate";
                        break;
                    case 5:
                        SortColumn = "CaseTypeMas.Mst_Desc";
                        break;
                    case 6:
                        SortColumn = "CaseLevelMas.Mst_Desc";
                        break;
                    case 7:
                        SortColumn = "CaseStatusMas.Mst_Desc";
                        break;
                    case 8:
                        SortColumn = "CC.ClientFileNo";
                        break;
                    case 9:
                        SortColumn = "CC.AccountContractNo";
                        break;
                    default:
                        SortColumn = "CC.OfficeFileNo";
                        break;
                }


                ReturnResult = new ReportCourtCases().getCourtCaseListWithPaging(start, searchvalue, Length, SortColumn, sortDirection, UserLocation.ToUpper().Substring(0, 1), DataFor, CaseLevel, DateFrom, DateTo, CallerName);
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<CourtCaseListForIndex> GetCaseListInvoice(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserLocation)
        {
            List<CourtCaseListForIndex> ReturnResult = new List<CourtCaseListForIndex>();

            var SortColumn = "";

            try
            {
                switch (sortcoloumnIndex)
                {
                    case 0:
                        SortColumn = "CC.OfficeFileNo";
                        break;
                    case 1:
                        SortColumn = "ClientMas.Mst_Desc";
                        break;
                    case 2:
                        SortColumn = "CC.Defendant";
                        break;
                    case 3:
                        SortColumn = "AgainstMas.Mst_Desc";
                        break;
                    case 4:
                        SortColumn = "CC.ReceptionDate";
                        break;
                    case 5:
                        SortColumn = "CaseTypeMas.Mst_Desc";
                        break;
                    case 6:
                        SortColumn = "CaseLevelMas.Mst_Desc";
                        break;
                    case 7:
                        SortColumn = "CaseStatusMas.Mst_Desc";
                        break;
                    case 8:
                        SortColumn = "CC.ClientFileNo";
                        break;
                    case 9:
                        SortColumn = "CC.AccountContractNo";
                        break;
                    default:
                        SortColumn = "CC.OfficeFileNo";
                        break;
                }


                ReturnResult = new ReportCourtCases().getCourtCaseListInvoiceWithPaging(start, searchvalue, Length, SortColumn, sortDirection, UserLocation.ToUpper().Substring(0, 1)).ToList();
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<CaseInvoiceList> GetInvoiceList(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserLocation, int CaseId = 0)
        {
            List<CaseInvoiceList> ReturnResult = new List<CaseInvoiceList>();

            var SortColumn = "";

            try
            {
                switch (sortcoloumnIndex)
                {
                    case 0:
                        SortColumn = "OfficeFileNo";
                        break;
                    case 1:
                        SortColumn = "CourtLevelDisp";
                        break;
                    case 2:
                        SortColumn = "InvoiceNumber";
                        break;
                    case 3:
                        SortColumn = "InvoiceDate";
                        break;
                    case 4:
                        SortColumn = "InvoiceStatus";
                        break;
                    case 5:
                        SortColumn = "InvoiceAmount";
                        break;
                    case 6:
                        SortColumn = "ClientName";
                        break;
                    case 7:
                        SortColumn = "AccountContractNo";
                        break;
                    case 8:
                        SortColumn = "ClientFileNo";
                        break;
                    default:
                        SortColumn = "OfficeFileNo";
                        break;
                }


                ReturnResult = new ReportCourtCases().getCaseInvoiceList(start, searchvalue, Length, SortColumn, sortDirection, UserLocation.ToUpper().Substring(0, 1), CaseId).ToList();
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<PVListForIndex> GetPVList(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserName,string DataFor)
        {
            List<PVListForIndex> ReturnResult = new List<PVListForIndex>();

            var SortColumn = "";

            try
            {
                if (DataFor == "REF")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.PV_No";
                            break;
                        case 3:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 4:
                            SortColumn = "Clients.Mst_Desc";
                            break;
                        case 5:
                            SortColumn = "CC.Defendant";
                            break;
                        case 6:
                            SortColumn = "CaseLevel.Mst_Desc";
                            break;
                        case 7:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 8:
                            SortColumn = "pv.Remarks";
                            break;
                        case 9:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 10:
                            SortColumn = "pv.Amount";
                            break;
                        case 11:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 12:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 13:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 14:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "NONREF")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.PV_No";
                            break;
                        case 3:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "pv.Remarks";
                            break;
                        case 5:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "pv.Amount";
                            break;
                        case 7:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 8:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 9:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 10:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "REFAPR")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.PV_No";
                            break;
                        case 3:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 4:
                            SortColumn = "Clients.Mst_Desc";
                            break;
                        case 5:
                            SortColumn = "CC.Defendant";
                            break;
                        case 6:
                            SortColumn = "CaseLevel.Mst_Desc";
                            break;
                        case 7:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 8:
                            SortColumn = "pv.Remarks";
                            break;
                        case 9:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 10:
                            SortColumn = "pv.Amount";
                            break;
                        case 11:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 12:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 13:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 14:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "NONREFAPR")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.PV_No";
                            break;
                        case 3:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "pv.Remarks";
                            break;
                        case 5:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "pv.Amount";
                            break;
                        case 7:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 8:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 9:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 10:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "PDC")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 1:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        case 2:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "pv.Remarks";
                            break;
                        case 5:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "pv.Amount";
                            break;
                        case 7:
                            SortColumn = "DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate)";
                            break;
                        default:
                            SortColumn = "Emp.LocationCode";
                            break;
                    }

                }
                else if (DataFor == "REFREJ")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.Payment_Head_Remarks";
                            break;
                        case 3:
                            SortColumn = "pv.PV_No";
                            break;
                        case 4:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 5:
                            SortColumn = "Clients.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "CC.Defendant";
                            break;
                        case 7:
                            SortColumn = "CaseLevel.Mst_Desc";
                            break;
                        case 8:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 9:
                            SortColumn = "pv.Remarks";
                            break;
                        case 10:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 11:
                            SortColumn = "pv.Amount";
                            break;
                        case 12:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 13:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 14:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 15:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "NONREFREJ")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 1:
                            SortColumn = "Emp.LocationCode";
                            break;
                        case 2:
                            SortColumn = "pv.Payment_Head_Remarks";
                            break;
                        case 3:
                            SortColumn = "pv.PV_No";
                            break;
                        case 4:
                            SortColumn = "PaymentHead.Mst_Desc";
                            break;
                        case 5:
                            SortColumn = "pv.Remarks";
                            break;
                        case 6:
                            SortColumn = "PaymentTo.Mst_Desc";
                            break;
                        case 7:
                            SortColumn = "pv.Amount";
                            break;
                        case 8:
                            SortColumn = "pv.BillNumber";
                            break;
                        case 9:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        case 10:
                            SortColumn = "BA.Mst_Desc";
                            break;
                        case 11:
                            SortColumn = "pv.Cheque_Date";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }

                }
                else if (DataFor == "INTRA")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "pv.PV_No";
                            break;
                        case 1:
                            SortColumn = "pv.Voucher_Date";
                            break;
                        case 2:
                            SortColumn = "ReasonCode.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "VoucherStatus.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "pv.Amount";
                            break;
                        case 5:
                            SortColumn = "payMode.Mst_Desc";
                            break;
                        default:
                            SortColumn = "pv.PV_No";
                            break;
                    }
                }

                ReturnResult = new ReportCourtCases().getPVListForIndexWithPaging(start, searchvalue, Length, SortColumn, sortDirection, UserName, DataFor).ToList();
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static PayVoucherDTView GetPVDTView(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserName,string DataFor)
        {
            PayVoucherDTView ReturnResult = new PayVoucherDTView();
            var SortColumn = "";

            try
            {
                if (DataFor == "PENDING")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 1, 12, 2, 3, 4, 5, 7, 9, 8, 10, 11 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "REFAPPROVED")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 13, 3, 4, 5, 6, 7, 8, 9, 11, 10, 19, 17, 15 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "NRAPPROVED")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 13, 7, 8, 9, 11, 10, 19, 17, 15 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "NEWPDC")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 14, 2, 16, 17, 7, 9, 10, 11 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "REJECT")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 12, 20, 3, 6, 7, 9, 11 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }

                ReturnResult = new ReportCourtCases().getPayVoucherDataViewForNewIndex(start, searchvalue, Length, SortColumn, sortDirection, UserName, DataFor);
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<InvoiceNotificationDTView> GetInvoiceNotificationDTView(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserName,string DataFor,string LocationId)
        {
            List<InvoiceNotificationDTView> ReturnResult = new List<InvoiceNotificationDTView>();
            var SortColumn = "";

            try
            {
                if (DataFor == "PV-TABLE")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 1, 12, 2, 3, 4, 5, 7, 9, 8, 10, 11 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "CASE-TABLE")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 13, 3, 4, 5, 6, 7, 8, 9, 11, 10, 19, 17, 15 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "TRANS-TABLE")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 13, 3, 4, 5, 6, 7, 8, 9, 11, 10, 19, 17, 15 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if(DataFor == "INV-TABLE")
                {
                    //{0,	1,	2,	3,	4,	5,	6,	7,	8,	9,	10}
                    //{"DaysCounter",	"VoucherTypeName",	"LocationCode",	"OfficeFileNo",	"ClientName",	"Defendant",	"PaymentHeadName",	"PaymentToName",	"Remarks",	"BillNumber",	Amount}

                    int[] qryColumn = { 2, 18, 13, 3, 4, 5, 6, 7, 8, 9, 11, 10, 19, 17, 15 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                

                ReturnResult = new ReportCourtCases().getInvoiceNotificationDTView(start, searchvalue, Length, SortColumn, sortDirection, UserName, DataFor, LocationId);
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<CaseRegistrationListForIndex> GetCaseRegistrationList(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserName,string DataFor, string LocationId, string FileStatus)
        {
            List<CaseRegistrationListForIndex> ReturnResult = new List<CaseRegistrationListForIndex>();

            var SortColumn = "";

            try
            {
                if (DataFor == "URGENT")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "CC.Defendant";
                            break;
                        case 5:
                            SortColumn = "CR.JudgementDate";
                            break;
                        case 6:
                            SortColumn = "ActLevel.Mst_Desc";
                            break;
                        case 7:
                            SortColumn = "EnfDispute.Mst_Desc";
                            break;
                        case 8:
                            SortColumn = "FileStatus.Mst_Desc";
                            break;
                        case 9:
                            SortColumn = "case when CR.FileStatus = '3' then CR.FileStatusRemarks end";
                            break;
                        case 10:
                            SortColumn = "CR.CourtMessage";
                            break;
                        case 11:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "SUPREME")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "CC.Defendant";
                            break;
                        case 4:
                            SortColumn = "CR.JudgementDate";
                            break;
                        case 5:
                            SortColumn = "FileStatus.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "CR.ActionDate";
                            break;
                        case 7:
                            SortColumn = "case when CR.FileStatus = '3' then CR.FileStatusRemarks end";
                            break;
                        case 8:
                            SortColumn = "CR.CourtMessage";
                            break;
                        case 9:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "APPEAL")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "CC.Defendant";
                            break;
                        case 5:
                            SortColumn = "CR.JudgementDate";
                            break;
                        case 6:
                            SortColumn = "FileStatus.Mst_Desc";
                            break;
                        case 7:
                            SortColumn = "CR.ActionDate";
                            break;
                        case 8:
                            SortColumn = "case when CR.FileStatus = '3' then CR.FileStatusRemarks end";
                            break;
                        case 9:
                            SortColumn = "CR.CourtMessage";
                            break;
                        case 10:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        case 11:
                            SortColumn = "case when CR.FileStatus = '7' then CAST(CR.FileStatusRemarks AS DECIMAL(10,3) end";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "NEWCASE")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "CC.Defendant";
                            break;
                        case 5:
                            SortColumn = "FileStatus.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "CR.ActionDate";
                            break;
                        case 7:
                            SortColumn = "case when CR.FileStatus = '3' then CR.FileStatusRemarks end";
                            break;
                        case 8:
                            SortColumn = "CR.CourtMessage";
                            break;
                        case 9:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        case 10:
                            SortColumn = "case when CR.FileStatus = '7' then CAST(CR.FileStatusRemarks AS DECIMAL(10,3) end";
                            break;
                        case 11:
                            SortColumn = "CC.ReceptionDate";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "PAYMENT")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "CC.Defendant";
                            break;
                        case 5:
                            SortColumn = "FileStatus.Mst_Desc";
                            break;
                        case 6:
                            SortColumn = "CR.ActionDate";
                            break;
                        case 7:
                            SortColumn = "case when CR.FileStatus = '3' then CR.FileStatusRemarks end";
                            break;
                        case 8:
                            SortColumn = "CR.CourtMessage";
                            break;
                        case 9:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        case 10:
                            SortColumn = "case when CR.FileStatus = '7' then CAST(CR.FileStatusRemarks AS DECIMAL(10,3) end";
                            break;
                        case 11:
                            SortColumn = "CC.ReceptionDate";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "ENF-PRIMARY")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "ENF-APPEAL")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "TOBEREG")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 2:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 3:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 4:
                            SortColumn = "CC.Defendant";
                            break;
                        case 5:
                            SortColumn = "CC.ReceptionDate";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }
                else if (DataFor == "STOPREG")
                {
                    switch (sortcoloumnIndex)
                    {
                        case 0:
                            SortColumn = "DATEDIFF(DAY, CR.JudgementDate, GETDATE())";
                            break;
                        case 1:
                            SortColumn = "Courts.Mst_Desc";
                            break;
                        case 2:
                            SortColumn = "case when CR.FileStatus = '6' then CR.FileStatusRemarks end";
                            break;
                        case 3:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                        case 4:
                            SortColumn = "ClientMas.Mst_Desc";
                            break;
                        case 5:
                            SortColumn = "CC.Defendant";
                            break;
                        case 6:
                            SortColumn = "case when CR.FileStatus = '10' then CR.FileStatusRemarks end";
                            break;
                        case 7:
                            SortColumn = "CR.CourtMessage";
                            break;
                        default:
                            SortColumn = "CC.OfficeFileNo";
                            break;
                    }

                }

                ReturnResult = new ReportCourtCases().getCaseRegistrationListForIndexWithPaging(start, searchvalue, Length, SortColumn, sortDirection, UserName, DataFor, LocationId, FileStatus).ToList();
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static List<SessionRollListForIndex> GetSessionRollList(int sortcoloumnIndex, int start, string searchvalue, int Length, string sortDirection, string UserName,string DataFor, string LocationId, string SessionLevel, DateTime DateFrom, DateTime DateTo, string CountLocationId, string LawyerId, string ClientCode)
        {
            List<SessionRollListForIndex> ReturnResult = new List<SessionRollListForIndex>();

            var SortColumn = "";

            try
            {
                if (DataFor == "ALLSESSIONS")
                {
                    int[] qryColumn = { 1, 2, 3, 4, 5, 6, 7 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "PRINT")
                {
                    int[] qryColumn = { 1, 2, 3, 4, 9, 5, 6, 7, 11, 12 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "FOLLOW")
                {
                    int[] qryColumn = { 1, 2, 14, 6, 7, 5, 8, 15, 19 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "SUSPENDED")
                {
                    int[] qryColumn = { 1, 2, 14, 6, 7, 5, 8, 15, 19 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "TOBEUPDATE")
                {
                    int[] qryColumn = { 1, 20, 2, 5, 6, 7, 8 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "JUDGEMENT")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "DIFFERENTPANELS")
                {
                    int[] qryColumn = { 1, 23, 3, 5, 6, 24 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "SESONCOMPLETE")
                {
                    int[] qryColumn = { 1, 2, 3, 4, 5, 6, 7 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "SESONONHOLD")
                {
                    int[] qryColumn = { 1, 25, 3, 4, 5, 6, 7 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "JUDGFOLLOW")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "JUDGCORR")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "BLUESTAMP")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "PROVTIME")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "REDSTAMP")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "CLIENTAPPROV")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "MISSINGDOCS")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }
                else if (DataFor == "REFUND34")
                {
                    int[] qryColumn = { 1, 21, 3, 5, 6, 7, 22 };
                    SortColumn = qryColumn[sortcoloumnIndex].ToString();
                }

                ReturnResult = new ReportCourtCases().getSessionRollForIndexWithPaging(start, searchvalue, Length, SortColumn, sortDirection, UserName, DataFor, LocationId, SessionLevel, DateFrom, DateTo, CountLocationId, LawyerId, ClientCode).ToList();
            }
            catch (Exception ex)
            {
            }
            return ReturnResult;

        }
        public static int GetTableTotal(string DataFor, string LocationId)
        {
            return new ReportCourtCases().getTableTotal(DataFor, LocationId); 
        }
        public static List<PetrolKMDetailVM> GetPetrolKMDetailVM(string PaymentHead, string PayTo)
        {
            var ReturnResult = new ReportCourtCases().getPetrolKMDetailVM(PaymentHead, PayTo);
            
            return ReturnResult;

        }
        public static List<PayVoucherCreatedVM> GetPayVoucherCreatedVM(string OfficeFileNo)
        {
            var ReturnResult = new ReportCourtCases().getPayVoucherCreatedVM(OfficeFileNo);
            
            return ReturnResult;

        }
        public static List<FeeDetailView> GetFeeDetailView(int InvoiceId)
        {
            var ReturnResult = new ReportCourtCases().getFeeDetailView(InvoiceId);
            
            return ReturnResult;

        }
        public static List<CaseInvoiceDetail> GetCaseInvoiceDetail(int Caseid)
        {
            var ReturnResult = new ReportCourtCases().getCaseInvoiceDetail(Caseid);
            
            return ReturnResult;

        }
        public static List<ClosureFormVM> GetClosureFormDetail(int Caseid, string CaseLevel,string EmployeeNumber)
        {
            var ReturnResult = new ReportCourtCases().getClosureFormDetail(Caseid,CaseLevel, EmployeeNumber);
            
            return ReturnResult;

        }
        public static string GenerateINVNumber(string LocationCode)
        {
            string INV_Number = string.Empty;
            using (var context = new RBACDbContext())
            {
                var p_LocationCode = new SqlParameter { ParameterName = "@LocationCode", Value = LocationCode };
                INV_Number = context.Database.SqlQuery<string>("SELECT dbo.FnGenerateINV_Number(@LocationCode)", p_LocationCode).FirstOrDefault();
            }
            return INV_Number;
        }
        public static string GetEmployeeLocation(string UserName)
        {
            RBACDbContext db = new RBACDbContext();
            return db.Employees.Where(w => w.EmployeeNumber == UserName).FirstOrDefault().LocationCode;
        }
        public static PayVoucher GetPayVoucherById(int VoucherNo)
        {

            DataTable dt = new ReportCourtCases().getPVList("A", VoucherNo);


            var ReturnResult = dt.ToObject<PayVoucher>();
        

            return ReturnResult;

        }
        public static PayVoucher GetEnforcementDetail(int CaseId, string EnforcemetDispute)
        {

            PayVoucher payVoucher = new PayVoucher();
            string RegistrationLevel = string.Empty;
            RBACDbContext db = new RBACDbContext();

            var ObjEnforcementDetail = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).FirstOrDefault();

            if (ObjEnforcementDetail == null)
            {
                payVoucher.EnforcementNo = RegistrationLevel;
                payVoucher.EnforcementCourt = RegistrationLevel;
            }
            else
            {
                payVoucher.EnforcementNo = ObjEnforcementDetail.EnforcementNo;
                payVoucher.EnforcementCourt = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == ObjEnforcementDetail.CourtLocationid).FirstOrDefault().Mst_Desc;


                /*
                 * PRIMARY  PLAINT	1
                 * APPEAL PLAINT	2
                 * PRIMARY OPJECTION	3
                 * APPEAL OPJECTION	4
                 

                if (EnforcemetDispute == "1")
                {
                    if (ObjEnforcementDetail.PrimaryPlaintCourt != "0")
                        payVoucher.EnforcementCourt = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == ObjEnforcementDetail.PrimaryPlaintCourt).FirstOrDefault().Mst_Desc;
                    else
                        payVoucher.EnforcementCourt = "";
                }

                if (EnforcemetDispute == "2")
                {
                    if (ObjEnforcementDetail.ApealPlaintCourt != "0")
                        payVoucher.EnforcementCourt = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == ObjEnforcementDetail.ApealPlaintCourt).FirstOrDefault().Mst_Desc;
                    else
                        payVoucher.EnforcementCourt = "";

                }

                if (EnforcemetDispute == "3")
                {
                    if (ObjEnforcementDetail.PrimaryObjectionCourt != "0")
                        payVoucher.EnforcementCourt = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == ObjEnforcementDetail.PrimaryObjectionCourt).FirstOrDefault().Mst_Desc;
                    else
                        payVoucher.EnforcementCourt = "";

                }

                if (EnforcemetDispute == "4")
                {
                    if (ObjEnforcementDetail.ApealObjectionCourt != "0")
                        payVoucher.EnforcementCourt = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == ObjEnforcementDetail.ApealObjectionCourt).FirstOrDefault().Mst_Desc;
                    else
                        payVoucher.EnforcementCourt = "";

                }
                */
            }
            return payVoucher;

        }
        public static List<PayVoucher> GetPayVoucherList(string LocationName, int VoucherNo)
        {

            DataTable dt = new ReportCourtCases().getPVList(LocationName, VoucherNo);


            var ReturnResult = dt.DataTableToList<PayVoucher>();
        

            return ReturnResult;

        }
        public static string GeneratePVNumber(int Voucher_No)
        {
            string PV_Vooucher = string.Empty;
            using (var context = new RBACDbContext())
            {
                var VoucherNo = new SqlParameter { ParameterName = "@VoucherNo", Value = Voucher_No };

                PV_Vooucher = context.Database.SqlQuery<string>("SELECT dbo.FnGeneratePV_Number(@VoucherNo)", VoucherNo).FirstOrDefault();


            }
            return PV_Vooucher;
        }
        public static string ProcessInvoiceNotification(string Uid)
        {
            var UniquiId = new SqlParameter { ParameterName = "@UniquiId", Value = HttpServerUtility.UrlTokenDecode(Uid) };
            var RetResult = new SqlParameter { ParameterName = "@RetResult", SqlDbType = SqlDbType.NVarChar, Size = 500, Direction = ParameterDirection.Output };

            //object[] xparams = {
            //    new SqlParameter("@UniquiId", HttpServerUtility.UrlTokenDecode(Uid)),
            //    new SqlParameter("@RetResult", SqlDbType.NVarChar,500) {Direction = ParameterDirection.Output}
            //};

            using (var context = new RBACDbContext())
            {
                context.Database.ExecuteSqlCommand("exec dbo.ProcessInvoiceNotification @UniquiId, @RetResult", UniquiId, RetResult);
            }

            var ReturnValue = ((SqlParameter)RetResult).Value;  

            string strResult = Convert.ToString(ReturnValue);

            return strResult == string.Empty ? "SUCCESS" : strResult;
        }
        public static int GetSessionRollId(int CaseId, string CaseLevel = null)
        {
            SessionsRoll _sessionsRoll = new SessionsRoll();
            using (var context = new RBACDbContext())
            {
                if (!string.IsNullOrEmpty(CaseLevel))
                    _sessionsRoll = context.SessionsRoll.Where(w => w.CaseId == CaseId && w.SessionLevel == CaseLevel).OrderByDescending(o => o.SessionRollId).FirstOrDefault();
                else
                    _sessionsRoll = context.SessionsRoll.Where(w => w.CaseId == CaseId).OrderByDescending(o => o.SessionRollId).FirstOrDefault();
            }
            if (_sessionsRoll == null)
                return 0;
            else
                return _sessionsRoll.SessionRollId;
        }
        public static int GetRegisterId(int CaseId)
        {
            using (var context = new RBACDbContext())
            {
                CaseRegistration _caseRegistration = context.CaseRegistration.Where(w => w.CaseId == CaseId).OrderByDescending(o => o.CaseRegistrationId).FirstOrDefault();
                if (_caseRegistration == null)
                {
                    var courtCases = context.CourtCase.Find(CaseId);

                    CaseRegistration caseRegistration = new CaseRegistration();
                    caseRegistration.CaseId = CaseId;
                    caseRegistration.ActionLevel = "1";
                    caseRegistration.ActionDate = courtCases.ReceptionDate ?? DateTime.Now;
                    caseRegistration.FileStatus = "1";
                    caseRegistration.EnforcementDispute = "0";
                    caseRegistration.CourtRegistration = "0";
                    caseRegistration.DepartmentType = "0";
                    caseRegistration.CourtDetailRegistered = false;
                    caseRegistration.IsDeleted = false;

                    context.CaseRegistration.Add(caseRegistration);
                    context.SaveChanges();

                    return caseRegistration.CaseRegistrationId;
                }
                else
                    return _caseRegistration.CaseRegistrationId;
            }
        }
        public static DataTable GetInvoiceDetailDt(int InvoiceId)
        {
            return new ReportCourtCases().getInvoiceDetailDt(InvoiceId);
        }
        public static List<DefendentTransferDTO> GetDefendentTransfer(int CaseId, string CaseLevel)
        {
            return new ReportCourtCases().GetDefendentTransfer(CaseId, CaseLevel);
        }
        public static DataTable ProcessDefendentTransfer(DefendentTransferDTO objDTO)
        {
            return new ReportCourtCases().ProcessDefendentTransfer(objDTO);
        }
        public static DefendentTransferDTO ProcessDefendentTransfer(int DefendentTransferId)
        {
            return new ReportCourtCases().GetDefendentTransfer(DefendentTransferId);
        }
        public static List<MasterSetups> GetSessionFileStatus(string[] FilterCode = null)
        {
            RBACDbContext db = new RBACDbContext();

            if (FilterCode == null)
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionFileStatus && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionFileStatus && m.Active_Flag == true && FilterCode.Contains(m.Mst_Value)).OrderBy(o => o.DisplaySeq).ToList();

            
        }
        public static List<MasterSetups> GetSessionOnHold()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.SessionOnHold && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static DataTable ProcessCourtDecisionHistory(CourtDecisionHistoryDTO objDTO)
        {
            return new ReportCourtCases().ProcessCourtDecisionHistory(objDTO);
        }
        public static DataTable GetDetailTable(string DataFor, int SessionRollId, int CaseId = 0)
        {
            return new ReportCourtCases().GetDetailTable(DataFor, SessionRollId, CaseId);
        }
        public static DataTable CheckInvoiceDuplicate(InvoiceCheckingDTO objDTO)
        {
            return new ReportCourtCases().CheckInvoiceDuplicate(objDTO);
        }
        public static string checkDuplicateCaseFile(string ValueToSearch, string TypeToSearch)
        {
            DataTable _result = new ReportCourtCases().ExecDuplicateCaseFile(ValueToSearch, TypeToSearch);
            string strResult = _result.Rows[0]["HtmlResult"].ToString();
            return strResult == string.Empty ? "" : strResult;
        }
        public static void ProcessCourtDecisionHistory(object objmodal,int Userid, string objName = null)
        {
            RBACDbContext db = new RBACDbContext();
            int SessionRollId = 0;

            if (string.IsNullOrEmpty(objName))
            {
               var modal = (SessionsRollVM)objmodal;

                CourtCases courtCases = db.CourtCase.Find(modal.CaseId);
                bool IsCreateHistory = false;

                if (string.IsNullOrEmpty(modal.CourtDecision) && string.IsNullOrEmpty(courtCases.CourtDecision))
                    IsCreateHistory = false;
                else
                {
                    var ar = new System.Globalization.CultureInfo("ar-OM");
                    if (string.Compare(modal.CourtDecision, courtCases.CourtDecision, ar, System.Globalization.CompareOptions.None) != 0)
                        IsCreateHistory = true;
                    else
                    {
                        if (courtCases.CurrentHearingDate != modal.CurrentHearingDate)
                            IsCreateHistory = true;
                        else
                            IsCreateHistory = false;
                    }

                    if (IsCreateHistory)
                    {
                        CourtDecisionHistoryDTO objDTO = new CourtDecisionHistoryDTO();
                        objDTO.Userid = Userid; // HttpContext.User.Identity.GetUserId();
                        objDTO.DataFor = "CREATE";
                        objDTO.SessionRollId = modal.SessionRollId;
                        objDTO.CaseId = modal.CaseId;
                        objDTO.CurrentHearingDate = modal.CurrentHearingDate;
                        objDTO.CourtDecision = modal.CourtDecision;
                        objDTO.TransportationSource = modal.TransportationSource;

                        DataTable _result = Helper.ProcessCourtDecisionHistory(objDTO);

                        string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                        string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();

                    }
                }
            }
            else if(objName == "ToBeRegisterVM")
            {
                var modal = (ToBeRegisterVM)objmodal;

                CourtCases courtCases = db.CourtCase.Find(modal.CaseId);

                bool IsCreateHistory = false;

                if (string.IsNullOrEmpty(modal.CourtDecision) && string.IsNullOrEmpty(courtCases.CourtDecision))
                    IsCreateHistory = false;
                else
                {
                    var ar = new System.Globalization.CultureInfo("ar-OM");
                    if (string.Compare(modal.CourtDecision, courtCases.CourtDecision, ar, System.Globalization.CompareOptions.None) != 0)
                        IsCreateHistory = true;
                    else
                    {
                        if (courtCases.CurrentHearingDate != modal.CurrentHearingDate)
                            IsCreateHistory = true;
                        else
                            IsCreateHistory = false;
                    }

                    if (IsCreateHistory)
                    {
                        SessionRollId = GetSessionRollId(modal.CaseId);

                        CourtDecisionHistoryDTO objDTO = new CourtDecisionHistoryDTO();
                        objDTO.Userid = Userid; // HttpContext.User.Identity.GetUserId();
                        objDTO.DataFor = "CREATE";
                        objDTO.SessionRollId = SessionRollId;
                        objDTO.CaseId = modal.CaseId;
                        objDTO.CurrentHearingDate = modal.CurrentHearingDate;
                        objDTO.CourtDecision = modal.CourtDecision;
                        objDTO.TransportationSource = modal.TransportationSource;

                        DataTable _result = ProcessCourtDecisionHistory(objDTO);

                        string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                        string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();

                    }
                }
            }
            else if(objName == "CourtCasesEnforcement")
            {
                var modal = (CourtCasesEnforcement)objmodal;

                CourtCases courtCases = db.CourtCase.Find(modal.CaseId);

                bool IsCreateHistory = false;

                if (string.IsNullOrEmpty(modal.CourtDecision) && string.IsNullOrEmpty(courtCases.CourtDecision))
                    IsCreateHistory = false;
                else
                {
                    var ar = new System.Globalization.CultureInfo("ar-OM");
                    if (string.Compare(modal.CourtDecision, courtCases.CourtDecision, ar, System.Globalization.CompareOptions.None) != 0)
                        IsCreateHistory = true;
                    else
                    {
                        if (courtCases.CurrentHearingDate != modal.CurrentHearingDate)
                            IsCreateHistory = true;
                        else
                            IsCreateHistory = false;
                    }

                    if (IsCreateHistory)
                    {
                        SessionRollId = GetSessionRollId(modal.CaseId);

                        CourtDecisionHistoryDTO objDTO = new CourtDecisionHistoryDTO();
                        objDTO.Userid = Userid; // HttpContext.User.Identity.GetUserId();
                        objDTO.DataFor = "CREATE";
                        objDTO.SessionRollId = SessionRollId;
                        objDTO.CaseId = modal.CaseId;
                        objDTO.CurrentHearingDate = modal.CurrentHearingDate;
                        objDTO.CourtDecision = modal.CourtDecision;

                        DataTable _result = Helper.ProcessCourtDecisionHistory(objDTO);

                        string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                        string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();

                    }
                }
            }
            else if(objName == "CaseRegistrationVM")
            {
                var modal = (CaseRegistrationVM)objmodal;

                CourtCases courtCases = db.CourtCase.Find(modal.CaseId);

                bool IsCreateHistory = false;

                if (string.IsNullOrEmpty(modal.CourtDecision) && string.IsNullOrEmpty(courtCases.CourtDecision))
                    IsCreateHistory = false;
                else
                {
                    var ar = new System.Globalization.CultureInfo("ar-OM");
                    if (string.Compare(modal.CourtDecision, courtCases.CourtDecision, ar, System.Globalization.CompareOptions.None) != 0)
                        IsCreateHistory = true;
                    else
                    {
                        if (courtCases.CurrentHearingDate != modal.CurrentHearingDate)
                            IsCreateHistory = true;
                        else
                            IsCreateHistory = false;
                    }

                    if (IsCreateHistory)
                    {
                        SessionRollId = GetSessionRollId(modal.CaseId);

                        CourtDecisionHistoryDTO objDTO = new CourtDecisionHistoryDTO();
                        objDTO.Userid = Userid; // HttpContext.User.Identity.GetUserId();
                        objDTO.DataFor = "CREATE";
                        objDTO.SessionRollId = SessionRollId;
                        objDTO.CaseId = modal.CaseId;
                        objDTO.CurrentHearingDate = modal.CurrentHearingDate;
                        objDTO.CourtDecision = modal.CourtDecision;
                        objDTO.TransportationSource = modal.TransportationSource;

                        DataTable _result = Helper.ProcessCourtDecisionHistory(objDTO);

                        string ProcessFlag = _result.Rows[0]["ProcessFlag"].ToString();
                        string ProcessMessage = _result.Rows[0]["ProcessMessage"].ToString();

                    }
                }
            }
        }
        public static string ProcessInvoiceREFNO(string Location)
        {
            string strResult = string.Empty;
            string strResPrefix = string.Empty;
            string strResNo = string.Empty;
            var dbPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Content");
            string dbfile = @"InvoiceReportReferences.xml";

            var FileToLoad = Path.Combine(dbPath, dbfile);

            XDocument xmlFile = XDocument.Load(FileToLoad);
            var query = from c in xmlFile.Elements("Location").Elements(Location)
                        select c;
            
            foreach (XElement loc in query)
            {
                strResNo = loc.Attribute("ReferenceNo").Value;
                int ResNo = int.Parse(strResNo);
                ResNo = ResNo + 1;
                strResNo = ResNo.ToString();

                switch (Location)
                {
                    case "Muscat":
                        strResPrefix = "MXL";
                        break;
                    case "Salalah":
                        strResPrefix = "SXL";
                        break;
                    default:
                        strResPrefix = "MXL";
                        break;
                }

                strResult = string.Format(@"{0}{1}", strResPrefix, strResNo.PadLeft(3,'0'));

                loc.Attribute("ReferenceNo").Value = strResNo;
            }

            xmlFile.Save(FileToLoad);

            return strResult;
        }
        public static void ProcessCaseRegistrationProgress(object objmodal, string objName = null, bool isCreated = false, string P_EnforcementDispute = null)
        {
            RBACDbContext db = new RBACDbContext();

            if (string.IsNullOrEmpty(objName))
            {
                var modal = (ToBeRegisterVM)objmodal;

                if (!string.IsNullOrEmpty(modal.CourtRefNo))
                {
                    string caseLevelCompare = (int.Parse(modal.CaseLevelCode) - 2).ToString();

                    var _caseRegistration = db.CaseRegistration.Where(w => w.CaseId == modal.CaseId && w.ActionLevel == caseLevelCompare && w.EnforcementDispute == "0" && !w.IsDeleted).OrderByDescending(o => o.CaseRegistrationId).FirstOrDefault();
                    if (_caseRegistration != null)
                    {
                        db.Entry(_caseRegistration).Entity.IsDeleted = true;
                        db.Entry(_caseRegistration).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            else if (objName == "FileClosureEnteryVM")
            {
                var modal = (CourtCases)objmodal;
                var _caseRegistration = db.CaseRegistration.Where(w => w.CaseId == modal.CaseId && !w.IsDeleted).ToList();
                if (_caseRegistration != null)
                {
                    foreach (var ep in _caseRegistration)
                    {
                        db.Entry(ep).Entity.IsDeleted = true;
                        db.Entry(ep).State = EntityState.Modified;
                    }

                    db.SaveChanges();
                }
            }
            else if (objName == "CourtCasesDetailVM")
            {
                var modal = (CourtCasesDetailVM)objmodal;
                var _caseRegistration = db.CaseRegistration.Where(w => w.CaseId == modal.CaseId && w.ActionLevel == modal.Courtid && w.EnforcementDispute == "0" && !w.IsDeleted).OrderByDescending(o => o.CaseRegistrationId).FirstOrDefault();

                if (_caseRegistration != null)
                {
                    db.Entry(_caseRegistration).Entity.IsDeleted = true;
                    db.Entry(_caseRegistration).State = EntityState.Modified;
                    db.SaveChanges();
                }

                //if (isCreated)
                //{
                //    if (_caseRegistration != null)
                //    {
                //        db.Entry(_caseRegistration).Entity.IsDeleted = true;
                //        db.Entry(_caseRegistration).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}
                //else
                //{
                //    if (_caseRegistration != null)
                //    {
                //        db.Entry(_caseRegistration).Entity.IsDeleted = true;
                //        db.Entry(_caseRegistration).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}
            }
            else if (objName == "CourtCasesEnforcement")
            {
                var modal = (CourtCasesEnforcement)objmodal;
                var _caseRegistration = db.CaseRegistration.Where(w => w.CaseId == modal.CaseId && w.ActionLevel == "4" && w.EnforcementDispute == P_EnforcementDispute && !w.IsDeleted).OrderByDescending(o => o.CaseRegistrationId).FirstOrDefault();

                if (_caseRegistration != null)
                {
                    db.Entry(_caseRegistration).Entity.IsDeleted = true;
                    db.Entry(_caseRegistration).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        public static List<MasterSetups> GetYesForSelect()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "PLEASE SELECT";
            YesNo.Mst_Value = "";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "YES";
            YesNo.Mst_Value = "1";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            return ResultList;
        }
        public static List<MasterSetups> GetAnnouncementType()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.AnnouncementType && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetInquiryResult(string[] FilterCode)
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.InquiryResult && m.Active_Flag == true && FilterCode.Contains(m.Remarks)).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetAuctionProcess()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.AuctionProcess && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetJudicialDecision()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.JudicialDecision && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetCauseOfRecovery()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CauseOfRecovery && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetJUDAuctionRepeat()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups AuctionRepeat = new MasterSetups();

            for (int i = 1; i <= 20; i++)
            {
                AuctionRepeat = new MasterSetups();
                AuctionRepeat.Mst_Desc = i.ToString();
                AuctionRepeat.Mst_Value = i.ToString();
                AuctionRepeat.DisplaySeq = i;
                ResultList.Add(AuctionRepeat);
            }


            return ResultList;
        }
        public static List<MasterSetups> GetTransSourceSelect()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.TransportationSource && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();

            //List<MasterSetups> ResultList = new List<MasterSetups>();
            //MasterSetups YesNo = new MasterSetups();
            //YesNo.Mst_Desc = "PLEASE SELECT";
            //YesNo.Mst_Value = "0";
            //YesNo.DisplaySeq = 0;
            //ResultList.Add(YesNo);

            //YesNo = new MasterSetups();
            //YesNo.Mst_Desc = "TRANSPORTATION انتقال إلى المحكمة";
            //YesNo.Mst_Value = "1";
            //YesNo.DisplaySeq = 1;
            //ResultList.Add(YesNo);

            //YesNo = new MasterSetups();
            //YesNo.Mst_Desc = "OTHER أخرى";
            //YesNo.Mst_Value = "2";
            //YesNo.DisplaySeq = 2;
            //ResultList.Add(YesNo);

            //return ResultList;
        }
        public static List<MasterSetups> GetCallerNames()
        {
            return GetCommonNameList();
        }
        public static List<MasterSetups> GetCommonNameList()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CallerName && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetFileStatus(string[] FilterCode = null)
        {
            RBACDbContext db = new RBACDbContext();

            if (FilterCode == null)
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            else
                return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.FileStatus && m.Active_Flag == true && FilterCode.Contains(m.Mst_Value)).OrderBy(o => o.DisplaySeq).ToList();

        }
        public static List<MasterSetups> GetOnHoldReason()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OnHoldReason && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();

        }
        public static List<MasterSetups> GetInvestmentYN()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.DepartmentType && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();

        }
        public static List<MasterSetups> GetYNForSelect()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "PLEASE SELECT";
            YesNo.Mst_Value = "";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "YES";
            YesNo.Mst_Value = "Y";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "NO";
            YesNo.Mst_Value = "N";
            YesNo.DisplaySeq = 2;
            ResultList.Add(YesNo);


            return ResultList;
        }
        public static List<MasterSetups> GetYNForSelectAR()
        {
            List<MasterSetups> ResultList = new List<MasterSetups>();
            MasterSetups YesNo = new MasterSetups();
            YesNo.Mst_Desc = "PLEASE SELECT";
            YesNo.Mst_Value = "";
            YesNo.DisplaySeq = 0;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "YES نعم";
            YesNo.Mst_Value = "Y";
            YesNo.DisplaySeq = 1;
            ResultList.Add(YesNo);

            YesNo = new MasterSetups();
            YesNo.Mst_Desc = "NO لا";
            YesNo.Mst_Value = "N";
            YesNo.DisplaySeq = 2;
            ResultList.Add(YesNo);


            return ResultList;
        }
        public static List<MasterSetups> GetSupremeStage()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.CourtDepartment && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetDisputeLevel()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.DisputeLevel && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetDisputeType()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.DisputeType && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }
        public static List<MasterSetups> GetJudgementLevel()
        {
            RBACDbContext db = new RBACDbContext();
            return db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.JudgementLevel && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
        }

        public static int IsClientAccess(string UserName)
        {
            using (var context = new RBACDbContext())
            {
                return context.ClientAccess.Where(w => w.UserName == UserName).Count();
            }
        }
        public static void UpdateClientAccess(string UserName,string strPassword)
        {
            //try
            //{

            //}
            //catch(Exception ex)
            //{
            //    string errM = ex.Message;
            //    string stktr = ex.StackTrace;
            //}
            using (var db = new RBACDbContext())
            {
                
                var clientAccess = db.ClientAccess.Where(w => w.UserName == UserName && !w.Inactive).FirstOrDefault();

                if (clientAccess != null)
                {
                    db.Entry(clientAccess).Entity.PassWord = strPassword;
                    db.Entry(clientAccess).Entity.LastModified = DateTime.UtcNow.AddHours(4);
                    db.Entry(clientAccess).State = EntityState.Modified;
                    db.SaveChanges();
                }
               
            }
        }
        public static List<Models.ClientAccessVM> GetClients(string ClientCode = null, string UserName = null)
        {
            List<Models.ClientAccessVM> ClientVM = new List<Models.ClientAccessVM>();

            using (var db = new RBACDbContext())
            {
                List<ClientAccess> Clients = new List<ClientAccess>();

                if (!string.IsNullOrEmpty(ClientCode) && string.IsNullOrEmpty(UserName))
                    Clients = db.ClientAccess.Where(w => w.ClientCode.Trim().Equals(ClientCode) && !w.Inactive).ToList();
                else if (string.IsNullOrEmpty(ClientCode) && !string.IsNullOrEmpty(UserName))
                    Clients = db.ClientAccess.Where(w => w.UserName.Trim().Equals(UserName) && !w.Inactive).ToList();
                else
                    Clients = db.ClientAccess.Where(w => !w.Inactive).ToList();

                ClientVM = (
                            from CL in Clients
                            join Client_MAS in db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.Client && m.Active_Flag == true) on CL.ClientCode equals Client_MAS.Mst_Value
                            select new Models.ClientAccessVM
                            {
                                ClientCode = CL.ClientCode,
                                ClientName = Client_MAS.Mst_Desc,
                                DisplayName = CL.DisplayName,
                                UserName = CL.UserName,
                                LastModified = CL.LastModified,
                                ClientId = CL.ClientId,
                                Inactive = CL.Inactive

                            }).ToList();
            }

            return ClientVM;

        }
        public static List<string> getDefaultParNames()
        {
            return new List<string>() { "@Pageno", "@filter", "@pagesize", "@Sorting", "@SortOrder" };
        }
        public static List<object> getDefaultParValues()
        {
            var start = (Convert.ToInt32(HttpContext.Current.Request["start"]));
            var Length = (Convert.ToInt32(HttpContext.Current.Request["length"])) == 0 ? 10 : (Convert.ToInt32(HttpContext.Current.Request["length"]));
            var searchvalue = HttpContext.Current.Request["search[value]"] ?? "";
            var sortcoloumnIndex = Convert.ToInt32(HttpContext.Current.Request["order[0][column]"]);
            var sortDirection = HttpContext.Current.Request["order[0][dir]"] ?? "asc";

            return new List<object>() { start, searchvalue, Length, sortcoloumnIndex, sortDirection };
        }
        public static DataTable getDataTable(string[] ParaName, object[] ParaValue, string procedureName = null)
        {
            List<SqlParameter> lstpar = new List<SqlParameter>();

            for (int i = 0; i < ParaName.Length; i++)
            {
                SqlParameter parToAdd = new SqlParameter(ParaName[i], ParaValue[i]);
                lstpar.Add(parToAdd);
            }

            var parameterList = lstpar.ToArray();

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName ?? "GetDataTable", parameterList).Tables[0];
        }
        public static DataSet getDataSet(string[] ParaName, object[] ParaValue, string[] TableNames, string procedureName = null)
        {
            List<SqlParameter> lstpar = new List<SqlParameter>();

            for (int i = 0; i < ParaName.Length; i++)
            {
                SqlParameter parToAdd = new SqlParameter(ParaName[i], ParaValue[i]);
                lstpar.Add(parToAdd);
            }

            var parameterList = lstpar.ToArray();

            return SqlHelper.ExecuteDataset(ConnectionString, CommandType.StoredProcedure, procedureName ?? "GetDataTable", TableNames, parameterList);
        }
        public static void GetCaseRegistrationVM(int CaseRegistrationId, ref CaseRegistrationVM ViewModal)
        {
            CaseRegistration caseRegistration = new CaseRegistration();
            
            using (var db = new RBACDbContext())
            {
                caseRegistration = db.CaseRegistration.Where(w => w.CaseRegistrationId == CaseRegistrationId && !w.IsDeleted).FirstOrDefault();
                if (caseRegistration != null)
                {
                    ViewModal.CaseRegistrationId = caseRegistration.CaseRegistrationId;
                    ViewModal.CaseId = caseRegistration.CaseId;
                    ViewModal.ActionDate = caseRegistration.ActionDate;
                    ViewModal.ActionLevel = caseRegistration.ActionLevel;
                    ViewModal.JudgementDate = caseRegistration.JudgementDate;
                    ViewModal.UrgentCaseDays = caseRegistration.UrgentCaseDays;
                    ViewModal.EnforcementDispute = caseRegistration.EnforcementDispute;
                    ViewModal.CourtRegistration = caseRegistration.CourtRegistration;
                    ViewModal.FileStatus = caseRegistration.FileStatus;
                    ViewModal.FileStatusRemarks = caseRegistration.FileStatusRemarks;
                    ViewModal.CourtMessage = caseRegistration.CourtMessage;
                    ViewModal.SendDate = caseRegistration.SendDate;
                    ViewModal.OmanPostNo = caseRegistration.OmanPostNo;
                    ViewModal.FirstReminderDate = caseRegistration.FirstReminderDate;
                    ViewModal.ReminderNo = caseRegistration.ReminderNo;
                    ViewModal.CourtRequestDate = caseRegistration.CourtRequestDate;
                    ViewModal.OfficeProcedure = caseRegistration.OfficeProcedure;
                    ViewModal.PaymentDate = caseRegistration.PaymentDate;
                    ViewModal.AssignedTo = caseRegistration.AssignedTo;
                    ViewModal.AssignedDate = caseRegistration.AssignedDate;
                    ViewModal.CourtDetailRegistered = caseRegistration.CourtDetailRegistered;
                    ViewModal.AdminFile = caseRegistration.AdminFile;
                    ViewModal.DepartmentType = caseRegistration.DepartmentType;
                    ViewModal.Voucher_No = caseRegistration.Voucher_No;
                    ViewModal.FormPrintDefendant = caseRegistration.FormPrintDefendant;
                    ViewModal.FormPrintLastDate = caseRegistration.FormPrintLastDate;
                    ViewModal.FormPrintWorkRequired = caseRegistration.FormPrintWorkRequired;
                    ViewModal.MainRemarks = caseRegistration.Remarks;
                    ViewModal.ClaimSummary = caseRegistration.ClaimSummary;
                    ViewModal.CourtFeeAmount = caseRegistration.CourtFeeAmount;
                    ViewModal.ElectronicNo = caseRegistration.ElectronicNo;
                    ViewModal.RealEstateYesNo = caseRegistration.RealEstateYesNo;
                    ViewModal.RealEstateDetail = caseRegistration.RealEstateDetail;
                    ViewModal.OnHoldDone = caseRegistration.OnHoldDone;
                    ViewModal.ConsultantId = caseRegistration.ConsultantId;
                    ViewModal.StopRegEmailDate = caseRegistration.StopRegEmailDate;
                    ViewModal.StopRegUserName = caseRegistration.StopRegUserName;
                    ViewModal.LawyerId = caseRegistration.LawyerId;
                    ViewModal.CourtLocationid = caseRegistration.CourtLocationid;
                    ViewModal.RegistrationDate = caseRegistration.RegistrationDate;
                    ViewModal.DisputeLevel = caseRegistration.DisputeLevel;
                    ViewModal.DisputeType = caseRegistration.DisputeType;
                    ViewModal.DisputrRegisterDate = caseRegistration.DisputrRegisterDate;
                    ViewModal.IsDeleted = caseRegistration.IsDeleted;
                    ViewModal.CourtRefNo = caseRegistration.FileStatusRemarks;
                    ViewModal.UpdatedOn = caseRegistration.UpdatedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? caseRegistration.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");

                    if (!string.IsNullOrEmpty(caseRegistration.DisputeLevel) && caseRegistration.DisputeLevel != "0")
                        ViewModal.DisputeLevelName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.DisputeLevel && w.Mst_Value == caseRegistration.DisputeLevel).FirstOrDefault().Mst_Desc;

                    if (!string.IsNullOrEmpty(caseRegistration.DisputeType) && caseRegistration.DisputeType != "0")
                        ViewModal.DisputeTypeName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.DisputeType && w.Mst_Value == caseRegistration.DisputeType).FirstOrDefault().Mst_Desc;

                    var CaseId = caseRegistration.CaseId;
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

                        var CurrentCaseLevel = courtCases.CaseLevelCode;
                        var CaseLevelCode = ViewModal.CaseLevelCode;

                        ViewModal.CaseId = courtCases.CaseId;
                        ViewModal.OfficeFileNo = courtCases.OfficeFileNo;
                        ViewModal.ReceptionDate = courtCases.ReceptionDate?.ToString("dd/MM/yyyy");
                        ViewModal.CaseTypeCode = courtCases.CaseTypeCode;
                        ViewModal.AgainstCode = courtCases.AgainstCode;
                        ViewModal.Defendant = courtCases.Defendant;
                        ViewModal.CaseLevelCode = ViewModal.CaseLevelCode ?? courtCases.CaseLevelCode;
                        ViewModal.ClientCaseType = courtCases.ClientCaseType;
                        ViewModal.AccountContractNo = courtCases.AccountContractNo;
                        ViewModal.ClientFileNo = courtCases.ClientFileNo;
                        ViewModal.ClaimAmount = courtCases.ClaimAmount;
                        ViewModal.IdRegistrationNo = courtCases.IdRegistrationNo;
                        ViewModal.OmaniExp = courtCases.OmaniExp;
                        ViewModal.CRRegistrationNo = courtCases.CRRegistrationNo;
                        ViewModal.CaseSubject = courtCases.CaseSubject;
                        ViewModal.CurrentHearingDate = courtCases.CurrentHearingDate;
                        ViewModal.CourtDecision = courtCases.CourtDecision;
                        ViewModal.SessionRemarks = courtCases.SessionRemarks;
                        ViewModal.Requirements = courtCases.Requirements;
                        ViewModal.SessionClientId = courtCases.SessionClientId;
                        ViewModal.SessionRollDefendentName = courtCases.SessionRollDefendentName;
                        ViewModal.SessionRollClientName = SessionRollClientName;
                        ViewModal.CourtFollowRequirement = courtCases.CourtFollowRequirement;
                        ViewModal.StopEnfRequest = courtCases.StopEnfRequest;

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

                        ViewModal.RealEstateYesNo = courtCases.RealEstateYesNo;
                        ViewModal.RealEstateDetail = courtCases.RealEstateDetail;
                        ViewModal.ClaimSummary = courtCases.ClaimSummary;
                        ViewModal.GovernorateId = courtCases.GovernorateId;
                        ViewModal.ClientName = ClientName;
                        ViewModal.ClientReply = courtCases.ClientReply;
                        ViewModal.CourtFollow = courtCases.CourtFollow;
                        ViewModal.FirstEmailDate = courtCases.FirstEmailDate;
                        ViewModal.CommissioningDate = courtCases.CommissioningDate;
                        ViewModal.TransportationSource = courtCases.TransportationSource;
                        ViewModal.NextHearingDate = courtCases.NextHearingDate;
                        ViewModal.TransportationFee = courtCases.TransportationFee;
                        ViewModal.UpdatedOn = courtCases.UpdatedOn?.ToString("dd/MM/yyyy HH:mm:ss") ?? courtCases.CreatedOn.ToString("dd/MM/yyyy HH:mm:ss");


                        var courtCasesDetail = db.CourtCasesDetail.Where(w => w.CaseId == CaseId && w.CaseLevelCode == CaseLevelCode).OrderByDescending(o => o.DetailId).FirstOrDefault();

                        if (courtCasesDetail != null)
                        {
                            ViewModal.CourtRefNo = courtCasesDetail.CourtRefNo;
                            ViewModal.RegistrationDate = courtCasesDetail.RegistrationDate;
                            ViewModal.CourtLocationid = courtCasesDetail.CourtLocationid;
                            ViewModal.DetailId = courtCasesDetail.DetailId;
                            ViewModal.ApealByWho = courtCasesDetail.ApealByWho;
                            ViewModal.CourtDepartment = courtCasesDetail.CourtDepartment;

                            if (!string.IsNullOrEmpty(courtCasesDetail.CourtLocationid) && courtCasesDetail.CourtLocationid != "0")
                                ViewModal.CourtLocationName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCasesDetail.CourtLocationid).FirstOrDefault().Mst_Desc;
                        }
                        else
                        {
                            if (ViewModal.PartialViewName == "TBR-REGISTERED_PRI" || ViewModal.PartialViewName == "TBR-REGISTERED_APL" || ViewModal.PartialViewName == "TBR-REGISTERED_SUP")
                            {
                                ViewModal.CourtRefNo = "";
                                ViewModal.RegistrationDate = (DateTime?)null;
                                ViewModal.CourtLocationid = "";
                                ViewModal.DetailId = 0;
                                ViewModal.ApealByWho = "";
                                ViewModal.CourtDepartment = "";
                                ViewModal.CourtLocationName = "";
                            }
                        }

                        CourtCasesEnforcement courtCasesEnforcement = db.CourtCasesEnforcement.Where(w => w.CaseId == CaseId).OrderByDescending(o => o.EnforcementId).FirstOrDefault();

                        if (courtCasesEnforcement != null)
                        {
                            ViewModal.ENFCourtRefNo = courtCasesEnforcement.EnforcementNo;
                            ViewModal.DetailId = courtCasesEnforcement.EnforcementId;

                            ViewModal.RegistrationDate = courtCasesEnforcement.RegistrationDate;
                            ViewModal.CourtLocationid = courtCasesEnforcement.CourtLocationid;


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

                            if (!string.IsNullOrEmpty(courtCasesEnforcement.CourtLocationid) && courtCasesEnforcement.CourtLocationid != "0")
                                ViewModal.ENFCourtLocationName = db.MasterSetup.Where(w => w.MstParentId == (int)MASTER_S.Location && w.Mst_Value == courtCasesEnforcement.CourtLocationid).FirstOrDefault().Mst_Desc;
                        }
                    }

                    var PayVoucher = db.PayVouchers.Find(caseRegistration.Voucher_No);
                    if (PayVoucher != null)
                    {
                        ViewModal.CourtType = PayVoucher.CourtType;
                        ViewModal.Payment_Head = PayVoucher.Payment_Head;
                        ViewModal.Remarks = PayVoucher.Remarks;
                        ViewModal.Payment_To = PayVoucher.Payment_To;
                        ViewModal.Amount = PayVoucher.Amount;
                        ViewModal.VatAmount = PayVoucher.VatAmount;
                        ViewModal.BillNumber = PayVoucher.BillNumber;
                        ViewModal.Debit_Account = PayVoucher.Debit_Account;
                        ViewModal.Cheque_Number = PayVoucher.Cheque_Number;
                        ViewModal.Cheque_Date = PayVoucher.Cheque_Date;
                    }
                }
            }
        }
        public static string GetGreatestJudgementDate(int SessionRollId)
        {
            string strJudgementDate = string.Empty;
            try
            {
                using (var context = new RBACDbContext())
                {
                    var p_SessionRollId = new SqlParameter { ParameterName = "@SessionRollId", Value = SessionRollId };
                    strJudgementDate = context.Database.SqlQuery<string>("SELECT dbo.FnGetGreatestJudgementDate(@SessionRollId)", p_SessionRollId).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                string errM = ex.Message;
                string stktr = ex.StackTrace;
            }
            

            return strJudgementDate;
        }
        public static List<MasterSetups> GetOfficeFileStatus(string FileStatusFilter, string[] FilterCode = null)
        {
            RBACDbContext db = new RBACDbContext();
            List<MasterSetups> lst = db.MasterSetup.Where(m => m.MstParentId == (int)MASTER_S.OfficeFileStatus && m.Active_Flag == true).OrderBy(o => o.DisplaySeq).ToList();
            List<MasterSetups> lstReturn = new List<MasterSetups>();

            if (FilterCode == null)
            {
                lstReturn = lst.Where(m => m.Remarks.Contains(FileStatusFilter)).OrderBy(o => o.DisplaySeq).ToList();
            }
            else
            {
                lstReturn = lst.Where(m => m.Remarks.Contains(FileStatusFilter) && FilterCode.Contains(m.Mst_Value)).OrderBy(o => o.DisplaySeq).ToList();
            }

            return lstReturn;
        }
    }
}