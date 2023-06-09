SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [REG_IN_PROG-SUPSUBM]
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
@TableColumn		NVARCHAR(MAX)  = ' CaseId,CaseRegistrationId,OfficeFileNo,ClientName,Defendant,FileStatusName,ActionCounter,JudDaysCounter,Notes,Remarks,Court,InvestmentType,InvestmentTypeName,ConsultantName,FormPrintWorkRequired,JDReceiveDate,AgainstName,FSort,FileStatus ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@SelectColumns		NVARCHAR(MAX) = '
			,sortDate 
			FROM (
				Select	
				 CC.CaseId
				,CR.CaseRegistrationId
				,CC.OfficeFileNo
				,ClientMas.Mst_Desc as ClientName
				,CC.Defendant as Defendant
				,FileStatus.Mst_Desc as FileStatusName
				,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
				,case 
					when CR.JudgementDate is null then 0 
					else DATEDIFF(DAY, dateadd(hour , 11, CR.JudgementDate),dateadd(hour , 11,GETDATE())) 
				 end as JudDaysCounter
				,CR.OfficeProcedure as Notes
				,CR.Remarks
				,case when CR.CourtLocationid = ''0'' then null else Courts.Mst_Desc end as Court
				,CR.DepartmentType as InvestmentType
				,DepType.Mst_Desc as InvestmentTypeName
				,CR.JudgementDate AS sortDate
				,case when CR.ConsultantId = ''0'' then null else SessLawyer.Mst_Desc end as ConsultantName
				,CR.FormPrintWorkRequired,SRV.AppealJDReceiveDate AS JDReceiveDate,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName
				,case when CR.FileStatus = ''11'' then 0 else 1 end as FSort,CC.OfficeFileStatus as FileStatus
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
									left join MASTER_S Courts on CR.CourtLocationid = Courts.Mst_Value and Courts.MstParentId = 4
									left join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
									left join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573 
									left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
									left join MASTER_S as SessLawyer on SessLawyer.MstParentId = 1408 and SessLawyer.Mst_Value = CR.ConsultantId
									left join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
									left Join SessionsRollsVW as SRV on SRV.CaseId = CR.CaseId
									',
@Where				NVARCHAR(MAX) = ' 
									where CR.IsDeleted = 0
									AND   CR.ActionLevel = ''3''
									AND   CC.OfficeFileStatus != ''OFS-2''
									AND   CC.CaseStatus = ''1''
									AND   CC.CaseLevelCode not in (''5'',''6'')
									AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
									AND  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN
	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by FSort desc, JudDaysCounter desc,sortDate) as RowNum, * 
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
							OR Courts.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR DepType.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR FileStatus.Mst_Desc like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CR.OfficeProcedure like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CR.FormPrintWorkRequired like N''%'+CONVERT(NVARCHAR,@filter)+'%''
							OR AgainstMas.Mst_Desc like ''%'+CONVERT(NVARCHAR,@filter)+'%''
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
	) AS FINAL ' + @FetchLimit+' ORDER BY FSort desc, JudDaysCounter DESC,RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END
GO
