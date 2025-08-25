using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using prjSpecialTopicWebAPI.Features.Usedbook.Mapping;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure.Fixtures
{
    [CollectionDefinition("MapperCollection")]
    public class MapperCollection : ICollectionFixture<AutoMapperFixture> { }

    public class AutoMapperFixture
    {
        public IMapper Mapper { get; }

        public AutoMapperFixture()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, NullLoggerFactory.Instance);

            // （可選）在測試專案裡立即驗證組態
            //mapperConfig.AssertConfigurationIsValid();

            Mapper = mapperConfig.CreateMapper();
        }
    }
}