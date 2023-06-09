SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE view [v_PayTo] as
Select cast(0 as int) AS MstId, 214 AS MstParentId,	Mst_Desc,	Mst_Value,	DisplaySeq, cast(1 as bit) AS Active_Flag,	NULL AS Remarks, cast(4 as int) AS CreatedBy, GETDATE() AS CreatedOn,	cast(4 as int) AS UpdatedBy,	GETDATE() AS UpdatedOn
from
(
Select Mst_Desc,	Mst_Value,	DisplaySeq from MASTER_S where MstParentId = 214
UNION ALL
SELECT FullName, EmployeeNumber, CAST( 100 + ROW_NUMBER() OVER (ORDER BY employeeid) as int) AS DisplaySeq FROM HR_Employee_s
) as Data
--order by DisplaySeq
GO
