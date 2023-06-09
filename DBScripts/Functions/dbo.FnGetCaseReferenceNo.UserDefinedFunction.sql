SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FnGetCaseReferenceNo](
     @CaseId int,
	 @CaseLevelNo nvarchar(5)
)
RETURNS nvarchar(200)
AS 
BEGIN

	DECLARE @CaseReferenceNo nvarchar(200);

	IF @CaseLevelNo = '6' 
		BEGIN 
			SELECT @CaseReferenceNo  = EnforcementNo FROM CourtCasesEnforcements WHERE CaseId = @CaseId   
		END 
	ELSE 
		BEGIN 
			SELECT @CaseReferenceNo  = CourtRefNo FROM CourtCasesDetails WHERE CaseId = @CaseId and CaseLevelCode = @CaseLevelNo
		END

	RETURN @CaseReferenceNo;
    
END;


GO
