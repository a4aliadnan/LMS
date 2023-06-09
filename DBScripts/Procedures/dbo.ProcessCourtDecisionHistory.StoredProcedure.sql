SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ProcessCourtDecisionHistory]
@UserId					INT,
@DataFor				VARCHAR(15),
@Id						INT,
@SessionRollId			INT,
@CaseId					INT,
@CurrentHearingDate		DATETIME2,
@CourtDecision			NVARCHAR(max),
@TransportationSource   VARCHAR(3)

AS
DECLARE 

@MCTDate		DATETIME2 = dateadd(hour , 11,GETDATE()),
@ProcessFlag	VARCHAR(1) = 'Y',
@ProcessMessage	VARCHAR(100),
@lcChk			INT = 0

BEGIN
  BEGIN TRY
	IF (@DataFor = 'CREATE')
		BEGIN
			SELECT @lcChk = COUNT(*) FROM CourtDecisionHistory WHERE CurrentHearingDate = @CurrentHearingDate AND CourtDecision = @CourtDecision
			IF @lcChk = 0
				BEGIN
					INSERT INTO CourtDecisionHistory (SessionRollId,CaseId,CurrentHearingDate,CourtDecision,CreatedBy,CreatedOn,TransportationSource) VALUES (@SessionRollId,@CaseId,@CurrentHearingDate,@CourtDecision,@UserId,@MCTDate,@TransportationSource)
				END

				SET @ProcessMessage = 'DATA INSERT SECCESSFULLY'
		END
	ELSE IF (@DataFor = 'UPDATE')
		BEGIN
			UPDATE CourtDecisionHistory SET CourtDecision = @CourtDecision, CurrentHearingDate = @CurrentHearingDate, UpdatedBy = @UserId, UpdatedOn = @MCTDate, TransportationSource = @TransportationSource WHERE Id = @Id
			SET @ProcessMessage = 'DATA UPDATED SECCESSFULLY'
		END
		
	ELSE IF (@DataFor = 'DELETE')
		BEGIN
			DELETE FROM CourtDecisionHistory WHERE Id = @Id		
			SET @ProcessMessage = 'DATA DELETED SECCESSFULLY'
		END	
	
	SELECT  @ProcessFlag as ProcessFlag , @ProcessMessage as ProcessMessage
  END TRY
  BEGIN CATCH
	SET @ProcessFlag = 'N'
	SET @ProcessMessage = ERROR_MESSAGE()
	SELECT  @ProcessFlag as ProcessFlag , @ProcessMessage as ProcessMessage
  END CATCH
END
GO
