SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [CaseNosVW] as 
Select *
from
(
SELECT	CaseId
		,STRING_AGG(CourtRefNo, '^ ') WITHIN GROUP (ORDER BY RegistrationDate DESC) AS CASE_NO
		,STRING_AGG(CaseLevelCode, '^ ') WITHIN GROUP (ORDER BY RegistrationDate DESC) AS CASE_LEVEL
		,STRING_AGG(CaseLevelCode + '|' + CourtRefNo, '^ ') WITHIN GROUP (ORDER BY RegistrationDate DESC) AS CASE_NO_CASE_LEVEL
FROM
(
SELECT 
		CCD.DetailId,
		CCD.CaseId,
		CCD.CourtRefNo,
		CCD.CourtLocationid,
		CCD.CaseLevelCode,
		CCD.ApealByWho,
		case when CCD.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation,
		CCD.RegistrationDate,
		null as ArrestOrderIssueDate,
		null as ArrestOrderNo,
		ISNULL(CCD.UpdatedOn,CCD.CreatedOn) as UpdatedOn,
		null as CurrentEnforcementLevel,
		null as ActionDate
FROM CourtCasesDetails as CCD
Left Join MASTER_S as LOC on LOC.Mst_Value = CCD.CourtLocationid and LOC.MstParentId = 4
UNION ALL
SELECT	CCE.EnforcementId as DetailId,
		CCE.CaseId,
		CCE.EnforcementNo as CourtRefNo,
		CCE.CourtLocationid, '6' as CaseLevelCode,
		CCE.EnforcementBy as ApealByWho,
		case when CCE.CourtLocationid = '0' then null else LOC.Mst_Desc end as CourtLocation,
		CCE.RegistrationDate,
		CCE.ArrestOrderIssueDate,
		CCE.ArrestOrderNo,
		ISNULL(CCE.UpdatedOn,CCE.CreatedOn) as UpdatedOn,
		case when CCE.EnforcementlevelId = '0' then null else ENFLVL.Mst_Desc end as CurrentEnforcementLevel,
		CCE.ActionDate
FROM CourtCasesEnforcements CCE
left join MASTER_S as LOC on LOC.Mst_Value = CCE.CourtLocationid and LOC.MstParentId = 4
left join MASTER_S as ENFLVL on ENFLVL.Mst_Value = CCE.EnforcementlevelId and ENFLVL.MstParentId = 265
) AS CASE_DETAIL
GROUP BY CaseId
) FINAL_DATA;


GO
