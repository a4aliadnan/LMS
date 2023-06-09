SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetPVSummaryData]
 AS
Select VoucherType.Mst_Desc as VoucherTypeName,Emp.LocationCode,pv.VoucherStatus, sum(pv.Amount) as PvAmount
from PayVoucher pv 
inner join MASTER_S as VoucherType on VoucherType.MstParentId = 224 and VoucherType.Mst_Value = pv.VoucherType
inner join USERS as U on pv.CreatedBy = U.UserId
inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
where pv."Status" != '-1'
group by VoucherType.Mst_Desc ,Emp.LocationCode,pv.VoucherStatus union all
Select 'PDC' as VoucherTypeName, Emp.LocationCode, '0' as VoucherStatus, sum(pv.Amount) as PvAmount 
from PayVoucher pv
inner join USERS as U on pv.CreatedBy = U.UserId
inner join HR_Employee_s Emp on Emp.EmployeeNumber = U.UserName
where pv.FutureChequeDate is not null AND ( ( pv."Status" = '0' AND pv.VoucherStatus = '0') OR ( pv."Status" = '0' AND pv.VoucherStatus = '1') )
group by Emp.LocationCode
order by 1,2

GO
