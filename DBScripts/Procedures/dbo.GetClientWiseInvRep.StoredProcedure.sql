SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetClientWiseInvRep]
    @Location varchar(1),
	@ClientCode varchar(5),
	@ReceptionDateFrom date,
	@ReceptionDateTo date
AS
Select	 CI.InvoiceNumber + ' | ' +  CC.OfficeFileNo as 'INVOICE NO'
		,FORMAT (CI.InvoiceDate, 'd-MMM-yy')  as 'DATE OF INV '
		,ClientMas.Mst_Desc as 'CLIENT NAME'
		,CC.Defendant as 'DEFENDANT '
		,FeeTypeMas.Mst_Desc as 'TYPE OF FEES'
		,CC.AccountContractNo as 'ACCOUNT NUMBER'
		,CC.ClaimAmount as 'CLAIM AMOUNT'
		,CIF.FeeAmount as 'AMOUNT OF INVOICE'
from CaseInvoices CI
inner join CourtCases as CC on CC.CaseId = CI.CaseId
inner join MASTER_S as ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
inner join CaseInvoiceFees CIF on CIF.InvoiceId = CI.InvoiceId
inner join MASTER_S as FeeTypeMas on CIF.FeeTypeId = FeeTypeMas.Mst_Value and FeeTypeMas.MstParentId = 232
where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
AND   (CC.ClientCode = @ClientCode OR @ClientCode = '0')
AND   CC.ReceptionDate BETWEEN  @ReceptionDateFrom AND @ReceptionDateTo
order by 1
GO
