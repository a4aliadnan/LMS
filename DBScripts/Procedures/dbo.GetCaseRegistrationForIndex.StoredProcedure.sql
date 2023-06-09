SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetCaseRegistrationForIndex]
@UserName varchar(5),
@DataFor varchar(15),
@Location varchar(1),
@FileStatus varchar(3),
@Pageno INT=1,
@filter VARCHAR(500)='',
@pagesize INT=20,  
@Sorting VARCHAR(500)='CC.OfficeFileNo',
@SortOrder VARCHAR(500)='desc' 
AS
DECLARE @Where VARCHAR(5000) = ' where 1=1'
DECLARE @Role VARCHAR(100)
DECLARE @FinalQuery NVARCHAR (MAX)
DECLARE @UserId int
DECLARE @RoleCount int

SET @UserId = (select UserId from USERS where UserName = @UserName)
SET @RoleCount = (select count(*) from LNK_USER_ROLE where UserId = @UserId and RoleId in (4)) -- 'VoucherApproval'


SET NOCOUNT ON;
DECLARE @SqlCount INT
DECLARE @From INT = @pageno
DECLARE @SQLQuery NVARCHAR (MAX)
DECLARE @SQLCountQuery NVARCHAR (MAX)
DECLARE @SelectDaysCount VARCHAR(MAX)

DECLARE @SQLSelectColumn NVARCHAR (MAX)

DECLARE @CheckStopRegistration INT = 0

SELECT @CheckStopRegistration = count(*) from CaseRegistrations where FileStatus = '11' and   IsDeleted = 0 and   DATEDIFF(DAY, UpdatedOn, GETDATE()) > 10
IF (@CheckStopRegistration > 0)
BEGIN
	update CaseRegistrations set IsDeleted = 1 where FileStatus = '11' and   IsDeleted = 0 and   DATEDIFF(DAY, UpdatedOn, GETDATE()) > 10
END

IF(@Sorting ='CC.OfficeFileNo')
BEGIN
 SET @Sorting = 'dbo.fnMixSort(CC.OfficeFileNo)'
END
IF(@DataFor = 'TOBEREG')
BEGIN
	SET @Where = @Where + ' AND CC.CaseLevelCode = ''1'' and  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') '
	IF(@filter !='')
		BEGIN
		
			SET @SQLCountQuery = 'SELECT @x = COUNT(*) from CourtCases as CC 
								join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241'+@Where+' 
								AND (
									CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									)'

			exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out

			IF(@pagesize > 0)
			BEGIN
			SET @SQLQuery = 'Select 
								 DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,
								 CC.CaseId
								,CC.OfficeFileNo
								,null as CourtRegistrationName
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
							from CourtCases as CC 
							join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241 '+@Where+' 
								AND (
									CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									) order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)' 
			END
			ELSE
			BEGIN
 			SET @SQLQuery = 'Select 
								 DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,
								 CC.CaseId
								,CC.OfficeFileNo
								,null as CourtRegistrationName
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
							from CourtCases as CC 
							join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241 '+@Where+' 
								AND (
									CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									) order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
			END

		END
	ELSE
		BEGIN
			SET @SQLCountQuery = 'SELECT @x = COUNT(*) from CourtCases as CC join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241'+@Where

			exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out

			IF(@pagesize > 0)
			BEGIN

			SET @SQLQuery = 'Select 
								 DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,
								 CC.CaseId
								,CC.OfficeFileNo
								,null as CourtRegistrationName
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'')  as ReceptionDate
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
							from CourtCases as CC 
							join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241 '+@Where+' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
			END
			ELSE
			BEGIN
			SET @SQLQuery = 'Select 
								 DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,
								 CC.CaseId
								,CC.OfficeFileNo
								,null as CourtRegistrationName
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'')  as ReceptionDate
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
							from CourtCases as CC 
							join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241 '+@Where+' order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
			END
		END
	END
