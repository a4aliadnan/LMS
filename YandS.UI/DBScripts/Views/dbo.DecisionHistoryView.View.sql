SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE View [DecisionHistoryView] as 
Select *
from
(
select CaseId, string_agg(DecisionHistory, CHAR(13)) as DecisionHistory
from 
(
select distinct CaseId,FORMAT (CurrentHearingDate, 'dd/MM/yyyy') + ' : ' + CourtDecision as DecisionHistory from CourtDecisionHistory
) DHist
group by CaseId
) FDATA
GO
