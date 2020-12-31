using NUnit.Framework;
using VelocityNET.Presentation.Blazor.Plugins;

namespace VelocityNET.Presentation.Blazor.Tests.PluginTests
{
    public class RoutingExtensionTests
    {
        [TestCase("/myapp/testing?test=1", ExpectedResult = "/myapp")]
        [TestCase("/myapp", ExpectedResult = "/myapp")]
        [TestCase("/myapp/", ExpectedResult = "/myapp")]
        [TestCase("/myapp/testing/test-page", ExpectedResult = "/myapp")]
        public string AppPathFromRelativePathTests(string input)
        {
            return input.ToAppPathFromBaseRelativePath();
        }
    }
}