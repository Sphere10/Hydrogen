// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using NUnit.Framework;

namespace Hydrogen.Tests;

/// <remarks>
/// During dev, bugs seemed to occur when clusters linked in descending order.
/// write unit tests which directly scramble the cluster links, different patterns (descending, random, etc).
/// </remarks>
[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class ClusteredContainerTests {

	[Test]
	public void EmptyOnActivation() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(0));
	}


	[Test]
	public void SingleCluster() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(1));
		var cluster = clusteredContainer.Clusters[0];
		Assert.That(cluster.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster.Prev, Is.EqualTo(99));
		Assert.That(cluster.Next, Is.EqualTo(99));
	}

	[Test]
	public void TwoClusters() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(2, 99);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}

	[Test]
	public void TwoClusters_ViaAppend() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.AppendClustersToEnd(0, 1);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}


	[Test]
	public void ThreeClusters_ExpandMiddle() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.AppendClustersToEnd(0, 1);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(3));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(2));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));

		var cluster3 = clusteredContainer.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster3.Prev, Is.EqualTo(0));
		Assert.That(cluster3.Next, Is.EqualTo(99));

	}

	[Test]
	public void FourClusters_ExpandMiddle() {
		// Chain 1: [0]		then	[0,2]
		// Chain 2: [1]		then	[1,3]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.AppendClustersToEnd(0, 1);
		clusteredContainer.AppendClustersToEnd(1, 1);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(4));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(2));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start ));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(3));

		var cluster3 = clusteredContainer.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster3.Prev, Is.EqualTo(0));
		Assert.That(cluster3.Next, Is.EqualTo(99));

		var cluster4 = clusteredContainer.Clusters[3];
		Assert.That(cluster4.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster4.Prev, Is.EqualTo(1));
		Assert.That(cluster4.Next, Is.EqualTo(88));


	}


	[Test]
	public void RemoveAll_1() {
		// chain 1 = [0]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.RemoveBackwards(0, 1);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(0));
	}

	[Test]
	public void RemoveFullChainEnd_1() {
		// chain 1 = [0,1]
		// chain 2 = [2]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(2, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.RemoveBackwards(1, 2);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}


	[Test]
	public void RemoveFullChainEnd_2() {
		// chain 1 = [0,2]
		// chain 2 = [1]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.AppendClustersToEnd(0, 1);
		clusteredContainer.RemoveBackwards(2, 2);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}

	
	[Test]
	public void RemoveFullChainEnd_2_ExtraQuantityNoEffect() {
		// chain 1 = [0,2]
		// chain 2 = [1]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.AppendClustersToEnd(0, 1);
		clusteredContainer.RemoveBackwards(2, 2 + 1);   // +1 has no effect
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster1.Prev, Is.EqualTo(88));
		Assert.That(cluster1.Next, Is.EqualTo(88));
	}

	[Test]
	public void RemoveFullChainEnd_3() {
		// chain 1 = [0]
		// chain 2 = [1,2]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(2, 88);
		clusteredContainer.RemoveBackwards(2, 2);
		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(1));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));
	}

	[Test]
	public void RemoveFromMiddle_1() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(2, 99); // chain 1 = [0,1]
		clusteredContainer.NewClusterChain(1, 88); // chain 2 = [2]
		clusteredContainer.RemoveNextClusters(1, 1);  // chain 1 = [0], chain 2 = [1]

		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));
	}

	[Test]
	public void RemoveFromMiddle_2() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99); // chain 1 = [0]
		clusteredContainer.NewClusterChain(2, 88); // chain 2 = [1,2]
		clusteredContainer.RemoveNextClusters(1, 1);  // chain 1 = [0], chain 2 = [1]

		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(99));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.Start | ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(88));
		Assert.That(cluster2.Next, Is.EqualTo(88));
	}


	[Test]
	public void RemoveFromMiddle_3() {
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99); // chain 1 = [0]
		clusteredContainer.NewClusterChain(1, 88); // chain 2 = [1]
		clusteredContainer.AppendClustersToEnd(0, 1);  // chain 1 = [0, 2], chain 2 = [1]
		clusteredContainer.RemoveNextClusters(1, 1);  // chain 1 = [0, 1]

		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));
	}

	
	[Test]
	public void RemoveFromMiddle_4() {
		// chain 1 - [0,1,2]
		// chain 2 - [3,4,5]
		// chain 3 - [6,7,8]

		// delete chain 2

		// chain1 = [0,1,2]
		// chain3 = [3,4,5]
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(3, 99); // chain 1 = [0,1,2]
		clusteredContainer.NewClusterChain(3, 88); // chain 2 = [3,4,5]
		clusteredContainer.NewClusterChain(3, 77); // chain 3 = [6,7,8]
		clusteredContainer.RemoveNextClusters(3, 3);

		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(6));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.None));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(2));

		var cluster3 = clusteredContainer.Clusters[2];
		Assert.That(cluster3.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster3.Prev, Is.EqualTo(1));
		Assert.That(cluster3.Next, Is.EqualTo(99));

		var cluster4 = clusteredContainer.Clusters[3];
		Assert.That(cluster4.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster4.Prev, Is.EqualTo(77));
		Assert.That(cluster4.Next, Is.EqualTo(4));

		var cluster5 = clusteredContainer.Clusters[4];
		Assert.That(cluster5.Traits, Is.EqualTo(ClusterTraits.None ));
		Assert.That(cluster5.Prev, Is.EqualTo(3));
		Assert.That(cluster5.Next, Is.EqualTo(5));

		var cluster6 = clusteredContainer.Clusters[5];
		Assert.That(cluster6.Traits, Is.EqualTo( ClusterTraits.End ));
		Assert.That(cluster6.Prev, Is.EqualTo(4));
		Assert.That(cluster6.Next, Is.EqualTo(77));
	}


	[Test]
	public void RemoveInterspersed() {
		// Chain 1: [0,2]		-> [0, 1]
		// Chain 2: [1,3]		-> removed
		var clusteredContainer = new ClusterMap(new ExtendedList<Cluster>(), 1);
		clusteredContainer.NewClusterChain(1, 99);
		clusteredContainer.NewClusterChain(1, 88);
		clusteredContainer.AppendClustersToEnd(0, 1);
		clusteredContainer.AppendClustersToEnd(1, 1);
		clusteredContainer.RemoveNextClusters(1, 2);

		Assert.That(clusteredContainer.Clusters.Count, Is.EqualTo(2));
		var cluster1 = clusteredContainer.Clusters[0];
		Assert.That(cluster1.Traits, Is.EqualTo(ClusterTraits.Start));
		Assert.That(cluster1.Prev, Is.EqualTo(99));
		Assert.That(cluster1.Next, Is.EqualTo(1));

		var cluster2 = clusteredContainer.Clusters[1];
		Assert.That(cluster2.Traits, Is.EqualTo(ClusterTraits.End ));
		Assert.That(cluster2.Prev, Is.EqualTo(0));
		Assert.That(cluster2.Next, Is.EqualTo(99));

	}

}
