SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE procedure [GetPVListReport]
    @Location varchar(3),
	@DateFrom date,
	@DateTo date
AS
Select	--pv.Voucher_No,
		 FORMAT (PV.Voucher_Date, 'dd/MM/yyyy') as "PAYMENT DATE"
		,PV.PV_No as "PAYMENT VOUCHER NO."
		,case when PV.Payment_Head = '0' then '' else PHeadMas.Mst_Desc end as "TYPE OF FEES"
		,PV.Amount as "AMOUNT (OMR)"
		,PV.Cheque_Number as "TRANSFER OR CHEQUE NO."
		,PTypeMas.Mst_Desc as "PAYMENT TYPE"
		,case when PV.Payment_To = '0' then '' else PayToMas.Mst_Desc end  as "PAY TO"
		,pv.CaseInvoices as "INVOICE NO."
		,LBA.Mst_Desc as "DEBITED BANK"
from PayVoucher as PV
join v_PayTo as PayToMas on PV.Payment_To = PayToMas.Mst_Value
join MASTER_S as PHeadMas on PV.Payment_Head = PHeadMas.Mst_Value and PHeadMas.MstParentId = 7
--join MASTER_S as PTypeMas on PV.Payment_Type = PTypeMas.Mst_Value and PTypeMas.MstParentId = 9
join MASTER_S as PTypeMas on PV.Payment_Mode = PTypeMas.Mst_Value and PTypeMas.MstParentId = 8
join Link_Bank_Account_View as LBA on LBA.LinkId = PV.Debit_Account
where (LEFT(PV.PV_No,1) = @Location OR @Location = 'A')
AND   PV.Voucher_Date BETWEEN  @DateFrom AND @DateTo
GO
