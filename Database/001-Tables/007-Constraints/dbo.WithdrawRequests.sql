IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'WithdrawRequests_Chats_id_fk'
))
BEGIN
    alter table dbo.WithdrawRequests
        add constraint WithdrawRequests_Chats_id_fk
            foreign key (chatId) references Chats
END


IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'WithdrawRequests_Currencies_id_fk'
))
BEGIN
    alter table dbo.WithdrawRequests
        add constraint WithdrawRequests_Currencies_id_fk
            foreign key (currencyId) references Currencies
END
