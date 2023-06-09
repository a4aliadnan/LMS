SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [FnGenerateCase_Number](
     @BranchCode nvarchar(5)
)
RETURNS nvarchar(20)
AS 
BEGIN

	DECLARE @CaseNo nvarchar(20);
	DECLARE @Exists int;

	select @Exists = count(*) from CourtCases where  Left(OfficeFileNo,1) = RIGHT(@BranchCode, 1);

	if @Exists > 0 
		SELECT  @CaseNo = Left(OfficeFileNo,1)  + cast(MAX(cast(SUBSTRING(OfficeFileNo,2,6) as int) +1) as varchar(15))
		from CourtCases
		where  Left(OfficeFileNo,1) = RIGHT(@BranchCode, 1)
		group by Left(OfficeFileNo,1);
	else
		set @CaseNo = RIGHT(@BranchCode, 1)  +  cast(1 as varchar(15));


    RETURN @CaseNo;
END;
GO
