IF(NOT EXISTS(select fk.name
              from sys.indexes AS fk
              where fk.name = 'PaymentMessages_telegramMessageId_index'
))
BEGIN
    create index PaymentMessages_telegramMessageId_index
        on PaymentMessages (telegramMessageId desc)
END

IF(NOT EXISTS(select fk.name
              from sys.indexes AS fk
              where fk.name = 'PaymentMessages_isCredited_index'
))
BEGIN
    create index PaymentMessages_isCredited_index
        on PaymentMessages (isAccounted)
END
GO