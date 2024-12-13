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
using Hydrogen.ObjectSpaces;
using NUnit.Framework;
using Hydrogen;

namespace Hydrogen.Tests;

[TestFixture]
public class ObjectSpacesTests {


	#region Traits


	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void CheckMerkleTraitSet(ObjectSpaceTestTraits testTraits) {
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		Assert.That(objectSpace.Definition.Traits.HasFlag(ObjectSpaceTraits.Merkleized), Is.EqualTo(testTraits.HasFlag(ObjectSpaceTestTraits.Merklized)));
	}

	#endregion

	#region Activation

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void ConstructorThrowsNothing(ObjectSpaceTestTraits testTraits) {

		Assert.That(() => { using var _ = ObjectSpaceTestsHelper.CreateStandard(testTraits); }, Throws.Nothing);
	}

	#endregion

	#region Save

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void SaveThrowsNothing(ObjectSpaceTestTraits testTraits) {
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var account = ObjectSpaceTestsHelper.CreateAccount();
		Assert.That(() => objectSpace.Save(account), Throws.Nothing);

	}

	#endregion

	#region Delete

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void DeleteThrowsNothing(ObjectSpaceTestTraits testTraits) {
		// note: long based property will index the property value (not checksum) since constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Delete(account1), Throws.Nothing);
	}

	#endregion

	#region Clear

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void Clear_1(ObjectSpaceTestTraits testTraits) {
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var savedAccount = ObjectSpaceTestsHelper.CreateAccount();
		objectSpace.Save(savedAccount);
		objectSpace.Clear("I CONSENT TO CLEAR ALL DATA");

		foreach (var dim in objectSpace.Dimensions)
			Assert.That(dim.Container.ObjectStream.Count, Is.EqualTo(0));
	}

	#endregion

	#region Indexes

	#region Unique Member (Checksummed)

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_GetViaIndex(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = "alpha";

		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account2.Name = "beta";

		objectSpace.Save(account1);
		objectSpace.Save(account2);

		var fetch1 = objectSpace.Get((Account x) => x.Name, "alpha");
		Assert.That(fetch1, Is.SameAs(account1));

		var fetch2 = objectSpace.Get((Account x) => x.Name, "beta");
		Assert.That(fetch2, Is.SameAs(account2));

		Assert.That(() => objectSpace.Get((Account x) => x.Name, "gamma"), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaAdd(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = account2.Name = "alpha";
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaUpdate_1(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account1.Name = "beta";
		Assert.That(() => objectSpace.Save(account1), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaUpdate_2(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account2.Name = "alpha";
		Assert.That(() => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_AllowsUpdate_ThrowsNothing(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = "alpha";
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account1), Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_SaveThenDeleteThenSave_ThrowsNothing(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		objectSpace.Delete(account1);
		var account3 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account3.Name = "alpha";
		Assert.That(() => objectSpace.Save(account3), Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_IgnoreNullPolicy(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits, nullPolicy: IndexNullPolicy.IgnoreNull);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = null;
		account2.Name = null;
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account2), Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_IndexNullValue(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits, nullPolicy: IndexNullPolicy.IndexNullValue);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = null;
		account2.Name = null;
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_Checksummed_ThrowOnNullValue(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits, nullPolicy: IndexNullPolicy.ThrowOnNull);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.Name = null;
		Assert.That(() => objectSpace.Save(account1), Throws.InvalidOperationException);
	}

	#endregion

	#region Unique Member

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_GetViaIndex(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random();
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.UniqueNumber = 1;

		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account2.UniqueNumber = 2;

		objectSpace.Save(account1);
		objectSpace.Save(account2);

		var fetch1 = objectSpace.Get((Account x) => x.UniqueNumber, 1);
		Assert.That(fetch1, Is.SameAs(account1));

		var fetch2 = objectSpace.Get((Account x) => x.UniqueNumber, 2);
		Assert.That(fetch2, Is.SameAs(account2));

		Assert.That(() => objectSpace.Get((Account x) => x.UniqueNumber, 3), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_ProhibitsDuplicate_ViaAdd(ObjectSpaceTestTraits testTraits) {
		// note: long based property will index the property value (not checksum) since constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account2.UniqueNumber = account1.UniqueNumber;
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_ProhibitsDuplicate_ViaUpdate_1(ObjectSpaceTestTraits testTraits) {
		// note: long based property will index the property value (not checksum) since constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account1.UniqueNumber = account2.UniqueNumber;

		Assert.That(() => objectSpace.Save(account1), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_ProhibitsDuplicate_ViaUpdate_2(ObjectSpaceTestTraits testTraits) {
		// note: long based property will index the property value (not checksum) since constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account2.UniqueNumber = account1.UniqueNumber;

		Assert.That(() => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_AllowsUpdate_ThrowsNothing(ObjectSpaceTestTraits testTraits) {
		// note: long based property will index the property value (not checksum) since constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		objectSpace.Save(account1);
		Assert.That(() => objectSpace.Save(account1), Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.AllTestCases))]
	public void UniqueMember_SaveThenDeleteThenSave_ThrowsNothing(ObjectSpaceTestTraits testTraits) {
		// note: string based property will use a checksum-based index since not constant length key
		using var objectSpace = ObjectSpaceTestsHelper.CreateStandard(testTraits);
		var rng = new Random(31337);
		var account1 = ObjectSpaceTestsHelper.CreateAccount(rng);
		var account2 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account1.UniqueNumber = 1;
		account2.UniqueNumber = 2;
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		objectSpace.Delete(account1);
		var account3 = ObjectSpaceTestsHelper.CreateAccount(rng);
		account3.UniqueNumber = 1;
		Assert.That(() => objectSpace.Save(account3), Throws.Nothing);
	}

	#endregion

	#endregion

}
