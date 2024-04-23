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
public class FileObjectSpacesTest : ObjectSpacesTestBase<FileObjectSpace> {

	protected override FileObjectSpace CreateObjectSpace(ObjectSpaceBuilder builder) => CreateFileObjectSpace(Tools.FileSystem.GetTempEmptyDirectory(true), false, builder);

	protected virtual FileObjectSpace CreateFileObjectSpace(string folder, bool keepFilesOnDispose, ObjectSpaceBuilder builder) {
		folder ??= Tools.FileSystem.GetTempEmptyDirectory(true);
		var filePath = Path.Combine(folder, "app.db");

		// tell builder to use as file
		builder.UseFile(filePath);

		var objectSpace = (FileObjectSpace)builder.Build();

		if (!keepFilesOnDispose)
			objectSpace.Disposables.Add(Tools.Scope.DeleteDirOnDispose(folder));

		return objectSpace;
	}


	#region Load

	[Test]
	public void LoadEmptyDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var objectSpace = CreateFileObjectSpace(folder, true, CreateStandardObjectSpace())) {
			objectSpace.Commit();
		}
		Assert.That(() => { using var _ = CreateFileObjectSpace(folder, false, CreateStandardObjectSpace()); }, Throws.Nothing);
	}

	[Test]
	public void LoadDoesntThrow() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var objectSpace = CreateFileObjectSpace(folder, true, CreateStandardObjectSpace())) {
			var savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}
		Assert.That(() => { using var _ = CreateFileObjectSpace(folder, false, CreateStandardObjectSpace()); }, Throws.Nothing);
	}

	#endregion

	#region Commit

	[Test]
	public void CommitNotThrows() {
		using var objectSpace = CreateObjectSpace();
		var account = CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Commit, Throws.Nothing);
	}

	[Test]
	public void CommitSaves() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var accountComparer = CreateAccountComparer();
		Account savedAccount, loadedAccount;
		using (var objectSpace = CreateFileObjectSpace(folder, true, CreateStandardObjectSpace())) {
			savedAccount = CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		using (var objectSpace = CreateFileObjectSpace(folder, false, CreateStandardObjectSpace())) {
			loadedAccount = objectSpace.Get<Account>(0);
		}

		Assert.That(loadedAccount, Is.EqualTo(savedAccount).Using(accountComparer));
	}

	#endregion

	#region Rollback

	[Test]
	public void RollbackNotThrows() {
		var objectSpace = CreateObjectSpace();
		var account = CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Rollback, Throws.Nothing);
	}

	[Test]
	public void Rollback() {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var objectSpace = CreateFileObjectSpace(folder, true, CreateStandardObjectSpace())) {
			var account = CreateAccount();
			objectSpace.Save(account);
			objectSpace.Rollback();
		}

		using (var objectSpace = CreateFileObjectSpace(folder, false, CreateStandardObjectSpace())) {
			Assert.That(objectSpace.Count<Account>(), Is.EqualTo(0));
		}

	}

	[Test]
	public void Rollback_2() {
		// TODO: make sure previous committed state properly rolled back 
		// make sure to update prior state before rolback to ensure update is rolled back
	}
	
	#endregion


	#region Clear

	[Test]
	public void ClearCommit() {
		using var objectSpace = CreateObjectSpace();
		var savedAccount = CreateAccount();
		objectSpace.Save(savedAccount);
		objectSpace.Clear();
		objectSpace.Commit();

		foreach(var dim in objectSpace.Dimensions)
			Assert.That(dim.ObjectStream.Count, Is.EqualTo(0));
	}

	#endregion


}
