SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetPetrolKMDetailVM]
   @PaymentHead varchar(3),
   @PayTo varchar(3)
AS
SELECT Voucher_No,PV_No,FORMAT (Voucher_Date, 'dd/MM/yyyy') as Voucher_Date,Amount,Remarks,Payment_Head_Remarks,Payment_To,hr.FullName,Payment_Head,
		cast(Payment_Head_Remarks as int) - ISNULL(v.value, cast(Payment_Head_Remarks as int)) AS RunningKM
from PayVoucher as PV
    OUTER APPLY (
        SELECT TOP (1) cast(Payment_Head_Remarks as int) value
        from PayVoucher
		where Payment_To = @PayTo
		and   Payment_Head =  @PaymentHead
	    AND   Voucher_No < PV.Voucher_No
        ORDER by Voucher_No DESC
    ) v
join HR_Employee_s HR on PV.Payment_To = HR.EmployeeNumber
where Payment_To = @PayTo
and   Payment_Head =  @PaymentHead

GO
