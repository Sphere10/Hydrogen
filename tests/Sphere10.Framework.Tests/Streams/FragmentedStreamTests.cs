using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {
	public class FragmentedStreamTests {

		private Random Random { get; } = new();

		[Test]
		public void Empty() {
			var fragments = new ByteArrayStreamFragmentProvider(Enumerable.Empty<byte[]>());
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();

			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void ReadAll() {
			var fragments = new ByteArrayStreamFragmentProvider();
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(299);

			stream.WriteBytes(data);
			expected.WriteBytes(data);

			stream.Seek(0, SeekOrigin.Begin);
			expected.Seek(0, SeekOrigin.Begin);

			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			
			Assert.AreEqual(expected.ReadBytes(data.Length), stream.ReadBytes(data.Length));
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void WriteUpdateAndAdd() {
			var fragments = new ByteArrayStreamFragmentProvider();
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(4);

			stream.WriteBytes(data);
			expected.WriteBytes(data);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());

			int halfway = data.Length / 2;
			byte[] newData = Random.NextBytes(data.Length);

			stream.Seek(halfway, SeekOrigin.Begin);
			expected.Seek(halfway, SeekOrigin.Begin);
			Assert.AreEqual(expected.Position, stream.Position);
			
			stream.WriteBytes(newData);
			expected.WriteBytes(newData);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		[TestCase(SeekOrigin.Begin, 11)]
		[TestCase(SeekOrigin.End, -11)]
		[TestCase(SeekOrigin.Current, 11)]
		public void SeekAndRead(SeekOrigin origin, int offset) {
			var fragments = new ByteArrayStreamFragmentProvider();
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
			var fragments = new ByteArrayStreamFragmentProvider();
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

		[Test]
		public void IncreaseSize() {
			var fragments = new ByteArrayStreamFragmentProvider();
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(Random.Next(1, 99));

			stream.WriteBytes(data);
			expected.WriteBytes(data);

			int newSpace = Random.Next(1, 250);
			stream.SetLength(stream.Length + newSpace);
			expected.SetLength(expected.Length + newSpace);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void DecreaseSize() {
			var fragments = new ByteArrayStreamFragmentProvider();
			var stream = new FragmentedStream(fragments);
			var expected = new MemoryStream();
			var data = Random.NextBytes(Random.Next(1, 99));

			stream.WriteBytes(data);
			expected.WriteBytes(data);
			
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.Position, stream.Position);

			int newSpace = Random.Next(1, (int)expected.Length);
			stream.SetLength(stream.Length - newSpace);
			expected.SetLength(expected.Length - newSpace);
			
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void WriteLastByteOfFragment() {
			var fragmentProvider = new ByteArrayStreamFragmentProvider(2);
			var stream = new FragmentedStream(fragmentProvider);
			Assert.That(stream.Position, Is.EqualTo(0));
		
			stream.WriteBytes(new byte[] { 0 });
			Assert.That(stream.Position, Is.EqualTo(1));
			Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(1));

			stream.WriteBytes(new byte[] { 1 });
			Assert.That(stream.Position, Is.EqualTo(2));
			Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(1));

			stream.WriteBytes(new byte[] { 2 });
			Assert.That(stream.Position, Is.EqualTo(3));
			Assert.That(fragmentProvider.FragmentCount, Is.EqualTo(2));
		}

		[Test]
		public void WriteBytesCorrectly() {
			var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
			var stream = new FragmentedStream(fragmentProvider);
			stream.WriteBytes(new byte[] { 0 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 1 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 2 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 3 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 4 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 5 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 6 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6 }).Using(ByteArrayEqualityComparer.Instance));

			stream.WriteBytes(new byte[] { 7 });
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }).Using(ByteArrayEqualityComparer.Instance));

		}


		[Test]
		public void SetLengthForward() {
			var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
			var stream = new FragmentedStream(fragmentProvider);
			stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
			stream.SetLength(5);
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 2, 3, 0 }).Using(ByteArrayEqualityComparer.Instance));
		}

		[Test]
		public void SetLengthBackward() {
			var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
			var stream = new FragmentedStream(fragmentProvider);
			stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
			stream.SetLength(2);
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1 }).Using(ByteArrayEqualityComparer.Instance));
		}

		[Test]
		public void SetLengthBackwardThenForward() {
			var fragmentProvider = new ByteArrayStreamFragmentProvider(3);
			var stream = new FragmentedStream(fragmentProvider);
			stream.WriteBytes(new byte[] { 0, 1, 2, 3 });
			stream.SetLength(2);
			stream.SetLength(3);
			Assert.That(stream.ToArray(), Is.EqualTo(new byte[] { 0, 1, 0 }).Using(ByteArrayEqualityComparer.Instance));
		}

		[Test]
		public void IntegrationTests([Values(0, 3, 111, 9371)] int maxSize, [Values(1, 3, 11, 10000)] int fragmentSize) {
			const int Interations = 100;
			var RNG = new Random(maxSize);
			var stream = new FragmentedStream(new ByteArrayStreamFragmentProvider(fragmentSize));
			var expected = new MemoryStream();
			for (var i = 0; i < Interations; i++) {
				Assert.AreEqual(expected.Position, stream.Position);
				Assert.AreEqual(expected.Length, stream.Length);
				Assert.AreEqual(expected.ToArray(), stream.ToArray());

				// 1. random seek
				var seekParam = RandomSeekParameters(RNG, stream.Position, stream.Length);
				stream.Seek(seekParam.Item1, seekParam.Item2);
				expected.Seek(seekParam.Item1, seekParam.Item2);
				// Check
				Assert.AreEqual(expected.Position, stream.Position);
				Assert.AreEqual(expected.Length, stream.Length);
				Assert.AreEqual(expected.ToArray(), stream.ToArray());

				// 2. write random bytes
				var remainingCapacity = (int)(maxSize - stream.Position);
				if (remainingCapacity > 0) {
					var fromBufferSize = RNG.Next(1, remainingCapacity * 2);  // allow from buffer to be 0..2*remaining
					var fromBuffer = RNG.NextBytes(fromBufferSize);
					// Copy from a random segment of fromBuffer into stream
					var segment = RNG.NextRange(fromBufferSize, rangeLength: RNG.Next(1, Math.Min(fromBufferSize, remainingCapacity)));
					if (segment.End >= segment.Start) {
						expected.Write(fromBuffer, segment.Start, segment.End - segment.Start + 1);
						stream.Write(fromBuffer, segment.Start, segment.End - segment.Start + 1);
					}
					// Check
					Assert.AreEqual(expected.Position, stream.Position);
					Assert.AreEqual(expected.Length, stream.Length);
					Assert.AreEqual(expected.ToArray(), stream.ToArray());
				}

				// 3. random read
				if (stream.Length > 0) {
					var segment = RNG.NextRange((int)stream.Length, rangeLength: Math.Max(1, RNG.Next(0, (int)stream.Length)));
					var count = segment.End - segment.Start + 1;
					expected.Seek(segment.Start, SeekOrigin.Begin);
					stream.Seek(segment.Start, SeekOrigin.Begin);
					// Check
					Assert.AreEqual(expected.ReadBytes(count), stream.ReadBytes(count));
					Assert.AreEqual(expected.Position, stream.Position);
					Assert.AreEqual(expected.Length, stream.Length);
					Assert.AreEqual(expected.ToArray(), stream.ToArray());
				}

				// 4. resize 
				var newLength = RNG.Next(0, maxSize);
				expected.SetLength(newLength);
				stream.SetLength(newLength);
		
				// Check
				Assert.AreEqual(expected.Position, stream.Position);
				Assert.AreEqual(expected.Length, stream.Length);
				Assert.AreEqual(expected.ToArray(), stream.ToArray());

			}
		}

		private Tuple<long, SeekOrigin> RandomSeekParameters(Random rng, long position, long length) {
			switch (rng.Next(0, 3)) {
				case 0:
					return Tuple.Create((long)rng.Next((int)-position, (int)(Math.Max(0, length - position))), SeekOrigin.Current);
				case 1:
					return Tuple.Create((long)rng.Next(0, (int)Math.Max(0, length)), SeekOrigin.Begin);
				case 2:
					return Tuple.Create((long)rng.Next((int)-length, 0 + 1), SeekOrigin.End);
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
