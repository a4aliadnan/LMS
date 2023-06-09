SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [GetCaseInvoiceListWithPaging]
    @Location varchar(1),
	@Pageno INT=1,
	@filter VARCHAR(500)='',
	@pagesize INT=20,  
	@Sorting VARCHAR(500)='OfficeFileNo',
	@SortOrder VARCHAR(500)='desc' ,
	@CaseId INT=0
AS
BEGIN
SET NOCOUNT ON;
DECLARE @SqlCount INT
DECLARE @From INT = @pageno
DECLARE @SQLQuery VARCHAR(5000)
IF(@Sorting ='OfficeFileNo')
BEGIN
 SET @Sorting = 'dbo.fnMixSort(OfficeFileNo)'
END
IF(@filter !='')
BEGIN
SET @SqlCount= ( SELECT COUNT(*) FROM CourtCases as CC
					join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join CaseInvoices CI on CI.CaseId = CC.CaseId
					join MASTER_S CourtTypeMas on CI.CourtType = CourtTypeMas.Mst_Value and CourtTypeMas.MstParentId = 15
					join MASTER_S InvStatusMas on CI.InvoiceStatus = InvStatusMas.Mst_Value and InvStatusMas.MstParentId = 247
					join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
					where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A') 
					AND   (CC.CaseId = @CaseId OR @CaseId = 0)
					AND (
						CC.OfficeFileNo LIKE '%'+@filter+'%' OR 
						dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) LIKE '%'+@filter+'%' OR 
						CI.InvoiceNumber LIKE '%'+@filter+'%' OR 
						CI.InvoiceDate LIKE '%'+@filter+'%' OR 
						InvStatusMas.Mst_Desc LIKE '%'+@filter+'%' OR
						(select sum(FeeAmount) as FeeAmount from CaseInvoiceFees CIF where CIF.InvoiceId = CI.InvoiceId) LIKE '%'+@filter+'%' OR
						ClientMas.Mst_Desc LIKE '%'+ @filter+'%' OR 
						CC.AccountContractNo LIKE '%' + @filter+'%' OR 
						CC.ClientFileNo LIKE '%' + @filter+'%'
						)
				)

SET @SQLQuery = 'Select
				CaseId,	OfficeFileNo,	ClientCode,	ClientName,	Defendant,	AgainstCode,	AgainstName,	ClaimAmount,
				AccountContractNo,	ClientFileNo,	InvoiceId,	InvoiceNumber,	InvoiceDate,	CourtType,	CourtName,	InvoiceStatus,
				InvoiceStatusName,	TransferType,	TransferNumber,	TransferDate,	Credit_Account,	CreditAccountName,	CourtLevelDisp,	InvoiceAmount,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from
				(
				Select	 CC.CaseId,CC.OfficeFileNo,CC.ClientCode,ClientMas.Mst_Desc as ClientName,CC.Defendant,CC.AgainstCode,AgainstMas.Mst_Desc as AgainstName,CC.ClaimAmount
						,CC.AccountContractNo,CC.ClientFileNo
						,CI.InvoiceId,CI.InvoiceNumber,CI.InvoiceDate,CI.CourtType,CourtTypeMas.Mst_Desc as CourtName,CI.InvoiceStatus,InvStatusMas.Mst_Desc as InvoiceStatusName
						,CI.TransferType,CI.TransferNumber,CI.TransferDate,CI.Credit_Account,BA.Mst_Desc as CreditAccountName
						,dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) as CourtLevelDisp
						,dbo.FnGetInvoiceTotalWithVAT(CI.InvoiceId) as InvoiceAmount
				from CourtCases as CC
				join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
				join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
				join CaseInvoices CI on CI.CaseId = CC.CaseId
				join MASTER_S CourtTypeMas on CI.CourtType = CourtTypeMas.Mst_Value and CourtTypeMas.MstParentId = 15
				join MASTER_S InvStatusMas on CI.InvoiceStatus = InvStatusMas.Mst_Value and InvStatusMas.MstParentId = 247
				join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
				where (LEFT(CC.OfficeFileNo,1) = ''' + @Location + ''' OR ''' + @Location + ''' = ''A'')    
				AND   (CC.CaseId = ' + CAST(@CaseId as nvarchar(10)) + ' OR ' + CAST(@CaseId as nvarchar(10)) + ' = 0)
				and(CC.OfficeFileNo like ''%'+CONVERT(VARCHAR,@filter)+'%'' 
				or dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) LIKE ''%'+CONVERT(VARCHAR,@filter)+'%'' 
				or CI.InvoiceNumber like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CI.InvoiceDate like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or InvStatusMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or (select sum(FeeAmount) as FeeAmount from CaseInvoiceFees CIF where CIF.InvoiceId = CI.InvoiceId) like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or ClientMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CC.AccountContractNo like ''%'+CONVERT(VARCHAR,@filter)+'%''
				or CC.ClientFileNo like ''%'+CONVERT(VARCHAR,@filter)+'%'') 
				) dat 
				order by CASE When InvoiceStatus = ''-1'' Then 1 When InvoiceStatus = ''1'' Then 3 When InvoiceStatus = ''2'' Then 2 else 0 end desc,InvoiceDate, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'

