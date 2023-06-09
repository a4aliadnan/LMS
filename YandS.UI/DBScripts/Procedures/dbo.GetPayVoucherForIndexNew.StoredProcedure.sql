SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetPayVoucherForIndexNew]
@UserName varchar(5),
@DataFor varchar(15),
@Pageno INT=1,
@filter VARCHAR(500)='',
@pagesize INT=20,  
@Sorting VARCHAR(500)='pv.PV_No',
@SortOrder VARCHAR(500)='desc' 
AS
DECLARE 
@SelectColumns	NVARCHAR (MAX),
@JoinTables		NVARCHAR (MAX),
@Where			NVARCHAR (MAX) = ' where 1=1',
@FilterText		NVARCHAR (MAX) = '',
@Role			VARCHAR(100),
@LocationId		VARCHAR(5),
@UserId			INT,
@RoleCount		INT,
@OrderBy		NVARCHAR (MAX),
@FetchLimit		NVARCHAR (MAX),
@SummaryQuery	NVARCHAR (MAX),
@FinalQuery		NVARCHAR (MAX),
@MuscatTTL		DECIMAL(16,3),
@SalalahTTL		DECIMAL(16,3),
@SqlCount		INT,
@SQLCountQuery	NVARCHAR (MAX)

BEGIN
SET NOCOUNT ON;

SET @LocationId = (select LocationCode from HR_Employee_s where EmployeeNumber = @UserName)
SET @UserId = (select UserId from USERS where UserName = @UserName)
SET @RoleCount = (select count(*) from LNK_USER_ROLE where UserId = @UserId and RoleId in (4)) -- 'VoucherApproval'

IF(@Sorting ='13')
BEGIN
 SET @Sorting = 'dbo.fnMixSort(pv.PV_No)'
 SET @OrderBy = ' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
END
ELSE IF(@Sorting ='18')
BEGIN
 SET @Sorting = 'pv.Voucher_Date'
 SET @OrderBy = ' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
END
ELSE
BEGIN
 SET @OrderBy = ' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
END

SET @SelectColumns = 'DATEDIFF(DAY, pv.CreatedOn, GETDATE()) AS DaysCounter,
					case when pv.VoucherType = 1 then
						 case when left(CC.OfficeFileNo,1) = ''M'' then ''Muscat'' else ''Salalah'' end
						 else 
						 case when Emp.LocationCode = ''MCT-M'' then ''Muscat'' else ''Salalah'' end
					end as LocationCode,
					CC.OfficeFileNo,
					case when CC.ClientCode = ''0'' then null else Clients.Mst_Desc end as ClientName,
					CC.Defendant,
					case when CC.CaseLevelCode = ''0'' then null else CaseLevel.Mst_Desc end as CaseLevel,
					case when pv.Payment_Head = ''0'' then null else PaymentHead.Mst_Desc end as PaymentHeadName,
					pv.Remarks,
					case when pv.Payment_To = ''0'' then null else PaymentTo.Mst_Desc end as PaymentToName,
					pv.BillNumber,
					pv.Amount,
					VoucherType.Mst_Desc as VoucherTypeName,
					pv.PV_No,
					DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) AS DaysLeft,
					FORMAT (pv.Cheque_Date, ''dd/MM/yyyy'') as W_D_Date,
					FORMAT (pv.FutureChequeDate, ''dd/MM/yyyy'') as FutureChequeDate,
					BA.Mst_Desc as "W_D_BANK",
					FORMAT (pv.Voucher_Date, ''dd/MM/yyyy'') as Voucher_Date,
					payMode.Mst_Desc as PaymentModeName,
					pv.Payment_Head_Remarks as RejectReason,
					null as REF_PAID,
					pv.Voucher_No,
					pv.VoucherStatus,
					case when pv.VoucherStatus = ''0'' then ''PENDING'' else VoucherStatus.Mst_Desc end as VoucherStatusName,
					pv."status" as "Status",
					case when pv."status" = ''0'' then ''Voucher Created'' else Stats.Mst_Desc end as StatusName,
					pv.Payment_Mode,
					pv.CaseInvoices,
					pv.Payment_Head,
					pv.Payment_To,
					pv.TransReasonCode,
					case when pv.TransReasonCode = ''0'' then null else ReasonCode.Mst_Desc end as TransReasonName,
					pv.PDCRefNo,
					pv.SpecialNotification,pv.VatAmount,case when pv.VatAmount is null then pv.Amount else pv.Amount + pv.VatAmount end as TotalAmount'
SET @JoinTables =	' 
					inner join MASTER_S as payMode on payMode.MstParentId = 8 and payMode.Mst_Value = pv.Payment_Mode
					inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
					inner join MASTER_S as VoucherStatus on VoucherStatus.MstParentId = 228 and VoucherStatus.Mst_Value = pv.VoucherStatus
					inner join MASTER_S as Stats on Stats.MstParentId = 10 and Stats.Mst_Value = pv."Status"
					inner join USERS as U on pv.CreatedBy = U.UserId
					inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = case when pv.VoucherType = 1 then 567 else 7 end and PaymentHead.Mst_Value = pv.Payment_Head
					inner join MASTER_S as ReasonCode on ReasonCode.MstParentId = 402 and ReasonCode.Mst_Value = pv.TransReasonCode
					inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = pv.Payment_To
					inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
					inner join Link_Bank_Account_View BA on BA.LinkId = pv.Debit_Account
					left join CourtCases CC on CC.CaseId = pv.CaseId
					left join MASTER_S as Clients on Clients.MstParentId = 241 and Clients.Mst_Value = CC.ClientCode
					left join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = pv.CourtType 
					'


