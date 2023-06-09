SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetCaseInvoiceDetail]
    @Caseid int
AS
select 
		 CI.CaseId as CaseId
		,CI.InvoiceId as InvoiceId
 		,CI.InvoiceNumber as InvoiceNumber
		,FORMAT (CI.InvoiceDate, 'dd/MM/yyyy')  as InvoiceDate
		,CaseLevelMas.Mst_Desc as CaseLevelName
		,FeeClassificationMas.Mst_Desc  as InvClassificationName
		,case when CIF.FeeTypeDetail is not null then FeeTypeMas.Mst_Desc + ' (' + CIF.FeeTypeDetail + ')' else FeeTypeMas.Mst_Desc end  as FeeTypeName
		,CIF.FeeAmount
 from CaseInvoices as CI 
 inner join CaseInvoiceFees CIF on CI.InvoiceId = CIF.InvoiceId
 inner join MASTER_S as CaseLevelMas on CIF.CaseLevel = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
  inner join MASTER_S as FeeClassificationMas on CIF.InvClassification = FeeClassificationMas.Mst_Value and FeeClassificationMas.MstParentId = 289
 inner join MASTER_S as FeeTypeMas on CIF.FeeTypeId = FeeTypeMas.Mst_Value and FeeTypeMas.MstParentId = 232
where CI.CaseId = @Caseid
ORDER BY 2


GO
