SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetCourtCasesByStatus]
    @Location varchar(1),
	@CaseStatus varchar(5),
	@ReceptionDateFrom date,
	@ReceptionDateTo date
AS
select 
	ROW_NUMBER() OVER (ORDER BY CaseId) AS 'S No.'
	,Defendant
	,AccountContractNo
	,ClaimAmount
	,max(PrimaryCourt) as 'Primary Court'
	,max(PrimaryCaseNo) as 'Primary Case No.'
	,max(AppealCourt) as 'Appeal Court'
	,max(AppealNo) as 'Appeal No.'
	,max(SupremeCaseNo) as 'Supreme Case No.'
	,max(ExecutionCourt) as 'Execution Court'
	,max(ExecutionNo) as 'Execution No.'
	,max(ArrestOrderNo) as 'Arrest Order No.'
	,Updates
from 
(
select CC.CaseId,CC.OfficeFileNo, CC.Defendant,CC.AccountContractNo,CC.ClaimAmount,cc.YandSNotes as 'Updates',CC.CaseStatus
		, Case when CD.Courtid = '1' then LocMasCD.Mst_Desc end as 'PrimaryCourt'
		, Case when CD.Courtid = '1' then CD.CourtRefNo end as 'PrimaryCaseNo'
		, Case when CD.Courtid = '2' then LocMasCD.Mst_Desc end as 'AppealCourt'
		, Case when CD.Courtid = '2' then CD.CourtRefNo end as 'AppealNo'
		, Case when CD.Courtid = '3' then CD.CourtRefNo end as 'SupremeCaseNo'
		, Case when CE.Courtid = '4' then LocMasCE.Mst_Desc end as 'ExecutionCourt'
		, Case when CE.Courtid = '4' then CE.EnforcementNo end as 'ExecutionNo'
		, Case when CE.Courtid = '4' then CE.ArrestOrderNo end as 'ArrestOrderNo'
from CourtCases CC 
left join CourtCasesDetails as CD      on CC.CaseId = CD.CaseId
left join CourtCasesEnforcements as CE on CC.CaseId = CE.CaseId
left join MASTER_S as LocMasCD     on CD.CourtLocationid = LocMasCD.Mst_Value and LocMasCD.MstParentId = 4
left join MASTER_S as LocMasCE     on CE.CourtLocationid = LocMasCE.Mst_Value and LocMasCE.MstParentId = 4
where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
AND   (CC.CaseStatus = @CaseStatus OR @CaseStatus = '0')   
AND   CC.ReceptionDate BETWEEN  @ReceptionDateFrom AND @ReceptionDateTo
) as data
group by CaseId,Defendant,AccountContractNo,ClaimAmount,Updates
ORDER BY 1

GO
