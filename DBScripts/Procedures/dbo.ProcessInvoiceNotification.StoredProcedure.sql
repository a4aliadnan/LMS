SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [ProcessInvoiceNotification](
     @UniquiId [binary](16),
	 @RetResult nvarchar(500) OUTPUT
)
AS 
SET NOCOUNT ON;
DECLARE
	@LC_COUNT INT = 0

BEGIN TRY
	
	SELECT @LC_COUNT = 1 FROM InvoiceNotification WHERE [UID] = @UniquiId
	IF @LC_COUNT = 0
	  BEGIN
		BEGIN TRANSACTION
    		INSERT INTO [dbo].[InvoiceNotification] ([UID],[UpdateDate]) values (@UniquiId,GETDATE())
		COMMIT TRANSACTION
		SET @RetResult = 'SUCCESS'
	  END	
	ELSE
		SET @RetResult = 'SUCCESS'
  END TRY
  BEGIN CATCH
	SELECT @RetResult = ERROR_MESSAGE()
-- Transaction uncommittable
    IF (XACT_STATE()) = -1
      ROLLBACK TRANSACTION
 
-- Transaction committable
    IF (XACT_STATE()) = 1
      COMMIT TRANSACTION
  END CATCH
GO
