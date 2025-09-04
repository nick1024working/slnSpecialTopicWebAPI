USE [TeamA_Project]
GO

update UsedBooks
set CategoryId = 17
where Id = 'AF50E46C-2E44-4BB4-B90D-45EDC0DF47E8';
go

update UsedBooks
set BindingId = 1, LanguageId = 1, CategoryId = 2
where Id = N'5541FC0E-DE33-4DDD-A819-EF8026FCFCAD';
go