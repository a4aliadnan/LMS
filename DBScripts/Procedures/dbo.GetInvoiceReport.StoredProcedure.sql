SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetInvoiceReport]
    @Location varchar(3),
	@DateFrom date,
	@DateTo date,
	@ClientCode varchar(4),
    @AgainstCode varchar(4),
    @ODBBankBranch varchar(4),
    @InvoiceStatus varchar(3),
   @ClientCaseType varchar(3),
   @FeeTypeId varchar(3)

AS

DECLARE
	@ClientDisp varchar(100),
	@AgainstDisp varchar(100),
	@LocationDisp varchar(100)

IF @ClientCode = '7'
	BEGIN
	
	SET @LocationDisp = CASE WHEN  @Location = 'A' THEN 'MUSCAT' WHEN  @Location = 'M' THEN 'MUSCAT' WHEN  @Location = 'S' THEN 'SALALAH' END
	SET @ClientDisp = CASE WHEN  @Location = 'A' THEN '3901-070808-001' WHEN  @Location = 'M' THEN '3901-070808-001' WHEN  @Location = 'S' THEN '3901-070808-002' END


	SELECT	ROW_NUMBER() OVER (ORDER BY "A/C NO.") as SNo, "A/C NO.","DEFENDANT NAME","INVOICE AMOUNT","DESCRIPTION","INVOICE NUMBER","INVOICE DATE",
			ExpectedFees as "Total Lawyer Fees", FeePaid as "Fees paid to date"
	from
	(
	SELECT CC.CaseId,CC.OfficeFileNo,
			FORMAT (CI.InvoiceDate, 'dd/MM/yyyy') as "INVOICE DATE",
			CI.InvoiceNumber as "INVOICE NUMBER",
			case when CC.ClientCode = '0' then null else ClientMast.Mst_Desc END as CLIENT,
			CC.CaseSubject,
			CASE WHEN CC.CaseSubject = '0' THEN NULL ELSE CaseSub.Mst_Desc END as "CASE SUBJECT",
			CASE WHEN CC.CaseTypeCode = '0' THEN NULL ELSE CaseType.Mst_Desc END as "CASE TYPE", 
			CC.ODBBankBranch,
			CASE WHEN CC.ODBBankBranch = '0' THEN NULL ELSE ODB.Mst_Desc END as "CLIENT BRANCH",
			CC.ClientCaseType,
			CASE WHEN CC.ClientCaseType = '0' THEN NULL ELSE ClientCasType.Mst_Desc END as "CLIENT CASE TYPE",
			CASE WHEN CC.AgainstCode = '0' THEN NULL ELSE CaseAgainst.Mst_Desc END as "CASE AGAINST",
			CC.Defendant as "DEFENDANT NAME",
			CC.AccountContractNo as "A/C NO.",
			CC.ClientFileNo as "CLIENT FILE NO.",
			CIF.FeeTypeId,
			--CASE WHEN CIF.FeeTypeId = '0' THEN NULL ELSE FeeDetail.Mst_Desc END as "DESCRIPTION",
			case when CIF.CaseLevel = '0' then '' else CaseLevel.Mst_Desc end +  
			case when CIF.FeeTypeId = '0' then '' when CIF.CaseLevel = '0' and CIF.FeeTypeId != '0' then FeeDetail.Mst_Desc else  ' - ' + FeeDetail.Mst_Desc end + ' - ' +
			case when CIF.FeeTypeCascadeDetail = '0' then '' else  FeeCascade.Mst_Desc end +
			case when CIF.FeeTypeDetail is null then '' when CIF.FeeTypeCascadeDetail = '0' and CIF.FeeTypeDetail is not null then CIF.FeeTypeDetail else ' - ' + CIF.FeeTypeDetail end
			as "DESCRIPTION", 
			case when CIF.FeeTypeCascadeDetail = '0' then '' else  FeeCascade.Mst_Desc end +
			case when CIF.FeeTypeDetail is null then '' when CIF.FeeTypeCascadeDetail = '0' and CIF.FeeTypeDetail is not null then CIF.FeeTypeDetail else ' - ' + CIF.FeeTypeDetail end as TypeOfFees, 
			--CourtType.Mst_Desc as "DESCRIPTION",
			--(select sum(FeeAmount) as FeeAmount from CaseInvoiceFees CIF where CIF.InvoiceId = CI.InvoiceId) as "INVOICE AMOUNT",
			case when CIF.VATPct is not null then  CIF.FeeAmount + (CIF.FeeAmount * (CIF.VATPct/100)) else CIF.FeeAmount end as  "INVOICE AMOUNT",
			CI.InvoiceStatus,
			CASE WHEN CI.InvoiceStatus = '0' THEN NULL ELSE InvStatus.Mst_Desc END as "INVOICE STATUS",
			CC.ClientCode,
			CC.AgainstCode,
			CI.TransferDate,
			CASE WHEN CI.TransferType = '0' THEN NULL ELSE TransType.Mst_Desc END as "MODE OF PAYMENT",
			CI.TransferNumber,
			CASE WHEN CI.Credit_Account = '0' THEN NULL ELSE BA.Mst_Desc END as "DEPOSIT BANK",
			CASE WHEN CIF.InvClassification = '0' THEN NULL ELSE Classification.Mst_Desc END as "INVOICE CLASSIFICATION",
			CI.ExpectedFees,
			(Select sum(FeeAmount) as FeePaid from CaseInvoiceFees CIFP where CI.InvoiceId = CIFP.InvoiceId and CIFP.FeeTypeId = '9' and CI.InvoiceStatus = '2') as FeePaid
	 FROM CaseInvoices CI
	inner join CourtCases CC on CI.CaseId = CC.CaseId
	inner join CaseInvoiceFees CIF on CI.InvoiceId = CIF.InvoiceId
	left join MASTER_S as CaseSub on CaseSub.MstParentId = 532 and CaseSub.Mst_Value = CC.CaseSubject
	left join MASTER_S as ODB on ODB.MstParentId = 18 and ODB.Mst_Value = CC.ODBBankBranch
	left join MASTER_S as ClientCasType on ClientCasType.MstParentId = 285 and ClientCasType.Mst_Value = CC.ClientCaseType
	left join MASTER_S as FeeDetail on FeeDetail.MstParentId = 232 and FeeDetail.Mst_Value = CIF.FeeTypeId
	left join MASTER_S as InvStatus on InvStatus.MstParentId = 247 and InvStatus.Mst_Value = CI.InvoiceStatus
	left join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
	left join MASTER_S as CourtType on CourtType.Mst_Value = CI.CourtType and CourtType.MstParentId = 15
	left join MASTER_S as CaseLevel on CaseLevel.Mst_Value = CIF.CaseLevel and CaseLevel.MstParentId = 15 
	left join MASTER_S as ClientMast on ClientMast.Mst_Value = CC.ClientCode and ClientMast.MstParentId = 241 
	left join MASTER_S as CaseType on CaseType.Mst_Value = CC.CaseTypeCode and CaseType.MstParentId = 14 
	left join MASTER_S as CaseAgainst on CaseAgainst.Mst_Value = CC.AgainstCode and CaseAgainst.MstParentId = 12
	left join MASTER_S as TransType on TransType.Mst_Value = CI.TransferType and TransType.MstParentId = 8
	left join Link_Bank_Account_View as BA on BA.LinkId = CI.Credit_Account
	left join MASTER_S as Classification on Classification.Mst_Value = CIF.InvClassification and Classification.MstParentId = 289
	where 
		CC.ClientCode = '7'
	and (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
	----AND CAST(CI.InvoiceDate AS DATE) BETWEEN CAST(''' + CONVERT(NVARCHAR(24), @DateFrom, 101) + ''' AS DATE) AND CAST(''' + CONVERT(NVARCHAR(24), @DateTo, 101)  +''' AS DATE)
	AND CI.InvoiceDate BETWEEN @DateFrom AND @DateTo
	--AND (CC.ClientCode = @ClientCode OR @ClientCode = '0')
	AND (CC.AgainstCode = @AgainstCode OR @AgainstCode = '0')
	AND (CC.ODBBankBranch = @ODBBankBranch OR @ODBBankBranch = '0')
	AND (CI.InvoiceStatus = @InvoiceStatus OR @InvoiceStatus = '0')
	AND (CC.ClientCaseType = @ClientCaseType OR @ClientCaseType = '0')
	AND (CIF.FeeTypeId = @FeeTypeId OR @FeeTypeId = '0')
	)DAT
	order by 1

	select sum(CIF.FeeAmount) as  "TTLAMOUNT" ,@LocationDisp,@ClientDisp
	FROM CaseInvoices CI
	inner join CourtCases CC on CI.CaseId = CC.CaseId
	inner join CaseInvoiceFees CIF on CI.InvoiceId = CIF.InvoiceId
	left join MASTER_S as CaseSub on CaseSub.MstParentId = 532 and CaseSub.Mst_Value = CC.CaseSubject
	left join MASTER_S as ODB on ODB.MstParentId = 18 and ODB.Mst_Value = CC.ODBBankBranch
	left join MASTER_S as ClientCasType on ClientCasType.MstParentId = 285 and ClientCasType.Mst_Value = CC.ClientCaseType
	left join MASTER_S as FeeDetail on FeeDetail.MstParentId = 232 and FeeDetail.Mst_Value = CIF.FeeTypeId
	left join MASTER_S as InvStatus on InvStatus.MstParentId = 247 and InvStatus.Mst_Value = CI.InvoiceStatus
	left join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
	left join MASTER_S as CourtType on CourtType.Mst_Value = CI.CourtType and CourtType.MstParentId = 15
	left join MASTER_S as CaseLevel on CaseLevel.Mst_Value = CIF.CaseLevel and CaseLevel.MstParentId = 15 
	left join MASTER_S as ClientMast on ClientMast.Mst_Value = CC.ClientCode and ClientMast.MstParentId = 241 
	left join MASTER_S as CaseType on CaseType.Mst_Value = CC.CaseTypeCode and CaseType.MstParentId = 14 
	left join MASTER_S as CaseAgainst on CaseAgainst.Mst_Value = CC.AgainstCode and CaseAgainst.MstParentId = 12
	left join MASTER_S as TransType on TransType.Mst_Value = CI.TransferType and TransType.MstParentId = 8
	left join Link_Bank_Account_View as BA on BA.LinkId = CI.Credit_Account
	left join MASTER_S as Classification on Classification.Mst_Value = CIF.InvClassification and Classification.MstParentId = 289
	where 
		CC.ClientCode = '7'
	and (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
	----AND CAST(CI.InvoiceDate AS DATE) BETWEEN CAST(''' + CONVERT(NVARCHAR(24), @DateFrom, 101) + ''' AS DATE) AND CAST(''' + CONVERT(NVARCHAR(24), @DateTo, 101)  +''' AS DATE)
	AND CI.InvoiceDate BETWEEN @DateFrom AND @DateTo
	--AND (CC.ClientCode = @ClientCode OR @ClientCode = '0')
	AND (CC.AgainstCode = @AgainstCode OR @AgainstCode = '0')
	AND (CC.ODBBankBranch = @ODBBankBranch OR @ODBBankBranch = '0')
	AND (CI.InvoiceStatus = @InvoiceStatus OR @InvoiceStatus = '0')
	AND (CC.ClientCaseType = @ClientCaseType OR @ClientCaseType = '0')
	AND (CIF.FeeTypeId = @FeeTypeId OR @FeeTypeId = '0')
	END

ELSE

	BEGIN

	SET @LocationDisp = 'Y & S ( ' + CASE WHEN  @Location = 'A' THEN 'ALL' WHEN  @Location = 'M' THEN 'MUSCAT' WHEN  @Location = 'S' THEN 'SALALAH' END + ' )'

	IF @ClientCode = '0'
	SET @ClientDisp = 'CLIENT : ALL'
	ELSE
	SELECT @ClientDisp = 'CLIENT : ' + Mst_Desc from MASTER_S where MstParentId = 241 and Mst_Value = @ClientCode


	IF @AgainstCode = '0'
	SET @AgainstDisp = 'AGAINST : ALL'
	ELSE
	SELECT @AgainstDisp = 'AGAINST : ' + Mst_Desc from MASTER_S where MstParentId = 12 and Mst_Value = @AgainstCode

	SELECT OfficeFileNo,"INVOICE DATE","INVOICE NUMBER",CLIENT,"CLIENT BRANCH","CLIENT CASE TYPE","CASE TYPE", "DEFENDANT NAME","CASE AGAINST",
			"A/C NO.","CLIENT FILE NO.","CLAIM AMOUNT","DESCRIPTION",TypeOfFees,"AMOUNT","VAT AMOUNT","INVOICE AMOUNT","INVOICE STATUS",TransferDate,"MODE OF PAYMENT",TransferNumber,"DEPOSIT BANK","INVOICE CLASSIFICATION"
	from
	(
	SELECT CC.OfficeFileNo,
			FORMAT (CI.InvoiceDate, 'dd/MM/yyyy') as "INVOICE DATE",
			CI.InvoiceNumber as "INVOICE NUMBER",
			case when CC.ClientCode = '0' then null else ClientMast.Mst_Desc END as CLIENT,
			CC.CaseSubject,
			CASE WHEN CC.CaseSubject = '0' THEN NULL ELSE CaseSub.Mst_Desc END as "CASE SUBJECT",
			CASE WHEN CC.CaseTypeCode = '0' THEN NULL ELSE CaseType.Mst_Desc END as "CASE TYPE", 
			CC.ODBBankBranch,
			CASE WHEN CC.ODBBankBranch = '0' THEN NULL ELSE ODB.Mst_Desc END as "CLIENT BRANCH",
			CC.ClientCaseType,
			CASE WHEN CC.ClientCaseType = '0' THEN NULL ELSE ClientCasType.Mst_Desc END as "CLIENT CASE TYPE",
			CASE WHEN CC.AgainstCode = '0' THEN NULL ELSE CaseAgainst.Mst_Desc END as "CASE AGAINST",
			CC.Defendant as "DEFENDANT NAME",
			CC.AccountContractNo as "A/C NO.",
			CC.ClientFileNo as "CLIENT FILE NO.",
			CIF.FeeTypeId,
			--CASE WHEN CIF.FeeTypeId = '0' THEN NULL ELSE FeeDetail.Mst_Desc END as "DESCRIPTION",
			case when CIF.CaseLevel = '0' then '' else CaseLevel.Mst_Desc end +  
			case when CIF.FeeTypeId = '0' then '' when CIF.CaseLevel = '0' and CIF.FeeTypeId != '0' then FeeDetail.Mst_Desc else  ' - ' + FeeDetail.Mst_Desc end 
			as "DESCRIPTION", 
			case when CIF.FeeTypeCascadeDetail = '0' then '' else  FeeCascade.Mst_Desc end +
			case when CIF.FeeTypeDetail is null then '' when CIF.FeeTypeCascadeDetail = '0' and CIF.FeeTypeDetail is not null then CIF.FeeTypeDetail else ' - ' + CIF.FeeTypeDetail end as TypeOfFees, 
			--CourtType.Mst_Desc as "DESCRIPTION",
			--(select sum(FeeAmount) as FeeAmount from CaseInvoiceFees CIF where CIF.InvoiceId = CI.InvoiceId) as "INVOICE AMOUNT",
			CIF.FeeAmount AS "AMOUNT",
			case when CIF.VATPct is not null then  CIF.FeeAmount * (CIF.VATPct/100) else 0 end as "VAT AMOUNT",
			case when CIF.VATPct is not null then  CIF.FeeAmount + (CIF.FeeAmount * (CIF.VATPct/100)) else CIF.FeeAmount end as  "INVOICE AMOUNT",
			CI.InvoiceStatus,
			CASE WHEN CI.InvoiceStatus = '0' THEN NULL ELSE InvStatus.Mst_Desc END as "INVOICE STATUS",
			CC.ClientCode,
			CC.AgainstCode,
			CI.TransferDate,
			CASE WHEN CI.TransferType = '0' THEN NULL ELSE TransType.Mst_Desc END as "MODE OF PAYMENT",
			CI.TransferNumber,
			CASE WHEN CI.Credit_Account = '0' THEN NULL ELSE BA.Mst_Desc END as "DEPOSIT BANK",
			CASE WHEN CIF.InvClassification = '0' THEN NULL ELSE Classification.Mst_Desc END as "INVOICE CLASSIFICATION"
			,CC.ClaimAmount as "CLAIM AMOUNT"
	 FROM CaseInvoices CI
	inner join CourtCases CC on CI.CaseId = CC.CaseId
	inner join CaseInvoiceFees CIF on CI.InvoiceId = CIF.InvoiceId
	left join MASTER_S as CaseSub on CaseSub.MstParentId = 532 and CaseSub.Mst_Value = CC.CaseSubject
	left join MASTER_S as ODB on ODB.MstParentId = 18 and ODB.Mst_Value = CC.ODBBankBranch
	left join MASTER_S as ClientCasType on ClientCasType.MstParentId = 285 and ClientCasType.Mst_Value = CC.ClientCaseType
	left join MASTER_S as FeeDetail on FeeDetail.MstParentId = 232 and FeeDetail.Mst_Value = CIF.FeeTypeId
	left join MASTER_S as InvStatus on InvStatus.MstParentId = 247 and InvStatus.Mst_Value = CI.InvoiceStatus
	left join MASTER_S as FeeCascade on FeeCascade.Mst_Value = CIF.FeeTypeCascadeDetail and FeeCascade.MstParentId = 567
	left join MASTER_S as CourtType on CourtType.Mst_Value = CI.CourtType and CourtType.MstParentId = 15
	left join MASTER_S as CaseLevel on CaseLevel.Mst_Value = CIF.CaseLevel and CaseLevel.MstParentId = 15 
	left join MASTER_S as ClientMast on ClientMast.Mst_Value = CC.ClientCode and ClientMast.MstParentId = 241 
	left join MASTER_S as CaseType on CaseType.Mst_Value = CC.CaseTypeCode and CaseType.MstParentId = 14 
	left join MASTER_S as CaseAgainst on CaseAgainst.Mst_Value = CC.AgainstCode and CaseAgainst.MstParentId = 12
	left join MASTER_S as TransType on TransType.Mst_Value = CI.TransferType and TransType.MstParentId = 8
	left join Link_Bank_Account_View as BA on BA.LinkId = CI.Credit_Account
	left join MASTER_S as Classification on Classification.Mst_Value = CIF.InvClassification and Classification.MstParentId = 289
	where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
	--AND CAST(CI.InvoiceDate AS DATE) BETWEEN CAST(''' + CONVERT(NVARCHAR(24), @DateFrom, 101) + ''' AS DATE) AND CAST(''' + CONVERT(NVARCHAR(24), @DateTo, 101)  +''' AS DATE)
	AND CI.InvoiceDate BETWEEN @DateFrom AND @DateTo
	AND (CC.ClientCode = @ClientCode OR @ClientCode = '0')
	AND (CC.AgainstCode = @AgainstCode OR @AgainstCode = '0')
	AND (CC.ODBBankBranch = @ODBBankBranch OR @ODBBankBranch = '0')
	AND (CI.InvoiceStatus = @InvoiceStatus OR @InvoiceStatus = '0')
	AND (CC.ClientCaseType = @ClientCaseType OR @ClientCaseType = '0')
	AND (CIF.FeeTypeId = @FeeTypeId OR @FeeTypeId = '0')
	)DAT
	order by 3

	SELECT @LocationDisp AS LocationDisp, @ClientDisp AS ClientDisp, @AgainstDisp AS AgainstDisp
	END
GO
