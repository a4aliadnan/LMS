SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [FnGetLocationName](
     @LocationCode nvarchar(5)
)
RETURNS nvarchar(200)
AS 
BEGIN
	DECLARE @LocationName nvarchar(200);

	SELECT  @LocationName = Mst_Desc
			from MASTER_S as cmas
			where  cmas.MstParentId = 4
			and    cmas.Mst_Value = @LocationCode;


	RETURN @LocationName;
    
END;

GO
