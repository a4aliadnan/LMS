SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FnGeneratePV_Number](
     @VoucherNo int
)
RETURNS nvarchar(20)
AS 
BEGIN

	DECLARE @PVNo nvarchar(10);
	DECLARE @BranchCOde  nvarchar(3);
	DECLARE @PVYear int;
	DECLARE @VType nvarchar(1);
	DECLARE @RetResult nvarchar(25);

	SELECT @VType = VoucherType from payVoucher where Voucher_No = @VoucherNo;

	IF @VType = 1 
		BEGIN
			SELECT @PVNo = right(replicate('0',5)+cast(COUNT(*) + 1 as varchar(5)),5)
			from payVoucher as pv
			WHERE pv.VoucherStatus = '1'
			AND   pv.VoucherType = '1'
			AND   dateadd(hour , 11, pv.AuthorizeDate) >= '2021-06-01'
			and   YEAR(pv.Voucher_Date) = (select YEAR(p.Voucher_Date) from payVoucher as p where p.Voucher_No = @VoucherNo);

			SELECT @BranchCOde = LEFT(CC.OfficeFileNo,1), @PVYear = RIGHT(YEAR(pv.Voucher_Date),2)
			from payVoucher as pv
			inner join CourtCases CC on pv.CaseId = CC.CaseId
			where pv.Voucher_No = @VoucherNo;

			SET @RetResult = @BranchCOde + '-RF-' +  @PVNo +'-' + CONVERT(varchar(2), @PVYear)
		END
	ELSE IF @VType = 2
		BEGIN
			SELECT @PVNo = right(replicate('0',5)+cast(COUNT(*) + 1 as varchar(5)),5)
			from payVoucher as pv
			WHERE pv.VoucherStatus = '1'
			AND   pv.VoucherType = '2'
			AND   dateadd(hour , 11, pv.AuthorizeDate) >= '2021-06-01'
			and   YEAR(pv.Voucher_Date) = (select YEAR(p.Voucher_Date) from payVoucher as p where p.Voucher_No = @VoucherNo);

			SELECT @BranchCOde = RIGHT(emp.LocationCode,1), @PVYear = RIGHT(YEAR(pv.Voucher_Date),2)
			from payVoucher as pv
			left join USERS usr on pv.CreatedBy = usr.UserId
			left join HR_Employee_s emp on usr.UserName = emp.EmployeeNumber
			where pv.Voucher_No = @VoucherNo;

			SET @RetResult = @BranchCOde + '-NR-' +  @PVNo +'-' + CONVERT(varchar(2), @PVYear)
		END
	ELSE
		BEGIN
			SELECT @PVNo = right(replicate('0',5)+cast(COUNT(*) + 1 as varchar(5)),5)
			from payVoucher as pv
			WHERE pv.VoucherStatus = '1'
			AND   pv.VoucherType = '3'
			and   YEAR(pv.Voucher_Date) = (select YEAR(p.Voucher_Date) from payVoucher as p where p.Voucher_No = @VoucherNo);

			SELECT @BranchCOde = RIGHT(emp.LocationCode,1), @PVYear = RIGHT(YEAR(pv.Voucher_Date),2)
			from payVoucher as pv
			left join USERS usr on pv.CreatedBy = usr.UserId
			left join HR_Employee_s emp on usr.UserName = emp.EmployeeNumber
			where pv.Voucher_No = @VoucherNo;

			SET @RetResult = @BranchCOde + '-INTRA-' +  @PVNo +'-' + CONVERT(varchar(2), @PVYear)
		END



    RETURN  @RetResult;
END;
GO
