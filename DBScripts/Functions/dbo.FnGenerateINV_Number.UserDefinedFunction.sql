SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FnGenerateINV_Number](
     @LocationCode nvarchar(5)
)
RETURNS nvarchar(5)
AS 
BEGIN

	DECLARE @INVNo nvarchar(5);

	SELECT @INVNo = right(replicate('0',5)+cast(COUNT(*) + 1 as varchar(5)),5)
	from CaseInvoices as inv
	left join USERS usr on inv.CreatedBy = usr.UserId
	left join HR_Employee_s emp on usr.UserName = emp.EmployeeNumber
	where emp.LocationCode = @LocationCode;

    RETURN  @INVNo;
END;
GO
