using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class ClusteredStreamContainerTests {

		[Test]
		public void AddEmpty([Values(1,4,32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) 
				Assert.That(stream.Length, Is.EqualTo(0));
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Listings.Count, Is.EqualTo(1));
		}

		[Test]
		public void AddNEmpty([Values(1, 4, 32)] int clusterSize, [Values(1,2,100)] int N) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			for(var i = 0; i < N; i++)
				using (var stream = streamContainer.Add())
					Assert.That(stream.Length, Is.EqualTo(0));
			Assert.That(streamContainer.Count, Is.EqualTo(N));
			Assert.That(streamContainer.Listings.Count, Is.EqualTo(N));
		}

		[Test]
		public void OpenEmpty([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (_ = streamContainer.Add());
			using (var stream = streamContainer.Open(0)) 
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_1([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) {
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Listings.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Listings[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_2([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) {
				stream.Write(new byte[] { 1 });
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Listings.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Listings[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void SetEmpty_3([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) {
				stream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
				stream.SetLength(0);
			}
			Assert.That(streamContainer.Listings.Count, Is.EqualTo(1));
			Assert.That(streamContainer.Listings[0].StartCluster, Is.EqualTo(-1));
			using (var stream = streamContainer.Open(0))
				Assert.That(stream.Length, Is.EqualTo(0));
		}

		[Test]
		public void Add1Byte([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1 } );
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		}

		[Test]
		public void Add2ShrinkFirst_1b([Values(1, 2, 3, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1 });
			streamContainer.CreateAllBytes(new byte[] { 2 });
			using (var stream = streamContainer.Open(0))
				stream.SetLength(0);

			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.Empty);
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		}

		[Test]
		public void Add2ShrinkFirst_2b([Values(1, 2, 3, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1,1 });
			streamContainer.CreateAllBytes(new byte[] { 2, 2 });

			using (var stream = streamContainer.Open(0))
				stream.SetLength(0);
			
			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.Empty);
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2,2 }));
		}

		[Test]
		public void Add2ShrinkSecond_2b([Values(1, 2, 3, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1, 1 });
			streamContainer.CreateAllBytes(new byte[] { 2, 2 });
			using (var stream = streamContainer.Open(1))
				stream.SetLength(0);

			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1, 1 }));
			Assert.That(streamContainer.ReadAll(1), Is.Empty);
		}

		[Test]
		public void AddNx1Byte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			for (var i = 0; i < N; i++) 
				streamContainer.CreateAllBytes(new byte[] { 1 });

			Assert.That(streamContainer.Count, Is.EqualTo(N));
			for (var i = 0; i < N; i++) {
				var streamData = streamContainer.Open(i).ReadAllAndDispose();
				Assert.That(streamData, Is.EqualTo(new byte[] { 1 }));
			}
		}

		[Test]
		public void AddNxMByte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2,4,100)]int M) {
			var rng = new Random(31337);
			var actual = new List<byte[]>();
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
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
		public void Remove1b([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamContainerHeader.ByteLength));
		}

		[Test]
		public void Remove2b([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 1, 2 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamContainerHeader.ByteLength));
		}

		[Test]
		public void Remove3b_Bug() {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, 1);
			streamContainer.CreateAllBytes(new byte[] { 1, 2, 3 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamContainerHeader.ByteLength));
		}

		[Test]
		public void AddString([Values(1, 4, 32)] int clusterSize) {
			const string data = "Hello Stream!";
			var dataBytes = Encoding.ASCII.GetBytes(data);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(dataBytes);
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(dataBytes));
		}

		[Test]
		public void RemoveString([Values(1, 4, 32)] int clusterSize) {
			const string data = "Hello Stream!";
			var dataBytes = Encoding.ASCII.GetBytes(data);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(dataBytes);
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(0));
			Assert.That(streamContainer.Clusters, Is.EqualTo(0));
			Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamContainerHeader.ByteLength));
		}

		[Test]
		public void UpdateWithSmallerStream([Values(1, 4, 32, 2048)] int clusterSize) {
			const string data1 = "Hello Stream! This is a long string which will be replaced by a smaller one.";
			const string data2 = "a";
			var data1Bytes = Encoding.ASCII.GetBytes(data1);
			var data2Bytes = Encoding.ASCII.GetBytes(data2);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) {
				stream.Write(data1Bytes);
				stream.SetLength(0);
				stream.Write(data2Bytes);
			}
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		}

		[Test]
		public void UpdateWithLargerStream([Values(1, 4, 32)] int clusterSize) {
			const string data1 = "a";
			const string data2 = "Hello Stream! This is a long string which did replace a smaller one.";
			var data1Bytes = Encoding.ASCII.GetBytes(data1);
			var data2Bytes = Encoding.ASCII.GetBytes(data2);
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			using (var stream = streamContainer.Add()) {
				stream.Write(data1Bytes);
				stream.SetLength(0);
				stream.Write(data2Bytes);
			}
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		}

		[Test]
		public void AddTwoRemoveFirst([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 0,1,2,3,4 });
			streamContainer.CreateAllBytes(new byte[] { 5, 6, 7, 8, 9 });
			streamContainer.Remove(0);
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5,6,7,8,9 }));
		}

		[Test]
		public void AddTwoRemoveAndReAdd([Values(1, 4, 32)] int clusterSize) {
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			streamContainer.CreateAllBytes(new byte[] { 0, 1, 2, 3, 4 });
			streamContainer.CreateAllBytes(new byte[] { 5, 6, 7, 8, 9 });
			streamContainer.Remove(0);
			streamContainer.CreateAllBytes(new byte[] { 0, 1, 2, 3, 4 });
			Assert.That(streamContainer.Count, Is.EqualTo(2));
			Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
			Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }));
		}


		// Corrupt Date Tests: inifinite loop (detect by max number of walks check in TryTraverse
		// Next
		// Prev
		// RandomCorruptData

		[Test]
		public void CircularNext() {
			const int clusterSize = 1;
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			

		}

		[Test]
		public void IntegrationTests([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize) {
			const int StreamStreamOperations = 100;
			var rng = new Random(31337);
			var expectedStreams = new List<Stream>();
			using var rootStream = new MemoryStream();
			var streamContainer = new ClusteredStreamContainer(rootStream, clusterSize);
			for (var i = 0; i < totalStreams * 2; i++) {
				if (i < totalStreams) {
					// Add a stream
					using (var actualStream = streamContainer.Add()) {
						var expectedStream = new MemoryStream();
						expectedStreams.Add(expectedStream);
						// Run a test on it
						AssertEx.StreamIntegrationTests(maxStreamSize, actualStream, expectedStream, StreamStreamOperations, rng);
					}
				} else {
					// Remove a prior stream
					var randomPriorIX = rng.Next(0, expectedStreams.Count);
					expectedStreams.RemoveAt(randomPriorIX);
					streamContainer.Remove(randomPriorIX);
				}

				// Update a random prior stream
				if (expectedStreams.Count > 0) {
					var priorIX = rng.Next(0, expectedStreams.Count);
					var expectedUpdateStream = expectedStreams[priorIX];
					expectedUpdateStream.Position = 0; // reset expected stream marker to 0, since actual is reset on dispose
					using var actualUpdateStream = streamContainer.Open(priorIX);
					AssertEx.StreamIntegrationTests(maxStreamSize, actualUpdateStream, expectedUpdateStream, StreamStreamOperations, rng);
				}
			}
			expectedStreams.ForEach(x => x.Dispose());
		}


		
	}
}
