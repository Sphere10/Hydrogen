// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ClusteredStreamsConcurrentStreamsTests : StreamPersistedCollectionTestsBase {

	[Test]
	public void ConcurrentStreams_OpenRead_SameTwice_Throws([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 5, policy: policy, autoLoad: true);
		var rng = new Random();
		streams.AddBytes(rng.NextBytes(100));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.OpenRead(0), Throws.InvalidOperationException);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}
	
	[Test]
	public void ConcurrentStreams_OpenRead_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.OpenRead(1), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_OpenRead_OpenWrite() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.OpenWrite(1), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_OpenRead_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
	}

	[Test]
	public void ConcurrentStreams_OpenRead_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	[Test]
	public void ConcurrentStreams_OpenRead_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.Remove(0), Throws.Exception.InstanceOf<InvalidOperationException>());
	}

	[Test]
	public void ConcurrentStreams_OpenRead_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.OpenRead(0);
		Assert.That(() => streams.Clear(), Throws.Exception.InstanceOf<InvalidOperationException>());
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_Add_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Add();
		Assert.That(() => streams.OpenRead(0), Throws.Nothing);
	}


	[Test]
	public void ConcurrentStreams_Add_OpenWrite() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Add();
		Assert.That(() => streams.OpenWrite(0), Throws.Nothing);
	}

	[Test]
	public void ConcurrentStreams_Add_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Add();
		Assert.That(() => streams.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_Add_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Add();
		Assert.That(() => streams.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	[Test]
	public void ConcurrentStreams_Add_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		using var _ = streams.Add();
		Assert.That(() => streams.Remove(0), Throws.InvalidOperationException);
	}

	[Test]
	public void ConcurrentStreams_Add_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Add();
		Assert.That(() => streams.Clear(), Throws.InvalidOperationException);
	}

	[Test]
	public void ConcurrentStreams_Insert_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Insert(0);
		Assert.That(() => streams.OpenRead(1), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_Insert_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Insert(0);
		Assert.That(() => streams.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConcurrentStreams_Insert_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Insert(0);
		Assert.That(() => streams.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	
	[Test]
	public void ConcurrentStreams_Insert_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Insert(0);
		Assert.That(() => streams.Remove(0), Throws.InvalidOperationException);
	}

	[Test]
	public void ConcurrentStreams_Insert_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, autoLoad: true);
		streams.AddBytes(rng.NextBytes(1000));
		using var _ = streams.Insert(0); 
		Assert.That(() => streams.Clear(), Throws.InvalidOperationException);
	}

	[Test]
	public void CusterSeeker_2ClusterChain_Becomes1([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using var stream1 = streams.Add();
		using var stream2 = streams.Add();
		stream1.WriteByte(1);
		stream1.WriteByte(1);
		stream2.WriteByte(2);
		stream1.SetLength(1);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);

	}


	[Test]
	public void ConcurrentStreams_IntegrationTest([Values(1, 5, 11, 33, 1000)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		// This test will open multiple streams and read/write to them randomly until they're all filled.
		// Then it will reverse until empty.
		// Then again until filled.
		// Then will verify data is as expected.

		const int Streams = 100;
		const int Length = 10000;
		const int GrowChunkSize = 100;
		const int ShrinkChunkSize = GrowChunkSize / 2;

		using var rootStream = new MemoryStream();
		var clusteredStreams = new ClusteredStreams(rootStream, policy: policy, autoLoad: true);
		var rng = new Random(31337 + (int)policy); 
		var streams = new ClusteredStream[Streams];

		// add the streams
		for (var i = 0; i < Streams; i++) {
			streams[i] = clusteredStreams.Add();
		}

		MutateStreamsRandomlyUntilAllFilled();
		MutateStreamsRandomlyUntilAllEmpty();
		MutateStreamsRandomlyUntilAllFilled();
		
		// dispose all the streams
		for (var i = 0; i < Streams; i++) {
			streams[i].Dispose();
		}

		// Verify all the data
		for (var i = 0; i < Streams; i++) {
			var actual = clusteredStreams.ReadAll(i);
			var expected = Enumerable.Range(0, Length).Select(x => x * i % 256).Select(x => (byte)x).ToArray();
			Assert.That(actual, Is.EqualTo(expected));
		}


		void MutateStreamsRandomlyUntilAllFilled() {
			var finished = false;
			while (!finished) {
				streams.Select((stream, i) => (stream, i)).ForEach( x => Process(x.i, x.stream, true));
				finished = streams.All(x => x.Length == Length);
			}
		}

		void MutateStreamsRandomlyUntilAllEmpty() {
			var finished = false;
			while (!finished) {
				streams.Select((stream, i) => (stream, i)).ForEach( x => Process(x.i, x.stream, false));
				finished = streams.All(x => x.Length == 0);
			}
		}

		void Process(int index, Stream stream, bool grow) {
			if (grow ? stream.Length < Length : stream.Length > 0) {
				switch (rng.Next(0, 3)) {
					case 0: 
						ReadRandomChunk(stream); 
						break;
					case 1: 
						WriteRandomChunk(index, stream, grow); 
						break;
					case 2: 
						RemoveRandomChunk(index, stream, grow); 
						break;
					default: 
						throw new Exception("Unexpected");
				}
			}
		}

		void ReadRandomChunk(Stream stream) {
			if (stream.Length > 0) {
				var ix1 = rng.Next(0, (int)stream.Length);
				var ix2 = rng.Next(0, (int)stream.Length);
				var from = Math.Min(ix1, ix2);
				var to = Math.Max(ix1, ix2);
				var len = to - from + 1;
				if (len > 0) {
					stream.Seek(from, SeekOrigin.Begin);
					var readData = stream.ReadBytes(len);
				}
			}
		}

		void WriteRandomChunk(int index, Stream stream, bool grow) {
			var remaining = Length - stream.Length;
			var chunkSize = rng.Next(0, (int)Math.Min(remaining, grow ? GrowChunkSize : ShrinkChunkSize) + 1); // add/remove half of what we normally add (so test finishes eventually)
			var indexesOfData = Enumerable.Range((int)stream.Length, chunkSize);
			var writeData = indexesOfData.Select(x => x*index % 256).Select(x => (byte)x).ToArray();
			stream.Seek(0, SeekOrigin.End);
			stream.Write(writeData);
		}

		void RemoveRandomChunk(int index, Stream stream, bool grow) {
			var removeSize = rng.Next(0, (grow ? ShrinkChunkSize : GrowChunkSize) + 1);  // add/remove half of what we normally add (so test finishes eventually)
			var newLen = (int)Math.Max(0, stream.Length - removeSize);
			stream.SetLength(newLen);
		}
		
	}
}
