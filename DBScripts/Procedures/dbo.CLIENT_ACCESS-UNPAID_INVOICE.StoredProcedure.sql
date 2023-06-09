SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CLIENT_ACCESS-UNPAID_INVOICE]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,OfficeFileNo,Defendant,DaysCounter,AccountContractNo,ClientFileNo,CaseLevelName,InvoiceDate,InvoiceId,InvoiceNumber,InvoiceAmount ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
								FROM (
										Select	DATEDIFF(DAY, dateadd(hour , 11, CI.InvoiceDate),dateadd(hour , 11,GETDATE())) as DaysCounter,
										CC.CaseId,CC.OfficeFileNo,CC.Defendant,CC.AccountContractNo,CC.ClientFileNo,
										dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) as CaseLevelName,
										CI.InvoiceDate,CI.InvoiceId,CI.InvoiceNumber,
										dbo.FnGetInvoiceTotalWithVAT(CI.InvoiceId) as InvoiceAmount
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
									Inner Join CaseInvoices CI on CI.CaseId = CC.CaseId
									',
@Where				NVARCHAR(MAX) = ' 
									where CC.CaseStatus = ''1''
									AND   CC.ClientCode = '''+@ClientCode+'''
									AND   CI.InvoiceStatus = ''1''
									AND   (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN


	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by InvoiceDate) as RowNum, * 
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
							OR dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR DATEDIFF(DAY, dateadd(hour , 11,GETDATE()),dateadd(day , CCE.SuspensionPeriod, CCE.SuspensionStartDate)) like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR CI.InvoiceNumber like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR dbo.FnGetInvoiceTotalWithVAT(CI.InvoiceId) like ''%'+CONVERT(NVARCHAR,@filter)+'%''
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
