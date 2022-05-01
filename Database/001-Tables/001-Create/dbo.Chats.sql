IF OBJECT_ID('[dbo].[Chats]') IS NULL
    create table dbo.Chats
    (
        id int
            constraint Chats_pk
                primary key nonclustered,
        language VARCHAR(5)  not null,
        balance FLOAT not null,
        demoBalance int not null
    )
GO


