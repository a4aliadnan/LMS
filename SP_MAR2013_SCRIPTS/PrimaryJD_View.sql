SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE View [dbo].[PrimaryJudgementDateVW] as 
Select *
from
(
select CaseId, JudgementsDate
	from
	(
		select distinct CaseId, JudgementsDate, RANK() over (partition by CaseId order by JudgementsDate desc) as rnk
		from JudgementDetailView
		Where CaseLevelCode = '3'
		and   JudgementsDate is not null
	) ddt
	where rnk = 1
) DAT



