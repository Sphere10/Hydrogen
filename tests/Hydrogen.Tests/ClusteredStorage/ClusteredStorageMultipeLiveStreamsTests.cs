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

/// <remarks>
/// During dev, bugs seemed to occur when clusters linked in descending order.
/// write unit tests which directly scramble the cluster links, different patterns (descending, random, etc).
/// </remarks>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ClusteredStorageMultipeLiveStreamsTests : StreamPersistedCollectionTestsBase {

	[Test]
	public void MultipleLiveStreams_OpenRead_SameTwice_Throws([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, 5, policy: policy, autoLoad: true);
		var rng = new Random();
		streamContainer.AddBytes(rng.NextBytes(100));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.OpenRead(0), Throws.InvalidOperationException);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}
	
	[Test]
	public void MultipleLiveStreams_OpenRead_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.OpenRead(1), Throws.Nothing);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_OpenRead_OpenWrite() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.OpenWrite(1), Throws.Nothing);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_OpenRead_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
	}

	[Test]
	public void MultipleLiveStreams_OpenRead_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	[Test]
	public void MultipleLiveStreams_OpenRead_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.Remove(0), Throws.Exception.InstanceOf<LockRecursionException>());
	}

	[Test]
	public void MultipleLiveStreams_OpenRead_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.OpenRead(0);
		Assert.That(() => streamContainer.Clear(), Throws.Exception.InstanceOf<InvalidOperationException>());
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_Add_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.OpenRead(0), Throws.Nothing);
	}


	[Test]
	public void MultipleLiveStreams_Add_OpenWrite() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.OpenWrite(0), Throws.Nothing);
	}

	[Test]
	public void MultipleLiveStreams_Add_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_Add_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	[Test]
	public void MultipleLiveStreams_Add_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.Remove(0), Throws.InvalidOperationException);
	}

	[Test]
	public void MultipleLiveStreams_Add_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Add();
		Assert.That(() => streamContainer.Clear(), Throws.InvalidOperationException);
	}

	[Test]
	public void MultipleLiveStreams_Insert_OpenRead() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Insert(0);
		Assert.That(() => streamContainer.OpenRead(1), Throws.Nothing);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_Insert_Add() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Insert(0);
		Assert.That(() => streamContainer.AddBytes(rng.NextBytes(1000)), Throws.Nothing);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);
	}

	[Test]
	public void MultipleLiveStreams_Insert_Insert_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Insert(0);
		Assert.That(() => streamContainer.InsertBytes(0, rng.NextBytes(1000)), Throws.InvalidOperationException);
	}

	
	[Test]
	public void MultipleLiveStreams_Insert_Remove_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Insert(0);
		Assert.That(() => streamContainer.Remove(0), Throws.InvalidOperationException);
	}

	[Test]
	public void MultipleLiveStreams_Insert_Clear_Throws() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(1000));
		using var _ = streamContainer.Insert(0); 
		Assert.That(() => streamContainer.Clear(), Throws.InvalidOperationException);
	}

	[Test]
	public void CusterSeeker_2ClusterChain_Becomes1([ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, clusterSize, policy: policy, autoLoad: true);
		using var scope1 = streamContainer.Add();
		using var scope2 = streamContainer.Add();
		scope1.Stream.WriteByte(1);
		scope1.Stream.WriteByte(1);
		scope2.Stream.WriteByte(2);
		scope1.Stream.SetLength(1);
		ClusteredStorageTestsHelper.AssertValidRecords(streamContainer);

	}


	[Test]
	public void MultipleLiveStreams_IntegrationTest([Values(1, 5, 11, 33, 1000)] int clusterSize, [ClusteredStoragePolicyTestValues] ClusteredStoragePolicy policy) {
		// This test will open multiple streams and read/write to them randomly until they're all filled.
		const int Streams = 100;
		const int Length = 10000;
		const int GrowChunkSize = 100;
		const int ShrinkChunkSize = GrowChunkSize / 2;

		using var rootStream = new MemoryStream();
		var streamContainer = new ClusteredStorage(rootStream, policy: policy, autoLoad: true);
		var rng = new Random(31337 + (int)policy); 
		var scopes = new ClusteredStreamScope[Streams];

		// add the streams
		for (var i = 0; i < Streams; i++) {
			scopes[i] = streamContainer.Add();
		}

		MutateStreamUntilAllFilled();
		MutateStreamsUntilAllEmpty();
		MutateStreamUntilAllFilled();
		
		// dispose all the scopes
		for (var i = 0; i < Streams; i++) {
			scopes[i].Dispose();
		}

		// Verify all the data
		for (var i = 0; i < Streams; i++) {
			var actual = streamContainer.ReadAll(i);
			var expected = Enumerable.Range(0, Length).Select(x => x * i % 256).Select(x => (byte)x).ToArray();
			Assert.That(actual, Is.EqualTo(expected));
		}


		void MutateStreamUntilAllFilled() {
			var finished = false;
			while (!finished) {
				scopes.Select((x, i) => (x.Stream, i)).ForEach( x => Process(x.i, x.Stream, true));
				finished = scopes.All(x => x.Stream.Length == Length);
			}
		}

		void MutateStreamsUntilAllEmpty() {
			var finished = false;
			while (!finished) {
				scopes.Select((x, i) => (x.Stream, i)).ForEach( x => Process(x.i, x.Stream, false));
				finished = scopes.All(x => x.Stream.Length == 0);
			}
		}

		void Process(int index, Stream stream, bool grow) {
			if (grow ? stream.Length < Length : stream.Length > 0) {
				switch (rng.Next(0, 3)) {
					case 0: 
						ReadRandomChunk(index, stream); 
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

		void ReadRandomChunk(int index, Stream stream) {
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
			var chunkSize = rng.Next(0, (int)Math.Min(remaining, grow ? GrowChunkSize : ShrinkChunkSize) + 1);
			var indexesOfData = Enumerable.Range((int)stream.Length, chunkSize);
			var writeData = indexesOfData.Select(x => x*index % 256).Select(x => (byte)x).ToArray();
			stream.Seek(0, SeekOrigin.End);
			stream.Write(writeData);
		}

		void RemoveRandomChunk(int index, Stream stream, bool grow) {
			var removeSize = rng.Next(0, (grow ? ShrinkChunkSize : GrowChunkSize) + 1);  // remove half of what we normally add (so test finishes eventually)
			var newLen = (int)Math.Max(0, stream.Length - removeSize);
			stream.SetLength(newLen);
		}
		
	}
}
