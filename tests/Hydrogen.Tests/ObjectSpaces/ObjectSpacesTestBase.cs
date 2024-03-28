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

	protected abstract ObjectSpace CreateObjectSpace(string filePath);

	#region Activation
	
	[Test]
	public void ConstructorDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		using var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));
		Assert.That(() => CreateObjectSpace(filePath), Throws.Nothing);
	}

	#endregion	

	#region Save

	[Test]
	public void SaveDoesntThrow() {
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var account = CreateAccount();
		Assert.That(() => objectSpace.Save(account), Throws.Nothing);
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

	#region Unique Key (Checksummed)

	[Test]
	public void UniqueKey_ChecksumBased_ProhibitsDuplicate_1() {
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
	public void UniqueKey_ChecksumBased_ReSave_1() {
		// note: string based property will use a checksum-based index since not constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		account1.Name = "alpha";
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account1), Throws.Nothing);
	}

	#endregion

	#region Unique Key

	[Test]
	public void UniqueKey_ProhibitsDuplicate_1() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		var account2 = CreateAccount(rng);
		account1.Name = "alpha";
		account2.Name = "beta";
		account2.UniqueNumber = account1.UniqueNumber;
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account2), Throws.InvalidOperationException);
	}

	[Test]
	public void UniqueKey_ReSave_1() {
		// note: long based property will index the property value (not checksum) since constant length key
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var rng = new Random();
		var account1 = CreateAccount(rng);
		objectSpace.Save(account1);
		Assert.That( () => objectSpace.Save(account1), Throws.Nothing);
	}

	#endregion

	#endregion

	#region Aux

	protected static ObjectSpaceBuilder PrepareObjectSpaceBuilder() {
		var builder = new ObjectSpaceBuilder();
		builder
			.AutoLoad()
			.AddDimension<Account>()
				.WithUniqueKeyOn(x => x.Name)
				.WithUniqueKeyOn(x => x.UniqueNumber)
				.UsingEqualityComparer(CreateAccountComparer())
				.Done()
			.AddDimension<Identity>()
				.WithUniqueKeyOn(x => x.Key)
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
			Name = "Savings",
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
			
	protected IScope<ObjectSpace> CreateObjectSpaceScope(string folder = null, bool keepFolder = false) {
		folder ??= Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		
		var objectSpace = CreateObjectSpace(filePath);
		var disposables = new Disposables();
		disposables.Add(objectSpace);
		if (!keepFolder)
			disposables.Add(Tools.Scope.DeleteDirOnDispose(folder));
		
		return new ActionScope<ObjectSpace>(objectSpace, _ => disposables.Dispose());
	}

	#endregion

}
