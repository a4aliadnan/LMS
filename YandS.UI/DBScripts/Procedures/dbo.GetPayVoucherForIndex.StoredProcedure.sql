SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetPayVoucherForIndex]
@UserName varchar(5),
@DataFor varchar(10),
@Pageno INT=1,
@filter VARCHAR(500)='',
@pagesize INT=20,  
@Sorting VARCHAR(500)='pv.PV_No',
@SortOrder VARCHAR(500)='desc' 
AS
DECLARE @Where VARCHAR(5000) = ' where 1=1'
DECLARE @Role VARCHAR(100)
DECLARE @LocationId VARCHAR(5)
DECLARE @FinalQuery NVARCHAR (MAX)
DECLARE @UserId int
DECLARE @RoleCount int
DECLARE @PaymentHeadJoin VARCHAR(200)

SET @LocationId = (select LocationCode from HR_Employee_s where EmployeeNumber = @UserName)
SET @UserId = (select UserId from USERS where UserName = @UserName)
SET @RoleCount = (select count(*) from LNK_USER_ROLE where UserId = @UserId and RoleId in (4)) -- 'VoucherApproval'


SET NOCOUNT ON;
DECLARE @SqlCount INT
DECLARE @From INT = @pageno
DECLARE @SQLQuery NVARCHAR (MAX)
DECLARE @SQLCountQuery NVARCHAR (MAX)
IF(@Sorting ='pv.PV_No')
BEGIN
 SET @Sorting = 'dbo.fnMixSort(pv.PV_No)'
END

IF (@DataFor in('REF','REFAPR','REFREJ'))
begin
	SET @PaymentHeadJoin = ' inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = 567 and PaymentHead.Mst_Value = pv.Payment_Head '
end
ELSE
begin
	SET @PaymentHeadJoin = ' inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = 7 and PaymentHead.Mst_Value = pv.Payment_Head '
