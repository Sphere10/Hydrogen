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
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

/// <remarks>
/// During dev, bugs seemed to occur when clusters linked in descending order.
/// write unit tests which directly scramble the cluster links, different patterns (descending, random, etc).
/// </remarks>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ClusteredStreamsTests : StreamPersistedCollectionTestsBase {


	[Test]
	public void CannotSwapOpenStream([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		using var stream = streams.Add();
		Assert.That(streams.Count, Is.EqualTo(2));
		stream.WriteBytes(new byte[] { 2, 2 });
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		Assert.That(() => streams.Swap(0, 1), Throws.Exception.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void TestCheckWipesOutOldData([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 32, policy: policy, autoLoad: true);
		using (streams.EnterAccessScope()) {
			streams.AddBytes(Tools.Array.Gen<byte>(64, 1));
			Assert.That(streams.Count, Is.EqualTo(1));
			Assert.That(streams.GetStreamDescriptor(0).StartCluster, Is.EqualTo(1));
			Assert.That(streams.GetStreamDescriptor(0).EndCluster, Is.EqualTo(2));
			Assert.That(streams.GetStreamDescriptor(0).Size, Is.EqualTo(64));
			Assert.That(streams.ClusterMap.Clusters.Count, Is.EqualTo(3));
			Assert.That(streams.ClusterMap.Clusters[0].Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
			Assert.That(streams.ClusterMap.Clusters[0].Next, Is.EqualTo(-1));
			Assert.That(streams.ClusterMap.Clusters[0].Prev, Is.EqualTo(-1));

			Assert.That(streams.ClusterMap.Clusters[1].Traits, Is.EqualTo(ClusterTraits.Start));
			Assert.That(streams.ClusterMap.Clusters[1].Next, Is.EqualTo(2));
			Assert.That(streams.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			Assert.That(streams.ClusterMap.Clusters[1].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			Assert.That(streams.ClusterMap.Clusters[2].Traits, Is.EqualTo(ClusterTraits.End));
			Assert.That(streams.ClusterMap.Clusters[2].Next, Is.EqualTo(0));
			Assert.That(streams.ClusterMap.Clusters[2].Prev, Is.EqualTo(1));
			Assert.That(streams.ClusterMap.Clusters[2].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			using (var stream = streams.OpenWrite(0)) {
				stream.SetLength(33);
			}

			Assert.That(streams.Count, Is.EqualTo(1));
			Assert.That(streams.GetStreamDescriptor(0).StartCluster, Is.EqualTo(1));
			Assert.That(streams.GetStreamDescriptor(0).EndCluster, Is.EqualTo(2));
			Assert.That(streams.GetStreamDescriptor(0).Size, Is.EqualTo(33));
			Assert.That(streams.ClusterMap.Clusters.Count, Is.EqualTo(3));
			Assert.That(streams.ClusterMap.Clusters[0].Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
			Assert.That(streams.ClusterMap.Clusters[0].Next, Is.EqualTo(-1));
			Assert.That(streams.ClusterMap.Clusters[0].Prev, Is.EqualTo(-1));

			Assert.That(streams.ClusterMap.Clusters[1].Traits, Is.EqualTo(ClusterTraits.Start));
			Assert.That(streams.ClusterMap.Clusters[1].Next, Is.EqualTo(2));
			Assert.That(streams.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			Assert.That(streams.ClusterMap.Clusters[1].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			Assert.That(streams.ClusterMap.Clusters[2].Traits, Is.EqualTo(ClusterTraits.End));
			Assert.That(streams.ClusterMap.Clusters[2].Next, Is.EqualTo(0));
			Assert.That(streams.ClusterMap.Clusters[2].Prev, Is.EqualTo(1));
			Assert.That(streams.ClusterMap.Clusters[2].Data, Is.EqualTo(((byte)1).ConcatWith(Tools.Array.Gen<byte>(31, 0)).ToArray()));

			StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		}
	}

	[Test]
	public void BugCase_1([Values(32)] int clusterSize, [Values(1)] int N, [Values(65)] int M, [Values(ClusteredStreamsPolicy.Debug)] ClusteredStreamsPolicy policy) {

		var actual = new List<byte[]>();
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using var stream = streams.Add();
			var data = Tools.Array.Gen<byte>(M, 1);
			actual.Add(data);
			stream.Write(data);
		}
		Assert.That(streams.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++) {
			var read = streams.ReadAll(i);
			Assert.That(read, Is.EqualTo(actual[i]));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void CheckStructure_1([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		using (var stream = streams.OpenWrite(0))
			stream.SetLength(0);

		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void CheckStructure_2([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		streams.AddBytes(new byte[] { 1 });
		using (var stream = streams.OpenWrite(0))
			stream.SetLength(0);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void CheckStructure_3([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		streams.AddBytes(new byte[] { 1 });
		using (var scope = streams.OpenWrite(1))
			scope.SetLength(0);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AlwaysRequiresLoad([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy);
		Assert.That(streams.RequiresLoad, Is.True);
	}

	[Test]
	public void LoadEmpty([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy);
	}

	[Test]
	public void ReloadEmpty([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new ClusteredStreams(clonedStream, 1, autoLoad: true);
		Assert.That(() => ClusterDiagnostics.VerifyClusters(loadedStreamContainer), Throws.Nothing);
	}


	[Test]
	public void AddEmpty([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streams.Count, Is.EqualTo(1));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add2Empty([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		using (var stream = streams.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streams.Count, Is.EqualTo(2));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddNull([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(null);
		using (var stream = streams.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streams.Count, Is.EqualTo(1));
	}

	[Test]
	public void AddManyEmpty([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using (var stream = streams.Add())
				Assert.That(stream.Length, Is.EqualTo(0));
		}
		Assert.That(streams.Count, Is.EqualTo(N));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void OpenEmpty([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (_ = streams.Add()) ;
		using (var stream = streams.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void SetEmpty_1([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add()) {
			stream.SetLength(0);
		}
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streams.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void SetEmpty_2([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add()) {
			stream.Write(new byte[] { 1 });
			stream.SetLength(0);
		}
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streams.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void SetEmpty_3([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add()) {
			stream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			stream.SetLength(0);
		}
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streams.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add1Byte([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add2x1Byte([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		streams.AddBytes(new byte[] { 1 });
		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add2ShrinkFirst_1b([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		streams.AddBytes(new byte[] { 2 });
		using (var stream = streams.OpenWrite(0))
			stream.SetLength(0);
		ClusterDiagnostics.VerifyClusters(streams);

		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.Empty);
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add2ShrinkFirst_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 1 });
		streams.AddBytes(new byte[] { 2, 2 });

		using (var stream = streams.OpenWrite(0))
			stream.SetLength(0);

		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.Empty);
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 2, 2 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Add2ShrinkSecond_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 1 });
		streams.AddBytes(new byte[] { 2, 2 });
		using (var stream = streams.OpenWrite(1))
			stream.SetLength(0);

		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 1, 1 }));
		Assert.That(streams.ReadAll(1), Is.Empty);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddNx1Byte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++)
			streams.AddBytes(new byte[] { 1 });

		Assert.That(streams.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++) {
			using (var stream = streams.OpenRead(i)) {
				var streamData = stream.ReadAll();
				Assert.That(streamData, Is.EqualTo(new byte[] { 1 }));
			}
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddNxMByte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var rng = new Random(31337);
		var actual = new List<byte[]>();
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using var stream = streams.Add();
			var data = rng.NextBytes(M);
			actual.Add(data);
			stream.Write(data);
		}
		Assert.That(streams.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++)
			Assert.That(streams.ReadAll(i), Is.EqualTo(actual[i]));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Insert1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.InsertBytes(0, new byte[] { 1 });
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Insert2x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.InsertBytes(0, new byte[] { 1 });
		streams.InsertBytes(0, new byte[] { 2 });
		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 2 }));
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Insert3x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.InsertBytes(0, new byte[] { 1 });
		streams.InsertBytes(0, new byte[] { 2 });
		streams.InsertBytes(0, new byte[] { 3 });
		Assert.That(streams.Count, Is.EqualTo(3));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 3 }));
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		Assert.That(streams.ReadAll(2), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Insert_BugCase() {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 32, autoLoad: true);
		streams.InsertBytes(0, new byte[] { 1 });
		streams.InsertBytes(0, Array.Empty<byte>());
		using (streams.EnterAccessScope()) {
			Assert.That(streams.ClusterMap.Clusters[1].Prev, Is.EqualTo(1));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Remove1b([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		streams.Remove(0);
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamsHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Remove2b([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 2 });
		streams.Remove(0);
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamsHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void Remove3b_Bug([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 2, 3 });
		streams.Remove(0);
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamsHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddString([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data = "Hello Stream!";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(dataBytes);
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(dataBytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void RemoveString([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(dataBytes);
		streams.Remove(0);
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamsHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void RemoveMiddle([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";

		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streams.AddBytes(Encoding.ASCII.GetBytes(data + $"{i}"));
		streams.Remove(50);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		Assert.That(streams.Count, Is.EqualTo(99));
		for (var i = 0; i < 99; i++) {
			var read = streams.ReadAll(i);
			Assert.That(read, Is.EqualTo(Encoding.ASCII.GetBytes(data + (i < 50 ? $"{i}" : $"{i + 1}"))));
			StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void RemoveLast([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streams.AddBytes(Encoding.ASCII.GetBytes(data + $"{i}"));
		streams.Remove(99);
		Assert.That(streams.Count, Is.EqualTo(99));
		for (var i = 0; i < 99; i++) {
			var read = streams.ReadAll(i);
			Assert.That(read, Is.EqualTo(Encoding.ASCII.GetBytes(data + $"{i}")));
			StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void UpdateWithSmallerStream([Values(1, 4, 32, 2048)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data1 = "Hello Stream! This is a long string which will be replaced by a smaller one.";
		const string data2 = "a";
		var data1Bytes = Encoding.ASCII.GetBytes(data1);
		var data2Bytes = Encoding.ASCII.GetBytes(data2);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add()) {
			stream.Write(data1Bytes);
			stream.SetLength(0);
			stream.Write(data2Bytes);
		}
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(data2Bytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void UpdateWithLargerStream([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data1 = "a";
		const string data2 = "Hello Stream! This is a long string which did replace a smaller one.";
		var data1Bytes = Encoding.ASCII.GetBytes(data1);
		var data2Bytes = Encoding.ASCII.GetBytes(data2);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streams.Add()) {
			stream.Write(data1Bytes);
			stream.SetLength(0);
			stream.Write(data2Bytes);
		}
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(data2Bytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddRemoveAllAddFirst([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streams.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streams.Remove(0);
		streams.Remove(0);
		streams.AddBytes(new byte[] { 9, 8, 7, 6, 5 });
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 9, 8, 7, 6, 5 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddTwoRemoveFirst([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streams.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streams.Remove(0);
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void AddTwoRemoveAndReAdd([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streams.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streams.Remove(0);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		Assert.That(streams.Count, Is.EqualTo(2));
		Assert.That(streams.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
		Assert.That(streams.ReadAll(1), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ClearTest_1() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streams.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ClearTest_2() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		streams.AddBytes(rng.NextBytes(100));
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streams.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ClearTest_3() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		streams.AddBytes(rng.NextBytes(100));
		streams.AddBytes(rng.NextBytes(100));
		streams.AddBytes(rng.NextBytes(100));
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streams.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ClearTest_Bug1() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		streams.AddBytes(rng.NextBytes(100));
		streams.AddBytes(rng.NextBytes(100));
		streams.AddBytes(rng.NextBytes(100));
		var listing0 = streams.GetStreamDescriptor(0); // force the cluster pointer in records fragment provider backwards
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));
		Assert.That(streams.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streams.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void TestRootStreamLengthConsistent([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 111;
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		Assert.That(rootStream.Length, Is.EqualTo(ClusteredStreamsHeader.ByteLength + 4 * (clusterSize + sizeof(byte) + sizeof(long) * 2)));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void TestClear([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void TestClear_2([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const string data = "Hello Stream!";
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.Clear();
		Assert.That(streams.Count, Is.EqualTo(0));

		var dataBytes = Encoding.ASCII.GetBytes(data);
		streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(dataBytes);
		Assert.That(streams.Count, Is.EqualTo(1));
		Assert.That(streams.ReadAll(0), Is.EqualTo(dataBytes));

		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void TestDanglingPrevOnFirstDataCluster([Values(1, 4, 32)] int clusterSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(rng.NextBytes(clusterSize * 2));
		streams.AddBytes(rng.NextBytes(clusterSize));
		streams.Remove(0);
		Assert.That(() => streams.ReadAll(0), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void ConsistentFastReadPrev([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streams.EnterAccessScope()) {
			for (var i = 0; i < streams.ClusterMap.Clusters.Count; i++) {
				var expected = streams.ClusterMap.Clusters[i].Prev;
				var actual = streams.ClusterMap.ReadClusterPrev(i);
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastReadNext([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streams.EnterAccessScope()) {
			for (var i = 0; i < streams.ClusterMap.Clusters.Count; i++) {
				var expected = streams.ClusterMap.Clusters[i].Next;
				var actual = streams.ClusterMap.ReadClusterNext(i);
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastWritePrev([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streams.EnterAccessScope()) {
			for (var i = 0; i < streams.ClusterMap.Clusters.Count; i++) {
				var expected = streams.ClusterMap.Clusters[i].Prev + 31337;
				streams.ClusterMap.WriteClusterPrev(i, expected);
				var actual = streams.ClusterMap.Clusters[i].Prev;
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastWriteNext([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streams.EnterAccessScope()) {
			for (var i = 0; i < streams.ClusterMap.Clusters.Count; i++) {
				var expected = streams.ClusterMap.Clusters[i].Next + 31337;
				streams.ClusterMap.WriteClusterNext(i, expected);
				var actual = streams.ClusterMap.Clusters[i].Next;
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void FastWriteClusterPrevDoesntCorrupt([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 1 });
		using (streams.EnterAccessScope()) {
			Assert.That(streams.ClusterMap.Clusters[0].Next, Is.EqualTo(1));
			Assert.That(streams.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			streams.ClusterMap.WriteClusterNext(0, 1); // corrupt root-stream by making cyclic dependency between clusters 9 an 10

			Assert.That(streams.ClusterMap.Clusters[0].Next, Is.EqualTo(1));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
	}

	[Test]
	public void CorruptData_ForwardsCyclicClusterChain([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		// corrupt root-stream, make tip cluster 18 have next to 10 creating a circular linked loop through forward traversal
		var nextOffset = rootStream.Length - clusterSize - ClusterSerializer.NextLength;
		var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness.LittleEndian), rootStream);
		rootStream.Seek(nextOffset, SeekOrigin.Begin);
		writer.Write(10L);
		Assert.That(() => streams.AppendBytes(0, new byte[] { 11 }), Throws.Exception);
	}

	[Test]
	public void CorruptData_PrevPointsNonExistentCluster([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		var firstStartCluster = streams.GetStreamDescriptor(0).StartCluster;
		using (streams.EnterAccessScope()) {
			streams.ClusterMap.WriteClusterPrev(firstStartCluster + 1, long.MaxValue);
			Assert.That(() => streams.Clear(0), Throws.Exception);
		}
		Assert.That(() => streams.Clear(0), Throws.Exception);
	}

	[Test]
	public void CorruptData_BackwardsCyclicClusterChainAtFirstData_Graceful([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
		streams.AddBytes(new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 });
		var firstStartCluster = streams.GetStreamDescriptor(0).StartCluster;
		var secondStartCluster = streams.GetStreamDescriptor(1).StartCluster;
		using (streams.EnterAccessScope()) {
			streams.ClusterMap.WriteClusterPrev(firstStartCluster, firstStartCluster + 1); // make start's previous point to next
			streams.ClusterMap.WriteClusterPrev(secondStartCluster, secondStartCluster + 1); // make start's previous point to next
		}
		Assert.That(() => streams.Clear(0), Throws.Exception);
		//Assert.That(() => streams.Clear(0), Throws.Nothing);
		// note: doesn't seem TraverseBack is ever called in fragment provider, so this error is seemingly inconsequential
	}

	[Test]
	public void CorruptData_BadHeaderVersion([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.VersionOffset;
		rootStream.WriteByte(2);

		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_Zero([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.ClusterSizeOffset;
		writer.Write(0);
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_TooLarge([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.ClusterSizeOffset;
		writer.Write(100);
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_TooBig([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.ClusterSizeOffset;
		writer.Write(clusterSize + 1);
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_TotalClusters_Zero([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.TotalClustersOffset;
		writer.Write(0);
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_TotalClusters_TooLarge([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.TotalClustersOffset;
		writer.Write(streams.ClusterMap.Clusters.Count + 1);
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_Records_TooSmall_HandlesGracefully([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.StreamCountOffset;
		writer.Write(streams.Count - 1);
		// note: Can't detect this scenario in integrity checks without examining data, so will
		// end up creating a corrupt data later. This is not ideal, but acceptable.
		Assert.That( () => ClusteredStreams.FromStream(rootStream), Throws.Nothing);
	}

	[Test]
	public void CorruptData_Records_TooLarge([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		streams.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = ClusteredStreamsHeader.StreamCountOffset;
		writer.Write((long)(streams.Count + 1));
		Assert.That(() => ClusteredStreams.FromStream(rootStream, autoLoad: true), Throws.InstanceOf<InvalidOperationException>());
	}

	[Test]
	public void LoadOneOneByteListing() {
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, 1, autoLoad: true);
		streams.AddBytes(new byte[] { 1 });
		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new ClusteredStreams(clonedStream, 1, autoLoad: true);
		Assert.That(ClusterDiagnostics.ToTextDump(streams), Is.EqualTo(ClusterDiagnostics.ToTextDump(loadedStreamContainer)));
	}

	[Test]
	public void LoadComplex([Values(1, 4, 32)] int clusterSize, [Values(0, 2, 4, 100)] int maxStreamSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var rng = new Random(31337 + (int)policy);
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streams.AddBytes(rng.NextBytes(maxStreamSize));

		for (var i = 0; i < 100; i++)
			streams.UpdateBytes(i, rng.NextBytes(maxStreamSize));

		for (var i = 0; i < 50; i++)
			streams.Swap(i, 100 - i - 1);

		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new ClusteredStreams(clonedStream, clusterSize, policy: policy, autoLoad: true);
		Assert.That(ClusterDiagnostics.ToTextDump(streams), Is.EqualTo(ClusterDiagnostics.ToTextDump(loadedStreamContainer)));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);

	}

	[Test]
	public void IntegrationTests_NoReservedRecords([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		IntegrationTestsInternal(clusterSize, totalStreams, maxStreamSize, 0, policy);
	}

	[Test]
	public void IntegrationTests_ReservedRecords([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 10)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize, [Values(1, 11, 111)] int reservedRecords,
												 [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		IntegrationTestsInternal(clusterSize, totalStreams, maxStreamSize, reservedRecords, policy);
	}

	public void IntegrationTestsInternal(int clusterSize, int totalStreams, int maxStreamSize, int reservedRecords, ClusteredStreamsPolicy policy) {
		// NOTE: change DebugMode to True when trying to isolate error, else leave False when confirmed working (for faster evaluation)
		const bool DebugMode = true;
		const int StreamStreamOperations = 100;
		var rng = new Random(31337 + (int)policy);
		var expectedStreams = new List<Stream>();
		using var rootStream = new MemoryStream();
		var streams = new ClusteredStreams(rootStream, clusterSize, reservedStreams: reservedRecords, policy: policy, autoLoad: true);

		// Populate reserved records
		for (var i = 0; i < reservedRecords; i++) {
			streams.UpdateBytes(i, Tools.Array.Gen(i, (byte)(i % 256)));
		}

		// Iterates double (first leg adds/inserts streams, second leg removes)
		for (var i = 0; i < totalStreams * 2; i++) {
			if (i < totalStreams) {
				Stream expectedStream = new MemoryStream();
				ClusteredStream newStream;
				// Add/insert a new stream
				if (i % 2 == 0) {
					newStream = streams.Add();
					expectedStreams.Add(expectedStream);
				} else {
					var insertIX = rng.Next(0, (int)streams.Count - reservedRecords);
					expectedStreams.Insert(insertIX, expectedStream);
					newStream = streams.Insert(insertIX + reservedRecords);
				}

				// Run a test on it
				using (newStream)
					AssertEx.StreamIntegrationTests(maxStreamSize, newStream, expectedStream, StreamStreamOperations, rng, runAsserts: DebugMode, extraTest: null /*() => ClusterDiagnostics.VerifyClusters(streams)*/);

			} else {
				// Remove a prior stream
				var randomPriorIX = rng.Next(0, expectedStreams.Count);
				expectedStreams[randomPriorIX].Dispose();
				expectedStreams.RemoveAt(randomPriorIX);
				streams.Remove(randomPriorIX + reservedRecords);
			}

			if (expectedStreams.Count > 0) {
				// Swap two existing streams
				var first = rng.Next(0, expectedStreams.Count);
				var second = rng.Next(0, expectedStreams.Count);
				expectedStreams.Swap(first, second);
				streams.Swap(first + reservedRecords, second + reservedRecords);

				// Update a random prior stream
				var priorIX = rng.Next(0, expectedStreams.Count);
				var expectedUpdateStream = expectedStreams[priorIX];
				expectedUpdateStream.Position = 0; // reset expected stream marker to 0, since actual is reset on dispose
				using var actualUpdateStream = streams.OpenWrite(priorIX + reservedRecords);

				AssertEx.StreamIntegrationTests(maxStreamSize, actualUpdateStream, expectedUpdateStream, StreamStreamOperations, rng, runAsserts: DebugMode, extraTest: null /*() => ClusterDiagnostics.VerifyClusters(streams)*/);

			}
			// Check all streams match (this will catch any errors, even when runAsserts is passed false above)
			for (var j = 0; j < expectedStreams.Count; j++) {
				Assert.That(expectedStreams[j].ReadAll(), Is.EqualTo(streams.ReadAll(j + reservedRecords)).Using(ByteArrayEqualityComparer.Instance));
			}

			StreamContainerTestsHelper.AssertValidStreamDescriptors(streams);
		}

		Debug.Assert(streams.Count == reservedRecords);

		// Check reserved records
		for (var i = 0; i < reservedRecords; i++) {
			Assert.That(streams.ReadAll(i), Is.EqualTo(Tools.Array.Gen(i, (byte)(i % 256))));
		}

	}

}

public static class StreamContainerTestsHelper {

	public static void AssertValidStreamDescriptors(ClusteredStreams streams) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		for (var i = 0; i < streams.Count; i++)
			AssertValidRecord(streams, i);
	}

	public static void AssertValidRecord(ClusteredStreams streams, int recordIndex) {
		using var _ = streams.EnterAccessScope();
		Guard.ArgumentNotNull(streams, nameof(streams));
		Guard.ArgumentLT(recordIndex, streams.Count, nameof(recordIndex));

		// Check all all record start/end markers match
		var record = streams.GetStreamDescriptor(recordIndex);
		Assert.That(record.Size, Is.GreaterThanOrEqualTo(0));
		if (record.Size == 0) {
			Assert.That(record.StartCluster, Is.EqualTo(Cluster.Null));
			Assert.That(record.EndCluster, Is.EqualTo(Cluster.Null));
		} else {
			Assert.That(streams.ClusterMap.ReadClusterPrev(record.StartCluster), Is.EqualTo(recordIndex), "Descriptor start cluster does not link to record");
			Assert.That(streams.ClusterMap.ReadClusterNext(record.EndCluster), Is.EqualTo(recordIndex), "Descriptor end cluster does not link to record");
			AssertClusterChainConsistency(streams, record.StartCluster, record.EndCluster);
		}
	}

	public static void AssertClusterChainConsistency(ClusteredStreams streams, long startCluster, long endCluster) {
		Assert.That(startCluster, Is.InRange(0, streams.ClusterMap.Clusters.Count - 1), "Start cluster doesn't exist");
		Assert.That(endCluster, Is.InRange(0, streams.ClusterMap.Clusters.Count - 1), "End cluster doesn't exist");
		var visited = new HashSet<long>();
		var cluster = startCluster;
		while (cluster != endCluster) {
			Assert.That(visited.Contains(cluster), Is.False);
			visited.Add(cluster);

			var clusterTraits = streams.ClusterMap.ReadClusterTraits(cluster);
			if (cluster == startCluster) {
				Assert.That(clusterTraits.HasFlag(ClusterTraits.Start), Is.True);
			} else if (cluster == endCluster) {
				Assert.That(clusterTraits.HasFlag(ClusterTraits.End), Is.True);
			} else {
				Assert.That(clusterTraits.HasFlag(ClusterTraits.Start), Is.False);
				Assert.That(clusterTraits.HasFlag(ClusterTraits.End), Is.False);
			}
			cluster = streams.ClusterMap.ReadClusterNext(cluster);
		}
	}
}
