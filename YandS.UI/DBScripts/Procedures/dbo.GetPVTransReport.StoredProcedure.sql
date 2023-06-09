SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetPVTransReport]
    @Location varchar(3),
	@DateFrom date,
	@DateTo date,
	@VoucherType varchar(3),
	@PayTo varchar(3)
AS

if @VoucherType = 1
begin
SELECT
	Voucher_Date as "DATE",
	case when LEFT( LocationCode,1) = 'M' then 'Muscat'	when LEFT( LocationCode,1) = 'S' then 'Salalah' end as "Branch",
	PV_No as "PV.NO.",
	PaymentModeName as "Mode of payment",
	OfficeFileNo as "Y & S File No",
	ClientName as "Client",
	Defendant as "Defendant",
	CaseLevel as "Case Level",
	PaymentHeadName as "PAY FOR",
	Remarks as "Remarks",
	PaymentToName as "PAY TO",
	Amount as "AMOUNT",
	VatAmount as "VAT AMOUNT",
	TotalAmount as "TOTAL AMOUNT (OMR)",
	BillNumber as "Reference",
	"W_D_BANK" as "WITHDRAWAL BANK",
	Cheque_Number as "CHQ/TRANSFER NUMBER",
	W_D_Date as "DATE OF WITHDRAWAL"

FROM
(
Select 
					pv.Voucher_No,
					pv.PV_No,
					FORMAT (pv.Voucher_Date, 'dd/MM/yyyy') as Voucher_Date,
					VoucherType,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = '0' then 'PENDING' else VoucherStatus.Mst_Desc end  as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = '0' then 'Voucher Created' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					payMode.Mst_Desc as PaymentModeName,
					pv.Amount,
					pv.CaseInvoices,
					Emp.LocationCode,
					pv.Payment_Head,
					case when pv.Payment_Head = '0' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Payment_To,
					case when pv.Payment_To = '0' then null else PaymentTo.Mst_Desc end as PaymentToName,
					FORMAT (pv.Cheque_Date, 'dd/MM/yyyy') as W_D_Date,
					FORMAT (pv.FutureChequeDate, 'dd/MM/yyyy') as FutureChequeDate,
					pv.TransReasonCode,
					case when pv.TransReasonCode = '0' then null else ReasonCode.Mst_Desc end as TransReasonName,pv.Remarks,pv.BillNumber,
					BA.Mst_Desc as "W_D_BANK",CC.OfficeFileNo,
					case when CC.ClientCode = '0' then null else Clients.Mst_Desc end as ClientName,CC.Defendant,
					case when CC.CaseLevelCode = '0' then null else CaseLevel.Mst_Desc end as CaseLevel, null as REF_PAID
					,Cheque_Number,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount
				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = 567 and PaymentHead.Mst_Value = pv.Payment_Head
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CC.CaseLevelCode
					where (LEFT(PV.PV_No,1) = @Location OR @Location = 'A')
					AND   PV.Voucher_Date BETWEEN  @DateFrom AND @DateTo
					AND   PV.VoucherType = @VoucherType
					AND   (PV.Payment_To = @PayTo OR @PayTo = '0')
					) DAT
end
else if @VoucherType = 2
begin
SELECT
	Voucher_Date as "DATE",
	case when LEFT( LocationCode,1) = 'M' then 'Muscat'	when LEFT( LocationCode,1) = 'S' then 'Salalah' end as "Branch",
	PV_No as "PV.NO.",
	PaymentModeName as "Mode of payment",
	PaymentHeadName as "PAY FOR",
	Remarks as "Remarks",
	PaymentToName as "PAY TO",
	Amount as "AMOUNT (OMR)",
	VatAmount as "VAT AMOUNT",
	TotalAmount as "TOTAL AMOUNT (OMR)",
	BillNumber as "Reference",
	"W_D_BANK" as "WITHDRAWAL BANK",
	Cheque_Number as "CHQ/TRANSFER NUMBER",
	W_D_Date as "DATE OF WITHDRAWAL"
