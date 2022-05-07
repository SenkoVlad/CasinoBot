IF(NOT EXISTS(select fk.name
              from sys.indexes AS fk
              where fk.name = 'GameResults_chatId_index'
))
BEGIN
    create index GameResults_chatId_index
        on GameResults (chatId)
END
GO