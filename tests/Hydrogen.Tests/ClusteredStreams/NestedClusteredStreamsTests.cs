// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class NestedClusteredStreamsTests {
	private const string TestString1 = "Hello World! This is a string containing a bunch of text";
	private const string TestString2 = "And this is another string containing a bunch of other text.";

	[Test]
	public void Simple( [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(1, 3, 512)] int clusterSize ) {
		using var rootStream = new MemoryStream();
		var parentContainer = new ClusteredStreams(rootStream, clusterSize: clusterSize, policy: policy, autoLoad: true);
		var scope = parentContainer.EnterAccessScope();
		using var child1 = parentContainer.Add();
		var childContainer = new ClusteredStreams(child1, clusterSize: clusterSize, policy: policy, autoLoad: true);
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString1));
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString2));

		Assert.That(childContainer.ReadAll(0), Is.EqualTo(Encoding.ASCII.GetBytes(TestString1))); 
		Assert.That(childContainer.ReadAll(1), Is.EqualTo(Encoding.ASCII.GetBytes(TestString2)));

	}

	[Test]
	public void Simple_Reload( [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(1, 3, 512)] int clusterSize ) {
		using var rootStream = new MemoryStream();
		// this scope creates the data
		{
			var parentContainer = new ClusteredStreams(rootStream, clusterSize: clusterSize, policy: policy, autoLoad: true);
			var scope = parentContainer.EnterAccessScope();
			using var child1 = parentContainer.Add();
			var childContainer = new ClusteredStreams(child1, clusterSize: clusterSize, policy: policy, autoLoad: true);
			childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString1));
			childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString2));
		}

		//this scope reads the data
		{
			var parentContainer = new ClusteredStreams(rootStream, clusterSize: clusterSize, policy: policy, autoLoad: true);
			var scope = parentContainer.EnterAccessScope();
			using var child1 = parentContainer.OpenWrite(0);
			var childContainer = new ClusteredStreams(child1, clusterSize: clusterSize, policy: policy, autoLoad: true);
			Assert.That(childContainer.ReadAll(0), Is.EqualTo(Encoding.ASCII.GetBytes(TestString1))); 
			Assert.That(childContainer.ReadAll(1), Is.EqualTo(Encoding.ASCII.GetBytes(TestString2)));
		}
	}

	[Test]
	public void ClearParentFails( [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(1, 3, 512)] int clusterSize ) {
		using var rootStream = new MemoryStream();
		var parentContainer = new ClusteredStreams(rootStream, clusterSize: clusterSize, policy: policy, autoLoad: true);
		var scope = parentContainer.EnterAccessScope();
		using var child1 = parentContainer.Add();
		var childContainer = new ClusteredStreams(child1, clusterSize: clusterSize, policy: policy, autoLoad: true);
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString1));
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString2));
		Assert.That(parentContainer.Clear, Throws.InvalidOperationException);
	}

	[Test]
	public void ClearChildSucceds( [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy, [Values(1, 3, 512)] int clusterSize ) {
		using var rootStream = new MemoryStream();
		var parentContainer = new ClusteredStreams(rootStream, clusterSize: clusterSize, policy: policy, autoLoad: true);
		var scope = parentContainer.EnterAccessScope();
		using var child1 = parentContainer.Add();
		var childContainer = new ClusteredStreams(child1, clusterSize: clusterSize, policy: policy, autoLoad: true);
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString1));
		childContainer.AddBytes(Encoding.ASCII.GetBytes(TestString2));
		Assert.That(childContainer.Clear, Throws.Nothing);
	}

}
