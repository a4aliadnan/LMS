SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetCourtCaseDetailForReport]
    @Location varchar(1),
	@ClientCode varchar(5),
	@AgainstCode varchar(5),
	@ClaimAmountFrom decimal(18,3),
	@ClaimAmountTo decimal(18,3),
	@CaseTypeCode varchar(5),
	@CaseLevelCode varchar(5),
	@CaseStatus varchar(5),
	@ReceptionDateFrom date,
	@ReceptionDateTo date,
	@RegistrationDateFrom date,
	@RegistrationDateTo date,
	@JudgmentDateFrom date,
	@JudgmentDateTo date,
	@PartialViewName varchar(25),
	@ApealByWho varchar(5) = null,
	@GovernorateId varchar(5) = null,
	@AgainstInsurance varchar(5) = null,
	@ODBBankBranch varchar(5) = null,
	@ClientCaseType varchar(5) = null,
	@OmaniExp varchar(5) = null,
	@EnforcementlevelId varchar(5) = null,
	@ReOpenEnforcement varchar(5) = null,
	@CourtLocationid varchar(6) = null,
	@ReportLang varchar(15) = null,
	@ReasonCode varchar(5) = '0'
AS
BEGIN
	IF @ReportLang = 'btnNBORep'
		BEGIN
				With tblPrimary as
					(
					 Select CaseId, CourtRefNo, RegistrationDate,COURT,
					 max(PrimaryFinalJDAmount) as PrimaryFinalJDAmount,
					 max(PrimaryJudgementsDate) as PrimaryJudgementsDate,
					 max(PrimaryJDReceiveDate) as PrimaryJDReceiveDate
					 from
					 (
						 Select CCD.CaseId, CCD.CourtRefNo, CCD.RegistrationDate, 
								case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as COURT,
								SR.PrimaryFinalJDAmount, SR.PrimaryJudgementsDate, SR.PrimaryJDReceiveDate
								,SR.SessionRollId
						 from CourtCasesDetails CCD
						 inner join CourtCases CC on CC.CaseId = CCD.CaseId and CC.ClientCode = '6'
						 left join SessionsRolls SR on CCD.CaseId = SR.CaseId
						 inner join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
						 where CCD.Courtid = '1'
						 AND   CCD.RegistrationDate BETWEEN  @RegistrationDateFrom AND @RegistrationDateTo  
					  ) as PData
					  Group by CaseId, CourtRefNo, RegistrationDate,COURT
					),
				tblAppeal as
					(
					 Select CaseId, CourtRefNo, RegistrationDate,COURT,
					 max(AppealJudgementsDate) as AppealJudgementsDate,
					 max(AppealFinalJDAmount) as AppealFinalJDAmount,
					 max(AppealJDReceiveDate) as AppealJDReceiveDate
					 from
					 (
						 Select CCD.CaseId, CCD.CourtRefNo, CCD.RegistrationDate, 
								case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as COURT,
								SR.AppealFinalJDAmount, SR.AppealJudgementsDate, SR.AppealJDReceiveDate
								,SR.SessionRollId
						 from CourtCasesDetails CCD
						 inner join CourtCases CC on CC.CaseId = CCD.CaseId and CC.ClientCode = '6'
						 left join SessionsRolls SR on CCD.CaseId = SR.CaseId
						 inner join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
						 where Courtid = '2'
						 AND   CCD.RegistrationDate BETWEEN  @RegistrationDateFrom AND @RegistrationDateTo
					  ) as PData
					  Group by CaseId, CourtRefNo, RegistrationDate,COURT
					),
				tblSupreme as
					(
					 Select CaseId, CourtRefNo, RegistrationDate,COURT,
					 max(SupremeJudgementsDate) as SupremeJudgementsDate,
					 max(SupremeFinalJDAmount) as SupremeFinalJDAmount,
					 max(SupremeJDReceiveDate) as SupremeJDReceiveDate
					 from
					 (
						 Select CCD.CaseId, CCD.CourtRefNo, CCD.RegistrationDate, 
								case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as COURT,
								SR.SupremeFinalJDAmount, SR.SupremeJudgementsDate, SR.SupremeJDReceiveDate
								,SR.SessionRollId
						 from CourtCasesDetails CCD
						 inner join CourtCases CC on CC.CaseId = CCD.CaseId and CC.ClientCode = '6'
						 left join SessionsRolls SR on CCD.CaseId = SR.CaseId
						 inner join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
						 where Courtid = '3'
						 AND   CCD.RegistrationDate BETWEEN  @RegistrationDateFrom AND @RegistrationDateTo
					  ) as PData
					  Group by CaseId, CourtRefNo, RegistrationDate,COURT
					),
				tblEnforcement as
					(
						 Select CCD.CaseId, CCD.EnforcementNo as CourtRefNo, CCD.RegistrationDate, 
								case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as COURT,
								CCD.ROPResultDt,CCD.MOHResultDt,CCD.BankResultDt,CCD.ArrestOrderIssueDate,Courtid
						 from CourtCasesEnforcements CCD
						 inner join CourtCases CC on CC.CaseId = CCD.CaseId and CC.ClientCode = '6'
						 inner join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
						 where Courtid = '4'
						 AND   CCD.RegistrationDate BETWEEN  @RegistrationDateFrom AND @RegistrationDateTo
					)
				Select row_number() over(order by CC.CaseId) as SrNo,
						--CC.CaseId,
						--CC.OfficeFileNo,
						FORMAT (CC.ReceptionDate, 'dd-MMM-yy') as ReceivedDate,
						'YandS Law Firm' as LawFirm,
						case when ISNULL(CC.AgainstCode,'0') = '0' then null else replace(CustType.Mst_Desc, 'AGAINST ', '') end as CustomerType,
						CC.Defendant,
						null as Branch,
						CC.AccountContractNo as CustomerNumber,
						CC.IdRegistrationNo,
						null as LoanType,
						case when CC.RealEstateYesNo = 'Y' then 'Yes' else 'No' end as Mortgage,
						CC.ClaimAmount,
						null as InterestRate,
						case when CC.AgainstCode = '3' then 'Defendant' else 'Plaintiff' end as BankStatus,
						--PRIMARY STAGE START
						tblPrimary.CourtRefNo as PrimaryNumber,
						tblPrimary.COURT as PrimaryCourt,
						FORMAT (tblPrimary.RegistrationDate, 'dd-MMM-yy') as PrimaryRegDate,
						tblPrimary.PrimaryFinalJDAmount as PrimaryDesreedAmt,
						FORMAT (tblPrimary.PrimaryJudgementsDate, 'dd-MMM-yy')  as PrimaryJudgementsDate,
						case when tblPrimary.PrimaryJDReceiveDate is not null then 'RCVD' end as PrimaryJDRcvd,
						--PRIMARY STAGE END
						--APPEAL STAGE START
						tblAppeal.CourtRefNo as AppealNumber,
						--tblAppeal.COURT as AppealCourt,
						FORMAT (tblAppeal.RegistrationDate, 'dd-MMM-yy') as AppealRegDate,
						FORMAT (tblAppeal.AppealJudgementsDate, 'dd-MMM-yy') as AppealJudgementsDate,
						tblAppeal.AppealFinalJDAmount as AppealDesreedAmt,
						case when tblAppeal.AppealJDReceiveDate is not null then 'RCVD' end as AppealJDRcvd,
						--APPEAL STAGE END
						--SUPREME STAGE START
						tblSupreme.CourtRefNo as SupremeNumber,
						--tblSupreme.COURT as SupremeCourt,
						FORMAT (tblSupreme.RegistrationDate, 'dd-MMM-yy') as SupremeRegDate,
						FORMAT (tblSupreme.SupremeJudgementsDate, 'dd-MMM-yy') as SupremeJudgementsDate,
						tblSupreme.SupremeFinalJDAmount as SupremeDesreedAmt,
						case when tblSupreme.SupremeJDReceiveDate is not null then 'RCVD' end as SupremeJDRcvd,
						case when tblSupreme.CourtRefNo is not null and tblEnforcement.CourtRefNo is not null then 'RCVD' end  as ExeCopyReceived,
						--SUPREME STAGE END
						--ENFORCEMENT STAGE START
						tblEnforcement.CourtRefNo as EnforcementNumber,
						tblEnforcement.COURT as EnforcementCourt,
						FORMAT (tblEnforcement.RegistrationDate, 'dd-MMM-yy') as EnforcementRegDate,
						case when tblEnforcement.ROPResultDt is not null then 'Y' end as ROP_Y_N,
						case when tblEnforcement.MOHResultDt is not null then 'Y' end as MOH_Y_N,
						case when tblEnforcement.BankResultDt is not null then 'Y' end as BANK_Y_N,
						case when tblEnforcement.ArrestOrderIssueDate is not null then 'Y' end as ARREST_Y_N,
						--ENFORCEMENT STAGE END
						FORMAT (CC.CurrentHearingDate, 'dd-MMM-yy') as UpdateDate,
						FORMAT (CC.NextHearingDate, 'dd-MMM-yy') as NextHearingDate,
						case when cc.CaseStatus = '1' then 'Ongoing' when cc.CaseStatus = '2' then 'Closed' end  as FileStatus,
						case when ISNULL(CC.AgainstCode,'0') = '0' then null when CC.AgainstCode = '3' then 'Against' else 'By' end  as Against,
						CC.CourtDecision
				from CourtCases as CC
				left join MASTER_S as CustType on CustType.Mst_Value = CC.AgainstCode and CustType.MstParentId = 12
				left join tblPrimary on tblPrimary.CaseId = CC.CaseId
				left join tblAppeal on tblAppeal.CaseId = CC.CaseId
				left join tblSupreme on tblSupreme.CaseId = CC.CaseId
				left join tblEnforcement on tblEnforcement.CaseId = CC.CaseId
				where CC.ClientCode = '6'
				AND   CC.ReceptionDate BETWEEN  @ReceptionDateFrom  AND  @ReceptionDateTo
				
		END	  
	ELSE IF @ReportLang = 'btnUFRep'
		BEGIN
				Select row_number() over(order by CC.CaseId) as "SI NO ",
						--CC.CaseId,
						--CC.OfficeFileNo,
						CC.AccountContractNo as LOANACCNO,
						CC.Defendant as "CUSTOMER NAME",
						FORMAT (CC.ReceptionDate, 'dd/MM/yyyy') as "FILE DATE RECEIV",
						(SELECT Max(v) FROM (VALUES (SR.PrimaryJudgementsDate), (SR.AppealJudgementsDate), (SR.SupremeJudgementsDate),(SR.EnforcementJudgementsDate)) AS value(v)) as "DECREE DATE",
						CC.ClaimAmount as "DECREE AMOUNT",
						case when CCE.CourtLocationid = '0' then null else LOC.Mst_Desc end as "PLACE OF COURT",
						CCE.RegistrationDate as "EXECUTION OPEN ON",
						CCE.EnforcementNo as "EXECUTION CASE NO.",
						case when CCE.EnforcementlevelId = '0' then null else ENFLVL.Mst_Desc end as "STATUS",
						CC.CourtDecision as "REMARKS",
						case when CC.LoanManager = '0' then null else LoanMan.Mst_Desc end as "DC_NAME",
						'YANDS' as "LAWYER FIRM "
				from CourtCases as CC
				left join SessionsRolls SR on SR.CaseId =  CC.CaseId and SR.DeletedOn is null
				left join CourtCasesEnforcements CCE on CCE.CaseId = CC.CaseId
				left join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
				left join MASTER_S as ENFLVL on ENFLVL.Mst_Value = CCE.EnforcementlevelId and ENFLVL.MstParentId = 265
				left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
				where CC.ClientCode = '170'
				AND   CC.ReceptionDate BETWEEN  @ReceptionDateFrom  AND  @ReceptionDateTo
				
		END
	ELSE 
	BEGIN
	DECLARE 
		@SQLQuery		NVARCHAR (MAX),
		@SelectColumns	NVARCHAR (MAX),
		@JoinTables		NVARCHAR (MAX),
		@Where			NVARCHAR (MAX) = ' where 1=1 '

			CREATE TABLE #temptbl4CaseRep (
						CaseId int,OfficeFileNo varchar(10),ClientName nvarchar(200),DefClientName nvarchar(200),AgainstName nvarchar(100),
						ReceptionDate DATETIME2,ReceiveLeveName nvarchar(100), AccountContractNo nvarchar(100),ClientFileNo nvarchar(100),
						ClaimAmount decimal(18,3),LegalNoticeDate nvarchar(30), CaseTypeName nvarchar(100),CaseLevelCode nvarchar(10),
						CaseLevelName nvarchar(100),CourtRefNo nvarchar(100),COURT nvarchar(100),RegistrationDate DATETIME2,
						JudgementsDate DATETIME2,JudgementReceive DATETIME2,EnfRegistrationDate DATETIME2,EnfCourtRefNo nvarchar(100),
						CurrentEnfLevel nvarchar(100),ArrestOrderIssueDate DATETIME2,ArrestOrderNo nvarchar(200),UpdatedOn varchar(30),
						CourtDecision nvarchar(MAX),NextHearingDate varchar(30),ODBBankBranchName nvarchar(100),ClientCaseTypeName nvarchar(100),
						DecisionHistory nvarchar(MAX),EnfReOpen nvarchar(100),NationalityName nvarchar(100),GovernorateName nvarchar(100),
						EnforcementBy nvarchar(5),EnforcementlevelId nvarchar(5),LoanManagertName nvarchar(100),ClosingReason nvarchar(100),
						ClosureDate DATETIME2
					 )

	IF @PartialViewName = '_litigation'
	BEGIN
		IF @ReportLang = 'btnENRep'
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.Defendant,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,NULL as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
			END
  		  ELSE
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.SessionRollDefendentName,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,NULL as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.SessionClientId = ClientMas.Mst_Value and ClientMas.MstParentId = 913
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
		
			END

			IF @RegistrationDateFrom != '1800-01-01'
			BEGIN
				SET @Where = @Where + ' AND   EXISTS (select 1 from CourtCasesDetailVW CCD where CCD.CaseId = CC.CaseId AND CCD.CaseLevelCode = CC.CaseLevelCode AND CCD.RegistrationDate BETWEEN  ''' + CAST(@RegistrationDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@RegistrationDateTo AS VARCHAR(30)) + ''') '
			END

			IF  @JudgmentDateFrom != '1800-01-01'
			BEGIN
				SET @Where = @Where + ' AND   EXISTS (select 1 from JudgementDetailView CCD where CCD.CaseId = CC.CaseId AND CCD.CaseLevelCode = CC.CaseLevelCode AND CCD.JudgementsDate BETWEEN  ''' + CAST(@JudgmentDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@JudgmentDateTo AS VARCHAR(30)) + ''') '
			END

			SET @Where = @Where + '
					AND	  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
					AND   (CC.ClientCode = '''+@ClientCode+''' OR '''+@ClientCode+''' = ''0'')
					AND   (ISNULL(CC.ClaimAmount,0) between ''' + CAST(@ClaimAmountFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ClaimAmountTo AS VARCHAR(30)) + ''')
					AND   (CC.CaseTypeCode = '''+@CaseTypeCode+''' OR '''+@CaseTypeCode+''' = ''0'')
					AND   (CC.CaseLevelCode = '''+@CaseLevelCode+''' OR '''+@CaseLevelCode+''' = ''0'')
					AND   (CC.CaseStatus = '''+@CaseStatus+''' OR '''+@CaseStatus+''' = ''0'')
					AND   ISNULL(CC.ReceptionDate,''01-01-1800'') BETWEEN  ''' + CAST(@ReceptionDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ReceptionDateTo AS VARCHAR(30)) + '''
					AND   (CC.ReasonCode = '''+@ReasonCode+''' OR '''+@ReasonCode+''' = ''0'')
				'
			IF @AgainstCode = '0'
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode IN (''1'',''2'') ' END
			ELSE
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode = '''+@AgainstCode+''' ' END

	END
	ELSE IF @PartialViewName = '_closed'
	BEGIN
		  IF @ReportLang = 'btnENRep'
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.Defendant,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,CC.ClosureDate as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
			END
  		  ELSE
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.SessionRollDefendentName,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,CC.ClosureDate as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.SessionClientId = ClientMas.Mst_Value and ClientMas.MstParentId = 913
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
		
			END

			SET @Where = @Where + '
					AND	  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
					AND   (CC.ClientCode = '''+@ClientCode+''' OR '''+@ClientCode+''' = ''0'')
					AND   (ISNULL(CC.ClaimAmount,0) between ''' + CAST(@ClaimAmountFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ClaimAmountTo AS VARCHAR(30)) + ''')
					AND   (CC.CaseTypeCode = '''+@CaseTypeCode+''' OR '''+@CaseTypeCode+''' = ''0'')
					AND   (CC.CaseLevelCode = '''+@CaseLevelCode+''' OR '''+@CaseLevelCode+''' = ''0'')
					AND   (CC.CaseStatus = '''+@CaseStatus+''' OR '''+@CaseStatus+''' = ''0'')
					AND   ISNULL(CC.ClosureDate,''01-01-1800'') BETWEEN  ''' + CAST(@ReceptionDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ReceptionDateTo AS VARCHAR(30)) + '''
					AND   (CC.ReasonCode = '''+@ReasonCode+''' OR '''+@ReasonCode+''' = ''0'')
				'
			IF @AgainstCode = '0'
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode IN (''1'',''2'') ' END
			ELSE
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode = '''+@AgainstCode+''' ' END
			

	END
	ELSE IF @PartialViewName = '_enforcement'
	BEGIN
	
		  IF @ReportLang = 'btnENRep'
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.Defendant,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,CC.ClosureDate as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
			END
  		  ELSE
			BEGIN
				SET @SelectColumns = ' CC.CaseId, CC.OfficeFileNo,ClientMas.Mst_Desc,CC.SessionRollDefendentName,AgainstMas.Mst_Desc,CC.ReceptionDate,RcvLvlMas.Mst_Desc
								,CC.AccountContractNo,CC.ClientFileNo,CC.ClaimAmount,NULL as LegalNoticeDate,CaseTypeMas.Mst_Desc
								,CC.CaseLevelCode,CaseLevelMas.Mst_Desc, NULL as CourtRefNo, NULL as COURT,NULL AS RegistrationDate
								,NULL as JudgementsDate,NULL as JudgementReceive,NULL as EnfRegistrationDate,NULL as EnfCourtRefNo
								,NULL as CurrentEnfLevel, NULL as ArrestOrderIssueDate, NULL as ArrestOrderNo, FORMAT(ISNULL(CC.UpdatedOn,CC.CreatedOn), ''dd/MM/yyyy'') as UpdatedOn
								,CC.CourtDecision,FORMAT(CC.NextHearingDate, ''dd/MM/yyyy''),ODBBranchMas.Mst_Desc,ClientCaseTypeMas.Mst_Desc, NULL as DecisionHistory
								,case when CC.ReOpenEnforcement = ''1'' then ''NO لا'' when CC.ReOpenEnforcement = ''2'' then ''YES نعم'' end as EnfReOpen
								,OmaniExp.Mst_Desc, GovName.Mst_Desc, NULL as EnforcementBy, NULL as EnforcementlevelId,LoanMan.Mst_Desc,CloseRes.Mst_Desc
								,CC.ClosureDate as ClosureDate
								FROM CourtCases as CC '
		
				SET @JoinTables = ' 
					join MASTER_S ClientMas on CC.SessionClientId = ClientMas.Mst_Value and ClientMas.MstParentId = 913
					join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
					join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
					join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
					join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
					join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18	
					join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
					left join MASTER_S OmaniExp on CC.OmaniExp = OmaniExp.Mst_Value and OmaniExp.MstParentId = 460
					left join MASTER_S GovName on CC.GovernorateId = GovName.Mst_Value and GovName.MstParentId = 1153
					left join MASTER_S LoanMan on CC.LoanManager = LoanMan.Mst_Value and LoanMan.MstParentId = 428
					left join MASTER_S CloseRes on CC.ReasonCode = CloseRes.Mst_Value and CloseRes.MstParentId = 395
					'
		
			END
		
			if @ApealByWho != '0'
			begin
				SET @Where = @Where + ' AND   EXISTS (select 1 from CourtCasesEnforcements CCE where CCE.CaseId = CC.CaseId and CCE.EnforcementBy = '''+@ApealByWho+''') '
			end

			if @CourtLocationid != '0'
			begin
				SET @Where = @Where + ' AND   EXISTS (select 1 from CourtCasesEnforcements CCE where CCE.CaseId = CC.CaseId and CCE.CourtLocationid = '''+@CourtLocationid+''') '
			end

			if @EnforcementlevelId != '0'
			begin
				SET @Where = @Where + ' AND   EXISTS (select 1 from CourtCasesEnforcements CCE where CCE.CaseId = CC.CaseId and CCE.EnforcementlevelId = '''+@EnforcementlevelId+''') '
			end

			IF @RegistrationDateFrom != '1800-01-01'
			BEGIN
				SET @Where = @Where + ' AND   EXISTS (select 1 from CourtCasesEnforcements CCE where CCE.CaseId = CC.CaseId AND CCE.RegistrationDate BETWEEN  ''' + CAST(@RegistrationDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@RegistrationDateTo AS VARCHAR(30)) + ''') '
			END

			SET @Where = @Where + '
					AND	  (LEFT(CC.OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
					AND   (CC.ClientCode = '''+@ClientCode+''' OR '''+@ClientCode+''' = ''0'')
					AND   (ISNULL(CC.ClaimAmount,0) between ''' + CAST(@ClaimAmountFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ClaimAmountTo AS VARCHAR(30)) + ''')
					AND   (CC.CaseTypeCode = '''+@CaseTypeCode+''' OR '''+@CaseTypeCode+''' = ''0'')
					AND   (CC.CaseLevelCode = '''+@CaseLevelCode+''' OR '''+@CaseLevelCode+''' = ''0'')
					AND   (CC.CaseStatus = '''+@CaseStatus+''' OR '''+@CaseStatus+''' = ''0'')
					AND   (CC.GovernorateId = '''+@GovernorateId+''' OR '''+@GovernorateId+''' = ''0'')
					AND   (CC.AgainstInsurance = '''+@AgainstInsurance+''' OR '''+@AgainstInsurance+''' = ''0'')
					AND   (CC.ODBBankBranch = '''+@ODBBankBranch+''' OR '''+@ODBBankBranch+''' = ''0'')
					AND   (CC.ClientCaseType = '''+@ClientCaseType+''' OR '''+@ClientCaseType+''' = ''0'')
					AND   (CC.OmaniExp = '''+@OmaniExp+''' OR '''+@OmaniExp+''' = ''0'')
					AND   (CC.ReOpenEnforcement = '''+@ReOpenEnforcement+''' OR '''+@ReOpenEnforcement+''' = ''0'')
					AND   ISNULL(CC.ReceptionDate,''01-01-1800'') BETWEEN  ''' + CAST(@ReceptionDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@ReceptionDateTo AS VARCHAR(30)) + '''
					AND   (CC.ReasonCode = '''+@ReasonCode+''' OR '''+@ReasonCode+''' = ''0'')
				'
			IF @AgainstCode = '0'
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode IN (''1'',''2'') ' END
			ELSE
				BEGIN SET @Where = @Where + ' AND   CC.AgainstCode = '''+@AgainstCode+''' ' END


	END


	set @SQLQuery = 'SELECT ' + @SelectColumns + @JoinTables + @Where + ' ORDER BY dbo.fnMixSort(CC.OfficeFileNo)'
	print @SQLQuery

		INSERT into #temptbl4CaseRep execute (@SQLQuery )

		DECLARE @LCCaseId INT
		DECLARE @LCCaseLevelCode NVARCHAR(5)
		DECLARE @c_rec CURSOR

		SET @c_rec = CURSOR FOR SELECT CaseId,CaseLevelCode	FROM #temptbl4CaseRep

		OPEN @c_rec
		FETCH NEXT
		FROM @c_rec INTO @LCCaseId, @LCCaseLevelCode
		WHILE @@FETCH_STATUS = 0
		BEGIN
			declare 
			@lc_CourtRefNo				nvarchar(100),
			@lc_LegalNoticeDate			nvarchar(30),
			@lc_COURT					nvarchar(100),
			@lc_COURT_ENF				nvarchar(100),
			@lc_RegistrationDate		DATETIME2,
			@lc_JudgementsDate			DATETIME2,
			@lc_JudgementReceive		DATETIME2,
			@lc_EnfRegistrationDate		DATETIME2,
			@lc_EnfCourtRefNo			nvarchar(100),
			@lc_CurrentEnfLevel			nvarchar(100),
			@lc_ArrestOrderIssueDate	DATETIME2,
			@lc_ArrestOrderNo			nvarchar(200),
			@lc_DecisionHistory			nvarchar(MAX),
			@lc_EnforcementBy			nvarchar(5),
			@lc_EnforcementlevelId		nvarchar(5)

			SET		@lc_LegalNoticeDate = dbo.FnGetLegalNoticeDate(@LCCaseId)

			SELECT @lc_CourtRefNo = CCD.CourtRefNo, @lc_COURT = case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end,
					@lc_RegistrationDate = CCD.RegistrationDate
			FROM CourtCasesDetails as CCD
			left join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
			left join MASTER_S as ENFBY on ENFBY.Mst_Value = CCD.ApealByWho and ENFBY.MstParentId = 391
			WHERE CCD.CaseId		= @LCCaseId
			AND   CCD.CaseLevelCode = @LCCaseLevelCode
		
			SELECT @lc_JudgementsDate = JudgementsDate, @lc_JudgementReceive = JudgementReceive
			FROM	JudgementDetailView 
			WHERE	CaseId			= @LCCaseId
			AND		CaseLevelCode	= @LCCaseLevelCode

			SELECT @lc_EnfRegistrationDate = CCE.RegistrationDate, @lc_EnfCourtRefNo = CCE.EnforcementNo,
					@lc_CurrentEnfLevel = case when CCE.EnforcementlevelId = '0' then null else ENFLVL.Mst_Desc end,
					@lc_ArrestOrderIssueDate = CCE.ArrestOrderIssueDate, @lc_ArrestOrderNo = CCE.ArrestOrderNo,
					@lc_EnforcementBy = CCE.EnforcementBy, @lc_EnforcementlevelId = CCE.EnforcementlevelId,
					@lc_COURT_ENF = case when CCE.CourtLocationid = '0' then null else LOC.Mst_Desc end
			FROM CourtCasesEnforcements CCE
			left join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
			left join MASTER_S as ENFLVL on ENFLVL.Mst_Value = CCE.EnforcementlevelId and ENFLVL.MstParentId = 265
			left join MASTER_S as ENFBY on ENFBY.Mst_Value = CCE.EnforcementBy and ENFBY.MstParentId = 391
			WHERE CCE.CaseId		= @LCCaseId

			SELECT	@lc_DecisionHistory	= DecisionHistory
			FROM	DecisionHistoryView  
			WHERE   CaseId = @LCCaseId
		
			UPDATE  #temptbl4CaseRep 
			SET		LegalNoticeDate			= @lc_LegalNoticeDate,
					CourtRefNo				= case when @LCCaseLevelCode = '6' then @lc_EnfCourtRefNo else @lc_CourtRefNo end, 
					COURT					= case when @LCCaseLevelCode = '6' then @lc_COURT_ENF else @lc_COURT end,
					RegistrationDate		= @lc_RegistrationDate,
					JudgementsDate			= @lc_JudgementsDate, 
					JudgementReceive		= @lc_JudgementReceive,
					EnfRegistrationDate		= @lc_EnfRegistrationDate, 
					EnfCourtRefNo			= @lc_EnfCourtRefNo, 
					CurrentEnfLevel			= @lc_CurrentEnfLevel, 
					ArrestOrderIssueDate	= @lc_ArrestOrderIssueDate,
					ArrestOrderNo			= @lc_ArrestOrderNo,
					EnforcementBy			= @lc_EnforcementBy,
					EnforcementlevelId		= @lc_EnforcementlevelId,
					DecisionHistory         = @lc_DecisionHistory
			WHERE	CaseId		= @LCCaseId
			--COLLATE DATABASE_DEFAULT

			FETCH NEXT
			FROM @c_rec INTO @LCCaseId, @LCCaseLevelCode
		END

		CLOSE @c_rec
		DEALLOCATE @c_rec

	IF @PartialViewName = '_litigation'
		BEGIN
		 set @SQLQuery = '	
				 Select OfficeFileNo as "OFFICE FILE NO"
				 ,case when ClientName like ''PLEASE SELECT%'' then null else ClientName end as "CLIENT NAME"
				 ,DefClientName as "DEFENDANT"
				 ,case when AgainstName like ''PLEASE SELECT%'' then null else AgainstName end as "AGAINST NAME"
				 ,ReceptionDate as "RECEIVE DATE"
				 ,case when ReceiveLeveName like ''PLEASE SELECT%'' then null else ReceiveLeveName end as "RECEIVE LEVEL NAME"
				 ,AccountContractNo as "ACCOUNT CONTRACT NO"
				 ,ClientFileNo as "CLIENT FILE NO"
				 ,ClaimAmount as "CLAIM AMOUNT"
				 ,LegalNoticeDate as "LEGAL NOTICE DATE"
				 ,case when CaseTypeName like ''PLEASE SELECT%'' then null else CaseTypeName end as "CASE TYPE NAME"
				 ,case when CaseLevelName like ''PLEASE SELECT%'' then null else CaseLevelName end as "CASE LEVEL NAME"
				 ,CourtRefNo as "CASE NO."
				 ,COURT as "COURT"
				 ,FORMAT(RegistrationDate, ''dd/MM/yyyy'') as "REGISTRATION DATE"
				 ,FORMAT(JudgementsDate, ''dd/MM/yyyy'') as "JUDGMENT DATE"
				 ,FORMAT(JudgementReceive, ''dd/MM/yyyy'') as "JUDGEMENT RECEIVE"
				 ,FORMAT(EnfRegistrationDate, ''dd/MM/yyyy'') as "ENFORCEMENT REGISTRATION DATE"
				 ,EnfCourtRefNo as "ENFORCEMENT NO"
				 ,FORMAT(ArrestOrderIssueDate, ''dd/MM/yyyy'') as "DATE OF ARREST ORDER"
				 ,ArrestOrderNo  as "ARREST ORDER NO"
				 ,UpdatedOn as "DATE OF UPDATE"
				 ,CourtDecision as "COURT DECISION"
				 ,DecisionHistory as "CASE HISTORY"
				 ,NextHearingDate as "NEXT HEARING DATE"
				 ,case when ODBBankBranchName like ''PLEASE SELECT%'' then null else ODBBankBranchName end as "ODB BANK BRANCH NAME"
				 ,case when ClientCaseTypeName like ''PLEASE SELECT%'' then null else ClientCaseTypeName end as "CLIENT CASE TYPE"
				 ,case when LoanManagertName like ''PLEASE SELECT%'' then null else LoanManagertName end as "LOAN MANAGER"
				 from #temptbl4CaseRep
				'

			SET @Where = ' WHERE 1=1 '

			--IF @CaseLevelCode = '6'
			--	BEGIN
			--		SET @Where = @Where + 'AND   ISNULL(EnfRegistrationDate,''01-01-1800'') BETWEEN  ''' + CAST(@RegistrationDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@RegistrationDateTo AS VARCHAR(30)) + ''' '
			--	END
			--ELSE IF @CaseLevelCode IN ('3','4','5')
			--	BEGIN
			--	    SET @Where = @Where + 'AND   ISNULL(RegistrationDate,''01-01-1800'') BETWEEN  ''' + CAST(@RegistrationDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@RegistrationDateTo AS VARCHAR(30)) + ''' '
			--		SET @Where = @Where + 'AND   ISNULL(JudgementsDate,''01-01-1800'') BETWEEN  ''' + CAST(@JudgmentDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@JudgmentDateTo AS VARCHAR(30)) + ''' '
			--	END

	
	END
	ELSE IF @PartialViewName = '_closed'
	BEGIN
		 set @SQLQuery = '	
				 Select OfficeFileNo as "OFFICE FILE NO"
				 ,case when ClientName like ''PLEASE SELECT%'' then null else ClientName end as "CLIENT NAME"
				 ,DefClientName as "DEFENDANT"
				 ,case when AgainstName like ''PLEASE SELECT%'' then null else AgainstName end as "AGAINST"
				 --,ReceptionDate as "RECEIVE DATE"
				 --,case when ReceiveLeveName like ''PLEASE SELECT%'' then null else ReceiveLeveName end as "RECEIVE LEVEL NAME"
				 ,AccountContractNo as "AC / CONTRACT NO"
				 ,ClientFileNo as "CIF"
				 ,ClaimAmount as "CLAIM AMOUNT"
				 --,LegalNoticeDate as "LEGAL NOTICE DATE"
				 --,case when CaseTypeName like ''PLEASE SELECT%'' then null else CaseTypeName end as "CASE TYPE NAME"
				 --,case when CaseLevelName like ''PLEASE SELECT%'' then null else CaseLevelName end as "CASE LEVEL NAME"
				 --,CourtRefNo as "CASE NO."
				 --,COURT as "COURT"
				 --,FORMAT(RegistrationDate, ''dd/MM/yyyy'') as "REGISTRATION DATE"
				 --,FORMAT(JudgementsDate, ''dd/MM/yyyy'') as "JUDGMENT DATE"
				 --,FORMAT(JudgementReceive, ''dd/MM/yyyy'') as "JUDGEMENT RECEIVE"
				 --,FORMAT(EnfRegistrationDate, ''dd/MM/yyyy'') as "ENFORCEMENT REGISTRATION DATE"
				 --,EnfCourtRefNo as "ENFORCEMENT NO"
				 --,FORMAT(ArrestOrderIssueDate, ''dd/MM/yyyy'') as "DATE OF ARREST ORDER"
				 --,ArrestOrderNo  as "ARREST ORDER NO"
				 --,UpdatedOn as "DATE OF UPDATE"
				 --,CourtDecision as "COURT DECISION"
				 --,DecisionHistory as "CASE HISTORY"
				 --,NextHearingDate as "NEXT HEARING DATE"
				 --,case when ODBBankBranchName like ''PLEASE SELECT%'' then null else ODBBankBranchName end as "ODB BANK BRANCH NAME"
				 --,case when ClientCaseTypeName like ''PLEASE SELECT%'' then null else ClientCaseTypeName end as "CLIENT CASE TYPE"
				 ,case when LoanManagertName like ''PLEASE SELECT%'' then null else LoanManagertName end as "MANAGER"
				 ,case when ClosingReason like ''PLEASE SELECT%'' then null else ClosingReason end as "REASON"
				 ,FORMAT(ClosureDate, ''dd/MM/yyyy'') as "CLOSURE DATE"
				 ,case when CaseLevelName like ''PLEASE SELECT%'' then null else CaseLevelName end as "CLOSURE LEVEL"
				 from #temptbl4CaseRep
				'

			SET @Where = ' WHERE 1=1 '


	END
	ELSE IF @PartialViewName = '_enforcement'
	BEGIN
		 set @SQLQuery = '	
				 Select OfficeFileNo as "OFFICE FILE NO"
				 ,case when ClientName like ''PLEASE SELECT%'' then null else ClientName end as "CLIENT NAME"
				 ,DefClientName as "DEFENDANT"
				 ,case when NationalityName like ''PLEASE SELECT%'' then null else NationalityName end as "NATIONALITY"
				 ,case when GovernorateName like ''PLEASE SELECT%'' then null else GovernorateName end as "GOVERNORATE"
				 ,case when AgainstName like ''PLEASE SELECT%'' then null else AgainstName end as "AGAINST NAME"
				 ,ReceptionDate as "RECEIVE DATE"
				 ,case when ReceiveLeveName like ''PLEASE SELECT%'' then null else ReceiveLeveName end as "RECEIVE LEVEL NAME"
				 ,AccountContractNo as "ACCOUNT CONTRACT NO"
				 ,ClientFileNo as "CLIENT FILE NO"
				 ,ClaimAmount as "CLAIM AMOUNT"
				 ,case when CaseTypeName like ''PLEASE SELECT%'' then null else CaseTypeName end as "CASE TYPE NAME"
				 ,FORMAT(EnfRegistrationDate, ''dd/MM/yyyy'') as "ENFORCEMENT REGISTRATION DATE"
				 ,COURT as "COURT"
				 ,EnfCourtRefNo as "ENFORCEMENT NO"
				 ,case when CurrentEnfLevel like ''PLEASE SELECT%'' then null else CurrentEnfLevel end as "CURRENT ENFORCEMENT LEVEL"
				 ,FORMAT(ArrestOrderIssueDate, ''dd/MM/yyyy'') as "DATE OF ARREST ORDER"
				 ,ArrestOrderNo  as "ARREST ORDER NO"
				 ,UpdatedOn as "DATE OF UPDATE"
				 ,CourtDecision as "COURT DECISION"
				 ,DecisionHistory as "CASE HISTORY"
				 ,case when ODBBankBranchName like ''PLEASE SELECT%'' then null else ODBBankBranchName end as "ODB BANK BRANCH NAME"
				 ,case when ClientCaseTypeName like ''PLEASE SELECT%'' then null else ClientCaseTypeName end as "ODB LOAN TYPE"
				 ,EnfReOpen as "RE-OPEN ENFC"
				 ,case when LoanManagertName like ''PLEASE SELECT%'' then null else LoanManagertName end as "LOAN MANAGER"
				 from #temptbl4CaseRep
				'

			SET @Where = ' WHERE 1=1 '
			--SET @Where = @Where + 'AND   ISNULL(EnfRegistrationDate,''01-01-1800'') BETWEEN  ''' + CAST(@RegistrationDateFrom AS VARCHAR(30)) + ''' AND ''' + CAST(@RegistrationDateTo AS VARCHAR(30)) + ''' '
			--SET @Where = @Where + 'AND   (EnforcementBy = '''+@ApealByWho+''' OR '''+@ApealByWho+''' = ''0'') '
			--SET @Where = @Where + 'AND   (EnforcementlevelId = '''+@EnforcementlevelId+''' OR '''+@EnforcementlevelId+''' = ''0'') '
	END

	--set @SQLQuery =  @SQLQuery + @Where + ' ORDER BY dbo.fnMixSort(OfficeFileNo)'

	print @SQLQuery
	exec (@SQLQuery)

	drop table #temptbl4CaseRep

	END		
END		
GO
