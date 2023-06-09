SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [SessionsRollsVW] as
Select	SessionRollId,CaseId,PrimaryJDReceiveDate,AppealJDReceiveDate,EnforcementJDReceiveDate,SupremeJDReceiveDate
from
	(
		Select	SessionRollId,CaseId,PrimaryJDReceiveDate,AppealJDReceiveDate,EnforcementJDReceiveDate,SupremeJDReceiveDate, rank() over (partition by CaseId order by SessionRollId desc) as rnk 
		from SessionsRolls 
		where (SELECT Max(v) FROM (VALUES (PrimaryJDReceiveDate), (AppealJDReceiveDate), (EnforcementJDReceiveDate),(SupremeJDReceiveDate)) AS value(v)) is not null
	) dat
where rnk = 1
GO
