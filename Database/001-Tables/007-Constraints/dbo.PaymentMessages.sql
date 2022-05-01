IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'PaymentMessages_Chats_id_fk'
))
BEGIN
    alter table PaymentMessages
	add constraint PaymentMessages_Chats_id_fk
		foreign key (chatId) references Chats
END
GO

IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'PaymentMessages_Currencies_id_fk'
))
BEGIN
    alter table PaymentMessages
	add constraint PaymentMessages_Currencies_id_fk
		foreign key (currencyId) references Currencies
END
GO

