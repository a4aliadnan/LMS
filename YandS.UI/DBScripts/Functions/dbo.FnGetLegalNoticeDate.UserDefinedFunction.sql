SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FnGetLegalNoticeDate](
     @CaseId int
)
RETURNS nvarchar(20)
AS 
BEGIN

	DECLARE @LegalNoticeDate nvarchar(20);

	select TOP 1 @LegalNoticeDate = FORMAT(SendDate, 'dd/MM/yyyy') from CaseRegistrations where CaseId = @CaseId order by CaseRegistrationId desc

	RETURN @LegalNoticeDate;
    
END;
GO
