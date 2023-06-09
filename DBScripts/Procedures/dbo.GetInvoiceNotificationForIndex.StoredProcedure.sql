SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetInvoiceNotificationForIndex]
@UserName varchar(5),
@DataFor varchar(15),
@Location varchar(1),
@Pageno INT=1,
@filter VARCHAR(500)='',
@pagesize INT=20,  
@Sorting VARCHAR(4000)='pv.PV_No',
@SortOrder VARCHAR(500)='desc' 
AS
SET NOCOUNT ON;
DECLARE
@Where				NVARCHAR(MAX) = ' where 1=1',
@FilterText			NVARCHAR(MAX) = '',
@FinalQuery			NVARCHAR(MAX),
@FetchLimit			NVARCHAR(MAX),
@TableColumn		NVARCHAR(MAX),
@TableQuery			NVARCHAR(MAX),
@QueryFirstPart		NVARCHAR(MAX) = 'SELECT * FROM ( SELECT ROW_NUMBER() OVER (order by DaysCounterUpdateDate,sortDate) as RowNum, * from ( SELECT '

BEGIN
IF (@DataFor = 'CASE-TABLE')
BEGIN
	SET @TableColumn = ' CaseId,OfficeFN,DaysCounter,COURT,ClientName,Defendant,AccountContractNo,CodeNumber,RegistrationDate,ReasonDesc, UIDENT,UpdateDate,DaysCounterUpdateDate,sortDate,count(*) OVER() AS TotalRecords FROM ( '
	SET @TableQuery  =
					   ' Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.RegistrationDate),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   CCE.CourtRefNo as CodeNumber,FORMAT (CCE.RegistrationDate, ''dd/MM/yyyy'') as RegistrationDate,''PRIMARY REGISTERED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''3'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate
											   ,CCE.RegistrationDate AS sortDate
										FROM CourtCases as CC
										inner join CourtCasesDetails as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''3'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo)
										where CONVERT(DATE,CCE.RegistrationDate) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''3''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.RegistrationDate),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   CCE.CourtRefNo as CodeNumber,FORMAT (CCE.RegistrationDate, ''dd/MM/yyyy'') as RegistrationDate,''APPEAL REGISTERED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''4'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											   ,CCE.RegistrationDate AS sortDate
										FROM CourtCases as CC
										inner join CourtCasesDetails as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''4'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo)
										where CONVERT(DATE,CCE.RegistrationDate) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''4''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.RegistrationDate),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   CCE.CourtRefNo as CodeNumber,FORMAT (CCE.RegistrationDate, ''dd/MM/yyyy'') as RegistrationDate,''SUPREME REGISTERED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''5'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											  ,CCE.RegistrationDate AS sortDate
										FROM CourtCases as CC
										inner join CourtCasesDetails as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''5'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.CourtRefNo)
										where CONVERT(DATE,CCE.RegistrationDate) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''5''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.RegistrationDate),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   CCE.EnforcementNo as CodeNumber,FORMAT (CCE.RegistrationDate, ''dd/MM/yyyy'') as RegistrationDate,''ENFORCEMENT REGISTERED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''1'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.EnforcementNo) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
												,CCE.RegistrationDate AS sortDate
										FROM CourtCases as CC
										inner join CourtCasesEnforcements as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''1'' + FORMAT(CCE.RegistrationDate, ''dd/MM/yyyy'') + CCE.EnforcementNo)
										where CONVERT(DATE,CCE.RegistrationDate) >= ''2022-01-25''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, ISNULL(CCE.ArrestOrderIssueDate,ISNULL(CCE.UpdatedOn,CCE.CreatedOn))),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   CCE.ArrestOrderNo as CodeNumber,FORMAT (ISNULL(CCE.ArrestOrderIssueDate,ISNULL(CCE.UpdatedOn,CCE.CreatedOn)), ''dd/MM/yyyy'') as RegistrationDate,''ARREST ORDER ISSUED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''2'' + CCE.ArrestOrderNo) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate
											,ISNULL(CCE.ArrestOrderIssueDate,ISNULL(CCE.UpdatedOn,CCE.CreatedOn)) AS sortDate
										FROM CourtCases as CC
										inner join CourtCasesEnforcements as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''2'' + CCE.ArrestOrderNo)
										where CONVERT(DATE,ISNULL(CCE.ArrestOrderIssueDate,ISNULL(CCE.UpdatedOn,CCE.CreatedOn))) >= ''2022-01-25''
										AND   CCE.ArrestOrderNo is not null
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.JudgementReceive),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   null as CodeNumber,FORMAT (CCE.JudgementReceive, ''dd/MM/yyyy'') as RegistrationDate,''PRIMARY JUDGEMENT RECEIVED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''6'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''PRIMARY JUDGEMENT RECEIVED'') as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											   ,CCE.JudgementReceive  AS sortDate
										FROM CourtCases as CC
										inner join JudgementDetailView as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''6'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''PRIMARY JUDGEMENT RECEIVED'')
										where CONVERT(DATE,CCE.JudgementReceive) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''3''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.JudgementReceive),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   null as CodeNumber,FORMAT (CCE.JudgementReceive, ''dd/MM/yyyy'') as RegistrationDate,''APPEAL JUDGEMENT RECEIVED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''7'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''APPEAL JUDGEMENT RECEIVED'') as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											   ,CCE.JudgementReceive  AS sortDate
										FROM CourtCases as CC
										inner join JudgementDetailView as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''7'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''APPEAL JUDGEMENT RECEIVED'')
										where CONVERT(DATE,CCE.JudgementReceive) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''4''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.JudgementReceive),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   null as CodeNumber,FORMAT (CCE.JudgementReceive, ''dd/MM/yyyy'') as RegistrationDate,''SUPREME JUDGEMENT RECEIVED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''8'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''SUPREME JUDGEMENT RECEIVED'') as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											   ,CCE.JudgementReceive  AS sortDate
										FROM CourtCases as CC
										inner join JudgementDetailView as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''8'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''SUPREME JUDGEMENT RECEIVED'')
										where CONVERT(DATE,CCE.JudgementReceive) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''5''
										UNION ALL
										Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.JudgementReceive),dateadd(hour , 11,GETDATE())) AS DaysCounter,
											   case when CCE.CourtLocationid = ''0'' then null else CourtLocation.Mst_Desc end as COURT,
											   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,CC.AccountContractNo,
											   null as CodeNumber,FORMAT (CCE.JudgementReceive, ''dd/MM/yyyy'') as RegistrationDate,''ENFORCEMENT JUDGEMENT RECEIVED'' AS ReasonDesc,
											   HASHBYTES(''MD5'', ''9'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''ENFORCEMENT JUDGEMENT RECEIVED'') as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
											   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
											   ,CCE.JudgementReceive  AS sortDate
										FROM CourtCases as CC
										inner join JudgementDetailView as CCE on CCE.CaseId = cc.CaseId
										inner join MASTER_S as CourtLocation on CourtLocation.MstParentId = 4 and CourtLocation.Mst_Value = CCE.CourtLocationid
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''9'' + FORMAT(CCE.JudgementReceive, ''dd/MM/yyyy'') + ''ENFORCEMENT JUDGEMENT RECEIVED'')
										where CONVERT(DATE,CCE.JudgementReceive) >= ''2022-01-25''
										and   CCE.CaseLevelCode = ''6'' '	
	IF(@filter !='')
	  BEGIN
		SET @FilterText = '
						AND (
							OfficeFN LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
							OR Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR ClientName LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR COURT LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR ReasonDesc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR AccountContractNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							) '
	   END
