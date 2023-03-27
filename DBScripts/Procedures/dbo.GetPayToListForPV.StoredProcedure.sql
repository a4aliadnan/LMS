SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetPayToListForPV]
 AS
Select MstId, MstParentId,	Mst_Desc,	Mst_Value,	DisplaySeq, Active_Flag,Remarks, CreatedBy, CreatedOn,UpdatedBy, UpdatedOn
from v_PayTo
order by DisplaySeq
GO
