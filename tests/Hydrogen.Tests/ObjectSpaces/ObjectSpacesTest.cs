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
public class ObjectSpacesTest {

	#region Tests

	[Test]
	public void ConstructorDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		using var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));
		Assert.That(() => new ObjectSpace(BuildFileDefinition(filePath), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default), Throws.Nothing);
	}

	[Test]
	public void Save() {
		using var scope = CreateObjectSpaceScope();
		var objectSpace = scope.Item;
		var account = CreateAccount();
		Assert.That(() => objectSpace.Save(account), Throws.Nothing);
	}

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

	#region Aux

	private static Account CreateAccount() {
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
			Quantity = 0
		};

		return account;
	}

	private static IEqualityComparer<Account> CreateAccountComparer() 
		=> EqualityComparerBuilder
			.For<Account>()
			.By(x => x.Name)
			.ThenBy(x => x.Quantity)
			.ThenBy(x => x.Identity, CreateIdentityComparer());

	private static IEqualityComparer<Identity> CreateIdentityComparer() 
		=> EqualityComparerBuilder
			.For<Identity>()
			.By(x => x.DSS)
			.ThenBy(x => x.Key, ByteArrayEqualityComparer.Instance);
			
	private static IScope<ObjectSpace> CreateObjectSpaceScope(string folder = null, bool keepFolder = false) {
		folder ??= Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		
		var objectSpace = new ObjectSpace(BuildFileDefinition(filePath), BuildSpaceDefinition(), SerializerFactory.Default, CreateComparerFactory());
		var disposables = new Disposables();
		disposables.Add(objectSpace);
		if (!keepFolder)
			disposables.Add(Tools.Scope.DeleteDirOnDispose(folder));
		
		return new ActionScope<ObjectSpace>(objectSpace, _ => disposables.Dispose());
	}

	private static ComparerFactory CreateComparerFactory() {
		var comparerFactory = new ComparerFactory(ComparerFactory.Default);
		comparerFactory.RegisterEqualityComparer(CreateAccountComparer());
		comparerFactory.RegisterEqualityComparer(CreateIdentityComparer());
		return comparerFactory;
	}

	private static HydrogenFileDescriptor BuildFileDefinition(string filePath)
		=> HydrogenFileDescriptor.From(
			filePath,
			8192,
			Tools.Memory.ToBytes(50, MemoryMetric.Megabyte),
			512,
			ClusteredStreamsPolicy.Default
		);

	private static ObjectSpaceDefinition BuildSpaceDefinition() {
		var definition = new ObjectSpaceDefinition {
			Containers = new ObjectSpaceDefinition.ContainerDefinition[] {
				new() {
					ObjectType = typeof(Account),
					Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
						new() {
							Type = ObjectSpaceDefinition.IndexType.FreeIndexStore,
							ReservedStreamIndex = 0,
						},
						new() {
							Type = ObjectSpaceDefinition.IndexType.UniqueKey,
							KeyMember = Tools.Mapping.GetMember<Account, string>(x => x.Name),
							ReservedStreamIndex = 1
						}
					}
				},
				new() {
					ObjectType = typeof(Identity),
					Indexes = new ObjectSpaceDefinition.IndexDefinition[] {
						new() {
							Type = ObjectSpaceDefinition.IndexType.FreeIndexStore,
							ReservedStreamIndex = 0,
						},
						new() {
							Type = ObjectSpaceDefinition.IndexType.UniqueKey,
							KeyMember = Tools.Mapping.GetMember<Identity, byte[]>(x => x.Key),
							ReservedStreamIndex = 0
						}
					}
				},


			}
		};


		return definition;
	}

	#endregion


	#region Inner Types

	public class Account {

		[UniqueProperty]
		public string Name { get; set; }

		public decimal Quantity { get; set; }

		public Identity Identity { get; set; }

	}

	public class Identity {

		public DSS DSS { get; set; }

		[UniqueProperty]
		public byte[] Key { get; set; }
	}

	#endregion
}
