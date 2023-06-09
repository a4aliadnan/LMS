SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

--DROP PROCEDURE [dbo].[GetDefendentTransferById]
CREATE PROCEDURE [GetDefendentTransferById]
@DefendentTransferId		INT
AS
BEGIN
SET NOCOUNT ON;

SELECT [DefendentTransferId]
      ,[CaseId]
      ,[CaseLevelCode]
      ,[TransferDate]
      ,[Amount]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[UpdatedBy]
      ,[UpdatedOn]
	  ,[MoneyTrRequestDate]
	  ,[MoneyTrCompleteDate]
  FROM [dbo].[DefendentTransfer] 
  where		[DefendentTransferId]		= @DefendentTransferId 
  --and		[CaseLevelCode] = @CaseLevel
  order by 1 desc

END
GO
