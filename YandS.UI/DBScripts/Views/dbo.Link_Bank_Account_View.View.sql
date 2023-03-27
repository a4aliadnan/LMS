SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE view [Link_Bank_Account_View] as
select BA.LinkId,BA.BankId,BA.AccountId,BankMas.Mst_Desc +' : '+ BA.AccountNumber as Mst_Desc
from Link_Bank_Account BA
join MASTER_S AccMas on BA.AccountId = AccMas.MstId and AccMas.MstParentId = 3
join MASTER_S BankMas on BA.BankId = BankMas.MstId and BankMas.MstParentId = 2


GO
