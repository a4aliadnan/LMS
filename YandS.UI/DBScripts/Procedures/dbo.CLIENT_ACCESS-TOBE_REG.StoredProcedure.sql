SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CLIENT_ACCESS-TOBE_REG]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,CaseRegistrationId,OfficeFileNo,Defendant,NULL as FileStatusName,UpdateDate,CourtDecision,AccountContractNo,ClientFileNo ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
			FROM (
				Select	
					  CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo,CC.Defendant as Defendant
					 ,FileStatus.Mst_Desc as FileStatusName,CC.CurrentHearingDate as UpdateDate,CC.CourtDecision
					 ,CC.AccountContractNo,CC.ClientFileNo
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
									Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
									',
@Where				NVARCHAR(MAX) = ' 
									where CR.IsDeleted = 0
									AND   CR.ActionLevel = ''1''
									AND   CR.FileStatus != ''2''
									AND   CC.CaseStatus = ''1''
									AND   CC.CaseLevelCode = ''1''
									AND   CC.ClientCode = '''+@ClientCode+'''
									AND   CC.AgainstCode IN (''1'', ''2'')
									AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
									AND  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN

	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by UpdateDate desc) as RowNum, * 
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
							OR FileStatus.Mst_Desc like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
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
