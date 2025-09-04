USE [TeamA_Project]
GO

begin tran

update UsedBooks
set CategoryId = 17
where Id = 'AF50E46C-2E44-4BB4-B90D-45EDC0DF47E8';
go

update UsedBooks
set BindingId = 1, LanguageId = 1, CategoryId = 2
where Id = N'5541FC0E-DE33-4DDD-A819-EF8026FCFCAD';
go

UPDATE UsedBooks
Set IsOnShelf = 0, IsSold = 1
Where Id in (
	N'7b20926b-17ed-4823-b24c-68f01c3918cc',
	N'bc22ecba-dc65-4660-8a93-d2a237651984',
	N'dd0547fe-fb2a-429a-829b-550d91773eb1',
	N'7ce93f97-5098-4ac5-b120-8d4938a85949',
	N'b309561c-3e73-4059-9345-7764a05a8434',
	N'5c0616fd-8381-4b5c-a10e-01211a079ea9',
	N'fe6af8a7-907b-4747-976b-165e10a3036f'
)
go

commit