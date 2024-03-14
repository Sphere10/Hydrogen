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
using System.Text;
using NUnit.Framework;
using Hydrogen.NUnit;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public sealed class TransactionalDictionaryTests : TransactionalDictionaryTestsBase {

	protected override IDisposable Create<TKey, TValue>(
		IItemSerializer<TKey> keySerializer, 
		IItemSerializer<TValue> valueSerializer, 
		IEqualityComparer<TKey> keyComparer, 
		IEqualityComparer<TValue> valueComparer, 
		ClusteredStreamsPolicy policy,
		out ITransactionalDictionary<TKey, TValue> clustered, 
		out string file
	) {
		file = Tools.FileSystem.GenerateTempFilename();
		var dir = Tools.FileSystem.GetTempEmptyDirectory(true);
		var fn = file;
		var disposable1 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => File.Delete(fn)));
		var disposable2 = Tools.Scope.ExecuteOnDispose(() => Tools.Lambda.ActionIgnoringExceptions(() => Tools.FileSystem.DeleteDirectory(dir)));
		clustered = new TransactionalDictionary<TKey, TValue>(
			HydrogenFileDescriptor.From(
				file, 
				dir, 
				containerPolicy: policy
			),
			keySerializer, 
			valueSerializer,
			new ItemDigestor<TKey>(CHF.Blake2b_128, keySerializer, Endianness.LittleEndian), 
			keyComparer, 
			valueComparer, 
			implementation: StreamMappedDictionaryImplementation.KeyValueListBased,
			accessMode: FileAccessMode.OpenOrCreate
		);
		return new Disposables(disposable1, disposable2, clustered);
	}

}