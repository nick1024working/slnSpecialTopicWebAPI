namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.Fixtures
{
    // [Collection] 屬性在 xUnit 一個類別只能貼一次
    // 建立 整合 Collection，一次綁多個 Fixture
    [CollectionDefinition("SharedTestCollection")]
    public class SharedTestCollection :
        ICollectionFixture<SqliteDbFixture>,
        ICollectionFixture<AutoMapperFixture>
    { }
}
