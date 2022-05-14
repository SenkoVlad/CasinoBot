IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'Payments_Chats_id_fk'
))
BEGIN
    alter table Payments
        add constraint Payments_Chats_id_fk
            foreign key (chatId) references Chats
END
go

IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'Payments_Currencies_id_fk'
))
BEGIN
    alter table Payments
        add constraint Payments_Currencies_id_fk
            foreign key (currencyId) references Currencies
END
go



