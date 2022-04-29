using System;
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


		[Test]
		public void AllowTipCursor_1() {
			using Stream stream = new MemoryStream();
			var bounded = new BoundedStream(stream, 0, 0) { AllowInnerResize = true };
			Assert.That(bounded.Length, Is.EqualTo(0));
			Assert.That(bounded.Position, Is.EqualTo(0));
		}

		[Test]
		public void AllowTipCursor_2() {
			var rng = new Random(31337);
			using Stream stream = new MemoryStream(rng.NextBytes(100));
			var bounded = new BoundedStream(stream, 0, 100) { AllowInnerResize = true };
			bounded.Position = 0;
			bounded.Write(rng.NextBytes(100));
			Assert.That(bounded.Position, Is.EqualTo(100));
		}


		[Test]
		public void SetLength_Does_Not_Change_LogicalLength() {
			var rng = new Random(31337);
			using Stream stream = new MemoryStream();
			var bounded = new BoundedStream(stream, 0, 99) { AllowInnerResize = true };
			bounded.SetLength(200);
			Assert.That(bounded.Position, Is.EqualTo(0));
			Assert.That(bounded.Length, Is.EqualTo(100));
		}
	}

}