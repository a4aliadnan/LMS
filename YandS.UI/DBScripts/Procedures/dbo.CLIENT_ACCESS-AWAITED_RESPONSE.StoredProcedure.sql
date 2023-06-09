SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CLIENT_ACCESS-AWAITED_RESPONSE]
@Pageno				INT=1,
@filter				NVARCHAR(500)='',
@pagesize			INT=20,  
@Sorting			NVARCHAR(500)='CC.OfficeFileNo',
@SortOrder			NVARCHAR(500)='desc',
@UserName			NVARCHAR(15),
@DataFor			NVARCHAR(50),
@Location			NVARCHAR(1),
@ClientCode			NVARCHAR(3)
AS
DECLARE
@FilterText			NVARCHAR(MAX) = '',
@FinalQuery			NVARCHAR(MAX),
@FinalSummaryQuery	NVARCHAR(MAX),
@FetchLimit			NVARCHAR(MAX),
@TableColumn		NVARCHAR(MAX)  = ' CaseId,OfficeFileNo,Defendant,AccountContractNo,ClientFileNo,CaseLevelName,CurrentEnforcementLevel,COURT,CourtRefNo,DaysCounter,CourtDecision,Requirements,FirstEmailDate,CurrentHearingDate,NULL as FileStatusName ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
								FROM (
									SELECT CC.CaseId,CC.OfficeFileNo,CC.Defendant,CCE.CourtLocation as COURT,CNW.CASE_NO as CourtRefNo,
									DATEDIFF(DAY, dateadd(hour , 11, CC.CurrentHearingDate),dateadd(hour , 11,GETDATE())) as DaysCounter,
									CC.CourtDecision,CC.Requirements,CC.FirstEmailDate,CCE.CurrentEnforcementLevel as CurrentEnforcementLevel,
									CC.AccountContractNo,CC.ClientFileNo,CaseLevelMas.Mst_Desc as CaseLevelName,CC.CurrentHearingDate, CR.FileStatusName,count(*) OVER() AS TotalRecords
									from CourtCases as CC
									',

@SummaryQuery		NVARCHAR(MAX) = '
									SELECT
									COUNT(*) as recordsTotal,
									count(case when left(CC.OfficeFileNo,1) = ''M'' then 1 end) MCTRecords,
									count(case when left(CC.OfficeFileNo,1) = ''S'' then 1 end) SLLRecords
									from CourtCases as CC
									',
@JoinTables			NVARCHAR(MAX) = ' 
									Inner Join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
									left  Join CourtCasesDetailView CCE on CCE.CaseId = CC.CaseId
									Left  Join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
									Left  Join CaseRegistrationsVW CR on CR.CaseId = CC.CaseId
									Left  Join CaseNosVW CNW on CNW.CaseId = CC.CaseId
									',
@Where				NVARCHAR(MAX) = ' 
									where CC.CaseStatus = ''1''
									AND   CC.ClientCode = '''+@ClientCode+'''
									AND   CC.ClientReply = ''1''
									AND   (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN


	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by CurrentHearingDate desc) as RowNum, * 
    from 
	  ( 
	   SELECT '
	SET @TableQuery  = @SelectColumns+@JoinTables+@Where

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							CC.OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CaseLevelMas.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CC.Defendant like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CCE.CourtLocation like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CC.CourtDecision like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CC.Requirements like N''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CNW.CASE_NO like N''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CCE.CurrentEnforcementLevel like N''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CR.FileStatusName like N''%'+CONVERT(NVARCHAR,@filter)+'%''
							)
							'
	END


	IF(@pagesize > 0)
	BEGIN
		SET @pagesize = @pagesize + @Pageno
		SET @Pageno = @Pageno + 1
		SET @FetchLimit = 'WHERE RowNum >= '+CONVERT(varchar,@Pageno)+' AND RowNum <= '+CONVERT(varchar,@pagesize)
	END
	ELSE
	BEGIN	
		SET @FetchLimit = ' ' 
	END

BEGIN	
	SET @FinalQuery = @QueryFirstPart + @TableColumn + @TableQuery + @FilterText + 
			' ) AS DAT ' + 
			  @AdditionalWhere + 
	  ' ) AS RowConstrainedResult 
	) AS FINAL ' + @FetchLimit+' ORDER BY RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END


GO
