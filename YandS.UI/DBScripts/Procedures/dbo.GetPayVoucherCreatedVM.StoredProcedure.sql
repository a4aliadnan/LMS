SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--DROP procedure [dbo].[GetPayVoucherCreatedVM]
CREATE procedure [GetPayVoucherCreatedVM]
   @OfficeFileNo varchar(10)
AS
SELECT TOP 3 case when CC.CaseLevelCode = '0' then null else CaseLevel.Mst_Desc end as CaseLevel,
		case when pv.Payment_Head = '0' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
		case when pv.Payment_To = '0' then null else PaymentTo.Mst_Desc end as PaymentToName,
		case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount,
		FORMAT (pv.Cheque_Date, 'dd/MM/yyyy') as W_D_Date
from PayVoucher pv
inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = case when pv.VoucherType = 1 then 567 else 7 end and PaymentHead.Mst_Value = pv.Payment_Head
inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
left join CourtCases CC on CC.CaseId = pv.CaseId
left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType
where cc.OfficeFileNo = @OfficeFileNo
order by Voucher_No desc

GO
