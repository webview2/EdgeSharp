using EdgeSharp.Core;
using EdgeSharp.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Xunit;

namespace EdgeSharp.Tests.Defaults
{
    [Collection(Constants.AppBuilderFeatureCollection)]
    public class ResourceRequestHandlerTests
    {
        AppBuilderFeature _fixture;

        public ResourceRequestHandlerTests(AppBuilderFeature fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void ValidateHandlerTest()
        {
            var config = _fixture.Provider.GetService<IConfiguration>();
            var handler = _fixture.Provider.GetService<IResourceRequestHandler>();

            Assert.NotNull(config);
            Assert.NotNull(handler);

            var browserArguments = handler?.EnvironmentOptions?.AdditionalBrowserArguments;

            if (!string.IsNullOrWhiteSpace(browserArguments))
            {
                var commandLineArgs = config?.CommandLineArgs ?? new Dictionary<string, string>();
                var commandLineOpts = config?.CommandLineOptions ?? new List<string>();

                foreach (var arg in commandLineArgs)
                {
                    Assert.Contains(arg.Key, browserArguments, System.StringComparison.InvariantCultureIgnoreCase);
                    Assert.Contains(arg.Value, browserArguments, System.StringComparison.InvariantCultureIgnoreCase);
                }

                foreach (var opt in commandLineOpts)
                {
                    Assert.Contains(opt, browserArguments, System.StringComparison.InvariantCultureIgnoreCase);
                }
            }
       }
    }
}
