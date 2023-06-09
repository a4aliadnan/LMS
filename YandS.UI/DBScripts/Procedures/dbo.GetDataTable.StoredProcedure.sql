SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [GetDataTable]
@Pageno		INT=1,
@filter		NVARCHAR(500)='',
@pagesize	INT=20,  
@Sorting	NVARCHAR(500)='CC.OfficeFileNo',
@SortOrder	NVARCHAR(500)='desc',
@UserName	NVARCHAR(5)=null,
@DataFor	NVARCHAR(25)=null,
@Location	NVARCHAR(1)=null,
@FileStatus NVARCHAR(3)=null
AS
BEGIN
 DECLARE @RC int,
 @UserId int

 IF @DataFor = 'TBR-NewCase'
	BEGIN
	   EXECUTE @RC = [dbo].[REG_IN_PROG-NEWCASES] 
	   @Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-AppSubmission'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-APPSUBM]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-SupSubmission'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-SUPSUBM]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-PriDispute'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-PRIDISPUTE]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-AplDispute'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-APLDISPUTE]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-SupDispute'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-SUPDISPUTE]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-TRansfer'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-TRANSFER]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-LegalNotice'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-LEGALNOTICE]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-CompDocs'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-COMPDOCS]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-Translation'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-TRANSLATION]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-ClientApproval'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-CLIENTAPPROVAL]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-Scanned'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-SCANNED]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-OnlineSubm'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-ONLINESUBM]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	
	END
 ELSE  IF @DataFor = 'TBR-CourtRequest'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-COURTREQUEST]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-WithLawyer'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-WITHLAWYER]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-Payment'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-PAYMENT]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-Remarks'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-REMARKS]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
 ELSE  IF @DataFor = 'TBR-Urgent'
	BEGIN
		EXECUTE @RC = [dbo].[REG_IN_PROG-URGENT]
		@Pageno ,@filter ,@pagesize ,@Sorting ,@SortOrder ,@UserName ,@DataFor ,@Location ,@FileStatus
	END
END
GO