FROM
(
Select 
					pv.Voucher_No,
					pv.PV_No,
					FORMAT (pv.Voucher_Date, 'dd/MM/yyyy') as Voucher_Date,
					VoucherType,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = '0' then 'PENDING' else VoucherStatus.Mst_Desc end  as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = '0' then 'Voucher Created' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					payMode.Mst_Desc as PaymentModeName,
					pv.Amount,
					pv.CaseInvoices,
					Emp.LocationCode,
					pv.Payment_Head,
					case when pv.Payment_Head = '0' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Payment_To,
					case when pv.Payment_To = '0' then null else PaymentTo.Mst_Desc end as PaymentToName,
					FORMAT (pv.Cheque_Date, 'dd/MM/yyyy') as W_D_Date,
					FORMAT (pv.FutureChequeDate, 'dd/MM/yyyy') as FutureChequeDate,
					pv.TransReasonCode,
					case when pv.TransReasonCode = '0' then null else ReasonCode.Mst_Desc end as TransReasonName,pv.Remarks,pv.BillNumber,
					BA.Mst_Desc as "W_D_BANK",CC.OfficeFileNo,
					case when CC.ClientCode = '0' then null else Clients.Mst_Desc end as ClientName,CC.Defendant,
					case when CC.CaseLevelCode = '0' then null else CaseLevel.Mst_Desc end as CaseLevel, null as REF_PAID
					,Cheque_Number,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount
				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = 7 and PaymentHead.Mst_Value = pv.Payment_Head
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CC.CaseLevelCode
					where (LEFT(PV.PV_No,1) = @Location OR @Location = 'A')
					AND   PV.Voucher_Date BETWEEN  @DateFrom AND @DateTo
					AND   PV.VoucherType = @VoucherType
					AND   (PV.Payment_To = @PayTo OR @PayTo = '0')
					) DAT
end
else if @VoucherType = 3
begin
SELECT
	Voucher_Date as "DATE",
	case when LEFT( LocationCode,1) = 'M' then 'Muscat'	when LEFT( LocationCode,1) = 'S' then 'Salalah' end as "BRANCH",
	PV_No as "TRA.NO.",
	PaymentModeName as "MODE",
	TransTypeName as "TRANSACTION TYPE",
	TransReasonName as "TRANSACTION REASON",
	Amount as "AMOUNT (OMR)",
	VatAmount as "VAT AMOUNT",
	TotalAmount as "TOTAL AMOUNT (OMR)",
	"W_D_BANK" as "TRANSACTION FROM",
	"DEPOSIT_BANK" as "TRANSACTION TO",
	Remarks as "Remarks",
	Cheque_Number as "CHEQUE / TRANSFER NO.",
	W_D_Date as "DATE OF WITHDRAWAL"
FROM
(
Select 
					pv.Voucher_No,
					pv.PV_No,
					FORMAT (pv.Voucher_Date, 'dd/MM/yyyy') as Voucher_Date,
					VoucherType,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = '0' then 'PENDING' else VoucherStatus.Mst_Desc end  as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = '0' then 'Voucher Created' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					payMode.Mst_Desc as PaymentModeName,
					pv.Amount,
					pv.CaseInvoices,
					Emp.LocationCode,
					pv.Payment_Head,
					case when pv.Payment_Head = '0' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Payment_To,
					case when pv.Payment_To = '0' then null else PaymentTo.Mst_Desc end as PaymentToName,
					FORMAT (pv.Cheque_Date, 'dd/MM/yyyy') as W_D_Date,
					FORMAT (pv.FutureChequeDate, 'dd/MM/yyyy') as FutureChequeDate,
					pv.TransReasonCode,
					case when pv.TransReasonCode = '0' then null else ReasonCode.Mst_Desc end as TransReasonName,
					pv.TransTypeCode,
					case when pv.TransTypeCode = '0' then null else TransTypeCode.Mst_Desc end as TransTypeName,pv.Remarks,pv.BillNumber,
					BA.Mst_Desc as "W_D_BANK",CC.OfficeFileNo,
					case when CC.ClientCode = '0' then null else Clients.Mst_Desc end as ClientName,CC.Defendant,
					case when CC.CaseLevelCode = '0' then null else CaseLevel.Mst_Desc end as CaseLevel, null as REF_PAID
					,BAC.Mst_Desc as "DEPOSIT_BANK",Cheque_Number,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount

				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = 7 and PaymentHead.Mst_Value = pv.Payment_Head
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join MASTER_S as TransTypeCode on TransTypeCode.MstParentId = 401 and TransTypeCode.Mst_Value = pv.TransTypeCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					inner join Link_Bank_Account_View BAC on BAC.LinkId = pv.Credit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CC.CaseLevelCode
					where (LEFT(PV.PV_No,1) = @Location OR @Location = 'A')
					AND   PV.Voucher_Date BETWEEN  @DateFrom AND @DateTo
					AND   PV.VoucherType = @VoucherType
					AND   (PV.Payment_To = @PayTo OR @PayTo = '0')
					) DAT
end
GO
