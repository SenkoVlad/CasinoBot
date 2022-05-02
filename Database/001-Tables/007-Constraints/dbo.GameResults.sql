IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'GameResults_BettingResults_id_fk'
))
BEGIN
    alter table GameResults
	add constraint GameResults_BettingResults_id_fk
		foreign key (bettingResultId) references BettingResults
END

IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'GameResults_Chats_id_fk'
))
BEGIN
    alter table GameResults
        add constraint GameResults_Chats_id_fk
	    	foreign key (chatId) references Chats
END
go
