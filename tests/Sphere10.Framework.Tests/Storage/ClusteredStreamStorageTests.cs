using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	// TODO:
	// - During dev, bugs seemed to occur when clusters linked in descending order
	// - write unit tests which directly scramble the cluster links, different patterns (descending, random, etc)
	public class ClusteredStreamStorageTests {

		[Test]
		public void AddEmpty([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add())
				Assert.That(stream.Length, Is.EqualTo(0));
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Records.Count, Is.EqualTo(1));
		}

		[Test]
		public void AddNull([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(null);
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Records.Count, Is.EqualTo(1));
		}

		[Test]
		public void AddManyEmpty([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			for (var i = 0; i < N; i++)
				using (var stream = streamContainer.Add())
					Assert.That(stream.Length, Is.EqualTo(0));
			Assert.That(streamContainer.Count, Is.EqualTo(N));
			Assert.That(streamContainer.Records.Count, Is.EqualTo(N));
		}

		[Test]
		public void OpenEmpty([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (_ = streamContainer.Add()) ;
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_1([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add()) {
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Records.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Records[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_2([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add()) {
				stream.Write(new byte[] { 1 });
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Records.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Records[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_3([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add()) {
				stream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Records.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Records[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void Add1Byte([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1 });
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		}

		[Test]
		public void Add2x1Byte([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1 });
			streamContainer.AddBytes(new byte[] { 1 });
			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		}

		[Test]
		public void Add2ShrinkFirst_1b([Values(1, 2, 3, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1 });
			streamContainer.AddBytes(new byte[] { 2 });
			using (var stream = streamContainer.Open(0))
				stream.SetLength(0);

			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.Empty);
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		}

		[Test]
		public void Add2ShrinkFirst_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 1 });
			streamContainer.AddBytes(new byte[] { 2, 2 });

			using (var stream = streamContainer.Open(0))
				stream.SetLength(0);

			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.Empty);
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2, 2 }));
		}

		[Test]
		public void Add2ShrinkSecond_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 1 });
			streamContainer.AddBytes(new byte[] { 2, 2 });
			using (var stream = streamContainer.Open(1))
				stream.SetLength(0);

			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1, 1 }));
			Assert.That(streamContainer.ReadAll(1), Is.Empty);
		}

		[Test]
		public void AddNx1Byte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			for (var i = 0; i < N; i++)
				streamContainer.AddBytes(new byte[] { 1 });

			Assert.That(streamContainer.Count, Is.EqualTo(N));
			for (var i = 0; i < N; i++) {
				var streamData = streamContainer.Open(i).ReadAllAndDispose();
				Assert.That(streamData, Is.EqualTo(new byte[] { 1 }));
			}
		}

		[Test]
		public void AddNxMByte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			var rng = new Random(31337);
			var actual = new List<byte[]>();
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			for (var i = 0; i < N; i++) {
				using var stream = streamContainer.Add();
				var data = rng.NextBytes(M);
				actual.Add(data);
				stream.Write(data);
			}
			Assert.That(streamContainer.Count, Is.EqualTo(N));
			for (var i = 0; i < N; i++)
				Assert.That(streamContainer.ReadAll(i), Is.EqualTo(actual[i]));
		}

		[Test]
		public void Insert1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.InsertBytes(0, new byte[] { 1 });
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		}

		[Test]
		public void Insert2x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.InsertBytes(0, new byte[] { 1 });
			streamContainer.InsertBytes(0, new byte[] { 2 });
			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 2 }));
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		}

		[Test]
		public void Insert3x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.InsertBytes(0, new byte[] { 1 });
			streamContainer.InsertBytes(0, new byte[] { 2 });
			streamContainer.InsertBytes(0, new byte[] { 3 });
			Assert.That(streamContainer.Count, Is.EqualTo(3));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 3 }));
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
			Assert.That(streamContainer.ReadAll(2), Is.EqualTo(new byte[] { 1 }));
		}


		[Test]
		public void Insert_BugCase() {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, 32);
			streamContainer.InsertBytes(0, new byte[] { 1 });
			streamContainer.InsertBytes(0, Array.Empty<byte>());
			Assert.That(streamContainer.Clusters[1].Prev, Is.EqualTo(1));
		}

		[Test]
		public void Remove1b([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters.Count, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamStorageHeader.ByteLength));
		}

		[Test]
		public void Remove2b([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 2 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters.Count, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamStorageHeader.ByteLength));
		}

		[Test]
		public void Remove3b_Bug([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, 1, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 2, 3 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamStorageHeader.ByteLength));
		}

		[Test]
		public void AddString([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const string data = "Hello Stream!";
			var dataBytes = Encoding.ASCII.GetBytes(data);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(dataBytes);
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(dataBytes));
		}

		[Test]
		public void RemoveString([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const string data = "Hello Stream!";
			var dataBytes = Encoding.ASCII.GetBytes(data);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(dataBytes);
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters.Count, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamStorageHeader.ByteLength));
		}

		[Test]
		public void UpdateWithSmallerStream([Values(1, 4, 32, 2048)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const string data1 = "Hello Stream! This is a long string which will be replaced by a smaller one.";
			const string data2 = "a";
			var data1Bytes = Encoding.ASCII.GetBytes(data1);
			var data2Bytes = Encoding.ASCII.GetBytes(data2);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add()) {
				stream.Write(data1Bytes);
				stream.SetLength(0);
				stream.Write(data2Bytes);
			}
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		}

		[Test]
		public void UpdateWithLargerStream([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const string data1 = "a";
			const string data2 = "Hello Stream! This is a long string which did replace a smaller one.";
			var data1Bytes = Encoding.ASCII.GetBytes(data1);
			var data2Bytes = Encoding.ASCII.GetBytes(data2);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			using (var stream = streamContainer.Add()) {
				stream.Write(data1Bytes);
				stream.SetLength(0);
				stream.Write(data2Bytes);
			}
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		}

		[Test]
		public void AddRemoveAllAddFirst([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
			streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
			streamContainer.Remove(0);
			streamContainer.Remove(0);
			streamContainer.AddBytes(new byte[] { 9, 8, 7, 6, 5 });
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 9, 8, 7, 6, 5 }));
		}

		[Test]
		public void AddTwoRemoveFirst([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
			streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
		}

		[Test]
		public void AddTwoRemoveAndReAdd([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
			streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
			streamContainer.Remove(0);
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }));
		}

		[Test]
		public void TestRootStreamLengthConsistent([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 111;
			var rng = new Random(31337);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamStorageHeader.ByteLength + 4 * (clusterSize + 9)));
		}

		[Test]
		public void TestClear([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			var rng = new Random(31337);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.Clear();
			Assert.That(streamContainer.Count, Is.EqualTo(0));
		}

		[Test]
		public void TestClear_2([Values(1, 4, 32)] int clusterSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const string data = "Hello Stream!";
			var rng = new Random(31337);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.AddBytes(rng.NextBytes(clusterSize));
			streamContainer.Clear();
			Assert.That(streamContainer.Count, Is.EqualTo(0));

			var dataBytes = Encoding.ASCII.GetBytes(data);
			streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(dataBytes);
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(dataBytes));
		}

		[Test]
		public void CorruptData_NextPointsNonExistentCluster([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			// corrupt root-stream, make tip cluster 17 have next to 100 creating a circular linked loop through forward traversal
			var cluster16NextPrevOffset = rootStream.Length - 9 * (streamContainer.ClusterEnvelopeSize + streamContainer.ClusterSize) + sizeof(byte) + sizeof(int);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness.LittleEndian), rootStream);
			rootStream.Seek(cluster16NextPrevOffset, SeekOrigin.Begin);
			writer.Write((int)123456);
			Assert.That(() => streamContainer.AppendBytes(0, new byte[] { 11 }), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_ForwardsCyclicClusterChain([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			var xxx = streamContainer.ToStringFullContents();
			// corrupt root-stream, make tip cluster 18 have next to 10 creating a circular linked loop through forward traversal
			var nextOffset = rootStream.Length - clusterSize - sizeof(uint);
			var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness.LittleEndian), rootStream);
			rootStream.Seek(nextOffset, SeekOrigin.Begin);
			writer.Write((int)10);
			Assert.That(() => streamContainer.AppendBytes(0, new byte[] { 11 }), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_PrevPointsNonExistentCluster([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			// make a 3 streams, corrupt middle back, should clear no problem
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			streamContainer.FastWriteClusterNext(9, 123456); // corrupt root-stream by make cluster 9 prev point back to NON-EXISTANT CLUSTER
			Assert.That(() => streamContainer.Clear(0), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_BackwardsCyclicClusterChain_Graceful([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			// make a 3 streams, corrupt middle back, should clear no problem
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
			streamContainer.AddBytes(new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 });
			streamContainer.FastWriteClusterPrev(9, 10); // corrupt root-stream by making cyclic dependency between clusters 9 an 10
			streamContainer.Clear(0);
			streamContainer.Clear(0);

			// note: doesn't seem TraverseBack is ever called in fragment provider, so this error is seemingly inconsequential
		}

		[Test]
		public void CorruptData_ClusterTraits([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const byte IllegalValue = 8;
			// make a 3 streams, corrupt middle back, should clear no problem
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
			streamContainer.FastWriteClusterTraits(10, (ClusterTraits)IllegalValue);
			Assert.That(() => streamContainer.ReadAll(0), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_BadHeaderVersion([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = 0;
			rootStream.WriteByte(2);

			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_BadClusterSize_Zero([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte);
			writer.Write(0);
			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_BadClusterSize_TooLarge([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte);
			writer.Write(100);
			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_BadClusterSize_TooBig([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte);
			writer.Write(clusterSize + 1);
			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_TotalClusters_Zero([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte) + sizeof(int);
			writer.Write(0);
			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_TotalClusters_TooLarge([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte) + sizeof(int);
			writer.Write(streamContainer.Clusters.Count + 1);
			Assert.That(() => ClusteredStreamStorage.Load(rootStream), Throws.TypeOf<CorruptDataException>());
		}

		[Test]
		public void CorruptData_Records_TooSmall_HandlesGracefully([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte) + sizeof(int) + sizeof(int);
			writer.Write(streamContainer.Records.Count - 1);
			// note: Can't detect this scenario in integrity checks without examining data, so will
			// end up creating a corrupt data later. This is not ideal, but acceptable.
			ClusteredStreamStorage.Load(rootStream);
		}

		[Test]
		public void CorruptData_Records_TooLarge_HandlesGracefully([Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
			rootStream.Position = sizeof(byte) + sizeof(int) + sizeof(int);
			writer.Write(streamContainer.Records.Count + 1);
			// note: Can't detect this scenario in integrity checks without examining data, so will
			// end up creating a corrupt data later. This is not ideal, but acceptable.
			ClusteredStreamStorage.Load(rootStream);
		}


		[Test]
		public void LoadEmpty() {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, 1);
			using var clonedStream = new MemoryStream(rootStream.ToArray());
			var loadedStreamContainer = new ClusteredStreamStorage(clonedStream, 1);
			Assert.That(() => loadedStreamContainer.ToStringFullContents(), Throws.Nothing);
		}

		[Test]
		public void LoadOneOneByteListing() {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, 1);
			streamContainer.AddBytes(new byte[] { 1 });
			using var clonedStream = new MemoryStream(rootStream.ToArray());
			var loadedStreamContainer = new ClusteredStreamStorage(clonedStream, 1);
			Assert.That(streamContainer.ToStringFullContents(), Is.EqualTo(loadedStreamContainer.ToStringFullContents()));
		}

		[Test]
		public void LoadComplex([Values(1, 4, 32)] int clusterSize,  [Values(0, 2, 4, 100)] int maxStreamSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {

			var rng = new Random(31337 + (int)recordsCachePolicy + (int)streamCachePolicy);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			for (var i = 0; i < 100; i++)
				streamContainer.AddBytes(rng.NextBytes(maxStreamSize));

			for (var i = 0; i < 100; i++)
				streamContainer.UpdateBytes(i, rng.NextBytes(maxStreamSize));

			for (var i = 0; i < 50; i++)
				streamContainer.Swap(i, 100 - i - 1);

			using var clonedStream = new MemoryStream(rootStream.ToArray());
			var loadedStreamContainer = new ClusteredStreamStorage(clonedStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };

			Assert.That(streamContainer.ToStringFullContents(), Is.EqualTo(loadedStreamContainer.ToStringFullContents()));
			
		}

		[Test]
		public void IntegrationTests([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy recordsCachePolicy, [Values(ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.Remember)] ClusteredStreamCachePolicy streamCachePolicy) {
			// NOTE: change DebugMode to True when trying to isolate error, else leave False when confirmed working (for faster evaluation)
			const bool DebugMode = false;
			const int StreamStreamOperations = 100;
			var rng = new Random(31337 + (int)recordsCachePolicy + (int)streamCachePolicy);
			var expectedStreams = new List<Stream>();
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamStorage(rootStream, clusterSize, recordsCachePolicy: recordsCachePolicy) { DefaultStreamPolicy = streamCachePolicy };
			// Iterates double (first leg adds/inserts streams, second leg removes)
			for (var i = 0; i < totalStreams * 2; i++) {
				if (i < totalStreams) {
					Stream expectedStream = new MemoryStream();
					Stream newStream;
					// Add/insert a new stream
					if (i % 2 == 0) {
						newStream = streamContainer.Add();
						expectedStreams.Add(expectedStream);
					} else {
						var insertIX = rng.Next(0, streamContainer.Count);
						expectedStreams.Insert(insertIX, expectedStream);
						newStream = streamContainer.Insert(insertIX);
					}

					// Run a test on it
					using (newStream) {
						AssertEx.StreamIntegrationTests(maxStreamSize, newStream, expectedStream, StreamStreamOperations, rng, runAsserts: DebugMode);
					}

				} else {
					// Remove a prior stream
					var randomPriorIX = rng.Next(0, expectedStreams.Count);
					expectedStreams[randomPriorIX].Dispose();
					expectedStreams.RemoveAt(randomPriorIX);
					streamContainer.Remove(randomPriorIX);
				}

				if (expectedStreams.Count > 0) {
					// Swap two existing streams
					var first = rng.Next(0, expectedStreams.Count);
					var second = rng.Next(0, expectedStreams.Count);
					expectedStreams.Swap(first, second);
					streamContainer.Swap(first, second);


					// Update a random prior stream
					var priorIX = rng.Next(0, expectedStreams.Count);
					var expectedUpdateStream = expectedStreams[priorIX];
					expectedUpdateStream.Position = 0; // reset expected stream marker to 0, since actual is reset on dispose
					using var actualUpdateStream = streamContainer.Open(priorIX);
					AssertEx.StreamIntegrationTests(maxStreamSize, actualUpdateStream, expectedUpdateStream, StreamStreamOperations, rng, runAsserts: DebugMode);
				}

				// Check all streams match (this will catch any errors, even when runAsserts is passed false above)
				for (var j = 0; j < expectedStreams.Count; j++)
					Assert.That(expectedStreams[j].ReadAll(), Is.EqualTo(streamContainer.ReadAll(j)).Using(ByteArrayEqualityComparer.Instance));
			}
			Debug.Assert(expectedStreams.Count == 0);
		}

		//[Test]
		//public void BigIntegrationTests() {
		//	IntegrationTests(1, 1000, 1000, ClusteredStreamCachePolicy.None, ClusteredStreamCachePolicy.None);
		//}
	}
}
