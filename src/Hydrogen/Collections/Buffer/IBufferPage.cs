// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public interface IBufferPage : IMemoryPage<byte> {
	ReadOnlySpan<byte> ReadSpan(long index, long count);

	bool AppendSpan(ReadOnlySpan<byte> items, out ReadOnlySpan<byte> overflow);

	void UpdateSpan(long index, ReadOnlySpan<byte> items);
}
