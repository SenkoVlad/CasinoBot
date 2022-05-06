IF OBJECT_ID('[dbo].[WithdrawRequests]') IS NULL
    create table WithdrawRequests
    (
        id int identity
            constraint WithdrawRequests_pk
                primary key nonclustered,
        chatId int not null,
        currencyId int not null,
        amount int not null,
        isAccounted BIT not null,
        createdDateTimeUtc DATETIME not null
    )
go
