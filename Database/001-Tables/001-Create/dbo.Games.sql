IF OBJECT_ID('[dbo].[Games]') IS NULL
create table Games
(
	id int identity
		constraint Games_pk
			primary key nonclustered,
	name VARCHAR(20) not null
)
go

