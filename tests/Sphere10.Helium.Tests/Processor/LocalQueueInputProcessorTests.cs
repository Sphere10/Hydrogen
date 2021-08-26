using NUnit.Framework;
using Sphere10.Helium.Processor;
using Sphere10.Helium.Queue;

namespace Sphere10.Helium.Tests.Processor
{
    /// <summary>
    /// UNIT-TEST: Testing inside methods to make dependencies are called and logic is performed.
    /// </summary>
    [TestFixture]
    public class LocalQueueInputProcessorTests
    {
        private ILocalQueueInputProcessor _localQueueInputProcessor;

        [SetUp]
        public void SetupHeliumPluginLoader()
        {
            /* THIS NEEDS TO BE MOCKED */
            var mockHeliumQueue = new LocalQueue(new LocalQueueSettings());

            _localQueueInputProcessor = new LocalQueueInputProcessor(mockHeliumQueue);
        }
    }
}