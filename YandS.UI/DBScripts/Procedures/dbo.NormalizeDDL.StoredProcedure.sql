SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [NormalizeDDL]
AS
DECLARE @lcCount int = 0

select @lcCount = count(*) 
from	CourtCases
where	ClientCode is null 
OR		AgainstCode is null
OR		ReceiveLevelCode is null
OR		CaseTypeCode is null
OR		CaseLevelCode is null
OR		ODBLoanCode is null
OR		ODBBankBranch is null
OR		PoliceStation is null
OR		PublicPlace is null
OR		PAPCPlace is null
OR		LaborConflicPlace is null
OR		CaseStatus is null
OR		ClientCaseType is null
OR		ParentCourtId is null
OR		ClientClassificationCode is null
OR		CaseSubject is null

print @lcCount

IF @lcCount > 0
	BEGIN
	 Update CourtCases set    ClientCode = '0' where  ClientCode is null
	 Update CourtCases set    AgainstCode = '0' where  AgainstCode is null
	 Update CourtCases set    ReceiveLevelCode = '0' where  ReceiveLevelCode is null
	 Update CourtCases set    CaseTypeCode = '0' where  CaseTypeCode is null
	 Update CourtCases set    CaseLevelCode = '0' where  CaseLevelCode is null
	 Update CourtCases set    ODBLoanCode = '0' where  ODBLoanCode is null
	 Update CourtCases set    ODBBankBranch = '0' where  ODBBankBranch is null
	 Update CourtCases set    PoliceStation = '0' where  PoliceStation is null
	 Update CourtCases set    PublicPlace = '0' where  PublicPlace is null
	 Update CourtCases set    PAPCPlace = '0' where  PAPCPlace is null
	 Update CourtCases set    LaborConflicPlace = '0' where  LaborConflicPlace is null
	 Update CourtCases set    CaseStatus = '0' where  CaseStatus is null
	 Update CourtCases set    ClientCaseType = '0' where  ClientCaseType is null
	 Update CourtCases set    ParentCourtId = '0' where  ParentCourtId is null
	 Update CourtCases set    ClientClassificationCode = '0' where  ClientClassificationCode is null
	 Update CourtCases set    CaseSubject = '0' where  CaseSubject is null

	END
ELSE
	BEGIN
		select @lcCount = count(*) from	CourtCases where	ClientCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 241)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ClientCode = '0' where  ClientCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 241)
			END

		select @lcCount = count(*) from	CourtCases where	AgainstCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 12)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    AgainstCode = '0' where  AgainstCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 12)
			END

		select @lcCount = count(*) from	CourtCases where	ReceiveLevelCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 13)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ReceiveLevelCode = '0' where  ReceiveLevelCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 13)
			END

		select @lcCount = count(*) from	CourtCases where	CaseTypeCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 14)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    CaseTypeCode = '0' where  CaseTypeCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 14)
			END

		select @lcCount = count(*) from	CourtCases where	CaseLevelCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 15)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    CaseLevelCode = '0' where  CaseLevelCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 15)
			END

		select @lcCount = count(*) from	CourtCases where	ODBLoanCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 16)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ODBLoanCode = '0' where  ODBLoanCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 16)
			END

		select @lcCount = count(*) from	CourtCases where	ODBBankBranch not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 18)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ODBBankBranch = '0' where  ODBBankBranch not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 18)
			END

		select @lcCount = count(*) from	CourtCases where	PoliceStation not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    PoliceStation = '0' where  PoliceStation not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
			END

		select @lcCount = count(*) from	CourtCases where	PublicPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    PublicPlace = '0' where  PublicPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
			END

		select @lcCount = count(*) from	CourtCases where	PAPCPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    PAPCPlace = '0' where  PAPCPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
			END

		select @lcCount = count(*) from	CourtCases where	LaborConflicPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    LaborConflicPlace = '0' where  LaborConflicPlace not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 4)
			END

		select @lcCount = count(*) from	CourtCases where	CaseStatus not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 252)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    CaseStatus = '0' where  CaseStatus not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 252)
			END
		
		select @lcCount = count(*) from	CourtCases where	ClientCaseType not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 285)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ClientCaseType = '0' where  ClientCaseType not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 285)
			END

		select @lcCount = count(*) from	CourtCases where	ParentCourtId not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 298)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ParentCourtId = '0' where  ParentCourtId not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 298)
			END

		select @lcCount = count(*) from	CourtCases where	ClientClassificationCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 523)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    ClientClassificationCode = '0' where  ClientClassificationCode not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 523)
			END

		select @lcCount = count(*) from	CourtCases where	CaseSubject not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 532)
		IF @lcCount > 0
			BEGIN
				Update CourtCases set    CaseSubject = '0' where  CaseSubject not in (select Mst_Value from [dbo].[MASTER_S] where [MstParentId] = 532)
			END
	END



GO
