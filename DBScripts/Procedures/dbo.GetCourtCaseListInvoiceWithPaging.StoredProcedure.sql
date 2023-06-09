SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetCourtCaseListInvoiceWithPaging]
@Location varchar(1),
@Pageno INT=1,
@filter VARCHAR(500)='',
@pagesize INT=20,  
@Sorting VARCHAR(500)='CC.OfficeFileNo',
@SortOrder VARCHAR(500)='desc' 
AS
BEGIN
SET NOCOUNT ON;
DECLARE @SqlCount INT
DECLARE @From INT = @pageno
DECLARE @SQLQuery VARCHAR(5000)
IF(@Sorting ='CC.OfficeFileNo')
BEGIN
 SET @Sorting = 'dbo.fnMixSort(CC.OfficeFileNo)'
END
IF(@filter !='')
BEGIN
SET @SqlCount= ( SELECT COUNT(*) FROM CourtCases as CC join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S CaseStatusMas on CC.CaseStatus = CaseStatusMas.Mst_Value and CaseStatusMas.MstParentId = 252
					where CC.CaseStatus != '-1' 
					and (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A') 
					AND (
						CC.OfficeFileNo LIKE '%'+@filter+'%' OR 
						ClientMas.Mst_Desc LIKE '%'+ @filter+'%' OR 
						CC.Defendant LIKE '%'+@filter+'%' OR 
						--AgainstMas.Mst_Desc LIKE '%' + @filter+'%' OR 
						CC.AccountContractNo LIKE '%' + @filter+'%' OR 
						CC.ClientFileNo LIKE '%' + @filter+'%' OR 
						CC.ClaimAmount LIKE '%' + @filter+'%' OR
						--CaseTypeMas.Mst_Desc LIKE '%' + @filter+'%' OR
						--CaseStatusMas.Mst_Desc LIKE '%' + @filter+'%' OR
						CaseLevelMas.Mst_Desc LIKE '%' + @filter+'%'
						)
				)

SET @SQLQuery = 'SELECT CC.CaseId,CC.OfficeFileNo,ClientMas.Mst_Desc as ClientName,CC.Defendant as DefClientName,AgainstMas.Mst_Desc as AgainstName
				,FORMAT (CC.ReceptionDate, ''dd/MM/yyyy'') as ReceptionDate,CC.AccountContractNo,CC.ClientFileNo,CaseTypeMas.Mst_Desc as CaseTypeName,CC.CaseLevelCode
				,CaseLevelMas.Mst_Desc as CaseLevelName,CC.CaseStatus,CaseStatusMas.Mst_Desc as CaseStatusName,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from CourtCases as CC
				join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
				join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
				join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
				join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
				join MASTER_S CaseStatusMas on CC.CaseStatus = CaseStatusMas.Mst_Value and CaseStatusMas.MstParentId = 252 
				where CC.CaseStatus != ''-1'' 
				and (LEFT(CC.OfficeFileNo,1) = ''' + @Location + ''' OR ''' + @Location + ''' = ''A'')    
				and(CC.OfficeFileNo like ''%'+CONVERT(VARCHAR,@filter)+'%'' 
				or ClientMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%'' 
				or CC.Defendant like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CC.AccountContractNo like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CC.ClientFileNo like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CC.ClaimAmount like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CaseLevelMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%'') 
				order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'

/*
or AgainstMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
or CaseTypeMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
or CaseStatusMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
*/
END
ELSE
BEGIN
SET @SqlCount=( SELECT COUNT(*) FROM CourtCases as CC where CC.CaseStatus != '-1' and (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A'))

SET @SQLQuery = 'SELECT CC.CaseId,CC.OfficeFileNo,ClientMas.Mst_Desc as ClientName,CC.Defendant as DefClientName,AgainstMas.Mst_Desc as AgainstName
				,FORMAT (CC.ReceptionDate, ''dd/MM/yyyy'') as ReceptionDate,CC.AccountContractNo,CC.ClientFileNo,CaseTypeMas.Mst_Desc as CaseTypeName,CC.CaseLevelCode
				,CaseLevelMas.Mst_Desc as CaseLevelName,CC.CaseStatus,CaseStatusMas.Mst_Desc as CaseStatusName,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from CourtCases as CC
				join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
				join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
				join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
				join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
				join MASTER_S CaseStatusMas on CC.CaseStatus = CaseStatusMas.Mst_Value and CaseStatusMas.MstParentId = 252 
				where CC.CaseStatus != ''-1'' 
				and (LEFT(CC.OfficeFileNo,1) = ''' + @Location + ''' OR ''' + @Location + ''' = ''A'')     
				order by '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
end

print @SQLQuery
exec (@SQLQuery)

END
GO