/*
or AgainstMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
or CaseTypeMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
or CaseStatusMas.Mst_Desc like ''%'+CONVERT(VARCHAR,@filter)+'%''
*/
END
ELSE
BEGIN
SET @SqlCount=( SELECT COUNT(*) FROM CourtCases as CC
					join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join CaseInvoices CI on CI.CaseId = CC.CaseId
					join MASTER_S CourtTypeMas on CI.CourtType = CourtTypeMas.Mst_Value and CourtTypeMas.MstParentId = 15
					join MASTER_S InvStatusMas on CI.InvoiceStatus = InvStatusMas.Mst_Value and InvStatusMas.MstParentId = 247
					join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
					where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A') 
					AND   (CC.CaseId = @CaseId OR @CaseId = 0))

SET @SQLQuery = 'Select
				CaseId,	OfficeFileNo,	ClientCode,	ClientName,	Defendant,	AgainstCode,	AgainstName,	ClaimAmount,
				AccountContractNo,	ClientFileNo,	InvoiceId,	InvoiceNumber,	InvoiceDate,	CourtType,	CourtName,	InvoiceStatus,
				InvoiceStatusName,	TransferType,	TransferNumber,	TransferDate,	Credit_Account,	CreditAccountName,	CourtLevelDisp,	InvoiceAmount,'+CONVERT(VARCHAR,@SqlCount)+' as TotalRecords
				from
				(
				Select	 CC.CaseId,CC.OfficeFileNo,CC.ClientCode,ClientMas.Mst_Desc as ClientName,CC.Defendant,CC.AgainstCode,AgainstMas.Mst_Desc as AgainstName,CC.ClaimAmount
						,CC.AccountContractNo,CC.ClientFileNo
						,CI.InvoiceId,CI.InvoiceNumber,CI.InvoiceDate,CI.CourtType,CourtTypeMas.Mst_Desc as CourtName,CI.InvoiceStatus,InvStatusMas.Mst_Desc as InvoiceStatusName
						,CI.TransferType,CI.TransferNumber,CI.TransferDate,CI.Credit_Account,BA.Mst_Desc as CreditAccountName
						,dbo.FnGetInvoiceCaseLevel(CC.CaseId,CI.CourtType) as CourtLevelDisp
						,dbo.FnGetInvoiceTotalWithVAT(CI.InvoiceId) as InvoiceAmount
				from CourtCases as CC
				join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
				join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
				join CaseInvoices CI on CI.CaseId = CC.CaseId
				join MASTER_S CourtTypeMas on CI.CourtType = CourtTypeMas.Mst_Value and CourtTypeMas.MstParentId = 15
				join MASTER_S InvStatusMas on CI.InvoiceStatus = InvStatusMas.Mst_Value and InvStatusMas.MstParentId = 247
				join Link_Bank_Account_View BA on BA.LinkId = CI.Credit_Account
				where (LEFT(CC.OfficeFileNo,1) = ''' + @Location + ''' OR ''' + @Location + ''' = ''A'')    
				AND   (CC.CaseId = ' + CAST(@CaseId as nvarchar(10)) + ' OR ' + CAST(@CaseId as nvarchar(10)) + ' = 0)
				) dat   
				order by CASE When InvoiceStatus = ''-1'' Then 1 When InvoiceStatus = ''1'' Then 3 When InvoiceStatus = ''2'' Then 2 else 0 end desc,InvoiceDate, '+ CONVERT(VARCHAR,@Sorting) +' '+ @SortOrder +' OFFSET '+CONVERT(varchar,@From)+' ROWS FETCH NEXT '+CONVERT(varchar,@pagesize)+' ROWS ONLY OPTION (RECOMPILE)'
end

print @SQLQuery
exec (@SQLQuery)

END
GO
