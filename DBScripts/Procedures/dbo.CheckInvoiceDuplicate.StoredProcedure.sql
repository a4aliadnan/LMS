SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [CheckInvoiceDuplicate]
@CaseId INT,
@InvoiceDate DATETIME2,
@FeeTypeId INT,
@FeeAmount DEC(13,3)
AS
BEGIN
SELECT count(*) as DupExists
from CaseInvoices CI 
join CaseInvoiceFees CIF on CIF.InvoiceId = CI.InvoiceId 
where CI.CaseId = @CaseId
and   CI.InvoiceDate = @InvoiceDate
and   CIF.FeeTypeId = @FeeTypeId
and   CIF.FeeAmount = @FeeAmount
END
GO
