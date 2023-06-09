SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [FnGetInvoiceCaseLevel](
     @CaseId int,
	 @CourtId nvarchar(5)
)
RETURNS nvarchar(200)
AS 
BEGIN

	DECLARE @CaseLevelName nvarchar(200);

	Select @CaseLevelName = Mst_Desc
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
			where  cmas.Mst_Value = @CourtId
			and cmas.MstParentId = 15
		) as Data;


	RETURN @CaseLevelName;
    
END;


GO
