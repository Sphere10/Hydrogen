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
public class PersistenceIgnoranceTests {


	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.PersistentIgnorantTestCases))]
	public void PI_Simple_1(TestTraits testTraits) {
		using var objectSpace = TestsHelper.CreateObjectSpace(testTraits);
		
		using (objectSpace.EnterAccessScope()) {
			var account = objectSpace.New<Account>();
			account.Name = "Herman";
			objectSpace.Flush();
		}
		Assert.That(objectSpace.Count<Account>(), Is.EqualTo(1));
		Assert.That(objectSpace.Count<Identity>(), Is.EqualTo(0));

		var acc = objectSpace.Get<Account>(0);

		Assert.That(acc.Name, Is.EqualTo("Herman"));

	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.PersistentIgnorantTestCases))]
	public void PI_Simple_2(TestTraits testTraits) {
		using var objectSpace = TestsHelper.CreateObjectSpace(testTraits);
		
		using (objectSpace.EnterAccessScope()) {
			

			var account1 = objectSpace.New<Account>();
			account1.Name = "Herman";
			account1.UniqueNumber = 0;

			var account2 = objectSpace.New<Account>();
			account2.Name = "Bob";
			account2.UniqueNumber = 1;

			objectSpace.Flush();
		}
		Assert.That(objectSpace.Count<Account>(), Is.EqualTo(2));
		Assert.That(objectSpace.Count<Identity>(), Is.EqualTo(0));

		var acc1 = objectSpace.Get<Account>(0);
		Assert.That(acc1.Name, Is.EqualTo("Herman"));
		Assert.That(acc1.UniqueNumber, Is.EqualTo(0));

		var acc2 = objectSpace.Get<Account>(1);
		Assert.That(acc2.Name, Is.EqualTo("Bob"));
		Assert.That(acc2.UniqueNumber, Is.EqualTo(1));
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.PersistentIgnorantTestCases))]
	public void PI_Simple_3(TestTraits testTraits) {
		using var objectSpace = TestsHelper.CreateObjectSpace(testTraits);
		
		using (objectSpace.EnterAccessScope()) {
			
			var identity = objectSpace.New<Identity>();
			identity.DSS = DSS.ECDSA_SECP256k1;
			identity.Key = [0x01, 0x02, 0x03, 0x04];
			identity.Group = 1;

			var account1 = objectSpace.New<Account>();
			account1.Name = "Herman";
			account1.Identity = identity;
			account1.UniqueNumber = 0;

			var account2 = objectSpace.New<Account>();
			account2.Name = "Bob";
			account2.Identity = identity;
			account2.UniqueNumber = 1;

			objectSpace.Flush();
		}
		Assert.That(objectSpace.Count<Account>(), Is.EqualTo(2));
		Assert.That(objectSpace.Count<Identity>(), Is.EqualTo(1));

		var acc1 = objectSpace.Get<Account>(0);
		Assert.That(acc1.Name, Is.EqualTo("Herman"));
		Assert.That(acc1.UniqueNumber, Is.EqualTo(0));

		var acc2 = objectSpace.Get<Account>(1);
		Assert.That(acc2.Name, Is.EqualTo("Bob"));
		Assert.That(acc2.UniqueNumber, Is.EqualTo(1));

		Assert.That(acc2.Identity, Is.Not.Null);
		Assert.That(acc2.Identity.DSS, Is.EqualTo(DSS.ECDSA_SECP256k1));
		Assert.That(acc2.Identity.Key, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03, 0x04 }));
		Assert.That(acc2.Identity.Group, Is.EqualTo(1));

		Assert.That(acc1.Identity, Is.SameAs(acc2.Identity));
		Assert.That(ReferenceEquals(acc1.Identity, acc2.Identity), Is.True);
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.PersistentIgnorantTestCases))]
	public void PI_Simple_3_Variant(TestTraits testTraits) {
		using var objectSpace = TestsHelper.CreateObjectSpace(testTraits);
		
		using (objectSpace.EnterAccessScope()) {
			// Accounts created first (serialized first)
			var account1 = objectSpace.New<Account>();
			account1.Name = "Herman";
			account1.UniqueNumber = 0;

			var account2 = objectSpace.New<Account>();
			account2.Name = "Bob";
			account2.UniqueNumber = 1;

			// Identity is created after accounts			
			var identity = objectSpace.New<Identity>();
			identity.DSS = DSS.ECDSA_SECP256k1;
			identity.Key = [0x01, 0x02, 0x03, 0x04];
			identity.Group = 1;

			// Set identity on accounts
			account1.Identity = identity;
			account2.Identity = identity;

			objectSpace.Flush();
		}
		Assert.That(objectSpace.Count<Account>(), Is.EqualTo(2));
		Assert.That(objectSpace.Count<Identity>(), Is.EqualTo(1));

		var acc1 = objectSpace.Get<Account>(0);
		Assert.That(acc1.Name, Is.EqualTo("Herman"));
		Assert.That(acc1.UniqueNumber, Is.EqualTo(0));

		var acc2 = objectSpace.Get<Account>(1);
		Assert.That(acc2.Name, Is.EqualTo("Bob"));
		Assert.That(acc2.UniqueNumber, Is.EqualTo(1));

		Assert.That(acc2.Identity, Is.Not.Null);
		Assert.That(acc2.Identity.DSS, Is.EqualTo(DSS.ECDSA_SECP256k1));
		Assert.That(acc2.Identity.Key, Is.EqualTo(new byte[] { 0x01, 0x02, 0x03, 0x04 }));
		Assert.That(acc2.Identity.Group, Is.EqualTo(1));
		
		Assert.That(acc1.Identity, Is.SameAs(acc2.Identity));
		Assert.That(ReferenceEquals(acc1.Identity, acc2.Identity), Is.True);
	}
}
