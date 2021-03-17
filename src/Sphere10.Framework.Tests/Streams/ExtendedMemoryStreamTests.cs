//-----------------------------------------------------------------------
// <copyright file="LargeCollectionTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable]
	public class ExtendedMemoryStreamTests {

		[Test]
		public void Empty() {
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void ReadAll() {
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			var data = new Random(31337).NextBytes(100);
			stream.WriteBytes(data);
			expected.WriteBytes(data);
			Assert.AreEqual(expected.ReadBytes(100), stream.ReadBytes(100));
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void RemoveAll() {
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			var data = new Random(31337).NextBytes(100);
			stream.WriteBytes(data);
			expected.WriteBytes(data);
			stream.SetLength(0);
			expected.SetLength(0);
			Assert.AreEqual(expected.Position, stream.Position);
			Assert.AreEqual(expected.Length, stream.Length);
			Assert.AreEqual(expected.ToArray(), stream.ToArray());
		}

		[Test]
		public void SeekEnd_Empty() {
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			stream.Seek(0, SeekOrigin.End);
			expected.Seek(0, SeekOrigin.End);
			Assert.AreEqual(expected.Position, stream.Position);
		}

		[Test]
		public void SeekEnd_NotEmpty() {
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			var data = new Random(31337).NextBytes(100);
			stream.WriteBytes(data);
			expected.WriteBytes(data);
			stream.Seek(0, SeekOrigin.End);
			expected.Seek(0, SeekOrigin.End);
			Assert.AreEqual(expected.Position, stream.Position);
		}


		[Test]
		public void IntegrationTests([Values(0, 3, 111, 9371)] int maxSize) {
			var RNG = new Random(maxSize);
			var stream = new ExtendedMemoryStream();
			var expected = new MemoryStream();
			for (var i = 0; i < 100; i++) {
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
