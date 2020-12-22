using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using VelocityNET.Presentation.Blazor.Components;
using VelocityNET.Presentation.Blazor.ViewModels;

namespace VelocityNET.Presentation.Blazor.Tests.ComponentTests
{

    public class Tests : Bunit.TestContext
    {
        [SetUp]
        public void Setup()
        {
            Services.AddTransient<BlockMenuViewModel>();
        }

        [Test]
        public void BlockMenuRenders()
        {
            var component = RenderComponent<BlockMenu>();

            Assert.AreEqual(1, component.RenderCount);
            Assert.NotNull(component.Instance);
        }
    }

}