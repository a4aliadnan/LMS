SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [CASE_MANAGEMENT-GET_SUPREME]
@Pageno				INT=1,
@filter				NVARCHAR(500)='',
@pagesize			INT=20,  
@Sorting			NVARCHAR(500)='CC.OfficeFileNo',
@SortOrder			NVARCHAR(500)='desc',
@UserName			NVARCHAR(5)=null,
@DataFor			NVARCHAR(25)=null,
@Location			NVARCHAR(1)=null
AS
BEGIN
DECLARE		
@SQLQuery			NVARCHAR(MAX),
@SQLSummaryQuery	NVARCHAR(MAX),
@OrderByClause		NVARCHAR(MAX),
@FinalQuery			NVARCHAR(MAX),
@FilterText			NVARCHAR(MAX) = '',
@DataForFilter		NVARCHAR(500)	= '',
@From				INT				= @pageno

		IF(@filter !='')
		BEGIN
			SET @FilterText = ' and OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
									or ClientName like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
									or DefClientName like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or AccountContractNo like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or ClientFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or CourtRefNo like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or COURT like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or case when CaseStatus = ''2'' then ''CLOSE'' else CaseLevelName end like ''%'+CONVERT(NVARCHAR,@filter)+'%''
									or CaseStatusName like ''%'+CONVERT(NVARCHAR,@filter)+'%''
								   '

		END

	 	 CREATE TABLE #T2 (
					CaseId int,OfficeFileNo varchar(10),ClientName varchar(200),DefClientName varchar(200),AgainstName varchar(50),
					ReceptionDate DATETIME2,AccountContractNo varchar(100),ClientFileNo varchar(100),CaseTypeName varchar(100),
					CaseLevelCode varchar(10),CaseLevelName varchar(100),CaseStatus varchar(10),CaseStatusName varchar(100),
					ToBeRegisterDays int,CourtRefNo varchar(100),COURT varchar(100),CurrentHearingDate DATETIME2,CourtDecision nvarchar(max),NextHearingDate DATETIME2,RegistrationDate DATETIME2
				 )

		SET @SQLQuery = 'SELECT CC.CaseId,CC.OfficeFileNo,ClientMas.Mst_Desc as ClientName,CC.Defendant as DefClientName,AgainstMas.Mst_Desc as AgainstName
						,CC.ReceptionDate,CC.AccountContractNo,CC.ClientFileNo,CaseTypeMas.Mst_Desc as CaseTypeName,CC.CaseLevelCode
						,CaseLevelMas.Mst_Desc as CaseLevelName,CC.CaseStatus,CaseStatusMas.Mst_Desc as CaseStatusName,
						0 ToBeRegisterDays,
						null as CourtRefNo,null as COURT,CC.CurrentHearingDate,CC.CourtDecision,CC.NextHearingDate, null as RegistrationDate
						from CourtCases as CC
						join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
						join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
						join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
						join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
						join MASTER_S CaseStatusMas on CC.CaseStatus = CaseStatusMas.Mst_Value and CaseStatusMas.MstParentId = 252 
						where	CC.CaseStatus = ''1'' 
						AND		CC.CaseLevelCode = ''5''
						AND		CC.StopEnfRequest = ''2''
						and		(LEFT(CC.OfficeFileNo,1) = ''' + @Location + ''' OR ''' + @Location + ''' = ''A'')'

		SET @OrderByClause = ' order by ToBeRegisterDays desc OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'

		-- SET ANSI_WARNINGS OFF (FOR CHECKING TRUNCATION
			INSERT into #T2 execute (@SQLQuery )
		-- SET ANSI_WARNINGS ON

			UPDATE  #T2
			SET		CourtRefNo = CourtCasesDetailVW.CourtRefNo, COURT = CourtCasesDetailVW.CourtLocation,
					 ToBeRegisterDays = case when CourtCasesDetailVW.RegistrationDate is not null then DATEDIFF(DAY, dateadd(hour , 11, CourtCasesDetailVW.RegistrationDate),dateadd(hour , 11,GETDATE())) end,
					 RegistrationDate = CourtCasesDetailVW.RegistrationDate
			FROM	#T2  
			JOIN    CourtCasesDetailVW on CourtCasesDetailVW.CaseId = #T2.CaseId and CourtCasesDetailVW.CaseLevelCode = #T2.CaseLevelCode
			COLLATE DATABASE_DEFAULT --(DIFFERENT COLLATE FIX)

		SET @SQLSummaryQuery = 'SELECT COUNT(*) as recordsTotal, count(case when left(OfficeFileNo,1) = ''M'' then 1 end) MCTRecords 
			, count(case when left(OfficeFileNo,1) = ''S'' then 1 end) SLLRecords
			from #T2'+ @FilterText

		SET @FinalQuery = 'SELECT *,count(*) OVER() AS TotalRecords	FROM #T2 where 1=1 ' + @FilterText + @OrderByClause

		print @FinalQuery
		exec (@FinalQuery)

		print @SQLSummaryQuery
		exec (@SQLSummaryQuery)

		drop table #T2


	END
GO
