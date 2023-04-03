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
