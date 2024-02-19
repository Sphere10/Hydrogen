// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Text;
using NUnit.Framework;
using System.IO;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class TransactionalListTests {

	[Test]
	public void LoadWhenNotRequiredDoesntBreak_BugCase([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.True);
			txnFile.Load();
			Assert.That(txnFile.RequiresLoad, Is.False);
			Assert.That( () => txnFile.Load(), Throws.Nothing);
		}
	}

	[Test]
	public void AddOne([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}
	}


	[Test]
	public void IntegrationTests_Commit([Values(0, 1, 50)] int maxCapacity, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int StringMinSize = 0;
		const int StringMaxSize = 100;
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From( file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
			txnFile.Commit();
		}
	}

	[Test]
	public void IntegrationTests_Rollback([Values(0, 1, 50)] int maxCapacity, [ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		const int StringMinSize = 0;
		const int StringMaxSize = 100;
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
		using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy:policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
			txnFile.Rollback();
		}
	}


	[Test]
	public void CanLoadPreviouslyCommittedState([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
		}
	}


	[Test]
	public void CanUpdatePreviouslyCommittedState([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Updated");
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(2));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			Assert.That(txnFile[1], Is.EqualTo("Updated"));
		}

	}

	[Test]
	public void CanUpdatePreviouslyRolledBackState_1([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Updated");
			txnFile.Rollback();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
		}
	}

	[Test]
	public void CanUpdatePreviouslyAbandonedState_1([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));


		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Updated");
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
		}
	}

	[Test]
	public void CanUpdatePreviouslyRolledBackState_2([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Rollback();
		}

		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Updated");
			txnFile.Commit();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Updated"));
		}

	}

	[Test]
	public void RollbackReturnsToPreviousCommittedState_BeforeScopeCloses([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Update(0, "Changed");
			txnFile.Add("Updated");
			Assert.That(txnFile[0], Is.EqualTo("Changed"));
			txnFile.Rollback();

			// ensure rollbacked state before scope exits
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
		}

	}

	[Test]
	public void RollbackReturnsToPreviousCommittedState_AfterScopeCloses([ClusteredStreamsPolicyTestValues] ClusteredStreamsPolicy policy) {
		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);

		using var disposables = new Disposables();
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))));
		disposables.Add(Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))));
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Add("Hello World!");
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
			txnFile.Commit();
		}

		// Update previous state
		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8))) {
			txnFile.Load();
			txnFile.Update(0, "Changed");
			txnFile.Add("Updated");
			txnFile.Rollback();
		}

		Assert.That(File.Exists(file), Is.EqualTo(true));
		Assert.That(Directory.Exists(dir), Is.EqualTo(true));
		Assert.That(Tools.FileSystem.CountDirectoryContents(dir), Is.EqualTo(0));

		using (var txnFile = new TransactionalList<string>(HydrogenFileDescriptor.From(file, dir, containerPolicy: policy), new StringSerializer(Encoding.UTF8), accessMode: FileAccessMode.OpenOrCreate)) {
			Assert.That(txnFile.RequiresLoad, Is.EqualTo(true));
			txnFile.Load();
			Assert.That(txnFile.Count, Is.EqualTo(1));
			Assert.That(txnFile[0], Is.EqualTo("Hello World!"));
		}

	}


}
