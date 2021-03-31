using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {
	public class FragmentedStreamTests {

		private Random Random { get; } = new();

		[Test]
		public void Empty() {
			var fragments = new ByteArrayFragmentProvider(Enumerable.Empty<byte[]>());
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();

			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void ReadAll() {
			var fragments = new ByteArrayFragmentProvider(new[] { new byte[10], new byte[10], new byte[10] });
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(50);

			stream.WriteBytes(data);
			expected.WriteBytes(data);

			Assert.AreEqual(expected.ReadBytes(30), stream.ReadBytes(30));
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		[TestCase(SeekOrigin.Begin, 11)]
		[TestCase(SeekOrigin.End, -11)]
		[TestCase(SeekOrigin.Current, 11)]
		public void SeekAndRead(SeekOrigin origin, int offset) {
			var fragments = new ByteArrayFragmentProvider(new[] { new byte[10], new byte[10], new byte[10] });
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(50);

			stream.WriteBytes(data);
			expected.WriteBytes(data);
			stream.Seek(0, SeekOrigin.Begin);
			expected.Seek(0, SeekOrigin.Begin);

			stream.Seek(offset, origin);
			expected.Seek(offset, origin);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.ReadBytes(1), stream.ReadBytes(1));
		}
		
		[Test]
		public void RemoveAll() {
			var fragments = new ByteArrayFragmentProvider(new[] { new byte[10], new byte[10], new byte[10] });
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(50);

			stream.WriteBytes(data);
			expected.WriteBytes(data);
			
			stream.SetLength(0);
			expected.SetLength(0);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}
	}
}
