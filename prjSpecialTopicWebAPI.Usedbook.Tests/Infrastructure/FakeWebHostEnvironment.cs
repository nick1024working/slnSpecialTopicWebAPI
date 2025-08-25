using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace prjSpecialTopicWebAPI.Usedbook.Tests.Infrastructure
{
    public class FakeWebHostEnvironment : IWebHostEnvironment
    {
        public string WebRootPath { get; set; } = "wwwroot";

        public string EnvironmentName { get; set; } = "Development";
        public string ApplicationName { get; set; } = "TestApp";
        public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
        public IFileProvider WebRootFileProvider { get; set; } = default!;
        public IFileProvider ContentRootFileProvider { get; set; } = default!;
    }

}
