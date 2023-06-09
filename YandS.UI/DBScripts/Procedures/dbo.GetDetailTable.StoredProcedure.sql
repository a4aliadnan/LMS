SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetDetailTable]
@DataFor				VARCHAR(15),
@SessionRollId			INT,
@CaseId					INT = NULL
AS
BEGIN
  IF (@DataFor = 'CASEHIST')
  BEGIN
	SELECT Id,SessionRollId,CaseId,CurrentHearingDate,CourtDecision,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn from CourtDecisionHistory where SessionRollId = @SessionRollId Order by 1 desc
  END
  ELSE IF (@DataFor = 'CASEHISTTEXT')
  BEGIN
		DECLARE @DecisionHistory nvarchar(max)
		SET @DecisionHistory = NULL

		SELECT @DecisionHistory = COALESCE(@DecisionHistory + '','') + FORMAT (CurrentHearingDate, 'dd/MM/yyyy') + ' : ' + CourtDecision
		FROM CourtDecisionHistory
		where CaseId = @CaseId
		order by Id

		SELECT replace(REPLACE(@DecisionHistory, CHAR(13) , '<br />'),CHAR(10), '<br />')  as CaseHistory
  END
END
GO
