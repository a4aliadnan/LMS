SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CheckDuplicateCaseFile](
     @ValueToSearch nvarchar(500),
	 @TypeToSearch nvarchar(100)
)
AS  
BEGIN
DECLARE @body nvarchar(4000)

IF @TypeToSearch = 'AccountContractNo'
begin
	set @body = cast( (
	select td = OfficeFileNo + '</td><td>' + cast( Mst_Desc as varchar(500) ) + '</td><td>' + cast( Defendant as varchar(500) )
	from (
		  select OfficeFileNo  = CC.OfficeFileNo,
				 Mst_Desc = Client.Mst_Desc,
				 Defendant     = CC.Defendant
		  FROM CourtCases CC
		  join MASTER_S Client on Client.Mst_Value = CC.ClientCode and Client.MstParentId = 241
		  where CC.AccountContractNo is not null AND CC.AccountContractNo = @ValueToSearch
		  ) as d
	for xml path( 'tr' ), type ) as nvarchar(4000) )

	set @body = '<table cellpadding="2" cellspacing="2" border="1">'
			  + '<tr><th>Office File No</th><th>Client Name</th><th>Opponent</th></tr>'
			  + replace( replace( @body, '&lt;', '<' ), '&gt;', '>' )
			  + '</table>'
end
ELSE IF @TypeToSearch = 'ClientFileNo'
begin
	set @body = cast( (
	select td = OfficeFileNo + '</td><td>' + cast( Mst_Desc as varchar(500) ) + '</td><td>' + cast( Defendant as varchar(500) )
	from (
		  select OfficeFileNo  = CC.OfficeFileNo,
				 Mst_Desc = Client.Mst_Desc,
				 Defendant     = CC.Defendant
		  FROM CourtCases CC
		  join MASTER_S Client on Client.Mst_Value = CC.ClientCode and Client.MstParentId = 241
		  where CC.ClientFileNo is not null AND CC.ClientFileNo = @ValueToSearch
		  ) as d
	for xml path( 'tr' ), type ) as nvarchar(4000) )

	set @body = '<table cellpadding="2" cellspacing="2" border="1">'
			  + '<tr><th>Office File No</th><th>Client Name</th><th>Opponent</th></tr>'
			  + replace( replace( @body, '&lt;', '<' ), '&gt;', '>' )
			  + '</table>'
end
ELSE IF @TypeToSearch = 'Defendant'
begin
	set @body = cast( (
	select td = OfficeFileNo + '</td><td>' + cast( Mst_Desc as varchar(500) ) + '</td><td>' + cast( Defendant as varchar(500) )
	from (
		  select OfficeFileNo  = CC.OfficeFileNo,
				 Mst_Desc = Client.Mst_Desc,
				 Defendant     = CC.Defendant
		  FROM CourtCases CC
		  join MASTER_S Client on Client.Mst_Value = CC.ClientCode and Client.MstParentId = 241
		  where CC.Defendant is not null AND CC.Defendant = @ValueToSearch
		  ) as d
	for xml path( 'tr' ), type ) as nvarchar(4000) )

	set @body = '<table cellpadding="2" cellspacing="2" border="1">'
			  + '<tr><th>Office File No</th><th>Client Name</th><th>Opponent</th></tr>'
			  + replace( replace( @body, '&lt;', '<' ), '&gt;', '>' )
			  + '</table>'
end

SELECT	@body as HtmlResult
END
GO
