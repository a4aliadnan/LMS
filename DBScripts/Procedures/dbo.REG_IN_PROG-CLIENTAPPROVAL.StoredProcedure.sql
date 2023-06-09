SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [REG_IN_PROG-CLIENTAPPROVAL]
@Pageno				INT=1,
@filter				NVARCHAR(500)='',
@pagesize			INT=20,  
@Sorting			NVARCHAR(500)='CC.OfficeFileNo',
@SortOrder			NVARCHAR(500)='desc',
@UserName			NVARCHAR(5)=null,
@DataFor			NVARCHAR(25)=null,
@Location			NVARCHAR(1)=null,
@FileStatus			NVARCHAR(3)=null
AS
DECLARE
@FilterText			NVARCHAR(MAX) = '',
@FinalQuery			NVARCHAR(MAX),
@FinalSummaryQuery	NVARCHAR(MAX),
@FetchLimit			NVARCHAR(MAX),
@TableColumn		NVARCHAR(MAX)  = ' CaseId,CaseRegistrationId,ActionCounter,OfficeFileNo,ClientName,Defendant,WorkRequired,Notes,InvestmentType,CaseLevelName ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
			,sortDate 
			FROM (
				Select	CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
				,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
				,CR.FormPrintWorkRequired as WorkRequired
				,case when CR.DepartmentType = ''0'' then null else InvstType.Mst_Desc end as InvestmentType
				,CR.OfficeProcedure as Notes,CR.ActionDate AS sortDate,CaseLevelMas.Mst_Desc as CaseLevelName
				from CaseRegistrations CR
				',

@SummaryQuery		NVARCHAR(MAX) = '
									SELECT
									COUNT(*) as recordsTotal,
									count(case when left(CC.OfficeFileNo,1) = ''M'' then 1 end) MCTRecords,
									count(case when left(CC.OfficeFileNo,1) = ''S'' then 1 end) SLLRecords
									from CaseRegistrations CR
									',
@JoinTables			NVARCHAR(MAX) = ' 
									Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
									Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
									Left  join MASTER_S InvstType on CR.DepartmentType = InvstType.Mst_Value and InvstType.MstParentId = 822
									Left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
									',
@Where				NVARCHAR(MAX) = ' 
									where CR.IsDeleted = 0
									AND   CC.OfficeFileStatus IN (''OFS-4'',''OFS-10'',''OFS-11'')
									AND   CC.CaseStatus = ''1''
									AND  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN

DECLARE @CheckStopRegistration INT = 0

	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by ActionCounter DESC,sortDate) as RowNum, * 
    from 
	  ( 
	   SELECT '
	SET @TableQuery  = @SelectColumns+@JoinTables+@Where

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							CC.OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR ClientMas.Mst_Desc like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CC.Defendant like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CR.FormPrintWorkRequired like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CR.OfficeProcedure like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
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
	) AS FINAL ' + @FetchLimit+' ORDER BY ActionCounter DESC,sortDate DESC,RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END
GO
