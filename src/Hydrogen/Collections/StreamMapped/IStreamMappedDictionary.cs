﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;


namespace Hydrogen;

public interface IStreamMappedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ILoadable, IDisposable {

	ObjectContainer ObjectContainer { get; }

	TKey ReadKey(long index);

	internal byte[] ReadKeyBytes(long index);

	TValue ReadValue(long index);

	internal byte[] ReadValueBytes(long index);

	bool TryFindKey(TKey key, out long index);

	bool TryFindValue(TKey key, out long index, out TValue value);

	void RemoveAt(long index);

}
