using System;
using System.Collections.Generic;
using System.IO;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;
using Hydrogen;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Hydrogen.Tests;

[TestFixture]
public class FileMappedSpecificTests {

	#region Load

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void LoadEmptyDoesntThrow(ObjectSpaceTestTraits traits) {
		// Create folder explicitly (will be retained after object space dispose)
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));

		// Create object space and keep files after dispose
		var activationArgs = new Dictionary<string, object> { ["folder"] = folder };
		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, activationArgs)) {
			objectSpace.Commit();
		}

		// Load object space from same files
		Assert.That(() => {
			using var space = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, activationArgs);
			if (space.RequiresLoad)
				space.Load();
		}, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void LoadDoesntThrow(ObjectSpaceTestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));

		var activationArgs = new Dictionary<string, object> { ["folder"] = folder };
		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, activationArgs)) {
			var savedAccount = ObjectSpaceTestsHelper.CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		// Load object space from same files
		Assert.That(() => {
			using var space = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, activationArgs);
			if (space.RequiresLoad)
				space.Load();
		}, Throws.Nothing);
	}

	#endregion

	#region Commit

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void CommitNotThrows(ObjectSpaceTestTraits traits) {
		using var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits);
		var account = ObjectSpaceTestsHelper.CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Commit, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void CommitSaves(ObjectSpaceTestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var accountComparer = ObjectSpaceTestsHelper.CreateAccountComparer();
		Account savedAccount, loadedAccount;
		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			savedAccount = ObjectSpaceTestsHelper.CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			loadedAccount = objectSpace.Get<Account>(0);
		}

		Assert.That(loadedAccount, Is.EqualTo(savedAccount).Using(accountComparer));
	}

	#endregion

	#region Rollback

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void RollbackNotThrows(ObjectSpaceTestTraits traits) {
		using var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits);
		var account = ObjectSpaceTestsHelper.CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Rollback, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void Rollback(ObjectSpaceTestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			var account = ObjectSpaceTestsHelper.CreateAccount();
			objectSpace.Save(account);
			objectSpace.Rollback();
		}

		using (var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			Assert.That(objectSpace.Count<Account>(), Is.EqualTo(0));
		}
	}

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void Rollback_2(ObjectSpaceTestTraits traits) {
		// TODO: make sure previous committed state properly rolled back 
		// make sure to update prior state before rollback to ensure update is rolled back
	}

	#endregion

	#region Clear

	[Test]
	[TestCaseSource(typeof(ObjectSpaceTestsHelper), nameof(ObjectSpaceTestsHelper.FileMappedTestCases))]
	public void ClearCommit(ObjectSpaceTestTraits traits) {
		using var objectSpace = (FileObjectSpace)ObjectSpaceTestsHelper.CreateStandard(traits);
		var savedAccount = ObjectSpaceTestsHelper.CreateAccount();
		objectSpace.Save(savedAccount);
		objectSpace.Clear("I CONSENT TO CLEAR ALL DATA");
		objectSpace.Commit();

		foreach (var dim in objectSpace.Dimensions)
			Assert.That(dim.Container.ObjectStream.Count, Is.EqualTo(0));
	}

	#endregion
}
