//-----------------------------------------------------------------------
// <copyright file="LargeCollectionTests.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using NUnit.Framework.Constraints;
using Sphere10.Framework;
using Sphere10.Framework.NUnit;

namespace Sphere10.Framework.Tests {

	[TestFixture]
	[Parallelizable(ParallelScope.Children)]
	public class TransactionalListTests {

		[Test]
		[Pairwise]
		public void IntegrationTests_Fixed_Commit([Values(0, 1, 200)] int maxCapacity) {
			const int StringMinSize = 0;
			const int StringMaxSize = 100;
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
			using (var txnFile = new TransactionalList<string>(new StringSerializer(Encoding.ASCII), file, dir, Guid.NewGuid(), Tools.Memory.ToBytes(10, MemoryMetric.Megabyte), Tools.Memory.ToBytes(5, MemoryMetric.Megabyte), maxCapacity)) {
				AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
				txnFile.Commit();
			}
		}

		[Test]
		[Pairwise]
		public void IntegrationTests_Fixed_Rollback([Values(0, 1, 200)] int maxCapacity) {
			const int StringMinSize = 0;
			const int StringMaxSize = 100;
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
			using (var txnFile = new TransactionalList<string>(new StringSerializer(Encoding.ASCII), file, dir, Guid.NewGuid(), Tools.Memory.ToBytes(10, MemoryMetric.Megabyte), Tools.Memory.ToBytes(5, MemoryMetric.Megabyte), maxCapacity)) {
				AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
				txnFile.Rollback();
			}
		}

		[Test]
		[Pairwise]
		public void IntegrationTests_Dynamic_Commit([Values(0, 1, 200)] int maxCapacity) {
			const int StringMinSize = 0;
			const int StringMaxSize = 100;
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
			using (var txnFile = new TransactionalList<string>(file, dir, Guid.NewGuid(), Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), new StringSerializer(Encoding.ASCII))) {
				AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
				txnFile.Commit();
			}
		}

		[Test]
		[Pairwise]
		public void IntegrationTests_Dynamic_Rollback([Values(0, 1, 200)] int maxCapacity) {
			const int StringMinSize = 0;
			const int StringMaxSize = 100;
			var file = Tools.FileSystem.GenerateTempFilename();
			var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file))))
			using (Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir))))
			using (var txnFile = new TransactionalList<string>(file, dir, Guid.NewGuid(), Tools.Memory.ToBytes(1, MemoryMetric.Megabyte), new StringSerializer(Encoding.ASCII))) {
				AssertEx.ListIntegrationTest(txnFile, maxCapacity, (rng, i) => Tools.Array.Gen(i, rng.NextString(StringMinSize, StringMaxSize)));
				txnFile.Rollback();
			}
		}
	}
}
