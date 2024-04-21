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

[TestFixture, Timeout(60000)]
public abstract class ObjectSpacesTestBase {

	protected abstract ObjectSpace CreateObjectSpace(string filePath, IndexNullPolicy nullValuePolicy = IndexNullPolicy.IgnoreNull);

	#region Activation
	
	[Test]
	public void ConstructorThrowsNothing() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		using var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));
		Assert.That(() => CreateObjectSpace(filePath), Throws.Nothing);
	}

	#endregion	

	#region Save

	[Test]
	public void SaveThrowsNothing() {
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var account = CreateAccount();
		Assert.That(() => objectSpace.Save(account), Throws.Nothing);
	}

	#endregion

	#region Delete
	
	[Test]
	public void DeleteThrowsNothing() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account1), Throws.Nothing);
	}

	#endregion

	#region Load

	[Test]
	public void LoadEmptyDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var scope = CreateObjectSpaceScope(folder, true)) {
			var objectSpace = scope.Item;
			objectSpace.Commit();
		}

		Assert.That(() => { using var _ = CreateObjectSpaceScope(folder); }, Throws.Nothing);

	}

	[Test]
	public void LoadDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var scope = CreateObjectSpaceScope(folder, true)) {
			var objectSpace = scope.Item;
			var savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}
		Assert.That(() => { using var _ = CreateObjectSpaceScope(folder); }, Throws.Nothing);
	}

	#endregion

	#region Clear

	[Test]
	public void Clear_1() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var accountComparer = CreateAccountComparer();
		Account savedAccount, loadedAccount;
		using (var scope = CreateObjectSpaceScope(folder, true)) {
			var objectSpace = scope.Item;
			savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Clear();
			objectSpace.Commit();

			foreach(var dim in objectSpace.Dimensions)
				Assert.That(dim.ObjectStream.Count, Is.EqualTo(0));
		}

	}

	#endregion

	#region Commit

	[Test]
	public void CommitNotThrows() {
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var account = CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Commit, Throws.Nothing);
	}

	[Test]
	public void CommitSaves() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var accountComparer = CreateAccountComparer();
		Account savedAccount, loadedAccount;
		using (var scope = CreateObjectSpaceScope(folder, true)) {
			var objectSpace = scope.Item;
			savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		using (var scope = CreateObjectSpaceScope(folder, false)) {
			var objectSpace = scope.Item;
			loadedAccount = objectSpace.Get<Account>(0);
		}

		Assert.That(loadedAccount, Is.EqualTo(savedAccount).Using(accountComparer));
	}

	#endregion

	#region Rollback

	[Test]
	public void RollbackNotThrows() {
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var account = CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Rollback, Throws.Nothing);
	}

	[Test]
	public void Rollback() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var scope = CreateObjectSpaceScope(folder, true)) {
			var objectSpace = scope.Item;
			var account = CreateAccount();
			objectSpace.Save(account);
			objectSpace.Rollback();
		}

		using (var scope = CreateObjectSpaceScope(folder, false)) {
			var objectSpace = scope.Item;
			Assert.That(objectSpace.Count<Account>(), Is.EqualTo(0));
		}

	}

	[Test]
	public void Rollback_2() {
		// TODO: make sure previous committed state properly rolled back 
		// make sure to update prior state before rolback to ensure update is rolled back
	}
	
	#endregion

	#region Indexes

	#region Unique Member (Checksummed)

	[Test]
	public void UniqueMember_Checksummed_GetViaIndex() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		account1.Name = "alpha";

		var account2 = CreateAccount(rng);
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
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaAdd() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = account2.Name = "alpha";
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaUpdate_1() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account1.Name = "beta";
		Assert.That( () => objectSpace.Save(account1), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_Checksummed_ProhibitsDuplicate_ViaUpdate_2() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account2.Name = "alpha";
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}
	
	[Test]
	public void UniqueMember_Checksummed_AllowsUpdate_ThrowsNothing() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		account1.Name = "alpha";
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account1), Throws.Nothing);
	}

	[Test]
	public void UniqueMember_Checksummed_SaveThenDeleteThenSave_ThrowsNothing() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		objectSpace.Delete(account1);
		var account3 = CreateAccount(rng);
		account3.Name = "alpha";
		Assert.That( () => objectSpace.Save(account3), Throws.Nothing);
	}

	[Test]
	public void UniqueMember_Checksummed_IgnoreNullPolicy() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope(nullValuePolicy: IndexNullPolicy.IgnoreNull);
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = null;
		account2.Name = null;
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account2), Throws.Nothing);
	}

	[Test]
	public void UniqueMember_Checksummed_IndexNullValue() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope(nullValuePolicy: IndexNullPolicy.IndexNullValue);
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = null;
		account2.Name = null;
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_Checksummed_ThrowOnNullValue() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope(nullValuePolicy: IndexNullPolicy.ThrowOnNull);
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		account1.Name = null;
		Assert.That(() => objectSpace.Save(account1), Throws.InvalidOperationException);
	}	

	#endregion

	#region Unique Member

	[Test]
	public void UniqueMember_GetViaIndex() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		account1.UniqueNumber = 1;

		var account2 = CreateAccount(rng);
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
	public void UniqueMember_ProhibitsDuplicate_ViaAdd() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account2.UniqueNumber = account1.UniqueNumber;
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_ProhibitsDuplicate_ViaUpdate_1() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account1.UniqueNumber = account2.UniqueNumber;
		
		Assert.That( () => objectSpace.Save(account1), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_ProhibitsDuplicate_ViaUpdate_2() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		account2.UniqueNumber = account1.UniqueNumber;
		
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueMember_AllowsUpdate_ThrowsNothing() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account1), Throws.Nothing);
	}
	
	[Test]
	public void UniqueMember_SaveThenDeleteThenSave_ThrowsNothing() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random(31337);
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.UniqueNumber = 1;
		account2.UniqueNumber = 2;
		objectSpace.Save(account1);
		objectSpace.Save(account2);
		objectSpace.Delete(account1);
		var account3 = CreateAccount(rng);
		account3.UniqueNumber = 1;
		Assert.That( () => objectSpace.Save(account3), Throws.Nothing);
	}

	#endregion

	#endregion

	#region Aux

	protected static ObjectSpaceBuilder PrepareObjectSpaceBuilder(IndexNullPolicy nullValuePolicy) {
		var builder = new ObjectSpaceBuilder();
		builder
			.AutoLoad()
			.AddDimension<Account>()
				.WithUniqueIndexOn(x => x.Name, nullPolicy: nullValuePolicy)
				.WithUniqueIndexOn(x => x.UniqueNumber, nullPolicy: nullValuePolicy)
				.UsingEqualityComparer(CreateAccountComparer())
				.Done()
			.AddDimension<Identity>()
				.WithUniqueIndexOn(x => x.Key)
				.UsingEqualityComparer(CreateIdentityComparer())
				.Done();

		return builder;
	}

	protected static Account CreateAccount(Random rng = null) {
		rng ??= new Random(31337);
		var secret = "MyPassword";

		var dss = DSS.PQC_WAMSSharp;
		var privateKey = Signers.CreatePrivateKey(dss, Hashers.Hash(CHF.SHA2_256, secret.ToAsciiByteArray()));
		var publicKey = Signers.DerivePublicKey(dss, privateKey, 0);

		var identity = new Identity {
			DSS = dss,
			Key = publicKey.RawBytes
		};

		var account = new Account {
			Identity = identity,
			Name = $"Savings {rng.NextInt64()}",
			UniqueNumber = rng.NextInt64(),
			Quantity = 0
		};

		return account;
	}

	protected static IEqualityComparer<Account> CreateAccountComparer() 
		=> EqualityComparerBuilder
			.For<Account>()
			.By(x => x.Name)
			.ThenBy(x => x.Quantity)
			.ThenBy(x => x.Identity, CreateIdentityComparer());

	protected static IEqualityComparer<Identity> CreateIdentityComparer() 
		=> EqualityComparerBuilder
			.For<Identity>()
			.By(x => x.DSS)
			.ThenBy(x => x.Key, ByteArrayEqualityComparer.Instance);
			
	protected IScope<ObjectSpace> CreateObjectSpaceScope(string folder = null, bool keepFolder = false, IndexNullPolicy nullValuePolicy = IndexNullPolicy.IgnoreNull) {
		folder ??= Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		
		var objectSpace = CreateObjectSpace(filePath, nullValuePolicy);
		var disposables = new Disposables();
		disposables.Add(objectSpace);
		if (!keepFolder)
			disposables.Add(Tools.Scope.DeleteDirOnDispose(folder));
		
		return new ActionScope<ObjectSpace>(objectSpace, _ => disposables.Dispose());
	}

	#endregion

}
