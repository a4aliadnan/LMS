SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetCaseInvoiceList]
    @Location varchar(1)
AS
Select
CaseId,	OfficeFileNo,	ClientCode,	ClientName,	Defendant,	AgainstCode,	AgainstName,	ClaimAmount,
AccountContractNo,	ClientFileNo,	InvoiceId,	InvoiceNumber,	InvoiceDate,	CourtType,	CourtName,	InvoiceStatus,
InvoiceStatusName,	TransferType,	TransferNumber,	TransferDate,	Credit_Account,	CreditAccountName,	CourtLevelDisp,	InvoiceAmount
from
(
Select	 CC.CaseId,CC.OfficeFileNo,CC.ClientCode,ClientMas.Mst_Desc as ClientName,CC.Defendant,CC.AgainstCode,AgainstMas.Mst_Desc as AgainstName,CC.ClaimAmount
		,CC.AccountContractNo,CC.ClientFileNo
		,CI.InvoiceId,CI.InvoiceNumber,CI.InvoiceDate,CI.CourtType,CourtTypeMas.Mst_Desc as CourtName,CI.InvoiceStatus,InvStatusMas.Mst_Desc as InvoiceStatusName
		,CI.TransferType,CI.TransferNumber,CI.TransferDate,CI.Credit_Account,BA.Mst_Desc as CreditAccountName
		,dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) as CourtLevelDisp,
		(select sum(FeeAmount) as FeeAmount from CaseInvoiceFees CIF where CIF.InvoiceId = CI.InvoiceId) as InvoiceAmount
from CourtCases as CC
join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
join CaseInvoices CI on CI.CaseId = CC.CaseId
join MASTER_S CourtTypeMas on CI.CourtType = CourtTypeMas.Mst_Value and CourtTypeMas.MstParentId = 15
join MASTER_S InvStatusMas on CI.InvoiceStatus = InvStatusMas.Mst_Value and InvStatusMas.MstParentId = 247
join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
) dat
--where dat.InvoiceAmount > 0
ORDER BY 1;
GO
