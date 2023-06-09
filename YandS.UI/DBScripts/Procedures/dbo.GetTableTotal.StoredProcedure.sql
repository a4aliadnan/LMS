SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetTableTotal]
   @DataFor varchar(10),
   @Location varchar(1)
AS
DECLARE @Where VARCHAR(5000) = ' where 1=1'
DECLARE @SQLQuery NVARCHAR (MAX)
BEGIN
	IF (@DataFor = 'TOBEREG')
		BEGIN
		 SELECT COUNT(*) AS TABLE_TOTAL FROM CourtCases CC where CC.CaseLevelCode = '1' and  (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A') 
		END
	ELSE
		BEGIN
			IF (@DataFor = 'URGENT')
				BEGIN
				 SET @Where = @Where + ' AND DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) >= 0 AND CR.FileStatus != ''10'''
			 
				END
			 ELSE IF (@DataFor = 'SUPREME')
				BEGIN
				SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0  AND CR.ActionLevel = ''3'') OR (CR.UrgentCaseDays is null AND CR.ActionLevel = ''3''))'
			
				END
			 ELSE IF (@DataFor = 'APPEAL')
				BEGIN
				 SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0  AND CR.ActionLevel = ''2'' ) OR (CR.UrgentCaseDays is null AND CR.ActionLevel = ''2''))'
			 
				END
			 ELSE IF (@DataFor = 'NEWCASE')
				BEGIN
				 SET @Where = @Where + ' AND CR.FileStatus != ''10'' AND ((DATEDIFF(DAY,(DATEADD(DAY, CR.UrgentCaseDays, CR.CreatedOn)), GETDATE()) < 0 AND CR.ActionLevel in(''1'',''4'')) OR (CR.UrgentCaseDays is null AND CR.ActionLevel in(''1'',''4''))) '
			 
				END
			 ELSE IF (@DataFor = 'STOPREG')
				BEGIN
				 SET @Where = @Where + ' AND DATEDIFF(DAY, CR.ActionDate, GETDATE()) <= 90 AND CR.FileStatus = ''10'''
			 
				END
		SET @Where = @Where + ' and  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'') '

		SET @SQLQuery = 'SELECT COUNT(*) AS TABLE_TOTAL from CaseRegistrations CR Inner Join CourtCases as CC on cc.CaseId = CR.CaseId '+@Where
		
		print @SQLQuery
		exec (@SQLQuery)

		END
END
GO
