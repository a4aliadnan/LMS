SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetInvoiceDetailDt]
   @P_InvoiceId int
AS


BEGIN
EXEC	[dbo].[NormalizeDDL]
END


Select 
		ClientMas.Mst_Desc as ClientName,
		CC.OfficeFileNo,
		CC.Defendant,
		ClientCasType.Mst_Desc ClientCaseTypeName,
		ODB.Mst_Desc ODBBranchName,
		CC.AccountContractNo,
		CC.ClientFileNo,
		CaseType.Mst_Desc CaseTypeName,
		CaseSub.Mst_Desc CaseSubjectName,
		CaseAgainst.Mst_Desc  CaseAgainst,
		CC.AgainstCode as CaseAgainstCode,
		ISNULL(CC.ClaimAmount,0) as ClaimAmount,
		BA.Mst_Desc as CreditAccountName,
		CC.CaseSubject,
		Enforcement.Mst_Desc EnforcementStageName,
		CC.ClientClassificationCode,
		caseclass.Mst_Desc ClientClassificationName,
		CC.ClientCaseType,
		CC.CaseTypeCode,
		CC.Subject as Subject
from CaseInvoices as CI
inner join CourtCases CC on CI.CaseId = CC.CaseId
inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
join MASTER_S CaseAgainst on CC.AgainstCode = CaseAgainst.Mst_Value and CaseAgainst.MstParentId = 12
join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
join MASTER_S as ClientCasType on ClientCasType.MstParentId = 285 and ClientCasType.Mst_Value = CC.ClientCaseType
join MASTER_S as ODB on ODB.MstParentId = 18 and ODB.Mst_Value = CC.ODBBankBranch
join MASTER_S as CaseType on CaseType.Mst_Value = CC.CaseTypeCode and CaseType.MstParentId = 14 
join MASTER_S as CaseSub on CaseSub.MstParentId = 532 and CaseSub.Mst_Value = CC.CaseSubject
join MASTER_S as Enforcement on Enforcement.MstParentId = 569 and Enforcement.Mst_Value = CI.EnforcementStage
join MASTER_S as caseclass on caseclass.MstParentId = 523 and caseclass.Mst_Value = CC.ClientClassificationCode
where CI.InvoiceId = @P_InvoiceId

GO
