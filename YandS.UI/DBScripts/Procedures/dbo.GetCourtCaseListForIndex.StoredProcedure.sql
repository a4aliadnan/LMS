SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [GetCourtCaseListForIndex]
    @Location varchar(1)
AS
Select 
CC.CaseId
,CC.OfficeFileNo
--,CC.ClientCode
,ClientMas.Mst_Desc as ClientName
,CC.Defendant as DefClientName
--,CC.AgainstCode
,AgainstMas.Mst_Desc as AgainstName
,CC.ReceptionDate
--,CC.ReceiveLevelCode
--,RcvLvlMas.Mst_Desc as ReceiveLevelName
,CC.AccountContractNo
,CC.ClientFileNo
--,CC.ClaimAmount
--,CC.CaseTypeCode
,CaseTypeMas.Mst_Desc as CaseTypeName
,CC.CaseLevelCode
,CaseLevelMas.Mst_Desc as CaseLevelName
--,CC.LegalNoticeDate
--,CC.ODBLoanCode
--,ODBLoanMas.Mst_Desc as ODBLoanName
--,CC.ODBBankBranch
--,ODBBranchMas.Mst_Desc as ODBBankBranchName
--,CC.PoliceNo
--,CC.PoliceStation
--,PolStatMas.Mst_Desc as PoliceStationName
--,CC.PublicProsecutionNo
--,CC.PublicPlace
--,PubPlaceMas.Mst_Desc as PublicPlaceName
--,CC.PAPCNo
--,CC.PAPCPlace
--,PAPCPlaceMas.Mst_Desc as PAPCPlaceName
--,CC.LaborConflicNo
--,CC.LaborConflicPlace
--,LabConfMas.Mst_Desc as LaborConflicPlaceName
--,CC.YandSNotes
--,CC.CreatedBy
--,CC.CreatedOn
--,CrtBy.Firstname as CreatedByName
--,CC.UpdatedBy
--,CC.UpdatedOn
--,UpdBy.Firstname as UpdatedByName
--,CC.PrivateFee
,CC.CaseStatus
,CaseStatusMas.Mst_Desc as CaseStatusName
--,CC.SpecialInstructions
--,CC.ClientCaseType
--,ClientCaseTypeMas.Mst_Desc as ClientCaseTypeName
--,CC.IdRegistrationNo
--,CC.ParentCourtId
--,ParentCourtMas.Mst_Desc as ParentCourtName
from CourtCases as CC
join MASTER_S ClientMas on CC.ClientCode = ClientMas.Mst_Value and ClientMas.MstParentId = 241
join MASTER_S AgainstMas on CC.AgainstCode = AgainstMas.Mst_Value and AgainstMas.MstParentId = 12
--join MASTER_S RcvLvlMas on CC.ReceiveLevelCode = RcvLvlMas.Mst_Value and RcvLvlMas.MstParentId = 13
join MASTER_S CaseTypeMas on CC.CaseTypeCode = CaseTypeMas.Mst_Value and CaseTypeMas.MstParentId = 14
join MASTER_S CaseLevelMas on CC.CaseLevelCode = CaseLevelMas.Mst_Value and CaseLevelMas.MstParentId = 15
--join MASTER_S ODBLoanMas on CC.ODBLoanCode = ODBLoanMas.Mst_Value and ODBLoanMas.MstParentId = 16
--join MASTER_S ODBBranchMas on CC.ODBBankBranch = ODBBranchMas.Mst_Value and ODBBranchMas.MstParentId = 18
--join MASTER_S PolStatMas on CC.PoliceStation = PolStatMas.Mst_Value and PolStatMas.MstParentId = 4
--join MASTER_S PubPlaceMas on CC.PublicPlace = PubPlaceMas.Mst_Value and PubPlaceMas.MstParentId = 4
--join MASTER_S PAPCPlaceMas on CC.PAPCPlace = PAPCPlaceMas.Mst_Value and PAPCPlaceMas.MstParentId = 4
--join MASTER_S LabConfMas on CC.LaborConflicPlace = LabConfMas.Mst_Value and LabConfMas.MstParentId = 4
--join USERS CrtBy on CrtBy.UserId = CC.CreatedBy
--left join USERS UpdBy on UpdBy.UserId = CC.UpdatedBy
join MASTER_S CaseStatusMas on CC.CaseStatus = CaseStatusMas.Mst_Value and CaseStatusMas.MstParentId = 252
--join MASTER_S ClientCaseTypeMas on CC.ClientCaseType = ClientCaseTypeMas.Mst_Value and ClientCaseTypeMas.MstParentId = 285
--join MASTER_S ParentCourtMas on CC.ParentCourtId = ParentCourtMas.Mst_Value and ParentCourtMas.MstParentId = 298
--where CaseId = 12
where (LEFT(CC.OfficeFileNo,1) = @Location OR @Location = 'A')
ORDER BY 1
;

GO
