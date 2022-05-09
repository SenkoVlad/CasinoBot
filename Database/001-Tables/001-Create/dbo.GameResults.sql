IF OBJECT_ID('[dbo].[GameResults]') IS NULL
create table GameResults
(
	id int identity
		constraint GameResults_pk
			primary key nonclustered,
	chatId BIGINT not null,
	bet int not null,
	bettingResultId int not null
)
go

