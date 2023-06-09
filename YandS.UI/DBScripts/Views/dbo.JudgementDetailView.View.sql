SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[JudgementDetailView] as 
Select *
from
(
select CaseId, '3' as CaseLevelCode, PrimaryJudgements as Judgement, PrimaryJudgementsDate as JudgementsDate, PrimaryJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, PrimaryIsFavorable as IsFavorable
FROM SessionsRolls 
where PrimaryJudgements is not null
and   DeletedOn is null
and   SessionFileStatus = 'OFS-17'
union all
select CaseId, '4' as CaseLevelCode, AppealJudgements as Judgement, AppealJudgementsDate as JudgementsDate, AppealJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, AppealIsFavorable as IsFavorable
FROM SessionsRolls 
where AppealJudgements is not null
and   DeletedOn is null
and   SessionFileStatus = 'OFS-17'
union all
select CaseId, '5' as CaseLevelCode, SupremeJudgements as Judgement, SupremeJudgementsDate as JudgementsDate, SupremeJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, 'Y' as IsFavorable
FROM SessionsRolls 
where SupremeJudgements is not null
and   DeletedOn is null
and   SessionFileStatus = 'OFS-17'
union all
select CaseId, '6' as CaseLevelCode, EnforcementJudgements as Judgement, EnforcementJudgementsDate as JudgementsDate, EnforcementJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, EnforcementIsFavorable as IsFavorable
FROM SessionsRolls 
where EnforcementJudgements is not null
and   DeletedOn is null
and   SessionFileStatus = 'OFS-17'
) DAT
GO
