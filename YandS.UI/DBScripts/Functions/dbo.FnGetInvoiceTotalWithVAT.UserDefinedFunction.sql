SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [FnGetInvoiceTotalWithVAT](
      @InvoiceId int
)
RETURNS decimal(18,3)
AS 
BEGIN

	DECLARE @InvoiceAmuont decimal(18,3) = 0,
			@FeeAmount decimal(18,3) = 0,
			@VATPct  decimal(18,3) = 0;

	DECLARE db_cursor CURSOR FOR 
	SELECT FeeAmount,VATPct
	FROM [dbo].[CaseInvoiceFees]
	WHERE [InvoiceId] = @InvoiceId 

	OPEN db_cursor
	FETCH NEXT FROM db_cursor INTO @FeeAmount,  @VATPct
	WHILE @@FETCH_STATUS = 0  
 
	BEGIN  
   		IF @VATPct IS NOT NULL OR @VATPct > 0
			BEGIN
			SET @InvoiceAmuont = @InvoiceAmuont + ( @FeeAmount + (@FeeAmount * (@VATPct/100)) )
			END
		ELSE
			BEGIN
			SET @InvoiceAmuont = @InvoiceAmuont + @FeeAmount
			END
 		FETCH NEXT FROM db_cursor INTO @FeeAmount,  @VATPct
	END 
	RETURN @InvoiceAmuont;
    
END
GO
