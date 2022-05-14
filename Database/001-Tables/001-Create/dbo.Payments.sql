IF OBJECT_ID('[dbo].[Payments]') IS NULL
create table Payments
(
	id int identity
		constraint Payments_pk
			primary key nonclustered,
	paymentId BIGINT not null,
	chatId BIGINT not null,
	currencyId int not null,
	totalAmount FLOAT not null,
	paymentDateTimeUtc DATETIME default GETUTCDATE() not null
)
go
