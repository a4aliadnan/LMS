SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CLIENT_ACCESS-SUSPENDED]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,OfficeFileNo,Defendant,SuspensionEndDate,DaysRemaining,COURT,CourtRefNo,SuspensionCause,JUDDecision,DaysCounter,CourtDecision ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
								FROM (
									SELECT CC.CaseId,CC.OfficeFileNo,CC.Defendant,
									case when CCE.CourtLocationid = ''0'' then null else LOC.Mst_Desc end as COURT,
									case when CCE.JUDDecisionId = ''0'' then null else JD.Mst_Desc end as JUDDecision,
									case when CCE.SuspensionCauseId = ''0'' then null else SC.Mst_Desc end as SuspensionCause,
									case when CCE.SuspensionPeriod > 0 then DATEDIFF(DAY, dateadd(hour , 11,GETDATE()),dateadd(day , CCE.SuspensionPeriod, CCE.ActionDate)) end as DaysRemaining,
									CNW.CASE_NO as CourtRefNo,CCE.SuspensionPeriod,
									DATEDIFF(DAY, dateadd(hour , 11, CC.CurrentHearingDate),dateadd(hour , 11,GETDATE())) as DaysCounter,
									dateadd(day , CCE.SuspensionPeriod, CCE.ActionDate) as SuspensionEndDate,CC.CourtDecision,count(*) OVER() AS TotalRecords
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
									Inner Join CourtCasesEnforcements CCE on CCE.CaseId = CC.CaseId
									Left  Join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
									Left  Join MASTER_S as JD on JD.Mst_Value = CCE.JUDDecisionId and JD.MstParentId = 1291
									Left  Join MASTER_S as SC on SC.Mst_Value = CCE.SuspensionCauseId and SC.MstParentId = 272
									Left  Join CaseNosVW CNW on CNW.CaseId = CC.CaseId
									',
@Where				NVARCHAR(MAX) = ' 
									where CC.CaseStatus = ''1''
									AND   CC.ClientCode = '''+@ClientCode+'''
									AND   CC.CaseLevelCode = ''6''
									AND   CCE.EnforcementlevelId = ''7''
									AND   CCE.EnforcementBy in (''0'', ''1'')
									AND   (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN


	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by DaysCounter) as RowNum, * 
    from 
	  ( 
	   SELECT '
	SET @TableQuery  = @SelectColumns+@JoinTables+@Where

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							CC.OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CC.Defendant like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR LOC.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CNW.CASE_NO like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CCE.SuspensionPeriod like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR DATEDIFF(DAY, dateadd(hour , 11,GETDATE()),dateadd(day , CCE.SuspensionPeriod, CCE.SuspensionStartDate)) like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR JD.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR SC.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CC.CourtDecision like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
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
