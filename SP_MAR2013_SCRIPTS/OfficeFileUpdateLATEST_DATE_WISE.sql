DECLARE @CaseId INT, 
		@FileStatus VARCHAR(10), 
		@FileStatusRemarks VARCHAR(10),
		@CaseLevelCode VARCHAR(10),
		@TableName VARCHAR(100),
		@OfficeFileStatus VARCHAR(10)

DECLARE @Prefix  VARCHAR(4) = 'OFS-'

DECLARE db_cursor CURSOR LOCAL FOR 	
Select	CaseId,FileStatus, FileStatusRemarks, CaseLevelCode, TableName
FROM 
(
	Select	CaseId,FileStatus, FileStatusRemarks, CaseLevelCode, UpdateDate, TableName
		   ,RANK() OVER (PARTITION BY CaseId ORDER BY UpdateDate DESC) AS RNK
	FROM
	(
		Select	CC.CaseId,ISNULL(CR.FileStatus,'0') as FileStatus,CR.FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'TBR' as TableName
		from CourtCases CC
		Inner Join CaseRegistrations CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		and CR.IsDeleted = 0
		and   ISNULL(CR.FileStatus,'0') != '0'
		union all
		Select	CC.CaseId,ISNULL(CR.EnforcementlevelId,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'ENF' as TableName
		from CourtCases CC
		Inner Join CourtCasesEnforcements CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		and   ISNULL(CR.EnforcementlevelId,'0') NOT IN( '0','9')
		and not exists 
			(Select * from CourtCasesEnforcements CCE 
				where CCE.CaseId = CR.CaseId 
				and   CCE.EnforcementlevelId = '4'
				and   CCE.AuctionProcess = '11'
			)
		union all
		Select	CC.CaseId,ISNULL(CR.CourtDepartment,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'SUP' as TableName
		from CourtCases CC
		Inner Join CourtCasesDetails CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		and   CC.CaseLevelCode = '5'
		and   ISNULL(CR.CourtDepartment,'0') != '0'
		union all
		Select	CC.CaseId,ISNULL(CR.SessionFileStatus,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'SR' as TableName
		from CourtCases CC
		Inner Join SessionsRolls CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		AND   CR.DeletedOn is null
		and   ISNULL(CR.SessionFileStatus,'0') in('OFS-16','OFS-17','OFS-18')
		union all
		Select	CC.CaseId,ISNULL(CR.AuctionProcess,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'AUCTION' as TableName
		from CourtCases CC
		Inner Join CourtCasesEnforcements CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		and   CR.EnforcementlevelId = '4'
		and   CR.AuctionProcess = '11' 
		union all
		Select	CC.CaseId,ISNULL(CR.CauseOfRecoveryId,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CR.UpdatedOn,CR.CreatedOn) as UpdateDate, 'CAUSE_RECOVERY' as TableName
		from CourtCases CC
		Inner Join CourtCasesEnforcements CR on CC.CaseId = CR.CaseId
		Where CC.CaseStatus = '1'
		and   CR.EnforcementlevelId = '9'
		union all
		Select	CC.CaseId,ISNULL(CC.ReasonCode,'0') as FileStatus, NULL as FileStatusRemarks,CC.CaseLevelCode, ISNULL(CC.UpdatedOn,CC.CreatedOn) as UpdateDate, 'CL' as TableName
		from CourtCases CC
		Where CC.CaseStatus = '2'
		and   CC.ReasonCode is not null
	) INNER_DAT
) DAT
WHERE RNK = 1
ORDER BY TableName, FileStatus

begin
	OPEN db_cursor
		FETCH NEXT FROM db_cursor INTO @CaseId, @FileStatus,@FileStatusRemarks,@CaseLevelCode,@TableName
		WHILE @@FETCH_STATUS = 0

	begin
	  IF @TableName = 'TBR'
		BEGIN
			IF @FileStatus = '0'	BEGIN SET @OfficeFileStatus = '0' END
			ELSE IF @FileStatus = '4'
				BEGIN
				  SET @OfficeFileStatus = '0'
				   IF		 @FileStatusRemarks = '1' BEGIN SET @OfficeFileStatus = @Prefix + '13' END
				   ELSE IF @FileStatusRemarks = '2' 
						BEGIN 
							IF		@CaseLevelCode = '4' BEGIN SET @OfficeFileStatus = @Prefix + '10' END
							ELSE IF @CaseLevelCode = '5' BEGIN SET @OfficeFileStatus = @Prefix + '11' END
							ELSE BEGIN SET @OfficeFileStatus = @Prefix + '4' END
						END
				   ELSE IF @FileStatusRemarks = '3' BEGIN SET @OfficeFileStatus = @Prefix + '15' END
				   ELSE IF @FileStatusRemarks = '5' BEGIN SET @OfficeFileStatus = @Prefix + '14' END
				END
			ELSE IF @FileStatus  = '10'	BEGIN	 SET @OfficeFileStatus = @Prefix + '4'	END
			ELSE	BEGIN SET @OfficeFileStatus = @Prefix + @FileStatus	END
		END
	  ELSE IF @TableName = 'SUP'
		BEGIN
			IF		@FileStatus  = '1'	BEGIN	 SET @OfficeFileStatus = @Prefix + '32'	END
			ELSE IF @FileStatus  = '2'	BEGIN	 SET @OfficeFileStatus = @Prefix + '16'	END
			ELSE IF @FileStatus  = '3'	BEGIN	 SET @OfficeFileStatus = @Prefix + '17'	END
			ELSE IF @FileStatus  = '4'	BEGIN	 SET @OfficeFileStatus = @Prefix + '57'	END
		END
	  ELSE IF @TableName = 'SR'
		BEGIN
			SET @OfficeFileStatus = @FileStatus
		END
	  ELSE IF @TableName = 'ENF'
		BEGIN
			IF @FileStatus  = '1'	BEGIN	 SET @OfficeFileStatus = @Prefix + '22'	END
			ELSE IF @FileStatus  = '2'	BEGIN	 SET @OfficeFileStatus = @Prefix + '23'	END
			ELSE IF @FileStatus  = '3'	BEGIN	 SET @OfficeFileStatus = @Prefix + '24'	END
			ELSE IF @FileStatus  = '4'	BEGIN	 SET @OfficeFileStatus = @Prefix + '25'	END
			ELSE IF @FileStatus  = '5'	BEGIN	 SET @OfficeFileStatus = @Prefix + '26'	END
			ELSE IF @FileStatus  = '6'	BEGIN	 SET @OfficeFileStatus = @Prefix + '27'	END
			ELSE IF @FileStatus  = '7'	BEGIN	 SET @OfficeFileStatus = @Prefix + '28'	END 
			ELSE IF @FileStatus  = '10'	BEGIN	 SET @OfficeFileStatus = @Prefix + '29'	END 
			ELSE IF @FileStatus  = '11'	BEGIN	 SET @OfficeFileStatus = @Prefix + '20'	END 
			ELSE IF @FileStatus  = '12'	BEGIN	 SET @OfficeFileStatus = @Prefix + '19'	END 
			ELSE IF @FileStatus  = '13'	BEGIN	 SET @OfficeFileStatus = @Prefix + '56'	END 
			ELSE IF @FileStatus  = '14'	BEGIN	 SET @OfficeFileStatus = @Prefix + '39'	END 
		END
	  ELSE IF @TableName = 'CAUSE_RECOVERY'
		BEGIN
			IF @FileStatus  in ('1','2') BEGIN	 SET @OfficeFileStatus = @Prefix + '30'	END
			ELSE IF @FileStatus  = '3'	 BEGIN	 SET @OfficeFileStatus = @Prefix + '29'	END
		END		
	  ELSE IF @TableName = 'AUCTION'
		BEGIN
			SET @OfficeFileStatus = @Prefix + '21'
		END	
	  ELSE IF @TableName = 'CL'
		BEGIN
			IF		@FileStatus  IN ('1','7')	BEGIN	 SET @OfficeFileStatus = @Prefix + '55'	END
			ELSE IF @FileStatus  = '2'	BEGIN	 SET @OfficeFileStatus = @Prefix + '51'	END
			ELSE IF @FileStatus  = '6'	BEGIN	 SET @OfficeFileStatus = @Prefix + '53'	END
		END
		--PRINT CONVERT(VARCHAR(10),@CaseId) + '|' + @TableName + '|' + @FileStatus + '|' + @OfficeFileStatus+ '|' + @CaseLevelCode 
		--PRINT @FileStatus
		--PRINT @OfficeFileStatus


		UPDATE CourtCases SET OfficeFileStatus = @OfficeFileStatus
		WHERE CaseId = @CaseId
		AND   OfficeFileStatus is null
		
		FETCH NEXT FROM db_cursor INTO @CaseId, @FileStatus,@FileStatusRemarks,@CaseLevelCode,@TableName
	end
end