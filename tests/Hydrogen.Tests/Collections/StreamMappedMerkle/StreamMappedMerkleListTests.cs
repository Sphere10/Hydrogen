// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using NUnit.Framework;
using System.IO;
using ExtensionProperties;
using Hydrogen.Tests.Merkle;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedMerkleListTests : MerkleListTestsBase {
	private const int DefaultClusterSize = 256;

	[Test]
	public void TestAdaptedScopes([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, 256, autoLoad: true);
		clusteredList.Add("beta");
		clusteredList.Insert(0, "alpha");
		clusteredList.Add("alphaa");
		clusteredList.RemoveAt(2);
		clusteredList.Insert(2, "gammaa");
		clusteredList.Add("delta");
		clusteredList.Update(2, "gamma");
		clusteredList.Add("epsilon");
		Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "alpha", "beta", "gamma", "delta", "epsilon" }, chf)));
	}
	
	[Test]
	public void Special_Remove([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, 256, autoLoad: true);
		clusteredList.Add("alpha");
		clusteredList.Insert(0, "beta");
		clusteredList.Add("gamma");
		clusteredList.RemoveAt(1);
		clusteredList.Add("delta");
		clusteredList.RemoveAt(0);
		clusteredList.RemoveAt(0);
		Assert.That(clusteredList.MerkleTree.Root, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new[] { "delta" }, chf)));
	}

	[Test]
	public void Special_RemoveAll([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, 256, autoLoad: true);
		clusteredList.Add("alpha");
		clusteredList.Insert(0, "beta");
		clusteredList.Add("gamma");
		clusteredList.RemoveAt(1);
		clusteredList.Add("delta");
		clusteredList.RemoveAt(0);
		clusteredList.RemoveAt(0);
		clusteredList.RemoveAt(0);
		Assert.That(clusteredList.MerkleTree.Root, Is.Null);
	}

	[Test]
	public void EnsureLazyCalculatedRootIsFlushedToHeaderStream([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, 256, autoLoad: true);
		clusteredList.Add("alpha");
		clusteredList.Add("beta");
		clusteredList.Add("gamma");

		var root = clusteredList.MerkleTree.Root; // this causes root to be calculated and written to stream

		var digestLen = Hashers.GetDigestSizeBytes(chf);
		using var _  = clusteredList.ObjectStream.Streams.EnterAccessScope();
		var smRoot = clusteredList.ObjectStream.Streams.Header.MapExtensionProperty(0, digestLen, new ConstantSizeByteArraySerializer(digestLen)).Value;

		Assert.That(smRoot, Is.EqualTo(root).Using(ByteArrayEqualityComparer.Instance));
		
	}

	[Test]
	public void EnsureLazyUncalculatedRootIsNotFlushedToHeaderStream([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf) {
		var memStream = new MemoryStream();
		using var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, 256, autoLoad: true);
		clusteredList.Add("alpha");
		clusteredList.Add("beta");
		clusteredList.Add("gamma");
		
		// The root has not been calculated at this point, so stream header should not contain anything

		var digestLen = Hashers.GetDigestSizeBytes(chf);
		using var _  = clusteredList.ObjectStream.Streams.EnterAccessScope();
		var smRoot = clusteredList.ObjectStream.Streams.Header.MapExtensionProperty(0, digestLen, new ConstantSizeByteArraySerializer(digestLen)).Value;

		Assert.That(smRoot, Is.EqualTo(new byte[digestLen]).Using(ByteArrayEqualityComparer.Instance));
		
	}


	protected override IDisposable CreateMerkleList([Values(CHF.SHA2_256, CHF.Blake2b_128)] CHF chf, out IMerkleList<string> merkleList) {
		var memStream = new MemoryStream();
		var clusteredList = new StreamMappedMerkleList<string>(memStream, chf, DefaultClusterSize, autoLoad: true);
		merkleList = clusteredList;
		return new Disposables(clusteredList, memStream);
	}

}