END
ELSE IF (@DataFor = 'PV-TABLE' )
BEGIN
	SET @TableColumn = ' CaseId,OfficeFN,DaysCounter,COURT,ClientName,Defendant,PaymentFor,PaymentTo,Amount,VatAmount,TotalAmount,AccountContractNo,CodeNumber,RegistrationDate,ReasonDesc,UIDENT,UpdateDate,DaysCounterUpdateDate,sortDate,count(*) OVER() AS TotalRecords FROM ( '
	SET @TableQuery  = ' Select CC.CaseId, CC.OfficeFileNo as OfficeFN,DATEDIFF(DAY, dateadd(hour , 11, CCE.Cheque_Date),dateadd(hour , 11,GETDATE())) AS DaysCounter,
										   case when CCE.CourtType = ''0'' then null else CaseLevel.Mst_Desc end as COURT,
										   case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName, CC.Defendant,
										   case when CCE.Payment_Head = ''0'' then null else PaymentHead.Mst_Desc end as PaymentFor,
										   case when CCE.Payment_To = ''0'' then null else PaymentTo.Mst_Desc end as PaymentTo,
										   CCE.Amount,CCE.VatAmount,case when CCE.VatAmount is null then CCE.Amount else CCE.Amount + CCE.VatAmount end as TotalAmount,
										   CC.AccountContractNo,
										   CCE.PV_No as CodeNumber,FORMAT (CCE.Cheque_Date, ''dd/MM/yyyy'') as RegistrationDate,''PV PROCESSED'' AS ReasonDesc,
										   HASHBYTES(''MD5'', ''10'' + FORMAT(CCE.Cheque_Date, ''dd/MM/yyyy'') + CCE.PV_No) as UIDENT, FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
										   case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate 
										  ,CCE.Cheque_Date  AS sortDate
									FROM CourtCases as CC
									inner join PayVoucher as CCE on CCE.CaseId = cc.CaseId
									inner join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CCE.CourtType
									inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
									left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''10'' + FORMAT(CCE.Cheque_Date, ''dd/MM/yyyy'') + CCE.PV_No)
									inner join MASTER_S as PaymentHead on PaymentHead.MstParentId = case when CCE.VoucherType = 1 then 567 else 7 end and PaymentHead.Mst_Value = CCE.Payment_Head
									inner join v_PayTo as PaymentTo on PaymentTo.MstParentId = 214 and PaymentTo.Mst_Value = CCE.Payment_To
									where CONVERT(DATE,CCE.AuthorizeDate) >= ''2022-01-25''
									AND   CCE.Cheque_Date is not null
									AND   CCE.VoucherType = ''1''
									AND   CCE.VoucherStatus = ''1'' '

	IF(@filter !='')
	  BEGIN
		SET @FilterText = '
						AND (
							OfficeFN LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
							OR Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR ClientName LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR COURT LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR PaymentFor LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR PaymentTo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR TotalAmount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							) '
	  END
