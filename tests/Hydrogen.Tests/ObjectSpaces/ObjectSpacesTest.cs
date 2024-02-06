// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture, Timeout(60000)]
public class ObjectSpacesTest {

	[Test]
	public void ConstructorDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		using var disposables = new Disposables(new ActionDisposable(() => Tools.FileSystem.DeleteDirectory(filePath)));
		Assert.That( () => new ObjectSpace(BuildFileDefinition(filePath), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default), Throws.Nothing);
	}

	[Test]
	public void WalkThrough() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");
		using var disposables = new Disposables(new ActionDisposable(() => Tools.FileSystem.DeleteDirectory(filePath)));
		using var appSpace = new ObjectSpace(BuildFileDefinition(filePath), BuildSpaceDefinition(), SerializerFactory.Default, ComparerFactory.Default);
		
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

		appSpace.Save(account);

		appSpace.Commit();

	}

	private static HydrogenFileDescriptor BuildFileDefinition(string filePath) 
		=> HydrogenFileDescriptor.From(
			filePath, 
			8192, 
			Tools.Memory.ToBytes(50, MemoryMetric.Megabyte), 
			512, 
			StreamContainerPolicy.Default
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

}
