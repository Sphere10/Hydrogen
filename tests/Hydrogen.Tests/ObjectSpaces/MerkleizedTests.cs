// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;


namespace Hydrogen.Tests.ObjectSpaces;

[TestFixture]
public class MerkleizedTests {


	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.MerkleizedTestCases))]
	public void CheckRootsChanged(TestTraits testTraits) {
		using var objectSpace = TestsHelper.CreateObjectSpace(testTraits);
		var chf = objectSpace.Definition.HashFunction;
		var digestSize = Hashers.GetDigestSizeBytes(chf);

		var savedAccount = TestsHelper.CreateAccount();
		var accountDigest = Hashers.Hash(chf, objectSpace.Serializers.GetSerializer<Account>().SerializeBytesLE(savedAccount));
		objectSpace.Save(savedAccount);
		
		// This is necessary to flush out merkle tree changes
		objectSpace.Flush(); 

		var dim1 = objectSpace.Dimensions[0];
		var dim2 = objectSpace.Dimensions[1];

		// Verify account dimension has single item root
		using var dim1Scope = dim1.Container.ObjectStream.EnterAccessScope();
		var accountRoot = dim1.Container.ObjectStream.Streams.Header.MapExtensionProperty(0, digestSize, new ConstantSizeByteArraySerializer(digestSize)).Value;
		Assert.That(accountRoot, Is.EqualTo(accountDigest).Using(ByteArrayEqualityComparer.Instance));

		// Verify identity dimension has null root  (note: null is stored as 0 bytes)
		using var dim2Scope = dim2.Container.ObjectStream.EnterAccessScope();
		var identityRoot = dim2.Container.ObjectStream.Streams.Header.MapExtensionProperty(0, digestSize, new ConstantSizeByteArraySerializer(digestSize)).Value;
		Assert.That(identityRoot, Is.EqualTo(new byte[digestSize]).Using(ByteArrayEqualityComparer.Instance));
			
		// Verify spatial root is both account/identity
		var spaceRoot = objectSpace.Streams.Header.MapExtensionProperty(0, digestSize, new ConstantSizeByteArraySerializer(digestSize)).Value;
		Assert.That(spaceRoot, Is.EqualTo(MerkleTree.ComputeMerkleRoot(new [] { accountRoot, identityRoot }, chf)).Using(ByteArrayEqualityComparer.Instance));

	}
}
