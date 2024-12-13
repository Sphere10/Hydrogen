using System;
using System.Collections.Generic;
using System.IO;
using Hydrogen.ObjectSpaces;
using NUnit.Framework;
using Hydrogen;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Hydrogen.Tests.ObjectSpaces;

[TestFixture]
public class FileMappedTests {

	#region Load

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void LoadEmptyDoesntThrow(TestTraits traits) {
		// Create folder explicitly (will be retained after object space dispose)
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));

		// Create object space and keep files after dispose
		var activationArgs = new Dictionary<string, object> { ["folder"] = folder };
		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, activationArgs)) {
			objectSpace.Commit();
		}

		// Load object space from same files
		Assert.That(() => {
			using var space = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, activationArgs);
			if (space.RequiresLoad)
				space.Load();
		}, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void LoadDoesntThrow(TestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var disposables = new Disposables(Tools.Scope.DeleteDirOnDispose(folder));

		var activationArgs = new Dictionary<string, object> { ["folder"] = folder };
		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, activationArgs)) {
			var savedAccount = TestsHelper.CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		// Load object space from same files
		Assert.That(() => {
			using var space = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, activationArgs);
			if (space.RequiresLoad)
				space.Load();
		}, Throws.Nothing);
	}

	#endregion

	#region Commit

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void CommitNotThrows(TestTraits traits) {
		using var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits);
		var account = TestsHelper.CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Commit, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void CommitSaves(TestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		var accountComparer = TestsHelper.CreateAccountComparer();
		Account savedAccount, loadedAccount;
		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			savedAccount = TestsHelper.CreateAccount();
			objectSpace.Save(savedAccount);
			objectSpace.Commit();
		}

		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			loadedAccount = objectSpace.Get<Account>(0);
		}

		Assert.That(loadedAccount, Is.EqualTo(savedAccount).Using(accountComparer));
	}

	#endregion

	#region Rollback

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void RollbackNotThrows(TestTraits traits) {
		using var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits);
		var account = TestsHelper.CreateAccount();
		objectSpace.Save(account);
		Assert.That(objectSpace.Rollback, Throws.Nothing);
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void Rollback(TestTraits traits) {
		var folder = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			var account = TestsHelper.CreateAccount();
			objectSpace.Save(account);
			objectSpace.Rollback();
		}

		using (var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits, new Dictionary<string, object> { ["folder"] = folder })) {
			Assert.That(objectSpace.Count<Account>(), Is.EqualTo(0));
		}
	}

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void Rollback_2(TestTraits traits) {
		// TODO: make sure previous committed state properly rolled back 
		// make sure to update prior state before rollback to ensure update is rolled back
	}

	#endregion

	#region Clear

	[Test]
	[TestCaseSource(typeof(TestsHelper), nameof(TestsHelper.FileMappedTestCases))]
	public void ClearCommit(TestTraits traits) {
		using var objectSpace = (FileObjectSpace)TestsHelper.CreateObjectSpace(traits);
		var savedAccount = TestsHelper.CreateAccount();
		objectSpace.Save(savedAccount);
		objectSpace.Clear("I CONSENT TO CLEAR ALL DATA");
		objectSpace.Commit();

		foreach (var dim in objectSpace.Dimensions)
			Assert.That(dim.Container.ObjectStream.Count, Is.EqualTo(0));
	}

	#endregion
}
