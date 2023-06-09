SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [GetDashBoardData]
 AS
Select CaseLevelMas.Mst_Value as CaseLevelCode, CaseLevelMas.Mst_Desc as CaseLevelName
	, RIGHT(BranchMas.Mst_Value,1) as Mst_Value, BranchMas.Mst_Desc as Mst_Desc
	,(Select count(*) from CourtCases CC where LEFT(CC.OfficeFileNo,1) = RIGHT(BranchMas.Mst_Value,1) and CC.CaseLevelCode = CaseLevelMas.Mst_Value) as DataValue      
from MASTER_S as CaseLevelMas , MASTER_S as BranchMas 
where CaseLevelMas.MstParentId = 15  and CAST(CaseLevelMas.Mst_Value AS int) IN (1,3,4,5,6)
and   BranchMas.MstParentId = 236 and RIGHT(BranchMas.Mst_Value,1) in ('S','M');
GO
