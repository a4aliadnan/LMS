SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetCaseLevelForInvoice]
  @CaseId int
 AS
Select MstId,	MstParentId,	Mst_Desc,	Mst_Value,	DisplaySeq,	Active_Flag,	Remarks,	CreatedBy,	CreatedOn,	UpdatedBy,	UpdatedOn
from
(
select	cmas.MstId,
		cmas.MstParentId,
		case when cd.CourtLocationid is not null then cmas.Mst_Desc + ' - ' + dbo.FnGetLocationName(cd.CourtLocationid) else cmas.Mst_Desc end as Mst_Desc,
		cmas.Mst_Value,
		cmas.DisplaySeq,
		cmas.Active_Flag,
		cmas.Remarks,
		cmas.CreatedBy,
		cmas.CreatedOn,
		cmas.UpdatedBy,
		cmas.UpdatedOn,
		cd.caseId,
		cd.Courtid as Courtid,
		cd.DetailId as DetailId,
		cd.CourtLocationid
	from MASTER_S as cmas
	left join CourtCasesDetails cd on cmas.Mst_Value = cd.CaseLevelCode and cd.CaseId = @CaseId
	where  CAST(cmas.Mst_Value AS int) between 1 and 6
	and cmas.MstParentId = 15
) as Data;
GO