ELSE
BEGIN
	BEGIN
		--IF (@DataFor = 'URGENT')
		--	BEGIN
		--	 SET @Where = @Where + ' AND DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) >= 0 AND CR.FileStatus != ''10'''
		--	 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.JudgementDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		-- ELSE IF (@DataFor = 'SUPREME')
		--	BEGIN
		--	SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0  AND CR.ActionLevel = ''3'') OR (CR.UrgentCaseDays is null AND CR.ActionLevel = ''3''))'
		--	SET @SelectDaysCount = ',DATEDIFF(DAY, CR.JudgementDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		-- ELSE IF (@DataFor = 'APPEAL')
		--	BEGIN
		--	 SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0  AND CR.ActionLevel = ''2'' ) OR (CR.UrgentCaseDays is null AND CR.ActionLevel = ''2''))'
		--	 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.JudgementDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		-- ELSE IF (@DataFor = 'NEWCASE')
		--	BEGIN
		--	 SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0 AND CR.ActionLevel in(''1'',''4'')) OR (CR.UrgentCaseDays is null AND CR.ActionLevel in(''1'',''4''))) '
		--	 SET @SelectDaysCount = ',DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		-- ELSE IF (@DataFor = 'PAYMENT')
		--	BEGIN
		--	 SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0 AND CR.ActionLevel in(''1'',''4'')) OR (CR.UrgentCaseDays is null AND CR.ActionLevel in(''1'',''4''))) '
		--	 SET @SelectDaysCount = ',DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		-- ELSE IF (@DataFor = 'STOPREG')
		--	BEGIN
		--	 SET @Where = @Where + ' AND DATEDIFF(DAY, CR.ActionDate, GETDATE()) <= 90 AND CR.FileStatus = ''10'''
		--	 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS DaysCounter,FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed '
		--	END
		--END
		--SET @Where = @Where + ' and  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') and  (CR.FileStatus = '''+@FileStatus+''' OR '''+@FileStatus+''' = ''A'') '
	     IF (@DataFor = 'SUPREME')
			BEGIN
			SET @Where = @Where + '  AND CR.FileStatus != ''8'' AND CR.ActionLevel = ''3'' '
			SET @SelectDaysCount = ',case when CR.FileStatus = ''11'' then DATEDIFF(DAY, CR.ActionDate, GETDATE()) else DATEDIFF(DAY, CR.JudgementDate, GETDATE()) end AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		 ELSE IF (@DataFor = 'APPEAL')
			BEGIN
			 SET @Where = @Where + '  AND CR.FileStatus != ''8'' AND CR.ActionLevel = ''2'' '
			 SET @SelectDaysCount = ',case when CR.FileStatus = ''11'' then DATEDIFF(DAY, CR.ActionDate, GETDATE()) else DATEDIFF(DAY, CR.JudgementDate, GETDATE()) end AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		 ELSE IF (@DataFor = 'NEWCASE')
			BEGIN
			 SET @Where = @Where + '  AND CR.FileStatus != ''8'' AND CR.ActionLevel = ''1'' '
			 SET @SelectDaysCount = ',case when CR.FileStatus = ''11'' then DATEDIFF(DAY, CR.ActionDate, GETDATE()) else DATEDIFF(DAY, CC.ReceptionDate, GETDATE()) end AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		 ELSE IF (@DataFor = 'PAYMENT')
			BEGIN
			 SET @Where = @Where + ' AND CR.FileStatus = ''8'' '
			 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		 ELSE IF (@DataFor = 'ENF-PRIMARY')
			BEGIN
			 SET @Where = @Where + ' AND CR.FileStatus != ''8'' AND CR.ActionLevel = ''4'' AND CR.EnforcementDispute in(''1'',''3'') '
			 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		 ELSE IF (@DataFor = 'ENF-APPEAL')
			BEGIN
			 SET @Where = @Where + ' AND CR.FileStatus != ''8'' AND CR.ActionLevel = ''4'' AND CR.EnforcementDispute in(''2'',''4'') '
			 SET @SelectDaysCount = ',DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS DaysCounter, FORMAT(CR.CreatedOn,''dd/MM/yyyy'') as CreatedOn,FORMAT(CR.UpdatedOn,''dd/MM/yyyy'') as UpdatedOn,DATEDIFF(DAY, CR.CreatedOn, GETDATE()) AS CreatedDaysPassed,DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed, DATEDIFF(DAY, CR.UpdatedOn, GETDATE()) AS UpdatedDaysPassed,case when CR.CourtDetailRegistered > 0 OR DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 then 1 else 0 end as IsUrgentCase, case when CR.Remarks is null then 0 else 1 end as IsMainRemarks '
			END
		END

		BEGIN
			IF (@DataFor = 'PAYMENT')
				BEGIN
					SET @Where = @Where + ' and CR.IsDeleted = 0 and  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') and 1 = (case when '''+@FileStatus+''' = ''0'' and PaymentDate is null then 1 when '''+@FileStatus+''' = ''1'' and PaymentDate is not null then 1 end )  '
				END
			 ELSE
				BEGIN
					SET @Where = @Where + ' and CR.IsDeleted = 0 and  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') and  (CR.FileStatus = '''+@FileStatus+''' OR '''+@FileStatus+''' = ''A'') '
				END
		END
	BEGIN
		IF(@filter !='')
		  BEGIN
			SET @SQLCountQuery = 'SELECT @x = COUNT(*) from CaseRegistrations CR
								 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
								 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
								 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
								 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
								 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
								 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788
								 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where+' 
								AND (
									Courts.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR CR.ActionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CR.JudgementDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR FileStatus.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
									OR ActLevel.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR EnfDispute.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR case when CR.FileStatus = ''4'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CR.ElectronicNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CR.CourtFeeAmount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									OR CR.CourtMessage LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
									)'

			exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out

			IF(@pagesize > 0)
				BEGIN
					SET @SQLQuery = 'Select 
										 CR.CaseId
 										,CR.CourtRegistration
										,CR.CaseRegistrationId
										,CR.FileStatus
										,CR.ActionLevel
										,CR.EnforcementDispute
										'+@SelectDaysCount+'
										,Courts.Mst_Desc as CourtRegistrationName
										,FORMAT(CR.ActionDate,''dd/MM/yyyy'') as ActionDate
										,CC.OfficeFileNo
										,ClientMas.Mst_Desc as ClientName
										,CC.Defendant as Defendant
										,FORMAT(CR.JudgementDate,''dd/MM/yyyy'') as JudgementDate
										,FileStatus.Mst_Desc as FileStatusName
										,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
										,ActLevel.Mst_Desc as ActionLevelName
										,EnfDispute.Mst_Desc as EnforcementDisputeName
										,case when CR.FileStatus = ''4'' then (SELECT Mst_Desc FROM MASTER_S WHERE MstParentId = 821 AND Mst_Value = CR.FileStatusRemarks) end as OnHoldReason
										,CR.ElectronicNo as ElectronicNo
										,CR.CourtFeeAmount as CourtFee
										,case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end as LawyerDetail
										,case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end as ReasonOfStopRegistration,CR.CourtMessage
										,FORMAT(CR.SendDate,''dd/MM/yyyy'') as SendDate,DATEDIFF(DAY, CR.SendDate, GETDATE()) AS SentCounter
										,FORMAT(CR.FirstReminderDate,''dd/MM/yyyy'') as FirstReminderDate,DATEDIFF(DAY, CR.FirstReminderDate, GETDATE()) AS ReminderCounter,CR.ReminderNo
										,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter,DATEDIFF(DAY, CR.CourtRequestDate, GETDATE()) AS CourtRequestCounter,CR.OfficeProcedure
										,DATEDIFF(DAY, CR.PaymentDate, GETDATE()) AS PaymentCounter,CR.AssignedTo,DATEDIFF(DAY, CR.AssignedDate, GETDATE()) AS AssignedCounter,CR.AdminFile
										,CCE.EnforcementNo as EnforcementNo
										,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter1,CR.OfficeProcedure as OfficeProcedure1
										,CR.DepartmentType,DepType.Mst_Desc as DepartmentTypeName
										,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
									from CaseRegistrations CR
													 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
													 left join CourtCasesEnforcements as CCE on cc.CaseId = CCE.CaseId
													 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
													 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
													 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
													 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
													 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
													 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where+' 
													AND (
														Courts.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR CR.ActionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.JudgementDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR FileStatus.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR ActLevel.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR EnfDispute.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''4'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.ElectronicNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.CourtFeeAmount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.CourtMessage LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														) order by case when ((CR.CourtDetailRegistered > 0 and CR.FileStatus != ''11'') OR (DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 and CR.FileStatus != ''11'')) then 1 when CR.FileStatus = ''11'' then -1 else 0 end desc, 7 desc, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
				END
			ELSE
				BEGIN
					SET @SQLQuery = 'Select 
										 CR.CaseId
 										,CR.CourtRegistration
										,CR.CaseRegistrationId
										,CR.FileStatus
										,CR.ActionLevel
										,CR.EnforcementDispute
										'+@SelectDaysCount+'
										,Courts.Mst_Desc as CourtRegistrationName
										,FORMAT(CR.ActionDate,''dd/MM/yyyy'') as ActionDate
										,CC.OfficeFileNo
										,ClientMas.Mst_Desc as ClientName
										,CC.Defendant as Defendant
										,FORMAT(CR.JudgementDate,''dd/MM/yyyy'') as JudgementDate
										,FileStatus.Mst_Desc as FileStatusName
										,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
										,ActLevel.Mst_Desc as ActionLevelName
										,EnfDispute.Mst_Desc as EnforcementDisputeName
										,case when CR.FileStatus = ''4'' then (SELECT Mst_Desc FROM MASTER_S WHERE MstParentId = 821 AND Mst_Value = CR.FileStatusRemarks) end as OnHoldReason
										,CR.ElectronicNo as ElectronicNo
										,CR.CourtFeeAmount as CourtFee
										,case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end as LawyerDetail
										,case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end as ReasonOfStopRegistration,CR.CourtMessage
										,FORMAT(CR.SendDate,''dd/MM/yyyy'') as SendDate,DATEDIFF(DAY, CR.SendDate, GETDATE()) AS SentCounter
										,FORMAT(CR.FirstReminderDate,''dd/MM/yyyy'') as FirstReminderDate,DATEDIFF(DAY, CR.FirstReminderDate, GETDATE()) AS ReminderCounter,CR.ReminderNo
										,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter,DATEDIFF(DAY, CR.CourtRequestDate, GETDATE()) AS CourtRequestCounter,CR.OfficeProcedure
										,DATEDIFF(DAY, CR.PaymentDate, GETDATE()) AS PaymentCounter,CR.AssignedTo,DATEDIFF(DAY, CR.AssignedDate, GETDATE()) AS AssignedCounter,CR.AdminFile
										,CCE.EnforcementNo as EnforcementNo
										,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter1,CR.OfficeProcedure as OfficeProcedure1
										,CR.DepartmentType,DepType.Mst_Desc as DepartmentTypeName
										,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
									from CaseRegistrations CR
													 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
													 left join CourtCasesEnforcements as CCE on cc.CaseId = CCE.CaseId
													 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
													 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
													 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
													 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
													 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
													 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where+'
													AND (
														Courts.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR CR.ActionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CC.OfficeFileNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR ClientMas.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CC.Defendant LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.JudgementDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR FileStatus.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR CC.ReceptionDate LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
														OR ActLevel.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR EnfDispute.Mst_Desc LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''4'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.ElectronicNo LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.CourtFeeAmount LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														OR CR.CourtMessage LIKE ''%'+CONVERT(VARCHAR,@filter)+'%''
														) order by case when ((CR.CourtDetailRegistered > 0 and CR.FileStatus != ''11'') OR (DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 and CR.FileStatus != ''11'')) then 1 when CR.FileStatus = ''11'' then -1 else 0 end desc, 7 desc, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
				END


			END
			ELSE
				BEGIN
					SET @SQLCountQuery = 'SELECT @x = COUNT(*) from CaseRegistrations CR
													 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
													 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
													 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
													 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
													 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
													 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
													 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where

					exec sp_executesql @SQLCountQuery, N'@x int out', @SqlCount out

			IF(@pagesize > 0)
				BEGIN
					SET @SQLQuery = 'Select 
								 CR.CaseId
 								,CR.CourtRegistration
								,CR.CaseRegistrationId
								,CR.FileStatus
								,CR.ActionLevel
								,CR.EnforcementDispute
								'+@SelectDaysCount+'
								,Courts.Mst_Desc as CourtRegistrationName
								,FORMAT(CR.ActionDate,''dd/MM/yyyy'') as ActionDate
								,CC.OfficeFileNo
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CR.JudgementDate,''dd/MM/yyyy'') as JudgementDate
								,FileStatus.Mst_Desc as FileStatusName
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
								,ActLevel.Mst_Desc as ActionLevelName
								,EnfDispute.Mst_Desc as EnforcementDisputeName
								,case when CR.FileStatus = ''4'' then (SELECT Mst_Desc FROM MASTER_S WHERE MstParentId = 821 AND Mst_Value = CR.FileStatusRemarks) end as OnHoldReason
								,CR.ElectronicNo as ElectronicNo
								,CR.CourtFeeAmount as CourtFee
								,case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end as LawyerDetail
								,case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end as ReasonOfStopRegistration,CR.CourtMessage
								,FORMAT(CR.SendDate,''dd/MM/yyyy'') as SendDate,DATEDIFF(DAY, CR.SendDate, GETDATE()) AS SentCounter
								,FORMAT(CR.FirstReminderDate,''dd/MM/yyyy'') as FirstReminderDate,DATEDIFF(DAY, CR.FirstReminderDate, GETDATE()) AS ReminderCounter,CR.ReminderNo
								,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter,DATEDIFF(DAY, CR.CourtRequestDate, GETDATE()) AS CourtRequestCounter,CR.OfficeProcedure
								,DATEDIFF(DAY, CR.PaymentDate, GETDATE()) AS PaymentCounter,CR.AssignedTo,DATEDIFF(DAY, CR.AssignedDate, GETDATE()) AS AssignedCounter,CR.AdminFile
								,CCE.EnforcementNo as EnforcementNo
								,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter1,CR.OfficeProcedure as OfficeProcedure1
								,CR.DepartmentType,DepType.Mst_Desc as DepartmentTypeName
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
								from CaseRegistrations CR
								 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
								 left join CourtCasesEnforcements as CCE on cc.CaseId = CCE.CaseId
								 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
								 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
								 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
								 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
								 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where+' order by case when ((CR.CourtDetailRegistered > 0 and CR.FileStatus != ''11'') OR (DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 and CR.FileStatus != ''11'')) then 1 when CR.FileStatus = ''11'' then -1 else 0 end desc, 7 desc, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
				END
			ELSE
				BEGIN
					SET @SQLQuery = 'Select 
								 CR.CaseId
 								,CR.CourtRegistration
								,CR.CaseRegistrationId
								,CR.FileStatus
								,CR.ActionLevel
								,CR.EnforcementDispute
								'+@SelectDaysCount+'
								,Courts.Mst_Desc as CourtRegistrationName
								,FORMAT(CR.ActionDate,''dd/MM/yyyy'') as ActionDate
								,CC.OfficeFileNo
								,ClientMas.Mst_Desc as ClientName
								,CC.Defendant as Defendant
								,FORMAT(CR.JudgementDate,''dd/MM/yyyy'') as JudgementDate
								,FileStatus.Mst_Desc as FileStatusName
								,FORMAT(CC.ReceptionDate,''dd/MM/yyyy'') as ReceptionDate
								,ActLevel.Mst_Desc as ActionLevelName
								,EnfDispute.Mst_Desc as EnforcementDisputeName
								,case when CR.FileStatus = ''4'' then (SELECT Mst_Desc FROM MASTER_S WHERE MstParentId = 821 AND Mst_Value = CR.FileStatusRemarks) end as OnHoldReason
								,CR.ElectronicNo as ElectronicNo
								,CR.CourtFeeAmount as CourtFee
								,case when CR.FileStatus = ''9'' then CR.FileStatusRemarks end as LawyerDetail
								,case when CR.FileStatus = ''10'' then CR.FileStatusRemarks end as ReasonOfStopRegistration,CR.CourtMessage
								,FORMAT(CR.SendDate,''dd/MM/yyyy'') as SendDate,DATEDIFF(DAY, CR.SendDate, GETDATE()) AS SentCounter
								,FORMAT(CR.FirstReminderDate,''dd/MM/yyyy'') as FirstReminderDate,DATEDIFF(DAY, CR.FirstReminderDate, GETDATE()) AS ReminderCounter,CR.ReminderNo
								,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter,DATEDIFF(DAY, CR.CourtRequestDate, GETDATE()) AS CourtRequestCounter,CR.OfficeProcedure
								,DATEDIFF(DAY, CR.PaymentDate, GETDATE()) AS PaymentCounter,CR.AssignedTo,DATEDIFF(DAY, CR.AssignedDate, GETDATE()) AS AssignedCounter,CR.AdminFile
								,CCE.EnforcementNo as EnforcementNo
								,DATEDIFF(DAY, CR.ActionDate, GETDATE()) AS ActionCounter1,CR.OfficeProcedure as OfficeProcedure1
								,CR.DepartmentType,DepType.Mst_Desc as DepartmentTypeName
								,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
								from CaseRegistrations CR
								 Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
								 left join CourtCasesEnforcements as CCE on cc.CaseId = CCE.CaseId
								 Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
								 Inner join MASTER_S ActLevel on CR.ActionLevel = ActLevel.Mst_Value and ActLevel.MstParentId = 785
								 Inner join MASTER_S EnfDispute on CR.EnforcementDispute = EnfDispute.Mst_Value and EnfDispute.MstParentId = 786
								 Inner join MASTER_S Courts on CR.CourtRegistration = Courts.Mst_Value and Courts.MstParentId = 4
								 Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 Inner join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822 '+@Where+' order by case when ((CR.CourtDetailRegistered > 0 and CR.FileStatus != ''11'') OR (DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) > 0 and CR.FileStatus != ''11'')) then 1 when CR.FileStatus = ''11'' then -1 else 0 end desc, 7 desc, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder
				END
			END
		END

END
print @SQLQuery
exec (@SQLQuery)
GO
