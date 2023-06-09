SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetHearingDetail]
  @DetailId int,
  @Courtid nvarchar(2)
 AS
SELECT Hearing.[FollowupId]
      ,Hearing.[DetailId]
      ,Hearing.[HearingDate]
      ,Hearing.[HearingRemarks]
      ,Hearing.[NextHearingDate]
      ,Hearing.[CreatedBy]
      ,Hearing.[CreatedOn]
      ,Hearing.[UpdatedBy]
      ,Hearing.[UpdatedOn]
  FROM [dbo].[CourtCasesFollowups] as Hearing
  join [dbo].[CourtCasesDetails] as CaseDetail on Hearing.DetailId = CaseDetail.DetailId
  join [dbo].[CourtCases] as Cases on Cases.CaseId = CaseDetail.CaseId
  where Hearing.DetailId     = @DetailId
  and   CaseDetail.[Courtid] =@Courtid;
GO
