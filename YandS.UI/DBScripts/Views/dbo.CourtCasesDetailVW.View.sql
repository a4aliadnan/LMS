SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [CourtCasesDetailVW] as 
Select *
from
(
SELECT DetailId, CaseId,CourtRefNo,CourtLocationid,CaseLevelCode,ApealByWho,CourtLocation,RegistrationDate, ArrestOrderIssueDate, ArrestOrderNo,UpdatedOn,CurrentEnforcementLevel,EnforcementBy,EnforcementlevelId
FROM
(
SELECT CCD.DetailId,CCD.CaseId,CCD.CourtRefNo,CCD.CourtLocationid,CCD.CaseLevelCode,CCD.ApealByWho
	, case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation
	  ,CCD.RegistrationDate, null as ArrestOrderIssueDate, null as ArrestOrderNo,ISNULL(CCD.UpdatedOn,CCD.CreatedOn) as UpdatedOn
	  , null as CurrentEnforcementLevel
	  , case when CCD.ApealByWho = '0' OR CCD.ApealByWho is null then null else ENFBY.Mst_Desc end as EnforcementBy
	  ,null as EnforcementlevelId
	  ,rank() over (partition by CCD.CaseId order by CCD.DetailId desc) as rnk
FROM CourtCasesDetails as CCD
left join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
left join MASTER_S as ENFBY on ENFBY.Mst_Value = CCD.ApealByWho and ENFBY.MstParentId = 391
) detData
UNION ALL
SELECT DetailId, CaseId,CourtRefNo,CourtLocationid,CaseLevelCode,ApealByWho,CourtLocation,RegistrationDate, ArrestOrderIssueDate, ArrestOrderNo,UpdatedOn,CurrentEnforcementLevel,EnforcementBy,EnforcementlevelId
FROM
(
SELECT CCE.EnforcementId as DetailId, CCE.CaseId,CCE.EnforcementNo as CourtRefNo,CCE.CourtLocationid, '6' as CaseLevelCode,CCE.EnforcementBy as ApealByWho
		, case when CCE.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation
		,CCE.RegistrationDate, CCE.ArrestOrderIssueDate, CCE.ArrestOrderNo,ISNULL(CCE.UpdatedOn,CCE.CreatedOn) as UpdatedOn
		, case when CCE.EnforcementlevelId = '0' then null else ENFLVL.Mst_Desc end as CurrentEnforcementLevel
		, case when CCE.EnforcementBy = '0' OR CCE.EnforcementBy is null then null else ENFBY.Mst_Desc end as EnforcementBy
		,CCE.EnforcementlevelId
		,rank() over (partition by CCE.CaseId order by CCE.EnforcementId desc) as rnk
FROM CourtCasesEnforcements CCE
left join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
left join MASTER_S as ENFLVL on ENFLVL.Mst_Value = CCE.EnforcementlevelId and ENFLVL.MstParentId = 265
left join MASTER_S as ENFBY on ENFBY.Mst_Value = CCE.EnforcementBy and ENFBY.MstParentId = 391
) enfData
) 
DAT
GO
