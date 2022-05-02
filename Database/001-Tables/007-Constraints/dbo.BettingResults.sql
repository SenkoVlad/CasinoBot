IF(NOT EXISTS(select fk.name
              from sys.foreign_keys AS fk
              where fk.name = 'BettingResults_Games_id_fk'
))
BEGIN
    alter table BettingResults
        add constraint BettingResults_Games_id_fk
            foreign key (gameId) references dbo.Games
END
go
