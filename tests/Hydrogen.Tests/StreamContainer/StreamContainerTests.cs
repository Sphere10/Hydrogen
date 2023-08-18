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
using FluentAssertions;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

/// <remarks>
/// During dev, bugs seemed to occur when clusters linked in descending order.
/// write unit tests which directly scramble the cluster links, different patterns (descending, random, etc).
/// </remarks>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamContainerTests : StreamPersistedCollectionTestsBase {


	[Test]
	public void CannotSwapOpenStream([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		using var stream = streamContainer.Add();
		Assert.That(streamContainer.Count, Is.EqualTo(2));
		stream.WriteBytes(new byte[] { 2, 2 });
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		Assert.That(() => streamContainer.Swap(0, 1), Throws.Exception.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void TestCheckWipesOutOldData([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 32, policy: policy, autoLoad: true);
		using (streamContainer.EnterAccessScope()) {
			streamContainer.AddBytes(Tools.Array.Gen<byte>(64, 1));
			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.GetStreamDescriptor(0).StartCluster, Is.EqualTo(1));
			Assert.That(streamContainer.GetStreamDescriptor(0).EndCluster, Is.EqualTo(2));
			Assert.That(streamContainer.GetStreamDescriptor(0).Size, Is.EqualTo(64));
			Assert.That(streamContainer.ClusterMap.Clusters.Count, Is.EqualTo(3));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Next, Is.EqualTo(-1));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Prev, Is.EqualTo(-1));

			Assert.That(streamContainer.ClusterMap.Clusters[1].Traits, Is.EqualTo(ClusterTraits.Start));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Next, Is.EqualTo(2));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			Assert.That(streamContainer.ClusterMap.Clusters[2].Traits, Is.EqualTo(ClusterTraits.End));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Next, Is.EqualTo(0));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Prev, Is.EqualTo(1));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			using (var stream = streamContainer.OpenWrite(0)) {
				stream.SetLength(33);
			}

			Assert.That(streamContainer.Count, Is.EqualTo(1));
			Assert.That(streamContainer.GetStreamDescriptor(0).StartCluster, Is.EqualTo(1));
			Assert.That(streamContainer.GetStreamDescriptor(0).EndCluster, Is.EqualTo(2));
			Assert.That(streamContainer.GetStreamDescriptor(0).Size, Is.EqualTo(33));
			Assert.That(streamContainer.ClusterMap.Clusters.Count, Is.EqualTo(3));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Next, Is.EqualTo(-1));
			Assert.That(streamContainer.ClusterMap.Clusters[0].Prev, Is.EqualTo(-1));

			Assert.That(streamContainer.ClusterMap.Clusters[1].Traits, Is.EqualTo(ClusterTraits.Start));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Next, Is.EqualTo(2));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Data, Is.EqualTo(Tools.Array.Gen<byte>(32, 1)));

			Assert.That(streamContainer.ClusterMap.Clusters[2].Traits, Is.EqualTo(ClusterTraits.End));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Next, Is.EqualTo(0));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Prev, Is.EqualTo(1));
			Assert.That(streamContainer.ClusterMap.Clusters[2].Data, Is.EqualTo(((byte)1).ConcatWith(Tools.Array.Gen<byte>(31, 0)).ToArray()));

			StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		}
	}

	[Test]
	public void BugCase_1([Values(32)] int clusterSize, [Values(1)] int N, [Values(65)] int M, [Values(StreamContainerPolicy.Debug)] StreamContainerPolicy policy) {

		var actual = new List<byte[]>();
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using var stream = streamContainer.Add();
			var data = Tools.Array.Gen<byte>(M, 1);
			actual.Add(data);
			stream.Write(data);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++) {
			var read = streamContainer.ReadAll(i);
			Assert.That(read, Is.EqualTo(actual[i]));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void CheckStructure_1([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		using (var stream = streamContainer.OpenWrite(0))
			stream.SetLength(0);

		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void CheckStructure_2([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		streamContainer.AddBytes(new byte[] { 1 });
		using (var stream = streamContainer.OpenWrite(0))
			stream.SetLength(0);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void CheckStructure_3([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		streamContainer.AddBytes(new byte[] { 1 });
		using (var scope = streamContainer.OpenWrite(1))
			scope.SetLength(0);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AlwaysRequiresLoad([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy);
		Assert.That(streamContainer.RequiresLoad, Is.True);
	}

	[Test]
	public void LoadEmpty([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy);
	}

	[Test]
	public void ReloadEmpty([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new StreamContainer(clonedStream, 1, autoLoad: true);
		Assert.That(() => ClusterDiagnostics.ToTextDump(loadedStreamContainer), Throws.Nothing);
	}


	[Test]
	public void AddEmpty([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add2Empty([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		using (var stream = streamContainer.Add())
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streamContainer.Count, Is.EqualTo(2));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddNull([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(null);
		using (var stream = streamContainer.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		Assert.That(streamContainer.Count, Is.EqualTo(1));
	}

	[Test]
	public void AddManyEmpty([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using (var stream = streamContainer.Add())
				Assert.That(stream.Length, Is.EqualTo(0));
			System.Console.WriteLine(ClusterDiagnostics.ToTextDump(streamContainer));
		}
		Assert.That(streamContainer.Count, Is.EqualTo(N));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void OpenEmpty([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (_ = streamContainer.Add()) ;
		using (var stream = streamContainer.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void SetEmpty_1([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add()) {
			stream.SetLength(0);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streamContainer.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void SetEmpty_2([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add()) {
			stream.Write(new byte[] { 1 });
			stream.SetLength(0);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streamContainer.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void SetEmpty_3([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add()) {
			stream.Write(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
			stream.SetLength(0);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.GetStreamDescriptor(0).StartCluster, Is.EqualTo(-1));
		using (var stream = streamContainer.OpenRead(0))
			Assert.That(stream.Length, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add1Byte([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add2x1Byte([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		streamContainer.AddBytes(new byte[] { 1 });
		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add2ShrinkFirst_1b([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		streamContainer.AddBytes(new byte[] { 2 });
		using (var stream = streamContainer.OpenWrite(0))
			stream.SetLength(0);
		ClusterDiagnostics.VerifyClusters(streamContainer);

		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.Empty);
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add2ShrinkFirst_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 1 });
		streamContainer.AddBytes(new byte[] { 2, 2 });

		using (var stream = streamContainer.OpenWrite(0))
			stream.SetLength(0);

		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.Empty);
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2, 2 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Add2ShrinkSecond_2b([Values(1, 2, 3, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 1 });
		streamContainer.AddBytes(new byte[] { 2, 2 });
		using (var stream = streamContainer.OpenWrite(1))
			stream.SetLength(0);

		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1, 1 }));
		Assert.That(streamContainer.ReadAll(1), Is.Empty);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddNx1Byte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++)
			streamContainer.AddBytes(new byte[] { 1 });

		Assert.That(streamContainer.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++) {
			using (var stream = streamContainer.OpenRead(i)) {
				var streamData = stream.ReadAll();
				Assert.That(streamData, Is.EqualTo(new byte[] { 1 }));
			}
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddNxMByte([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		var rng = new Random(31337);
		var actual = new List<byte[]>();
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < N; i++) {
			using var stream = streamContainer.Add();
			var data = rng.NextBytes(M);
			actual.Add(data);
			stream.Write(data);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(N));
		for (var i = 0; i < N; i++)
			Assert.That(streamContainer.ReadAll(i), Is.EqualTo(actual[i]));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Insert1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.InsertBytes(0, new byte[] { 1 });
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Insert2x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.InsertBytes(0, new byte[] { 1 });
		streamContainer.InsertBytes(0, new byte[] { 2 });
		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 2 }));
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Insert3x1b([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int N, [Values(2, 4, 100)] int M, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.InsertBytes(0, new byte[] { 1 });
		streamContainer.InsertBytes(0, new byte[] { 2 });
		streamContainer.InsertBytes(0, new byte[] { 3 });
		Assert.That(streamContainer.Count, Is.EqualTo(3));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 3 }));
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 2 }));
		Assert.That(streamContainer.ReadAll(2), Is.EqualTo(new byte[] { 1 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Insert_BugCase() {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 32, autoLoad: true);
		streamContainer.InsertBytes(0, new byte[] { 1 });
		streamContainer.InsertBytes(0, Array.Empty<byte>());
		using (streamContainer.EnterAccessScope()) {
			Assert.That(streamContainer.ClusterMap.Clusters[1].Prev, Is.EqualTo(1));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Remove1b([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		streamContainer.Remove(0);
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(StreamContainerHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Remove2b([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 2 });
		streamContainer.Remove(0);
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(StreamContainerHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void Remove3b_Bug([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 2, 3 });
		streamContainer.Remove(0);
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(StreamContainerHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddString([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data = "Hello Stream!";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(dataBytes);
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(dataBytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void RemoveString([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(dataBytes);
		streamContainer.Remove(0);
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.ClusterMap.Clusters.Count, Is.EqualTo(0));
		Assert.That(rootStream.Length, Is.EqualTo(StreamContainerHeader.ByteLength));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void RemoveMiddle([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";

		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streamContainer.AddBytes(Encoding.ASCII.GetBytes(data + $"{i}"));
		streamContainer.Remove(50);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		Assert.That(streamContainer.Count, Is.EqualTo(99));
		for (var i = 0; i < 99; i++) {
			var read = streamContainer.ReadAll(i);
			Assert.That(read, Is.EqualTo(Encoding.ASCII.GetBytes(data + (i < 50 ? $"{i}" : $"{i + 1}"))));
			StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void RemoveLast([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data = "Hello Stream! This is a long sentence which should span various clusters. If it's too short, won't be a good test...";
		var dataBytes = Encoding.ASCII.GetBytes(data);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streamContainer.AddBytes(Encoding.ASCII.GetBytes(data + $"{i}"));
		streamContainer.Remove(99);
		Assert.That(streamContainer.Count, Is.EqualTo(99));
		for (var i = 0; i < 99; i++) {
			var read = streamContainer.ReadAll(i);
			Assert.That(read, Is.EqualTo(Encoding.ASCII.GetBytes(data + $"{i}")));
			StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void UpdateWithSmallerStream([Values(1, 4, 32, 2048)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data1 = "Hello Stream! This is a long string which will be replaced by a smaller one.";
		const string data2 = "a";
		var data1Bytes = Encoding.ASCII.GetBytes(data1);
		var data2Bytes = Encoding.ASCII.GetBytes(data2);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add()) {
			stream.Write(data1Bytes);
			stream.SetLength(0);
			stream.Write(data2Bytes);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void UpdateWithLargerStream([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data1 = "a";
		const string data2 = "Hello Stream! This is a long string which did replace a smaller one.";
		var data1Bytes = Encoding.ASCII.GetBytes(data1);
		var data2Bytes = Encoding.ASCII.GetBytes(data2);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		using (var stream = streamContainer.Add()) {
			stream.Write(data1Bytes);
			stream.SetLength(0);
			stream.Write(data2Bytes);
		}
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(data2Bytes));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddRemoveAllAddFirst([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streamContainer.Remove(0);
		streamContainer.Remove(0);
		streamContainer.AddBytes(new byte[] { 9, 8, 7, 6, 5 });
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 9, 8, 7, 6, 5 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddTwoRemoveFirst([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streamContainer.Remove(0);
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void AddTwoRemoveAndReAdd([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		streamContainer.AddBytes(new byte[] { 5, 6, 7, 8, 9 });
		streamContainer.Remove(0);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4 });
		Assert.That(streamContainer.Count, Is.EqualTo(2));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(new byte[] { 5, 6, 7, 8, 9 }));
		Assert.That(streamContainer.ReadAll(1), Is.EqualTo(new byte[] { 0, 1, 2, 3, 4 }));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void ClearTest_1() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streamContainer.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void ClearTest_2() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streamContainer.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void ClearTest_3() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streamContainer.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void ClearTest_Bug1() {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.AddBytes(rng.NextBytes(100));
		streamContainer.AddBytes(rng.NextBytes(100));
		var listing0 = streamContainer.GetStreamDescriptor(0); // force the cluster pointer in records fragment provider backwards
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		Assert.That(streamContainer.Header.StreamCount, Is.EqualTo(0));
		Assert.That(streamContainer.Header.TotalClusters, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void TestRootStreamLengthConsistent([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 111;
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		Assert.That(rootStream.Length, Is.EqualTo(StreamContainerHeader.ByteLength + 4 * (clusterSize + sizeof(byte) + sizeof(long) * 2)));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void TestClear([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void TestClear_2([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const string data = "Hello Stream!";
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.Clear();
		Assert.That(streamContainer.Count, Is.EqualTo(0));

		var dataBytes = Encoding.ASCII.GetBytes(data);
		streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(dataBytes);
		Assert.That(streamContainer.Count, Is.EqualTo(1));
		Assert.That(streamContainer.ReadAll(0), Is.EqualTo(dataBytes));

		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void TestDanglingPrevOnFirstDataCluster([Values(1, 4, 32)] int clusterSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		var rng = new Random(31337);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(rng.NextBytes(clusterSize * 2));
		streamContainer.AddBytes(rng.NextBytes(clusterSize));
		streamContainer.Remove(0);
		Assert.That(() => streamContainer.ReadAll(0), Throws.Nothing);
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void ConsistentFastReadPrev([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streamContainer.EnterAccessScope()) {
			for (var i = 0; i < streamContainer.ClusterMap.Clusters.Count; i++) {
				var expected = streamContainer.ClusterMap.Clusters[i].Prev;
				var actual = streamContainer.ClusterMap.ReadClusterPrev(i);
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastReadNext([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streamContainer.EnterAccessScope()) {
			for (var i = 0; i < streamContainer.ClusterMap.Clusters.Count; i++) {
				var expected = streamContainer.ClusterMap.Clusters[i].Next;
				var actual = streamContainer.ClusterMap.ReadClusterNext(i);
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastWritePrev([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streamContainer.EnterAccessScope()) {
			for (var i = 0; i < streamContainer.ClusterMap.Clusters.Count; i++) {
				var expected = streamContainer.ClusterMap.Clusters[i].Prev + 31337;
				streamContainer.ClusterMap.WriteClusterPrev(i, expected);
				var actual = streamContainer.ClusterMap.Clusters[i].Prev;
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void ConsistentFastWriteNext([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		using (streamContainer.EnterAccessScope()) {
			for (var i = 0; i < streamContainer.ClusterMap.Clusters.Count; i++) {
				var expected = streamContainer.ClusterMap.Clusters[i].Next + 31337;
				streamContainer.ClusterMap.WriteClusterNext(i, expected);
				var actual = streamContainer.ClusterMap.Clusters[i].Next;
				Assert.That(actual, Is.EqualTo(expected));
			}
		}
	}

	[Test]
	public void FastWriteClusterPrevDoesntCorrupt([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 1 });
		using (streamContainer.EnterAccessScope()) {
			Assert.That(streamContainer.ClusterMap.Clusters[0].Next, Is.EqualTo(1));
			Assert.That(streamContainer.ClusterMap.Clusters[1].Prev, Is.EqualTo(0));
			streamContainer.ClusterMap.WriteClusterNext(0, 1); // corrupt root-stream by making cyclic dependency between clusters 9 an 10

			Assert.That(streamContainer.ClusterMap.Clusters[0].Next, Is.EqualTo(1));
		}
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
	}

	[Test]
	public void CorruptData_ForwardsCyclicClusterChain([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		// corrupt root-stream, make tip cluster 18 have next to 10 creating a circular linked loop through forward traversal
		var nextOffset = rootStream.Length - clusterSize - ClusterSerializer.NextLength;
		var writer = new EndianBinaryWriter(EndianBitConverter.For(Endianness.LittleEndian), rootStream);
		rootStream.Seek(nextOffset, SeekOrigin.Begin);
		writer.Write(10L);
		Assert.That(() => streamContainer.AppendBytes(0, new byte[] { 11 }), Throws.Exception);
	}

	[Test]
	public void CorruptData_PrevPointsNonExistentCluster([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		var firstStartCluster = streamContainer.GetStreamDescriptor(0).StartCluster;
		using (streamContainer.EnterAccessScope()) {
			streamContainer.ClusterMap.WriteClusterPrev(firstStartCluster + 1, long.MaxValue);
			Assert.That(() => streamContainer.Clear(0), Throws.Exception);
		}
		Assert.That(() => streamContainer.Clear(0), Throws.Exception);
	}

	[Test]
	public void CorruptData_BackwardsCyclicClusterChainAtFirstData_Graceful([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		// make a 3 streams, corrupt middle back, should clear no problem
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
		streamContainer.AddBytes(new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 });
		var firstStartCluster = streamContainer.GetStreamDescriptor(0).StartCluster;
		var secondStartCluster = streamContainer.GetStreamDescriptor(1).StartCluster;
		using (streamContainer.EnterAccessScope()) {
			streamContainer.ClusterMap.WriteClusterPrev(firstStartCluster, firstStartCluster + 1); // make start's previous point to next
			streamContainer.ClusterMap.WriteClusterPrev(secondStartCluster, secondStartCluster + 1); // make start's previous point to next
		}
		Assert.That(() => streamContainer.Clear(0), Throws.Exception);
		//Assert.That(() => streamContainer.Clear(0), Throws.Nothing);
		// note: doesn't seem TraverseBack is ever called in fragment provider, so this error is seemingly inconsequential
	}

	[Test]
	public void CorruptData_BadHeaderVersion([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.VersionOffset;
		rootStream.WriteByte(2);

		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_Zero([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.ClusterSizeOffset;
		writer.Write(0);
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_TooLarge([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.ClusterSizeOffset;
		writer.Write(100);
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_BadClusterSize_TooBig([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.ClusterSizeOffset;
		writer.Write(clusterSize + 1);
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_TotalClusters_Zero([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.TotalClustersOffset;
		writer.Write(0);
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_TotalClusters_TooLarge([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.TotalClustersOffset;
		writer.Write(streamContainer.ClusterMap.Clusters.Count + 1);
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.TypeOf<InvalidOperationException>());
	}

	[Test]
	public void CorruptData_Records_TooSmall_HandlesGracefully([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.StreamCountOffset;
		writer.Write(streamContainer.Count - 1);
		// note: Can't detect this scenario in integrity checks without examining data, so will
		// end up creating a corrupt data later. This is not ideal, but acceptable.
		Assert.That( () => StreamContainer.FromStream(rootStream), Throws.Nothing);
	}

	[Test]
	public void CorruptData_Records_TooLarge([StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		const int clusterSize = 1;
		using var rootStream = new MemoryStream();
		var writer = new EndianBinaryWriter(EndianBitConverter.Little, rootStream);
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
		rootStream.Position = StreamContainerHeader.StreamCountOffset;
		writer.Write((long)(streamContainer.Count + 1));
		Assert.That(() => StreamContainer.FromStream(rootStream, autoLoad: true), Throws.InstanceOf<InvalidOperationException>());
	}

	[Test]
	public void LoadOneOneByteListing() {
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, 1, autoLoad: true);
		streamContainer.AddBytes(new byte[] { 1 });
		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new StreamContainer(clonedStream, 1, autoLoad: true);
		Assert.That(ClusterDiagnostics.ToTextDump(streamContainer), Is.EqualTo(ClusterDiagnostics.ToTextDump(loadedStreamContainer)));
	}

	[Test]
	public void LoadComplex([Values(1, 4, 32)] int clusterSize, [Values(0, 2, 4, 100)] int maxStreamSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		var rng = new Random(31337 + (int)policy);
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, policy: policy, autoLoad: true);
		for (var i = 0; i < 100; i++)
			streamContainer.AddBytes(rng.NextBytes(maxStreamSize));

		for (var i = 0; i < 100; i++)
			streamContainer.UpdateBytes(i, rng.NextBytes(maxStreamSize));

		for (var i = 0; i < 50; i++)
			streamContainer.Swap(i, 100 - i - 1);

		using var clonedStream = new MemoryStream(rootStream.ToArray());
		var loadedStreamContainer = new StreamContainer(clonedStream, clusterSize, policy: policy, autoLoad: true);
		Assert.That(ClusterDiagnostics.ToTextDump(streamContainer), Is.EqualTo(ClusterDiagnostics.ToTextDump(loadedStreamContainer)));
		StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);

	}

	[Test]
	public void IntegrationTests_NoReservedRecords([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 100)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize, [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		IntegrationTestsInternal(clusterSize, totalStreams, maxStreamSize, 0, policy);
	}

	[Test]
	public void IntegrationTests_ReservedRecords([Values(1, 4, 32)] int clusterSize, [Values(1, 2, 10)] int totalStreams, [Values(0, 2, 4, 100)] int maxStreamSize, [Values(1, 11, 111)] int reservedRecords,
												 [StreamContainerPolicyTestValues] StreamContainerPolicy policy) {
		IntegrationTestsInternal(clusterSize, totalStreams, maxStreamSize, reservedRecords, policy);
	}

	public void IntegrationTestsInternal(int clusterSize, int totalStreams, int maxStreamSize, int reservedRecords, StreamContainerPolicy policy) {
		// NOTE: change DebugMode to True when trying to isolate error, else leave False when confirmed working (for faster evaluation)
		const bool DebugMode = true;
		const int StreamStreamOperations = 100;
		var rng = new Random(31337 + (int)policy);
		var expectedStreams = new List<Stream>();
		using var rootStream = new MemoryStream();
		var streamContainer = new StreamContainer(rootStream, clusterSize, reservedStreams: reservedRecords, policy: policy, autoLoad: true);

		// Populate reserved records
		for (var i = 0; i < reservedRecords; i++) {
			streamContainer.UpdateBytes(i, Tools.Array.Gen(i, (byte)(i % 256)));
		}

		// Iterates double (first leg adds/inserts streams, second leg removes)
		for (var i = 0; i < totalStreams * 2; i++) {
			if (i < totalStreams) {
				Stream expectedStream = new MemoryStream();
				ClusteredStream newStream;
				// Add/insert a new stream
				if (i % 2 == 0) {
					newStream = streamContainer.Add();
					expectedStreams.Add(expectedStream);
				} else {
					var insertIX = rng.Next(0, (int)streamContainer.Count - reservedRecords);
					expectedStreams.Insert(insertIX, expectedStream);
					newStream = streamContainer.Insert(insertIX + reservedRecords);
				}

				// Run a test on it
				using (newStream)
					AssertEx.StreamIntegrationTests(maxStreamSize, newStream, expectedStream, StreamStreamOperations, rng, runAsserts: DebugMode, extraTest: null /*() => ClusterDiagnostics.VerifyClusters(streamContainer)*/);

			} else {
				// Remove a prior stream
				var randomPriorIX = rng.Next(0, expectedStreams.Count);
				expectedStreams[randomPriorIX].Dispose();
				expectedStreams.RemoveAt(randomPriorIX);
				streamContainer.Remove(randomPriorIX + reservedRecords);
			}

			if (expectedStreams.Count > 0) {
				// Swap two existing streams
				var first = rng.Next(0, expectedStreams.Count);
				var second = rng.Next(0, expectedStreams.Count);
				expectedStreams.Swap(first, second);
				streamContainer.Swap(first + reservedRecords, second + reservedRecords);

				// Update a random prior stream
				var priorIX = rng.Next(0, expectedStreams.Count);
				var expectedUpdateStream = expectedStreams[priorIX];
				expectedUpdateStream.Position = 0; // reset expected stream marker to 0, since actual is reset on dispose
				using var actualUpdateStream = streamContainer.OpenWrite(priorIX + reservedRecords);

				AssertEx.StreamIntegrationTests(maxStreamSize, actualUpdateStream, expectedUpdateStream, StreamStreamOperations, rng, runAsserts: DebugMode, extraTest: null /*() => ClusterDiagnostics.VerifyClusters(streamContainer)*/);

			}
			// Check all streams match (this will catch any errors, even when runAsserts is passed false above)
			for (var j = 0; j < expectedStreams.Count; j++) {
				Assert.That(expectedStreams[j].ReadAll(), Is.EqualTo(streamContainer.ReadAll(j + reservedRecords)).Using(ByteArrayEqualityComparer.Instance));
			}

			StreamContainerTestsHelper.AssertValidStreamDescriptors(streamContainer);
		}

		Debug.Assert(streamContainer.Count == reservedRecords);

		// Check reserved records
		for (var i = 0; i < reservedRecords; i++) {
			Assert.That(streamContainer.ReadAll(i), Is.EqualTo(Tools.Array.Gen(i, (byte)(i % 256))));
		}

	}

}

public static class StreamContainerTestsHelper {

	public static void AssertValidStreamDescriptors(StreamContainer streams) {
		Guard.ArgumentNotNull(streams, nameof(streams));
		for (var i = 0; i < streams.Count; i++)
			AssertValidRecord(streams, i);
	}

	public static void AssertValidRecord(StreamContainer streams, int recordIndex) {
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

	public static void AssertClusterChainConsistency(StreamContainer streams, long startCluster, long endCluster) {
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
