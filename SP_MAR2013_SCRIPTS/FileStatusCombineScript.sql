UPDATE SessionsRolls SET SessionFileStatus = 'OFS-16' where SessionFileStatus = '1'
UPDATE SessionsRolls SET SessionFileStatus = 'OFS-17' where SessionFileStatus = '2'
UPDATE SessionsRolls SET SessionFileStatus = 'OFS-18' where SessionFileStatus = '3'

update SessionsRolls set SessionFileStatus = 	'OFS-19'	where CaseId =	2437	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-21'	where CaseId =	2582	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	2631	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-21'	where CaseId =	3116	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-21'	where CaseId =	3190	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	4910	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	5187	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	6965	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	458	and DeletedOn is null
update SessionsRolls set SessionFileStatus = 	'OFS-20'	where CaseId =	723	and DeletedOn is null

--select * from SessionsRolls where SessionFileStatus = '4' and DeletedOn is null
--SELECT * FROM CourtCases WHERE CaseId IN (4327,4519,2572,6849)
--select * from SessionsRolls where CaseId IN (4327,4519) --AND SessionFileStatus = '4' and DeletedOn is null
-- select distinct AuctionProcess from CourtCasesEnforcements Order by 1
--select distinct FileStatus from CaseRegistrations Order by 1
--select * from CaseRegistrations where FileStatus = '4'

UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-22' where EnforcementlevelId = '1'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-23' where EnforcementlevelId = '2'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-24' where EnforcementlevelId = '3'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-25' where EnforcementlevelId = '4' AND AuctionProcess IN ('1','2','3','4','5','6','7','9','12','13','14','16','17','18','19','20')
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-26' where EnforcementlevelId = '5'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-27' where EnforcementlevelId = '6'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-28' where EnforcementlevelId = '7'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-29' where EnforcementlevelId = '10'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-20' where EnforcementlevelId = '11'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-19' where EnforcementlevelId = '12'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-56' where EnforcementlevelId = '13'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-39' where EnforcementlevelId = '14'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-30' where EnforcementlevelId = '9' AND CauseOfRecoveryId IN ('1','2')
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-29' where EnforcementlevelId = '9' AND CauseOfRecoveryId = '3'
UPDATE CourtCasesEnforcements SET EnforcementlevelId = 'OFS-21' where EnforcementlevelId = '4' AND AuctionProcess = '11'

UPDATE CourtCasesDetails SET CourtDepartment = 'OFS-32' Where CourtDepartment = '1'
UPDATE CourtCasesDetails SET CourtDepartment = 'OFS-16' Where CourtDepartment = '2'
UPDATE CourtCasesDetails SET CourtDepartment = 'OFS-17' Where CourtDepartment = '3'
UPDATE CourtCasesDetails SET CourtDepartment = 'OFS-57' Where CourtDepartment = '4'

UPDATE CaseRegistrations SET FileStatus = 'OFS-' + FileStatus Where FileStatus IN ('1','3','5','6','7','8','9','12')
UPDATE CaseRegistrations SET FileStatus = 'OFS-13' Where FileStatus = '4' AND FileStatusRemarks = '1'
UPDATE CaseRegistrations SET FileStatus = 'OFS-10'
from CourtCases 
join CaseRegistrations on CaseRegistrations.CaseId = CourtCases.CaseId
Where CourtCases.CaseLevelCode = '4'
AND   CaseRegistrations.FileStatus = '4' AND   CaseRegistrations.FileStatusRemarks = '2'
UPDATE CaseRegistrations SET FileStatus = 'OFS-11'
from CourtCases 
join CaseRegistrations on CaseRegistrations.CaseId = CourtCases.CaseId
Where CourtCases.CaseLevelCode = '5'
AND   CaseRegistrations.FileStatus = '4' AND   CaseRegistrations.FileStatusRemarks = '2'

UPDATE CaseRegistrations SET FileStatus = 'OFS-15' Where FileStatus = '4' AND FileStatusRemarks = '3'
UPDATE CaseRegistrations SET FileStatus = 'OFS-14' Where FileStatus = '4' AND FileStatusRemarks = '5'

UPDATE CaseRegistrations SET FileStatus = 'OFS-4' Where FileStatus = '4'

UPDATE CourtCases SET ReasonCode = 'OFS-55' Where ReasonCode IN ('1','7')
UPDATE CourtCases SET ReasonCode = 'OFS-51' Where ReasonCode  = '2'
UPDATE CourtCases SET ReasonCode = 'OFS-53' Where ReasonCode  = '6'