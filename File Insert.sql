Create Proc spInsertStates
@StateTableType StateTableType READONLY
AS
BEGIN
    Insert Into State (CountryId, StateName)
    Select CountryId, StateName  from @StateTableType
END

CREATE TYPE StateTableType AS TABLE
(
    CountryId int,
    StateName NVARCHAR(50)
)

Create Proc spInsertCountry
@CountryTableType CountryTableType READONLY
AS
BEGIN
    Insert Into Country(CountryName)
    Select CountryName from @CountryTableType
END

CREATE TYPE CountryTableType AS TABLE
(
    CountryName NVARCHAR(50)
)

drop type StateTableType
drop proc spInsertStates

select * from State
select * from Country

select Country.CountryName, State.StateName from Country join State 
on Country.CountryId=State.CountryId

delete from State

alter table Country alter column CountryCode varchar(5) Null