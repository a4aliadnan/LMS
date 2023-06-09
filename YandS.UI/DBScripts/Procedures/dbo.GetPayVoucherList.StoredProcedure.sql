SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetPayVoucherList]
    @Location varchar(3),
	@VoucherNo int
AS
Select 
					pv.Voucher_No,
					pv.Voucher_Date,
					pv.Payment_Type,
					pv.Debit_Account,
					pv.Payment_Head,
					pv.Credit_Account,
					pv.Amount,
					pv.Remarks,
					pv.Payment_Mode,
					pv.Cheque_Number,
					pv.Cheque_Date,
					pv.Received_on,
					pv."status" as "Status",
					pv.Payment_To,
					case when pv.Payment_Head = '0' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					PType.Mst_Desc as PaymentTypeName,
					payMode.Mst_Desc as PaymentModeName,
					BA.Mst_Desc as DebitAccountName,
					BAC.Mst_Desc as CreditAccountName,
					case when pv."status" = '0' then 'Voucher Created' else Stats.Mst_Desc end as StatusName,
					case when pv.Payment_To = '0' then null else PaymentTo.Mst_Desc end as PaymentToName,
					pv.VoucherType,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = '0' then 'PENDING' else VoucherStatus.Mst_Desc end  as VoucherStatusName,
					pv.CourtType,
					case when pv.CourtType = '0' then null else CaseLevel.Mst_Desc end as CourtTypeName,
					LEFT(Emp.LocationCode,3) as LocationCode,
					case when Emp.LocationCode = 'MCT-M' then 'Muscat' else 'Salalah' end as LocationName,
					pv.PV_No,
					pv.TransTypeCode,
					case when pv.TransTypeCode = '0' then null else TrType.Mst_Desc end as TransTypeName,
					pv.TransReasonCode,
					case when pv.TransReasonCode = '0' then null else ReasonCode.Mst_Desc end as TransReasonName,
					pv.CaseInvoices,
					case when pv.CaseId is null then 0 else pv.CaseId end as CaseId,	
					pv.BillNumber,
					pv.FutureChequeDate
				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
				inner join MASTER_S as PType on PType.MstParentId = 9 and PType.Mst_Value = pv.Payment_Type
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = case when pv.VoucherType = 1 then 567 else 7 end and PaymentHead.Mst_Value = pv.Payment_Head
					inner join MASTER_S as TrType on TrType.MstParentId = 401 and TrType.Mst_Value = pv.TransTypeCode
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					inner join Link_Bank_Account_View BAC on BAC.LinkId = pv.Credit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType
					where 1 = Case When  @Location = 'A' then 1 
								Else
								Case when dateadd(hour , 11, pv.AuthorizeDate) >= '2021-06-01' then
								  Case when Left(PV_No,1) =  LEFT(@Location,1) then 1 else 0 End
								Else
								  Case when Left(PV_No,3) =  @Location then 1 else 0 End
								End
							 End
							 --(LEFT(Emp.LocationCode,1) = LEFT(@Location,1) OR @Location = 'A')
					AND		(pv.Voucher_No = @VoucherNo OR @VoucherNo = 0)
GO
