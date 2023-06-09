SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetPayVoucherById]
    @VoucherById int
AS
SELECT pv.Voucher_No
      ,pv.Voucher_Date
      ,pv.Payment_Type
      ,pv.Debit_Account
      ,pv.Payment_Head
      ,pv.Credit_Account
      ,pv.Amount
      ,pv.Remarks
      ,pv.Payment_Mode
      ,pv.Cheque_Number
      ,pv.Received_on
      ,pv."Status"
      ,pv.CreatedBy
      ,pv.CreatedOn
      ,pv.UpdatedBy
      ,pv.UpdatedOn
	  ,payHead.Mst_Desc as PaymentHeadName
	  ,payType.Mst_Desc as PaymentTypeName
	  ,bank_mas.Mst_Desc + ' - ' + account_mas.Mst_Desc as DebitAccountName
	  ,bank_mascr.Mst_Desc + ' - ' + account_mascr.Mst_Desc as CreditAccountName
	  ,payMode.Mst_Desc as PaymentModeName
	  ,payStatus.Mst_Desc as StatusName
  FROM PayVoucher as pv
  left join [dbo].[Link_Bank_Account] as BA on pv.Debit_Account = BA.LinkId
  left join [dbo].[MASTER_S] as bank_mas on bank_mas.MstId = BA.BankId
  left join [dbo].[MASTER_S] as account_mas on account_mas.MstId = BA.AccountId
  left join [dbo].[Link_Bank_Account] as BACR on pv.Credit_Account = BACR.LinkId
  left join [dbo].[MASTER_S] as bank_mascr on bank_mascr.MstId = BACR.BankId
  left join [dbo].[MASTER_S] as account_mascr on account_mascr.MstId = BACR.AccountId
  left join [dbo].[MASTER_S] as payHead on payHead.MstParentId = 7 and payHead.Mst_Value = pv.Payment_Head
  left join [dbo].[MASTER_S] as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
  left join [dbo].[MASTER_S] as payType on payType.MstParentId = 9 and payType.Mst_Value = pv.Payment_Type
  left join [dbo].[MASTER_S] as payStatus on payStatus.MstParentId = 10 and payStatus.Mst_Value = pv."Status"
  WHERE pv.Voucher_No = @VoucherById;
GO
