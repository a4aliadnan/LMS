SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--DROP PROCEDURE [dbo].[GetDefendentTransfer]
CREATE PROCEDURE [GetDefendentTransfer]
@CaseId		INT,
@CaseLevel	VARCHAR(3) 
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
  FROM [dbo].[DefendentTransfer] 
  where		[CaseId]		= @CaseId 
  --and		[CaseLevelCode] = @CaseLevel
  order by 1 desc

END
GO
