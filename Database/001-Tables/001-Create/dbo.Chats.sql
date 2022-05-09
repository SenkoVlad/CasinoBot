IF OBJECT_ID('[dbo].[Chats]') IS NULL
    create table dbo.Chats
    (
        id BIGINT
            constraint Chats_pk
                primary key nonclustered,
        language VARCHAR(5)  not null,
        balance FLOAT not null,
        demoBalance int not null,
        createDateTimeUtc DATETIME
    )
GO


