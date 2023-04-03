USE [DB_A63AFB_yands]
GO

/****** Object:  View [dbo].[JudgementDetailView]    Script Date: 03/04/2023 6:19:20 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[JudgementDetailView] as 
Select *
from
(
select SessionRollId, CaseId, '3' as CaseLevelCode, PrimaryJudgements as Judgement, PrimaryJudgementsDate as JudgementsDate, PrimaryJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, DeletedOn, JudgementLevel
FROM SessionsRolls 
where PrimaryJudgements is not null
union all
select SessionRollId, CaseId, '4' as CaseLevelCode, AppealJudgements as Judgement, AppealJudgementsDate as JudgementsDate, AppealJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, DeletedOn, JudgementLevel
FROM SessionsRolls 
where AppealJudgements is not null
union all
select SessionRollId, CaseId, '5' as CaseLevelCode, SupremeJudgements as Judgement, SupremeJudgementsDate as JudgementsDate, SupremeJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, DeletedOn, JudgementLevel
FROM SessionsRolls 
where SupremeJudgements is not null
union all
select SessionRollId, CaseId, '6' as CaseLevelCode, EnforcementJudgements as Judgement, EnforcementJudgementsDate as JudgementsDate, EnforcementJDReceiveDate as JudgementReceive,CountLocationId as CourtLocationid, DeletedOn, JudgementLevel
FROM SessionsRolls 
where EnforcementJudgements is not null
) DAT
GO