using System.IO;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{

    public class BoundedStreamTests
    {
        [Test]
        public void BoundedStreamAbsoluteSeek()
        {
            var inner = new MemoryStream();
            var bounded = new BoundedStream(inner, 100, 199);

            Assert.Throws<StreamOutOfBoundsException>(() => bounded.Seek(0, SeekOrigin.Begin));
            bounded.Seek(bounded.MinAbsolutePosition, SeekOrigin.Begin);
        }

        [Test]
        public void BoundedStreamRelativeSeek()
        {
            var inner = new MemoryStream();
            var bounded = new BoundedStream(inner, 100, 199)
            {
                UseRelativeOffset = true
            };

            Assert.Throws<StreamOutOfBoundsException>(() => bounded.Seek(bounded.MinAbsolutePosition, SeekOrigin.Begin));
            bounded.Seek(0, SeekOrigin.Begin);
        }
    }

}