IF(@pagesize > 0)
	BEGIN
		SET @FetchLimit = ' OFFSET '+CONVERT(varchar,@Pageno)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
	END
ELSE
	BEGIN
		SET @FetchLimit = ' '
	END
PRINT @RoleCount
IF (@RoleCount > 0)
	BEGIN
		IF (@DataFor = 'PENDING')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND pv.Status != ''-1'' AND pv.VoucherStatus = ''0'' AND (( pv.FutureChequeDate IS NOT NULL AND DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) <= 5) OR (pv.FutureChequeDate IS NULL)) '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'REFAPPROVED')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''1'' '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0), @SalalahTOTAL = ISNULL(sum(case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0) from PayVoucher as pv inner join CourtCases CC on CC.CaseId = pv.CaseId ' +@Where
		END
		ELSE IF (@DataFor = 'NRAPPROVED')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''1'' '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0), @SalalahTOTAL = ISNULL(sum(case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0) from PayVoucher as pv inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'NEWPDC')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND ( ( pv.Status = ''0'' AND pv.VoucherStatus = ''0'') OR ( pv.Status = ''0'' AND pv.VoucherStatus = ''1'') ) AND DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) > 5 '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'REJECT')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND (pv.Status = ''-1'' OR pv.VoucherStatus = ''2'') '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
	END
ELSE
	BEGIN
		IF (@DataFor = 'REFAPPROVED')
			BEGIN	SET @Where = @Where + ' AND left(cc.OfficeFileNo,1)  = right('''+@LocationId+''',1) ' END
		ELSE
			BEGIN	SET @Where = @Where + ' AND Emp.LocationCode = '''+@LocationId+''' ' END
		
		IF (@DataFor = 'PENDING')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND (( pv.Status = ''0'' AND pv.VoucherStatus = ''0'') OR ( pv.Status = ''0'' AND pv.VoucherStatus = ''1'')) AND (( pv.FutureChequeDate IS NOT NULL AND DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) <= 5) OR (pv.FutureChequeDate IS NULL)) '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'REFAPPROVED')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''1'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''1'' '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0), @SalalahTOTAL = ISNULL(sum(case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0) from PayVoucher as pv inner join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'NRAPPROVED')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType = ''2'' AND pv.Status != ''-1'' AND pv.VoucherStatus = ''1'' '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0), @SalalahTOTAL = ISNULL(sum(case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end),0) from PayVoucher as pv inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'NEWPDC')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND ( ( pv.Status = ''0'' AND pv.VoucherStatus = ''0'') OR ( pv.Status = ''0'' AND pv.VoucherStatus = ''1'') ) AND DATEDIFF(DAY, GETDATE(),pv.FutureChequeDate) > 5 '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
		ELSE IF (@DataFor = 'REJECT')
		BEGIN
			 SET @Where = @Where + ' AND pv.VoucherType IN (''1'',''2'') AND (pv.Status = ''-1'' OR pv.VoucherStatus = ''2'') '
			 SET @SummaryQuery = 'select @MuscatTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''M'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''MCT-M'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0), @SalalahTOTAL = ISNULL(sum(case when pv.VoucherType = 1 then case when left(cc.OfficeFileNo,1) = ''S'' then pv.Amount + ISNULL(pv.VatAmount,0) end else case when Emp.LocationCode = ''SAL-S'' then pv.Amount + ISNULL(pv.VatAmount,0) end end),0) from PayVoucher as pv left join CourtCases CC on CC.CaseId = pv.CaseId inner join USERS as U on pv.CreatedBy = U.UserId inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName ' +@Where
		END
	END

IF(@filter !='')
BEGIN
	SET @FilterText = '
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
						) '

	SET @SQLCountQuery = 'SELECT @x = COUNT(*) from PayVoucher pv'+@JoinTables + @Where + @FilterText
END
ELSE
BEGIN
 SET @SQLCountQuery = 'SELECT @x = COUNT(*) from PayVoucher pv'+@JoinTables + @Where
END


SET @FinalQuery = 'Select '+@SelectColumns+' from PayVoucher pv '+@JoinTables + @Where + @FilterText + @OrderBy + @FetchLimit

--print @FinalQuery
exec (@FinalQuery)
BEGIN
	exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out
	exec sp_executesql @SummaryQuery, N'@MuscatTOTAL decimal(16,3) out,@SalalahTOTAL decimal(16,3) out', @MuscatTTL out,@SalalahTTL out
END
--print @SQLCountQuery
--print @SummaryQuery

SELECT @SqlCount as recordsTotal,@MuscatTTL as MuscatTTL,@SalalahTTL as SalalahTTL
END
GO
