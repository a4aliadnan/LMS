SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetClosureFormDetail]
   @CaseId int,
   @CourtLevel varchar(15),
   @StaffNumber varchar(3)
AS

DECLARE @UserName VARCHAR(200)
SELECT @UserName = FullName FROM HR_Employee_s where  EmployeeNumber = @StaffNumber

if @CourtLevel = 'FileClosure'

Select FORMAT(GETDATE(),'dd/MM/yyyy') as DateOfClosure,@UserName as ClosedBy, InvType.Mst_Desc as InvoiceStatus, FORMAT(CI.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, CI.InvoiceNumber, 
case when CIF.CaseLevel = '0' then '' else CourtLevelCIF.Mst_Desc end +  
case when CIF.FeeTypeId = '0' then '' else  ' - ' + FeeType.Mst_Desc end + 
case when CIF.FeeTypeCascadeDetail = '0' then '' else  ' - ' + FeeCascade.Mst_Desc end +
case when CIF.FeeTypeDetail is null then '' else ' - ' + CIF.FeeTypeDetail end as TypeOfFees, 
CourtType.Mst_Desc as CourtLevel,
		null NextCaseLevel,
		null NextCaseLevelCode,
		CC.OfficeFileNo,
		CC.Defendant,
		case when CC.ClientCode = '0' then '' else ClientMas.Mst_Desc end as ClientName
from CourtCases CC
--Left join CourtCasesDetails as CCD on CC.CaseId = CCD.CaseId
Left Join CaseInvoices as CI on CC.CaseId = CI.CaseId
Left Join CaseInvoiceFees as CIF on CIF.InvoiceId = CI.InvoiceId
Left Join MASTER_S as FeeType on FeeType.Mst_Value = CIF.FeeTypeId and FeeType.MstParentId = 232
Left Join MASTER_S as InvType on InvType.Mst_Value = CI.InvoiceStatus and InvType.MstParentId = 247
Left Join MASTER_S as CourtType on CourtType.Mst_Value = CC.CaseLevelCode and CourtType.MstParentId = 15 
Left Join MASTER_S as CourtLevelCIF on CourtLevelCIF.Mst_Value = CIF.CaseLevel and CourtLevelCIF.MstParentId = 15
Left Join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
Left Join MASTER_S as ClientMas on ClientMas.Mst_Value = CC.ClientCode and ClientMas.MstParentId = 241
where CC.CaseId = @CaseId
--and   CI.CourtType = @CourtLevel
else
Select FORMAT(GETDATE(),'dd/MM/yyyy') as DateOfClosure,@UserName as ClosedBy, InvType.Mst_Desc as InvoiceStatus, FORMAT(CI.InvoiceDate,'dd/MM/yyyy') as InvoiceDate, CI.InvoiceNumber, 
case when CIF.CaseLevel = '0' then '' else CourtLevelCIF.Mst_Desc end +  
case when CIF.FeeTypeId = '0' then '' else  ' - ' + FeeType.Mst_Desc end + 
case when CIF.FeeTypeCascadeDetail = '0' then '' else  ' - ' + FeeCascade.Mst_Desc end +
case when CIF.FeeTypeDetail is null then '' else ' - ' + CIF.FeeTypeDetail end as TypeOfFees, CourtType.Mst_Desc as CourtLevel,
		CCD.NextCaseLevel,
		CCD.NextCaseLevelCode,
		CC.OfficeFileNo,
		CC.Defendant,
		case when CC.ClientCode = '0' then '' else ClientMas.Mst_Desc end as ClientName
from CourtCasesDetails as CCD
inner join CourtCases CC on CC.CaseId = CCD.CaseId
Left Join CaseInvoices as CI on CCD.CaseId = CI.CaseId
Join CaseInvoiceFees as CIF on CIF.InvoiceId = CI.InvoiceId and CIF.CaseLevel = CCD.CaseLevelCode
Left Join MASTER_S as FeeType on FeeType.Mst_Value = CIF.FeeTypeId and FeeType.MstParentId = 232
Left Join MASTER_S as InvType on InvType.Mst_Value = CI.InvoiceStatus and InvType.MstParentId = 247
Left Join MASTER_S as CourtType on CourtType.Mst_Value = CCD.CaseLevelCode and CourtType.MstParentId = 15 
Left Join MASTER_S as CourtLevelCIF on CourtLevelCIF.Mst_Value = CIF.CaseLevel and CourtLevelCIF.MstParentId = 15
Left Join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
Left Join MASTER_S as ClientMas on ClientMas.Mst_Value = CC.ClientCode and ClientMas.MstParentId = 241
where CCD.CaseId = @CaseId
and   CCD.CaseLevelCode = @CourtLevel

GO
