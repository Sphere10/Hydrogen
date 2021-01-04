using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VelocityNET.Presentation.Hydrogen.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests
{
    public class TestPlugin : Plugin
    {
        public TestPlugin()
        {
            Apps = new[]
            {
                new Hydrogen.Plugins.App("/", "Home", "abc", new[]
                {
                    new AppBlock("test", "abc", new[]
                    {
                        new AppBlockPage("/test", "test page", "abc", new[]
                        {
                            new MenuItem("Test Menu", "/app1/page1", new List<MenuItem>())
                        })
                    })
                }),
                new Hydrogen.Plugins.App("/app1", "app1", "abc", new[]
                {
                    new AppBlock("app1", "abc", new[]
                    {
                        new AppBlockPage("/app1/page1", "app1 page", "abc", new[]
                        {
                            new MenuItem("Test Menu", "/app1/page1", new List<MenuItem>())
                        })
                    })
                })
            };
        }

        public override IEnumerable<IApp> Apps { get; }

        protected override void ConfigureServicesInternal(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<TestViewModel>();
        }
    }

    internal class TestViewModel
    {
    }
}