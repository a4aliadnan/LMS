SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CLIENT_ACCESS-CLOSED_FILES]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,OfficeFileNo,Defendant,AccountContractNo,ClientFileNo,AgainstName,ClosureLevelName,ClosureReasonName,FinanceFileClosureDate,ClaimAmount ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
								FROM (
										Select	
										CC.CaseId,CC.OfficeFileNo,CC.Defendant,CC.AccountContractNo,CC.ClientFileNo
										,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName
										,CaseLevelMas.Mst_Desc as ClosureLevelName
										,ClosureReason.Mst_Desc as ClosureReasonName,CC.FinanceFileClosureDate
										,CC.ClaimAmount
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
									Left Join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
									Left Join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
									Left Join MASTER_S ClosureReason on CC.ReasonCode = ClosureReason.Mst_Value and ClosureReason.MstParentId = 395

									',
@Where				NVARCHAR(MAX) = ' 
									where CC.CaseStatus = ''2''
									AND   CC.ClientCode = '''+@ClientCode+'''
									AND   (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN


	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by CaseId desc) as RowNum, * 
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
							OR CC.AccountContractNo like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CC.ClientFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR AgainstMas.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CaseLevelMas.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR ClosureReason.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CC.FinanceFileClosureDate like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CC.ClaimAmount like ''%'+CONVERT(NVARCHAR,@filter)+'%''
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