END
ELSE IF (@DataFor = 'TRANS-TABLE' )
BEGIN
	SET @TableColumn = ' CaseId,OfficeFN,DaysCounter,COURT,CurrentCaseLevel,ClientName,Defendant,UIDENT,UpdateDate,DaysCounterUpdateDate,RegistrationDate,CourtDecision,sortDate,TransSource,count(*) OVER() AS TotalRecords FROM ( '
	SET @TableQuery  = ' Select	CC.CaseId,CC.OfficeFileNo as OfficeFN,
										DATEDIFF(DAY, dateadd(hour , 11, CDH.CreatedOn),dateadd(hour , 11,GETDATE())) AS DaysCounter,
										CCD.CourtLocation AS COURT,
										case when CC.CaseLevelCode = ''0'' then null else CaseLevel.Mst_Desc end as CurrentCaseLevel,
										case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName,
										CC.Defendant,
										HASHBYTES(''MD5'', ''11'' + FORMAT(CDH.CurrentHearingDate, ''dd/MM/yyyy'') + CONVERT(VARCHAR, CDH.Id)) as UIDENT, 
										FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
										case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate,
										FORMAT(CDH.CreatedOn, ''dd/MM/yyyy'') as RegistrationDate,
										CC.CourtDecision,
										CDH.CreatedOn  AS sortDate,
										case when CDH.TransportationSource = ''0'' then null else TransSource.Mst_Desc end as TransSource
										FROM CourtCases as CC
										inner join CourtCasesDetailVW CCD on CCD.CaseId = CC.CaseId and CCD.CaseLevelCode = CC.CaseLevelCode
										inner join CourtDecisionHistory CDH on CDH.CaseId = CC.CaseId
										inner join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CC.CaseLevelCode
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join MASTER_S as TransSource on TransSource.MstParentId = 1385 and TransSource.Mst_Value = CDH.TransportationSource
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''11'' + FORMAT(CDH.CurrentHearingDate, ''dd/MM/yyyy'') + CONVERT(VARCHAR, CDH.Id))
										where cc.TransportationFee = ''1''
										and   CONVERT(DATE,CDH.CreatedOn) >= ''2022-08-29'' '

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							OfficeFN LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
							OR Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR ClientName LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR COURT LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR CurrentCaseLevel LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							) '

	END
