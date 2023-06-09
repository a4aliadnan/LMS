SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create view [v_BankTransferAuth] as
Select cast(0 as int) AS MstId, 214 AS MstParentId,	Mst_Desc,	Mst_Value,	DisplaySeq, cast(1 as bit) AS Active_Flag,	NULL AS Remarks, cast(4 as int) AS CreatedBy, GETDATE() AS CreatedOn,	cast(4 as int) AS UpdatedBy,	GETDATE() AS UpdatedOn
from
(
Select 'Please Select' as Mst_Desc, '0' as Mst_Value, CAST(0 as int) AS DisplaySeq
UNION ALL
Select FullName as Mst_Desc, EmployeeNumber as Mst_Value,	CAST(ROW_NUMBER() OVER (ORDER BY employeeid) as int) AS DisplaySeq from HR_Employee_s where EmployeeNumber in ('2','3')
) as Data
--order by DisplaySeq
GO
