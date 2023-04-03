DECLARE @EnforcementId INT, @CurrentDisputeLevelandType VARCHAR(2)
DECLARE db_cursor CURSOR LOCAL FOR 	
select EnforcementId,RN as CurrentDisputeLevelandType
from
(
select EnforcementId,  CurrentDisputeLevelandType, RN , RANK() OVER (Partition by EnforcementId Order by RN desc) RNK
from 
(
select EnforcementId,  PrimaryObjectionNo as CurrentDisputeLevelandType, '1' as RN from CourtCasesEnforcements
union all
select EnforcementId,  ApealObjectionNo as CurrentDisputeLevelandType, '2' as RN from CourtCasesEnforcements
union all
select EnforcementId,  SupremeObjectionNo as CurrentDisputeLevelandType, '3' as RN from CourtCasesEnforcements
union all
select EnforcementId,  PrimaryPlaintNo as CurrentDisputeLevelandType, '4' as RN from CourtCasesEnforcements
union all
select EnforcementId,  ApealPlaintNo as CurrentDisputeLevelandType, '5' as RN from CourtCasesEnforcements
union all
select EnforcementId,  SupremePlaintNo as CurrentDisputeLevelandType, '6' as RN from CourtCasesEnforcements
) DAT
where CurrentDisputeLevelandType is not null
) DTS
where RNK = 1
begin
	OPEN db_cursor
		FETCH NEXT FROM db_cursor INTO @EnforcementId, @CurrentDisputeLevelandType
		WHILE @@FETCH_STATUS = 0

	begin
		UPDATE CourtCasesEnforcements SET CurrentDisputeLevelandType = @CurrentDisputeLevelandType
		WHERE EnforcementId = @EnforcementId
		
		FETCH NEXT FROM db_cursor INTO @EnforcementId, @CurrentDisputeLevelandType
	end
end