END
ELSE IF (@DataFor = 'INV-TABLE' )
BEGIN
	SET @TableColumn = ' CaseId,OfficeFN,DaysCounter,COURT,ClientName,Defendant,UIDENT,UpdateDate,DaysCounterUpdateDate,ClosingNotes,SugestedDate,sortDate,count(*) OVER() AS TotalRecords FROM ( '
	SET @TableQuery  = ' Select	CC.CaseId,
										CC.OfficeFileNo as OfficeFN,
										DATEDIFF(DAY, dateadd(hour , 11, GETDATE()),dateadd(hour , 11,CC.SuggestedDate)) AS DaysCounter,
										case when CC.CaseLevelCode = ''0'' then null else CaseLevel.Mst_Desc end as COURT,
										case when CC.ClientCode = ''0'' then null else ClientName.Mst_Desc end as ClientName,
										CC.Defendant,
										HASHBYTES(''MD5'', ''12'' + FORMAT(CC.SuggestedDate, ''dd/MM/yyyy'') + CONVERT(VARCHAR, CC.CaseId)) as UIDENT, 
										FORMAT(INV.UpdateDate, ''dd/MM/yyyy'') as UpdateDate,
										case when INV.UpdateDate is null then -1 else DATEDIFF(DAY, dateadd(hour , 11, INV.UpdateDate),dateadd(hour , 11,GETDATE())) end AS DaysCounterUpdateDate,
										CC.ClosingNotes,
										FORMAT(CC.SuggestedDate, ''dd/MM/yyyy'') as SugestedDate,
										CC.SuggestedDate  AS sortDate
										FROM CourtCases as CC
										inner join MASTER_S as CaseLevel on CaseLevel.MstParentId = 15 and CaseLevel.Mst_Value = CC.CaseLevelCode
										inner join MASTER_S as ClientName on ClientName.MstParentId = 241 and ClientName.Mst_Value = CC.ClientCode
										left join InvoiceNotification as INV on INV.[UID] = HASHBYTES(''MD5'', ''12'' + FORMAT(CC.SuggestedDate, ''dd/MM/yyyy'') + CONVERT(VARCHAR, CC.CaseId))
										where cc.FinalOnvoiceOnHold = ''1'' '

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							OfficeFN LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
							OR Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR ClientName LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR COURT LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							OR SugestedDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
							) '
	END
END

BEGIN SET @Where = @Where + ' AND DaysCounterUpdateDate <= 60 AND  (LEFT(OfficeFN,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') ' END

IF(@pagesize > 0)
	BEGIN
		SET @pagesize = @pagesize + @Pageno
		SET @Pageno = @Pageno + 1
		SET @FetchLimit = 'WHERE RowNum >= '+CONVERT(varchar,@Pageno)+' AND RowNum <= '+CONVERT(varchar,@pagesize)+' ORDER BY DaysCounterUpdateDate,RowNum '
	END
ELSE
	BEGIN	
		SET @FetchLimit = ' ' 
	END

BEGIN	
	SET @FinalQuery = @QueryFirstPart + @TableColumn + @TableQuery + ' ) AS DAT ' + @Where + @FilterText + ' ) AS RowConstrainedResult ) AS FINAL ' + @FetchLimit
END

print @FinalQuery
exec (@FinalQuery)

END
GO
