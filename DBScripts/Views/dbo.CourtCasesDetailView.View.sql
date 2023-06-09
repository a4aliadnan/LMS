SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [CourtCasesDetailView] as 
Select *
from
(
SELECT DetailId, CaseId,CourtRefNo,CourtLocationid,CaseLevelCode,ApealByWho,CourtLocation,RegistrationDate, ArrestOrderIssueDate, ArrestOrderNo,UpdatedOn,CurrentEnforcementLevel,ActionDate
		,rank() over (partition by CaseId order by UpdatedOn desc) as Frnk
from
(
SELECT DetailId, CaseId,CourtRefNo,CourtLocationid,CaseLevelCode,ApealByWho,CourtLocation,RegistrationDate, ArrestOrderIssueDate, ArrestOrderNo,UpdatedOn,null as CurrentEnforcementLevel,null as ActionDate
FROM
(
SELECT CCD.DetailId,CCD.CaseId,CCD.CourtRefNo,CCD.CourtLocationid,CCD.CaseLevelCode,CCD.ApealByWho
	, case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation
	  ,CCD.RegistrationDate, null as ArrestOrderIssueDate, null as ArrestOrderNo,ISNULL(CCD.UpdatedOn,CCD.CreatedOn) as UpdatedOn
	  ,rank() over (partition by CCD.CaseId order by ISNULL(CCD.UpdatedOn,CCD.CreatedOn) desc) as rnk
FROM CourtCasesDetails as CCD
left join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
) detData
where rnk = 1
UNION ALL
SELECT DetailId, CaseId,CourtRefNo,CourtLocationid,CaseLevelCode,ApealByWho,CourtLocation,RegistrationDate, ArrestOrderIssueDate, ArrestOrderNo,UpdatedOn,CurrentEnforcementLevel,ActionDate
FROM
(
SELECT CCE.EnforcementId as DetailId, CCE.CaseId,CCE.EnforcementNo as CourtRefNo,CCE.CourtLocationid, '6' as CaseLevelCode,CCE.EnforcementBy as ApealByWho
		, case when CCE.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation
		,CCE.RegistrationDate, CCE.ArrestOrderIssueDate, CCE.ArrestOrderNo,ISNULL(CCE.UpdatedOn,CCE.CreatedOn) as UpdatedOn
		, case when CCE.EnforcementlevelId = '0' then null else ENFLVL.Mst_Desc end as CurrentEnforcementLevel,CCE.ActionDate
		,rank() over (partition by CCE.CaseId order by CCE.EnforcementId desc) as rnk
FROM CourtCasesEnforcements CCE
left join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
left join MASTER_S as ENFLVL on ENFLVL.Mst_Value = CCE.EnforcementlevelId and ENFLVL.MstParentId = 265
) enfData
where rnk = 1
) 
DAT
)
FDATA
where Frnk = 1
GO
