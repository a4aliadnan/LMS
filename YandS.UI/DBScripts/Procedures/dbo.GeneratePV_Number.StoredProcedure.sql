SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GeneratePV_Number]
    @VoucherNo int,
	@ResultPV nvarchar(20) OUTPUT
AS

SET NOCOUNT ON;  

DECLARE @PVNo nvarchar(10);
DECLARE @BranchCOde  nvarchar(3);
DECLARE @PVYear int;

SELECT @PVNo = right(replicate('0',5)+cast(COUNT(*) + 1 as varchar(15)),10)
from payVoucher as pv
WHERE pv.VoucherStatus = '1'
and   YEAR(pv.Voucher_Date) = (select YEAR(p.Voucher_Date) from payVoucher as p where p.Voucher_No = @VoucherNo);


SELECT @BranchCOde = emp.LocationCode, @PVYear = YEAR(pv.Voucher_Date)
from payVoucher as pv
left join USERS usr on pv.CreatedBy = usr.UserId
left join HR_Employee_s emp on usr.UserName = emp.EmployeeNumber
where pv.Voucher_No = @VoucherNo;

SET @ResultPV = @BranchCOde + '-' +  @PVNo +'-' + CONVERT(varchar(4), @PVYear);

RETURN
GO
