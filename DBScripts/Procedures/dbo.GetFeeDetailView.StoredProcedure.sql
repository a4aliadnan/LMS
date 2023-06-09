SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetFeeDetailView]
   @P_InvoiceId int
AS
Select 
		case when CIF.InvClassification = '0' then '' else CIFClassification.Mst_Desc end as InvClassification,
		case when CIF.CaseLevel = '0' then '' else CourtLevelCIF.Mst_Desc end as CaseLevel,
		case when CIF.FeeTypeId = '0' then '' else  FeeType.Mst_Desc end as Descriptions,
		case when CIF.FeeTypeCascadeDetail = '0' then '' else  FeeCascade.Mst_Desc end as Details,
		CIF.FeeTypeDetail as Numbers,
		FeeAmount as Amounts,
		CIF.VATPct,
		case when CIF.VATPct is null then null else FeeAmount * (CIF.VATPct/100) end as VATAmount,
		case when CIF.VATPct is null then FeeAmount else FeeAmount + (FeeAmount * (CIF.VATPct/100)) end as TotalAmount
from CaseInvoiceFees as CIF
inner join CaseInvoices as CI on CIF.InvoiceId = CI.InvoiceId
Left Join MASTER_S as CIFClassification on CIFClassification.Mst_Value = CIF.InvClassification and CIFClassification.MstParentId = 289
Left Join MASTER_S as FeeType on FeeType.Mst_Value = CIF.FeeTypeId and FeeType.MstParentId = 232
Left Join MASTER_S as CourtLevelCIF on CourtLevelCIF.Mst_Value = CIF.CaseLevel and CourtLevelCIF.MstParentId = 15 
Left Join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
where CIF.InvoiceId = @P_InvoiceId


GO
