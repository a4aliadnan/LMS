SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [REG_IN_PROG-NEWCASES]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,CaseRegistrationId,DaysCounter,OfficeFileNo,ClientName,Defendant,FileStatusName,ActionCounter,OfficeProcedure,DaysCounterUpdateDate,FSort,FileStatus,OnHoldDone,CourtDecision ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
			,sortDate 
			FROM (
				Select	CC.CaseId,CR.CaseRegistrationId,DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE())) as DaysCounter,CC.OfficeFileNo
				,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,FileStatus.Mst_Desc as FileStatusName
				,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
				,DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE())) as DaysCounterUpdateDate
				,CR.OfficeProcedure as OfficeProcedure,CC.ReceptionDate AS sortDate
				,case when CR.FileStatus = ''11'' then 0 when CR.OnHoldDone = ''1'' then 2 else 1 end as FSort,CC.OfficeFileStatus as FileStatus,CR.OnHoldDone,CC.CourtDecision
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
									Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573 
									',
@Where				NVARCHAR(MAX) = ' 
									where CR.IsDeleted = 0
									AND   CR.ActionLevel = ''1''
									AND   CC.OfficeFileStatus != ''OFS-2''
									AND   CC.CaseStatus = ''1''
									AND   CC.CaseLevelCode = ''1''
									AND  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN

DECLARE @CheckStopRegistration INT = 0

SELECT @CheckStopRegistration = count(*) from CaseRegistrations where FileStatus = '11' and   IsDeleted = 0 and   DATEDIFF(DAY, UpdatedOn, GETDATE()) > 10
IF (@CheckStopRegistration > 0)
BEGIN
	update CaseRegistrations set IsDeleted = 1 where FileStatus = '11' and   IsDeleted = 0 and   DATEDIFF(DAY, UpdatedOn, GETDATE()) > 10
END

	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by FSort DESC,DaysCounterUpdateDate desc,sortDate) as RowNum, * 
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
							OR FileStatus.Mst_Desc like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
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
	) AS FINAL ' + @FetchLimit+' ORDER BY FSort DESC,DaysCounterUpdateDate DESC,RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END
GO
