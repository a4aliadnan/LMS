USE [DB_A63AFB_yands]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ProcessDefendentTransfer]
@UserId					INT,
@DataFor				VARCHAR(15),
@DefendentTransferId	INT,
@CaseId					INT,
@CaseLevelCode			VARCHAR(3),
@TransferDate			DATETIME2,
@Amount					DECIMAL(13,3),
@MoneyTrRequestDate		DATETIME2,
@MoneyTrCompleteDate	DATETIME2

AS
DECLARE 

@MCTDate		DATETIME2 = dateadd(hour , 11,GETDATE()),
@ProcessFlag	VARCHAR(1) = 'Y',
@ProcessMessage	VARCHAR(100),
@LDefTransferId	INT

BEGIN
	IF(YEAR(@TransferDate) = '1900')
	begin
	 SET @TransferDate = null
	end

	IF(YEAR(@MoneyTrRequestDate) = '1900')
	begin
	 SET @MoneyTrRequestDate = null
	end

	IF(YEAR(@MoneyTrCompleteDate) = '1900')
	begin
	 SET @MoneyTrCompleteDate = null
	end
  BEGIN TRY
	SET @LDefTransferId = @DefendentTransferId
	IF (@DataFor = 'CREATE')
		BEGIN
			INSERT INTO DefendentTransfer (CaseId,CaseLevelCode,TransferDate,Amount,CreatedBy,CreatedOn,MoneyTrRequestDate,MoneyTrCompleteDate) VALUES (@CaseId,@CaseLevelCode,@TransferDate,@Amount,@UserId,@MCTDate,@MoneyTrRequestDate,@MoneyTrCompleteDate)
			SET @ProcessMessage = 'DATA INSERT SECCESSFULLY'
			SET @LDefTransferId = @@IDENTITY
		END
	ELSE IF (@DataFor = 'UPDATE')
		BEGIN
			UPDATE DefendentTransfer 
				SET TransferDate = @TransferDate, 
					Amount = @Amount, 
					UpdatedBy = @UserId, 
					UpdatedOn = @MCTDate,
					MoneyTrRequestDate = @MoneyTrRequestDate, 
					MoneyTrCompleteDate = @MoneyTrCompleteDate 
					WHERE DefendentTransferId = @DefendentTransferId
			SET @ProcessMessage = 'DATA UPDATED SECCESSFULLY'
		END
		
	ELSE IF (@DataFor = 'DELETE')
		BEGIN
			DELETE FROM DefendentTransfer WHERE DefendentTransferId = @DefendentTransferId		
			SET @ProcessMessage = 'DATA DELETED SECCESSFULLY'
		END	
	ELSE IF (@DataFor = 'DELETE_ALL')
		BEGIN
			DELETE FROM DefendentTransfer WHERE CaseId = @CaseId AND CaseLevelCode = @CaseLevelCode
			UPDATE CourtCasesEnforcements set MoneyTrRequestDate = null where CaseId = @CaseId
			SET @ProcessMessage = 'ALL DATA DELETED SECCESSFULLY'
		END	
	
	SELECT  @ProcessFlag as ProcessFlag , @ProcessMessage as ProcessMessage, @LDefTransferId as DefendentTransferId
  END TRY
  BEGIN CATCH
	SET @ProcessFlag = 'N'
	SET @ProcessMessage = ERROR_MESSAGE()
	SELECT  @ProcessFlag as ProcessFlag , @ProcessMessage as ProcessMessage
  END CATCH
END