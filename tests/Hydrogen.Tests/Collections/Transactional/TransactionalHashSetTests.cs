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
using NUnit.Framework;

namespace Hydrogen.Tests;

[TestFixture]
public class TransactionalHashSetTests : SetTestsBase {
	private const int DefaultClusterDataSize = 32;

	protected override IDisposable CreateSet<TValue>(IEqualityComparer<TValue> comparer, out ISet<TValue> set) {

		var file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		
		var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(file)));
		var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));

		var serializer = ItemSerializer<TValue>.Default;
		var hashset = new TransactionalHashSet<TValue>(HydrogenFileDescriptor.From(file, dir), serializer, comparer: comparer);
		if (hashset.RequiresLoad)
			hashset.Load();
		set = hashset;
		return new Disposables(disposable1, disposable2, hashset);

	}
}
