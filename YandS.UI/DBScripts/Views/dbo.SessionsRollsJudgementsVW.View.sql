SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE view [SessionsRollsJudgementsVW] as
Select	SessionRollId,CaseId,PrimaryJudgementsDate,AppealJudgementsDate,EnforcementJudgementsDate,SupremeJudgementsDate
from
	(
		Select	SessionRollId,CaseId,PrimaryJudgementsDate,AppealJudgementsDate,EnforcementJudgementsDate,SupremeJudgementsDate, rank() over (partition by CaseId order by SessionRollId desc) as rnk 
		from SessionsRolls 
		where (SELECT Max(v) FROM (VALUES (PrimaryJudgementsDate), (AppealJudgementsDate), (EnforcementJudgementsDate),(SupremeJudgementsDate)) AS value(v)) is not null
	) dat
where rnk = 1
GO
