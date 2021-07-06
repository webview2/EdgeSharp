using EdgeSharp.Core;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace EdgeSharp.Tests
{
    public class AppBuilderFeature : IDisposable
    {
        public AppBuilderFeature()
        {
            var testApp = new TestApp();
            var services = new ServiceCollection();
            testApp.ConfigureServices(services);
            testApp.ConfigureCoreServices(services);

            Provider = testApp.BuildServiceProvider(services);
            testApp.Initialize(Provider);
            testApp.RegisterActionRoutes(Provider);

            ServiceLocator.Bootstrap(Provider);
        }

        public IServiceProvider Provider { get; }

        public void Dispose()
        {
            (Provider as ServiceProvider)?.Dispose();
        }
    }

    [CollectionDefinition(Constants.AppBuilderFeatureCollection)]
    public class AppBuilderFeatureCollection : ICollectionFixture<AppBuilderFeature>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    public class TestApp : EdgeSharpApp
    {
    }
}
