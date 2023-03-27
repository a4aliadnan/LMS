SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [REG_IN_PROG-URGENT]
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
@TableColumn		NVARCHAR(MAX)  = ' TableSort,PVName,CaseId,CaseRegistrationId,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired,CaseLevelName ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@QueryInnerPart		NVARCHAR(MAX) = '
							Select	1 as TableSort,''APPEAL LEVEL INV_Y'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''2''
							AND   CR.DepartmentType = ''2'' -- INVESTMENT = YES
							AND   CR.FileStatus != ''2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''4'',''6'')
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) > 12
							union all
							Select	2 as TableSort,''APPEAL LEVEL INV_N'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''2''
							AND   CR.DepartmentType = ''1'' -- INVESTMENT = NO
							AND   CR.FileStatus != ''2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''4'',''6'')
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) > 25
							union all
							Select	3 as TableSort,''SUPREME LEVEL'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''3''
							AND   CR.FileStatus != ''2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''5'',''6'')
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE())) > 35
							union all
							Select	4 as TableSort,''PRIMARY DISPUTE'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CR.FileStatus != ''2''
							AND   CR.DisputeLevel = ''1''
							AND   CC.CaseStatus = ''1''
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) > 10
							union all
							Select	5 as TableSort,''APPEAL DISPUTE'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left  Join SessionsRollsJudgementsVW as SRV on SRV.CaseId = CR.CaseId
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CR.FileStatus != ''2''
							AND   CR.DisputeLevel = ''2''
							AND   CC.CaseStatus = ''1''
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, SRV.PrimaryJudgementsDate),dateadd(hour , 11,GETDATE())) > 4
							union all
							Select	6 as TableSort,''SUPREME DISPUTE'' as PVName,CC.CaseId,CR.CaseRegistrationId,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,CR.Remarks,CR.ActionDate AS sortDate
							,case when CR.FileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CaseLevelMas.Mst_Desc as CaseLevelName,REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') as AgainstName,CR.FormPrintWorkRequired
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CR.FileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 788 
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left  Join SessionsRollsJudgementsVW as SRV on SRV.CaseId = CR.CaseId
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CR.FileStatus != ''2''
							AND   CR.DisputeLevel = ''3''
							AND   CC.CaseStatus = ''1''
							AND   1 = case when CR.FileStatus = ''11'' AND DATEDIFF(hour, dateadd(hour , 11, CR.UpdatedOn),dateadd(hour , 11,GETDATE())) > 120 then 0 else 1 end
							AND	  DATEDIFF(DAY, dateadd(hour , 11, SRV.AppealJudgementsDate),dateadd(hour , 11,GETDATE())) > 35
',
@SelectColumns		NVARCHAR(MAX),
@SummaryQuery		NVARCHAR(MAX),
@JoinTables			NVARCHAR(MAX) = ' ',
@Where				NVARCHAR(MAX) = '  where (LEFT(OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN

	SET @SelectColumns = '
							,sortDate 
			FROM (
			SELECT	TableSort,PVName,CaseId,CaseRegistrationId,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired,CaseLevelName
					,Remarks,sortDate
					FROM
					(
					'+@QueryInnerPart+' 
					) DAT
	'
	SET @SummaryQuery = '
							SELECT
									COUNT(*) as recordsTotal,
									count(case when left(OfficeFileNo,1) = ''M'' then 1 end) MCTRecords,
									count(case when left(OfficeFileNo,1) = ''S'' then 1 end) SLLRecords
									from (
			SELECT	TableSort,PVName,CaseId,CaseRegistrationId,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired,CaseLevelName
					,Remarks,sortDate
					FROM
					(
					'+@QueryInnerPart+'
					) DAT
									     ) MASTDAT
	'
	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by ActionCounter,sortDate) as RowNum, * 
    from 
	  ( 
	   SELECT '
	SET @TableQuery  = @SelectColumns+@JoinTables+@Where

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR ClientName like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR Defendant like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR FileStatusName like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR Notes like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR FormPrintWorkRequired like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR AgainstName like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CaseLevelName like ''%'+CONVERT(NVARCHAR,@filter)+'%''
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
	) AS FINAL ' + @FetchLimit+' ORDER BY ActionCounter,RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END
GO
