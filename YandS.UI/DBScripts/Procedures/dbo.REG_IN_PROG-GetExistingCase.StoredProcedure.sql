SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [REG_IN_PROG-GetExistingCase]
   @P_OfficeFileNo VARCHAR(10),
   @P_DataForTable VARCHAR(50)
AS

IF @P_DataForTable in ('TBR-AppSubmission','TBR-SupSubmission')
	BEGIN
	SELECT CC.CaseId
			,CC.OfficeFileNo
			,ClientMas.Mst_Desc as ClientName
			,CC.Defendant
			,CC.AccountContractNo
			,CC.ClientFileNo
			,CC.ReceptionDate
			,CC.ClaimAmount
			,case when CC.SessionClientId = '0' then null else ClientArMas.Mst_Desc end as SessionRollClientName 
			,CC.SessionRollDefendentName as SessionRollDefendentName
			,Case when CR.EnforcementDispute = '0' then TblActionLevel.Mst_Desc else TblEnfDispute.Mst_Desc end as RegistrationLevelDetail
	from  CourtCases CC
	left join  CaseRegistrations CR on CC.CaseId = CR.CaseId AND CR.IsDeleted = 0 AND case when @P_DataForTable = 'TBR-AppSubmission' then '2' when @P_DataForTable = 'TBR-SupSubmission' then '3' end = CR.ActionLevel
	Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
	Left  join MASTER_S ClientArMas on CC.SessionClientId = ClientArMas.Mst_Value and ClientArMas.MstParentId = 913
	left join  MASTER_S TblActionLevel on CR.ActionLevel = TblActionLevel.Mst_Value and TblActionLevel.MstParentId = 785
	left join  MASTER_S TblEnfDispute on CR.EnforcementDispute = TblEnfDispute.Mst_Value and TblEnfDispute.MstParentId = 786
	where  CC.OfficeFileNo = @P_OfficeFileNo

	Select COUNT(*) AS caseRegisteredExists
	FROM CaseRegistrations CR 
	join  CourtCases CC on CC.CaseId = CR.CaseId
	where CR.IsDeleted = 0
	AND   CC.OfficeFileStatus != 'OFS-2'
	AND   CC.CaseStatus = '1'
	AND   1 = case	when @P_DataForTable = 'TBR-AppSubmission' AND  CC.CaseLevelCode not in ('4','6') then 1
					when @P_DataForTable = 'TBR-SupSubmission' AND  CC.CaseLevelCode not in ('5','6') then 1
			  end
	AND   CC.OfficeFileNo = @P_OfficeFileNo
	AND    case when @P_DataForTable = 'TBR-AppSubmission' then '2'
				when @P_DataForTable = 'TBR-SupSubmission' then '3'
		   end = CR.ActionLevel
	END
ELSE 
	BEGIN
	SELECT CC.CaseId
			,CC.OfficeFileNo
			,ClientMas.Mst_Desc as ClientName
			,CC.Defendant
			,CC.AccountContractNo
			,CC.ClientFileNo
			,CC.ReceptionDate
			,CC.ClaimAmount
			,case when CC.SessionClientId = '0' then null else ClientArMas.Mst_Desc end as SessionRollClientName 
			,CC.SessionRollDefendentName as SessionRollDefendentName
			,Case when CR.EnforcementDispute = '0' then TblActionLevel.Mst_Desc else TblEnfDispute.Mst_Desc end as RegistrationLevelDetail
	from  CourtCases CC
	left join  CaseRegistrations CR on CC.CaseId = CR.CaseId AND CR.IsDeleted = 0 AND CR.ActionLevel = '4'
	Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
	Left  join MASTER_S ClientArMas on CC.SessionClientId = ClientArMas.Mst_Value and ClientArMas.MstParentId = 913
	left join  MASTER_S TblActionLevel on CR.ActionLevel = TblActionLevel.Mst_Value and TblActionLevel.MstParentId = 785
	left join  MASTER_S TblEnfDispute on CR.EnforcementDispute = TblEnfDispute.Mst_Value and TblEnfDispute.MstParentId = 786
	where  CC.OfficeFileNo = @P_OfficeFileNo
	

	Select COUNT(*) AS caseRegisteredExists
	FROM CaseRegistrations CR 
	join  CourtCases CC on CC.CaseId = CR.CaseId
	where CR.IsDeleted = 0
	AND    CC.OfficeFileNo = @P_OfficeFileNo
	AND    CR.ActionLevel = '4'
	END
GO
