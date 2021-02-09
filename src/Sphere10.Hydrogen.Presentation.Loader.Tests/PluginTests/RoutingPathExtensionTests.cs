using NUnit.Framework;
using Sphere10.Hydrogen.Presentation.Loader.Plugins;

namespace Sphere10.Hydrogen.Presentation.Loader.Tests.PluginTests {
    public class RoutingPathExtensionTests {
        [TestCase("/myapp/testing?test=1", ExpectedResult = "/myapp")]
        [TestCase("/", ExpectedResult = "/")]
        [TestCase("/myapp", ExpectedResult = "/myapp")]
        [TestCase("/myapp/", ExpectedResult = "/myapp")]
        [TestCase("/myapp/testing/test-page", ExpectedResult = "/myapp")]
        public string AppPathFromRelativePathTests(string input) {
            return input.ToAppPathFromBaseRelativePath();
        }

        [TestCase("/myapp/testing?test=1", ExpectedResult = "/myapp/testing")]
        [TestCase("/myapp/testing/abc?test=1&test2=2", ExpectedResult = "/myapp/testing/abc")]
        [TestCase("/myapp/", ExpectedResult = "/myapp/")]
        [TestCase("/myapp", ExpectedResult = "/myapp")]
        public string TrimQueryFromRelativePath(string input) {
            return input.TrimQueryParameters();
        }
    }
}