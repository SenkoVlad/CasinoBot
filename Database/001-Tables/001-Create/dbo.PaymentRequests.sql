IF OBJECT_ID('[dbo].[PaymentRequests]') IS NULL
create table dbo.PaymentRequests
(
	id int identity
		constraint PaymentRequests_pk
			primary key nonclustered,
	chatId int not null,
	depositAmount FLOAT not null,
	currencyId int not null,
	telegramMessageId int not null,
	parseDateTimeUtc DATETIME not null,
	messageDateTimeUtc DATETIME not null,
    isAccounted        BIT DEFAULT 0  NOT NULL
)
GO