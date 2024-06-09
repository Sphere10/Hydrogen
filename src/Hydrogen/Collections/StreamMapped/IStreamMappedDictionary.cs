// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;

public interface IStreamMappedDictionary : IStreamMappedCollection {
	internal byte[] ReadKeyBytes(long index);
	internal byte[] ReadValueBytes(long index);
}

public interface IStreamMappedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IStreamMappedDictionary, ILoadable, IDisposable  {

	TKey ReadKey(long index);

	TValue ReadValue(long index);

	bool TryFindKey(TKey key, out long index);

	bool TryFindValue(TKey key, out long index, out TValue value);

	void RemoveAt(long index);

	new void Clear();

}
