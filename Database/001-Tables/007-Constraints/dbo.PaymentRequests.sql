IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'PaymentRequests_Chats_id_fk'
))
BEGIN
    alter table PaymentRequests
	add constraint PaymentRequests_Chats_id_fk
		foreign key (chatId) references dbo.Chats
END
GO

IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'PaymentRequests_Currencies_id_fk'
))
BEGIN
    alter table PaymentRequests
	add constraint PaymentRequests_Currencies_id_fk
		foreign key (currencyId) references Currencies
END
GO

