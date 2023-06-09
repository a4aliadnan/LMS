SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE procedure [GetPVReportPivot]
    @Location varchar(3),
	@DateFrom date,
	@DateTo date
AS

DECLARE @WhereClause NVARCHAR (MAX)

SET @WhereClause = ' AND (LEFT(PV.PV_No,1) = ''' + CAST(@Location as VARCHAR) + ''' OR ''' + CAST(@Location as VARCHAR) + ''' = ''A'')  AND CAST(PV.Voucher_Date AS DATE) BETWEEN CAST(''' + CONVERT(NVARCHAR(24), @DateFrom, 101) + ''' AS DATE) AND CAST(''' + CONVERT(NVARCHAR(24), @DateTo, 101)  +''' AS DATE)'

DECLARE @columnHeaders NVARCHAR (MAX)

SELECT @columnHeaders  = COALESCE (@columnHeaders + ',[' + PHeadMas.Mst_Desc + ']', '[' + PHeadMas.Mst_Desc + ']')
FROM    MASTER_S as PHeadMas
	WHERE PHeadMas.MstParentId = 7
	AND   CAST(PHeadMas.Mst_Value AS int) > 0
GROUP BY PHeadMas.Mst_Desc
ORDER BY PHeadMas.Mst_Desc

print @columnHeaders

/* GRAND TOTAL COLUMN */
DECLARE @GrandTotalCol	NVARCHAR (MAX)
SELECT @GrandTotalCol = COALESCE (@GrandTotalCol + 'ISNULL([' + CAST (PHeadMas.Mst_Desc AS VARCHAR) +'],0) + ', 'ISNULL([' + CAST(PHeadMas.Mst_Desc AS VARCHAR)+ '],0) + ')
FROM 
    MASTER_S as PHeadMas
	WHERE PHeadMas.MstParentId = 7
	AND   CAST(PHeadMas.Mst_Value AS int) > 0
GROUP BY PHeadMas.Mst_Desc
ORDER BY  PHeadMas.Mst_Desc
SET @GrandTotalCol = LEFT (@GrandTotalCol, LEN (@GrandTotalCol)-1)

print @GrandTotalCol

/* GRAND TOTAL ROW */
DECLARE @GrandTotalRow	NVARCHAR(MAX)
SELECT @GrandTotalRow = COALESCE(@GrandTotalRow + ',ISNULL(SUM([' + CAST(PHeadMas.Mst_Desc AS VARCHAR)+']),0)', 'ISNULL(SUM([' + CAST(PHeadMas.Mst_Desc AS VARCHAR)+']),0)')
FROM 
    MASTER_S as PHeadMas
	WHERE PHeadMas.MstParentId = 7
	AND   CAST(PHeadMas.Mst_Value AS int) > 0
GROUP BY PHeadMas.Mst_Desc
ORDER BY  PHeadMas.Mst_Desc

print @GrandTotalRow

/* MAIN QUERY */
DECLARE @FinalQuery NVARCHAR (MAX)
SET @FinalQuery = 	'SELECT *, ('+ @GrandTotalCol + ') AS [Grand Total] INTO #temp_PayVouchersTotal
			FROM
				(
					Select 
					 --cast(PV_DATA.Voucher_No as nvarchar) as Voucher_No
					 --cast(PV_DATA.Voucher_Date as nvarchar) as "DATE"
					 FORMAT (PV_DATA.Voucher_Date, ''dd/MM/yyyy'') as "DATE"
					,PV_DATA.PV_No as "PAYMENT VOUCHER NO."
					,PV_DATA.Cheque_Number as "CHEQUE NO."
					,PV_DATA.Descriptions as "DESCRIPTION"
					,PV_DATA.Amount as Amount2
					--,cast(PV_DATA.Amount as nvarchar) as Amount
					--,PV_DATA.Payment_Head
					,PHeadMas.Mst_Desc as PaymentHead
					FROM MASTER_S as PHeadMas
					left join (
					Select pv.Voucher_No, PV.Voucher_Date, PV.PV_No, PV.Cheque_Number, PayToMas.Mst_Desc as Descriptions,PV.Amount,PV.Payment_Head
					from PayVoucher as PV
					join v_PayTo as PayToMas on PV.Payment_To = PayToMas.Mst_Value
					WHERE PV.VoucherType IN (''1'',''2'')
					'+@WhereClause+'
					) PV_DATA on PV_DATA.Payment_Head = PHeadMas.Mst_Value and PHeadMas.MstParentId = 7
					WHERE PHeadMas.MstParentId = 7
					AND   CAST(PHeadMas.Mst_Value AS int) > 0
				) A
			PIVOT
				(
				 SUM(Amount2) FOR PaymentHead IN ('+@columnHeaders +')
				) B
WHERE "DATE" IS NOT NULL
ORDER BY 1
SELECT * FROM #temp_PayVouchersTotal UNION ALL
SELECT ''Grand Total'','''','''','''','+@GrandTotalRow +', ISNULL (SUM([Grand Total]),0) FROM #temp_PayVouchersTotal
DROP TABLE #temp_PayVouchersTotal'
 PRINT 'Pivot Query '+@FinalQuery
EXECUTE(@FinalQuery)
GO
