using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

public static class TestDbContextFactory
{
    public static TeamAProjectContext CreateSqliteInMemoryContext()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TeamAProjectContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var context = new TeamAProjectContext(options);
        context.Database.EnsureCreated();

        return context;
    }
}
