SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [REG_IN_PROG-URGENT]
@Pageno				INT=1,
@filter				NVARCHAR(500)='',
@pagesize			INT=20,  
@Sorting			NVARCHAR(500)='CC.OfficeFileNo',
@SortOrder			NVARCHAR(500)='desc',
@UserName			NVARCHAR(5)=null,
@DataFor			NVARCHAR(25)=null,
@Location			NVARCHAR(1)=null,
@FileStatus			NVARCHAR(3)=null
AS
DECLARE
@FilterText			NVARCHAR(MAX) = '',
@FinalQuery			NVARCHAR(MAX),
@FinalSummaryQuery	NVARCHAR(MAX),
@FetchLimit			NVARCHAR(MAX),
@TableColumn		NVARCHAR(MAX)  = ' TableSort,PVName,CaseId,CaseRegistrationId,DaysCounterDisplay,InvestmentTypeName,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired,CaseLevelName,DaysCounter ',
@TableQuery			NVARCHAR(MAX),
@AdditionalWhere	NVARCHAR(MAX) = ' where 1=1',
@QueryFirstPart		NVARCHAR(MAX),
@QueryInnerPart		NVARCHAR(MAX) = '
							Select	1 as TableSort,''NEW CASES'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Receipt file: 30 =>>> ''  + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CC.ReceptionDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''1''
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CC.ReceptionDate),dateadd(hour , 11,GETDATE())) > 30
							union all
							Select	2 as TableSort,''App Submission INV_Y'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Primary JUG.: 10 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,PJV.JudgementsDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							Inner join PrimaryJudgementDateVW PJV on PJV.CaseId = CR.CaseId
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''2''
							AND   CR.DepartmentType = ''2'' -- INVESTMENT = YES
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''4'',''6'')
							AND	  DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) > 10
							union all
							Select	3 as TableSort,''App Submission INV_N'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Primary JUG.: 20 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,PJV.JudgementsDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							Inner join PrimaryJudgementDateVW PJV on PJV.CaseId = CR.CaseId
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''2''
							AND   CR.DepartmentType = ''1'' -- INVESTMENT = NO
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''4'',''6'')
							AND	  DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) > 20
							union all
							Select	4 as TableSort,''Sup Submission'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Appeal JUG.: 30 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,PJV.JudgementsDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							Inner join AppealJudgementDateVW PJV on PJV.CaseId = CR.CaseId
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''3''
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CC.CaseLevelCode not in (''5'',''6'')
							AND	  DATEDIFF(DAY, dateadd(hour , 11, PJV.JudgementsDate),dateadd(hour , 11,GETDATE())) > 30
							union all
							Select	5 as TableSort,''Pri Dispute'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Primary Disp.: 15 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CR.DisputeLevel = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) > 15
							union all
							Select	6 as TableSort,''Appeal Dispute'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Appeal Disp.: 4 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CR.DisputeLevel = ''2''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) > 3
							union all
							Select	7 as TableSort,''Supreme Dispute'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''Supreme Disp.: 30 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CR.ActionLevel = ''4''
							AND   CC.OfficeFileStatus != ''OFS-2''
							AND   CC.CaseStatus = ''1''
							AND   CR.DisputeLevel = ''3''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.DisputrRegisterDate),dateadd(hour , 11,GETDATE())) > 30
							union all
							Select	8 as TableSort,''COURT MSG'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''COURT MSG: 1 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CC.OfficeFileStatus = ''OFS-7''
							AND   CC.CaseStatus = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) >= 1
							union all
							Select	9 as TableSort,''WITH LAWYER'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''WITH LAWYER: 6 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CC.OfficeFileStatus = ''OFS-12''
							AND   CC.CaseStatus = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) >= 6
							union all
							Select	10 as TableSort,''TRANSLATION'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''TRANSLATION: 5 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CC.OfficeFileStatus = ''OFS-15''
							AND   CC.CaseStatus = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) >= 5
							union all
							Select	11 as TableSort,''SUBMISSION APPROVAL'' as PVName,CC.CaseId,CR.CaseRegistrationId
							,''SUBMISSION APPROVAL: 9 =>>> '' + convert(varchar(5),DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE()))) as DaysCounterDisplay
							,CaseLevelMas.Mst_Desc as CaseLevelName
							,case when convert(int,CR.DepartmentType) > 0 then DepType.Mst_Desc end as InvestmentTypeName
							,CC.OfficeFileNo
							,ClientMas.Mst_Desc as ClientName,CC.Defendant as Defendant
							,case when convert(int,CC.AgainstCode) > 0 then REPLACE(LTRIM(RTRIM(AgainstMas.Mst_Desc)), ''AGAINST '', '''') end as AgainstName
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as ActionCounter
							,case when CC.OfficeFileStatus = ''0'' then null else FileStatus.Mst_Desc end as FileStatusName
							,CR.FormPrintWorkRequired
							,CR.OfficeProcedure as Notes
							,DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) as DaysCounter
							,CR.Remarks,CR.DisputrRegisterDate AS sortDate
							,CR.DepartmentType as InvestmentType
							from CaseRegistrations CR
							Inner Join CourtCases as CC on cc.CaseId = CR.CaseId
							Inner join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
							Inner join MASTER_S FileStatus on CC.OfficeFileStatus = FileStatus.Mst_Value and FileStatus.MstParentId = 1573
							left  join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
							left  join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
							left join MASTER_S DepType on CR.DepartmentType = DepType.Mst_Value and DepType.MstParentId = 822
							where CR.IsDeleted = 0
							AND   CC.OfficeFileStatus in (''OFS-4'',''OFS-10'',''OFS-11'')
							AND   CC.CaseStatus = ''1''
							AND	  DATEDIFF(DAY, dateadd(hour , 11, CR.ActionDate),dateadd(hour , 11,GETDATE())) >= 9
',
@SelectColumns		NVARCHAR(MAX),
@SummaryQuery		NVARCHAR(MAX),
@JoinTables			NVARCHAR(MAX) = ' ',
@Where				NVARCHAR(MAX) = '  where (LEFT(OfficeFileNo,1) = '''+@Location+''' OR '''+@Location+''' = ''A'')
									'

BEGIN

	SET @SelectColumns = '
							,sortDate 
			FROM (
			SELECT	TableSort,PVName,CaseId,CaseRegistrationId,DaysCounterDisplay,CaseLevelName,InvestmentTypeName,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired
					,DaysCounter,Remarks,sortDate
					FROM
					(
					'+@QueryInnerPart+' 
					) DAT
	'
	SET @SummaryQuery = '
							SELECT
									COUNT(*) as recordsTotal,
									count(case when left(OfficeFileNo,1) = ''M'' then 1 end) MCTRecords,
									count(case when left(OfficeFileNo,1) = ''S'' then 1 end) SLLRecords
									from (
			SELECT	TableSort,PVName,CaseId,CaseRegistrationId,DaysCounterDisplay,CaseLevelName,InvestmentTypeName,OfficeFileNo,ClientName,Defendant,AgainstName,ActionCounter,FileStatusName,Notes,FormPrintWorkRequired
					,DaysCounter,Remarks,sortDate
					FROM
					(
					'+@QueryInnerPart+'
					) DAT
									     ) MASTDAT
	'
	SET @QueryFirstPart = '
SELECT '+@TableColumn+' 
 FROM 
  ( 
   SELECT ROW_NUMBER() OVER (order by TableSort,DaysCounter desc,sortDate desc) as RowNum, * 
    from 
	  ( 
	   SELECT '
	SET @TableQuery  = @SelectColumns+@JoinTables+@Where

	IF(@filter !='')
	BEGIN
		SET @FilterText = '
						AND (
							OfficeFileNo like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR ClientName like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR Defendant like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR FileStatusName like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR Notes like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR FormPrintWorkRequired like N''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR AgainstName like ''%'+CONVERT(NVARCHAR,@filter)+'%'' 
							OR CaseLevelName like ''%'+CONVERT(NVARCHAR,@filter)+'%''
							)
							'
	END


	IF(@pagesize > 0)
	BEGIN
		SET @pagesize = @pagesize + @Pageno
		SET @Pageno = @Pageno + 1
		SET @FetchLimit = 'WHERE RowNum >= '+CONVERT(varchar,@Pageno)+' AND RowNum <= '+CONVERT(varchar,@pagesize)
	END
	ELSE
	BEGIN	
		SET @FetchLimit = ' ' 
	END

BEGIN	
	SET @FinalQuery = @QueryFirstPart + @TableColumn + @TableQuery + @FilterText + 
			' ) AS DAT ' + 
			  @AdditionalWhere + 
	  ' ) AS RowConstrainedResult 
	) AS FINAL ' + @FetchLimit+' ORDER BY TableSort,DaysCounter desc,RowNum '
	
	SET @FinalSummaryQuery = @SummaryQuery+@JoinTables+@Where+@FilterText
END

print @FinalQuery
exec (@FinalQuery)
print @FinalSummaryQuery
exec (@FinalSummaryQuery)

END
GO
