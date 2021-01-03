using NUnit.Framework;
using VelocityNET.Presentation.Hydrogen.Loader.Plugins;

namespace VelocityNET.Presentation.Hydrogen.Loader.Tests.PluginTests
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