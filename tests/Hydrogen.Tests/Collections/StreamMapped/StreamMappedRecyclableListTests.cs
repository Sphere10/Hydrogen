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
using System.Linq;
using System.Text;
using NUnit.Framework;
using Hydrogen.NUnit;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Hydrogen.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class StreamMappedRecyclableListTests : RecyclableListTestsBase {

	protected override IDisposable CreateList<T>(IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out IRecyclableList<T> list) {
		var stream = new MemoryStream();
		var smrlist = StreamMappedFactory.CreateRecyclableList<T>(
			stream,
			32, 
			serializer,
			autoLoad: true
		);
		list = smrlist;
		return new Disposables(smrlist, stream);
	}

}