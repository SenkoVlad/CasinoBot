IF OBJECT_ID('[dbo].[PaymentMessages]') IS NULL
create table dbo.PaymentMessages
(
	id int identity
		constraint PaymentMessages_pk
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