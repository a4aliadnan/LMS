SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE view [CaseRegistrationsVW] as
Select	CaseId,ActionDate,FileStatus,FileStatusName
from
	(
		Select	CaseId,ActionDate,FileStatus,FStatus.Mst_Desc as FileStatusName, rank() over (partition by CaseId order by CaseRegistrationId desc) as rnk 
		from CaseRegistrations CR
		join MASTER_S as FStatus on FStatus.MstParentId = 788 and FStatus.Mst_Value = CR.FileStatus
	) dat
where rnk = 1
GO
