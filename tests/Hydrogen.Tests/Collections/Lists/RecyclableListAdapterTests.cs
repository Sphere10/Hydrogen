// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Hydrogen.Tests;

public class RecyclableListAdapterTests : RecyclableListTestsBase {

	protected override IDisposable CreateList<T>(IItemSerializer<T> serializer, IEqualityComparer<T> comparer, out IRecyclableList<T> list) {
		list = new RecyclableListAdapter<T>(new ExtendedList<T>(comparer), new StackList<long>());
		return new Disposables();
	}

}