end

	IF (@RoleCount > 0)
	BEGIN
		 IF (@DataFor = 'REF')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''0'''
			END
		 ELSE IF (@DataFor = 'PDC')
			BEGIN
			SET @Where = @Where + ' AND pv.FutureChequeDate is not null AND ( ( pv.Status = ''0'' AND pv.VoucherStatus = ''0'') OR ( pv.Status = ''0'' AND pv.VoucherStatus = ''1'') )'
			END
		 ELSE IF (@DataFor = 'NONREF')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''0'''
			END
		 ELSE IF (@DataFor = 'REFAPR')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.VoucherStatus = ''1'''
			END
		 ELSE IF (@DataFor = 'NONREFAPR')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.VoucherStatus = ''1'''
			END
		 ELSE IF (@DataFor = 'REFREJ')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.VoucherStatus = ''2'''
			END
		 ELSE IF (@DataFor = 'NONREFREJ')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.VoucherStatus = ''2'''
			END
		ELSE IF (@DataFor = 'INTRA')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''3'' AND pv.VoucherStatus = ''0'''
			END
		END
	ELSE
		BEGIN
		 SET @Where = @Where + ' AND Emp.LocationCode = '''+@LocationId+''''
		 IF (@DataFor = 'REF')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''0'''
			END
		 ELSE IF (@DataFor = 'PDC')
			BEGIN
			SET @Where = @Where + ' AND pv.FutureChequeDate is not null AND ( ( pv.Status = ''0'' AND pv.VoucherStatus = ''0'') OR ( pv.Status = ''0'' AND pv.VoucherStatus = ''1'') )'
			END
		 ELSE IF (@DataFor = 'NONREF')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''0'''
			END
		 ELSE IF (@DataFor = 'REFAPR')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.VoucherStatus = ''1'''
			END
		 ELSE IF (@DataFor = 'NONREFAPR')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.VoucherStatus = ''1'''
			END
		 ELSE IF (@DataFor = 'REFREJ')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.VoucherStatus = ''2'''
			END
		 ELSE IF (@DataFor = 'NONREFREJ')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.VoucherStatus = ''2'''
			END
		ELSE IF (@DataFor = 'INTRA')
			BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''3''  AND ((CAST(pv.Payment_Mode AS int) < 3 and cast(pv.VoucherStatus as int)  != 0 ) or (CAST(pv.Payment_Mode AS int) = 3 and cast(pv.VoucherStatus as int)  = 1 and cast(pv."status" as int) > 0 ) or (CAST(pv.Payment_Mode AS int) = 3 and cast(pv.VoucherStatus as int)  = 2 and cast(pv."status" as int) = 0 ))'
			END

		END

IF(@filter !='')
BEGIN
SET @SQLCountQuery = 'SELECT @x = COUNT(*) from PayVoucher pv 
					inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					'+@PaymentHeadJoin+'
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType'+@Where+' 
					AND (
						pv.PV_No LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR pv.Voucher_Date LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR VoucherType.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR PaymentHead.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR PaymentTo.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Amount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR VoucherStatus.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR payMode.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR ReasonCode.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Cheque_Date LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Remarks LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.BillNumber LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR BA.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR Clients.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CaseLevel.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						)'

exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out


SET @SQLQuery = 'Select 
					pv.Voucher_No,
					pv.PV_No,
					FORMAT (pv.Voucher_Date, ''dd/MM/yyyy'') as Voucher_Date,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = ''0'' then ''PENDING'' else VoucherStatus.Mst_Desc end as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = ''0'' then ''Voucher Created'' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					payMode.Mst_Desc as PaymentModeName,
					pv.Amount,
					pv.CaseInvoices,
					CASE WHEN pv.VoucherType = ''1'' THEN 
						case when LEFT(CC.OfficeFileNo,1) = ''M'' then ''Muscat'' else ''Salalah'' end 
					ELSE
						case when Emp.LocationCode = ''MCT-M'' then ''Muscat'' else ''Salalah'' end 
					END as LocationCode,
					pv.Payment_Head,
					case when pv.Payment_Head = ''0'' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Payment_To,
					case when pv.Payment_To = ''0'' then null else PaymentTo.Mst_Desc end as PaymentToName,
					FORMAT (pv.Cheque_Date, ''dd/MM/yyyy'') as W_D_Date,
					FORMAT (pv.FutureChequeDate, ''dd/MM/yyyy'') as FutureChequeDate,
					pv.TransReasonCode,
					case when pv.TransReasonCode = ''0'' then null else ReasonCode.Mst_Desc end as TransReasonName,pv.Remarks,pv.BillNumber,
					BA.Mst_Desc as "W_D_BANK",CC.OfficeFileNo,
					case when CC.ClientCode = ''0'' then null else Clients.Mst_Desc end as ClientName,CC.Defendant,
					case when CC.CaseLevelCode = ''0'' then null else CaseLevel.Mst_Desc end as CaseLevel, null as REF_PAID,
					DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) AS DaysLeft,pv.Payment_Head_Remarks as RejectReason, pv.PDCRefNo,pv.SpecialNotification,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount
					,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					'+@PaymentHeadJoin+'
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType'+@Where+' 
					AND (
						pv.PV_No LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR pv.Voucher_Date LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR VoucherType.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR PaymentHead.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR PaymentTo.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Amount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR VoucherStatus.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR payMode.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
						OR ReasonCode.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Cheque_Date LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.Remarks LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR pv.BillNumber LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR BA.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR Clients.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						OR CaseLevel.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
						) order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
END
ELSE
BEGIN
SET @SQLCountQuery = 'SELECT @x = COUNT(*) from PayVoucher pv 
					inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					'+@PaymentHeadJoin+'
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType'+@Where

exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out


SET @SQLQuery = 'Select 
					pv.Voucher_No,
					pv.PV_No,
					FORMAT (pv.Voucher_Date, ''dd/MM/yyyy'') as Voucher_Date,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.VoucherStatus,
					case when pv.VoucherStatus = ''0'' then ''PENDING'' else VoucherStatus.Mst_Desc end  as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = ''0'' then ''Voucher Created'' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					payMode.Mst_Desc as PaymentModeName,
					pv.Amount,
					pv.CaseInvoices,
					CASE WHEN pv.VoucherType = ''1'' THEN 
						case when LEFT(CC.OfficeFileNo,1) = ''M'' then ''Muscat'' else ''Salalah'' end 
					ELSE
						case when Emp.LocationCode = ''MCT-M'' then ''Muscat'' else ''Salalah'' end 
					END as LocationCode,
					pv.Payment_Head,
					case when pv.Payment_Head = ''0'' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Payment_To,
					case when pv.Payment_To = ''0'' then null else PaymentTo.Mst_Desc end as PaymentToName,
					FORMAT (pv.Cheque_Date, ''dd/MM/yyyy'') as W_D_Date,
					FORMAT (pv.FutureChequeDate, ''dd/MM/yyyy'') as FutureChequeDate,
					pv.TransReasonCode,
					case when pv.TransReasonCode = ''0'' then null else ReasonCode.Mst_Desc end as TransReasonName,pv.Remarks,pv.BillNumber,
					BA.Mst_Desc as "W_D_BANK",CC.OfficeFileNo,
					case when CC.ClientCode = ''0'' then null else Clients.Mst_Desc end as ClientName,CC.Defendant,
					case when CC.CaseLevelCode = ''0'' then null else CaseLevel.Mst_Desc end as CaseLevel, null as REF_PAID,
					DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) AS DaysLeft,pv.Payment_Head_Remarks as RejectReason, pv.PDCRefNo,pv.SpecialNotification,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount
					,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from PayVoucher pv 
				inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					'+@PaymentHeadJoin+'
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType'+@Where+' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
END
print @SQLQuery
exec (@SQLQuery)


GO
