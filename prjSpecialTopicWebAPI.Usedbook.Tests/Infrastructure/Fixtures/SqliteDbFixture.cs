using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using prjSpecialTopicWebAPI.Models;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.Fixtures
    {
    [CollectionDefinition("SqliteDbCollection")]
    public class SqliteDbCollection : ICollectionFixture<SqliteDbFixture> { }

    public class SqliteDbFixture : IDisposable
    {
        public TeamAProjectContext DbContext { get; }
        private readonly SqliteConnection _connection;

        public SqliteDbFixture()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<TeamAProjectContext>()
                .UseSqlite(_connection)
                .EnableSensitiveDataLogging()
                .Options;

            DbContext = new TeamAProjectContext(options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            DbContext.Dispose();
            _connection.Dispose();
        }
    }
}
