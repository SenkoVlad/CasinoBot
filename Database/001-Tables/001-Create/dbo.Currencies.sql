IF OBJECT_ID('[dbo].[Currencies]') IS NULL
create table dbo.Currencies
(
	id int not null
		constraint Currencies_pk
			primary key nonclustered,
	name VARCHAR(5) not null,
	coefficient FLOAT not null
)
go