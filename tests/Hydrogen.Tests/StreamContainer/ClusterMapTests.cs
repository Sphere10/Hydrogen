// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Hydrogen.Tests;

/// <remarks>
/// During dev, bugs seemed to occur when clusters linked in descending order.
/// write unit tests which directly scramble the cluster links, different patterns (descending, random, etc).
/// </remarks>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ClusterMapTests {

	public enum ClusterMapImplementationType {
		InMemory,
		StreamMappedNoCache,
		StreamMappedCached
	}

	private IDisposable Create(ClusterMapImplementationType implementationType, int clusterSize, out ClusterMap clusterMap) {
		switch (implementationType) {
			case ClusterMapImplementationType.InMemory:
				clusterMap = new InMemoryClusterMap(new ExtendedList<Cluster>(), clusterSize);
				return new Disposables();
			case ClusterMapImplementationType.StreamMappedNoCache:
				var memoryStream = new MemoryStream();
				clusterMap = new StreamMappedClusterMap(memoryStream, 0, new ClusterSerializer(clusterSize), false, autoLoad: true);
				return new Disposables(memoryStream);
			case ClusterMapImplementationType.StreamMappedCached:
				memoryStream = new MemoryStream();
				clusterMap = new StreamMappedClusterMap(memoryStream, 0, new ClusterSerializer(clusterSize), true, autoLoad: true);
				return new Disposables(memoryStream);
			default:
				throw new ArgumentOutOfRangeException(nameof(implementationType), implementationType, null);
		}
	}

	[Test]
	public void EmptyOnActivation([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(0));
	}


	[Test]
	public void SingleCluster([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(1));
		var cluster = clusterMap.Clusters[0];
		Assert.That(cluster.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster.Prev, Is.EqualTo(99));
		Assert.That(cluster.Next, Is.EqualTo(99));
	}

	[Test]
	public void TwoClusters([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(2, 99);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}

	[Test]
	public void TwoClusters_ViaAppend([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.AppendClustersToEnd(0, 1);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}


	[Test]
	public void ThreeClusters_ExpandMiddle([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.AppendClustersToEnd(0, 1);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(3));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(2));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));

		var cluster3 = clusterMap.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster3.Prev, Is.EqualTo(0));
		Assert.That(cluster3.Next, Is.EqualTo(99));

	}

	[Test]
	public void FourClusters_ExpandMiddle([Values] ClusterMapImplementationType implementationType) {
		// Chain 1: [0]		then	[0,2]
		// Chain 2: [1]		then	[1,3]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.AppendClustersToEnd(0, 1);
		clusterMap.AppendClustersToEnd(1, 1);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(4));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(2));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(3));

		var cluster3 = clusterMap.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster3.Prev, Is.EqualTo(0));
		Assert.That(cluster3.Next, Is.EqualTo(99));

		var cluster4 = clusterMap.Clusters[3];
		Assert.That(cluster4.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster4.Prev, Is.EqualTo(1));
		Assert.That(cluster4.Next, Is.EqualTo(88));


	}


	[Test]
	public void RemoveAll_1([Values] ClusterMapImplementationType implementationType) {
		// chain 1 = [0]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.RemoveBackwards(0, 1);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(0));
	}

	[Test]
	public void RemoveFullChainEnd_1([Values] ClusterMapImplementationType implementationType) {
		// chain 1 = [0,1]
		// chain 2 = [2]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(2, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.RemoveBackwards(1, 2);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}


	[Test]
	public void RemoveFullChainEnd_2([Values] ClusterMapImplementationType implementationType) {
		// chain 1 = [0,2]
		// chain 2 = [1]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.AppendClustersToEnd(0, 1);
		clusterMap.RemoveBackwards(2, 2);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}


	[Test]
	public void RemoveFullChainEnd_2_ExtraQuantityNoEffect([Values] ClusterMapImplementationType implementationType) {
		// chain 1 = [0,2]
		// chain 2 = [1]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.AppendClustersToEnd(0, 1);
		clusterMap.RemoveBackwards(2, 2 + 1);   // +1 has no effect
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}

	[Test]
	public void RemoveFullChainEnd_3([Values] ClusterMapImplementationType implementationType) {
		// chain 1 = [0]
		// chain 2 = [1,2]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(2, 88);
		clusterMap.RemoveBackwards(2, 2);
		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));
	}

	[Test]
	public void RemoveFromMiddle_1([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(2, 99); // chain 1 = [0,1]
		clusterMap.NewClusterChain(1, 88); // chain 2 = [2]
		clusterMap.RemoveNextClusters(1, 1);  // chain 1 = [0], chain 2 = [1]

		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));
	}

	[Test]
	public void RemoveFromMiddle_2([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99); // chain 1 = [0]
		clusterMap.NewClusterChain(2, 88); // chain 2 = [1,2]
		clusterMap.RemoveNextClusters(1, 1);  // chain 1 = [0], chain 2 = [1]

		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));
	}

	[Test]
	public void RemoveFromMiddle_3([Values] ClusterMapImplementationType implementationType) {
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99); // chain 1 = [0]
		clusterMap.NewClusterChain(1, 88); // chain 2 = [1]
		clusterMap.AppendClustersToEnd(0, 1);  // chain 1 = [0, 2], chain 2 = [1]
		clusterMap.RemoveNextClusters(1, 1);  // chain 1 = [0, 1]

		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));
	}


	[Test]
	public void RemoveFromMiddle_4([Values] ClusterMapImplementationType implementationType) {
		// chain 1 - [0,1,2]
		// chain 2 - [3,4,5]
		// chain 3 - [6,7,8]

		// delete chain 2

		// chain1 = [0,1,2]
		// chain3 = [3,4,5]
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(3, 99); // chain 1 = [0,1,2]
		clusterMap.NewClusterChain(3, 88); // chain 2 = [3,4,5]
		clusterMap.NewClusterChain(3, 77); // chain 3 = [6,7,8]
		clusterMap.RemoveNextClusters(3, 3);

		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(6));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.None));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(2));

		var cluster3 = clusterMap.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster3.Prev, Is.EqualTo(1));
		Assert.That(cluster3.Next, Is.EqualTo(99));

		var cluster4 = clusterMap.Clusters[3];
		Assert.That(cluster4.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster4.Prev, Is.EqualTo(77));
		Assert.That(cluster4.Next, Is.EqualTo(4));

		var cluster5 = clusterMap.Clusters[4];
		Assert.That(cluster5.Traits, Is.EqualTo(ClusterTraits.None));
		Assert.That(cluster5.Prev, Is.EqualTo(3));
		Assert.That(cluster5.Next, Is.EqualTo(5));

		var cluster6 = clusterMap.Clusters[5];
		Assert.That(cluster6.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster6.Prev, Is.EqualTo(4));
		Assert.That(cluster6.Next, Is.EqualTo(77));
	}


	[Test]
	public void RemoveInterspersed([Values] ClusterMapImplementationType implementationType) {
		// Chain 1: [0,2]		-> [0, 1]
		// Chain 2: [1,3]		-> removed
		using var _ = Create(implementationType, 1, out var clusterMap);
		clusterMap.NewClusterChain(1, 99);
		clusterMap.NewClusterChain(1, 88);
		clusterMap.AppendClustersToEnd(0, 1);
		clusterMap.AppendClustersToEnd(1, 1);
		clusterMap.RemoveNextClusters(1, 2);

		Assert.That(clusterMap.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusterMap.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusterMap.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}




}
