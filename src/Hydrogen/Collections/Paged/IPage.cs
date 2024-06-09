// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen;

public interface IPage<TItem> : IEnumerable<TItem> {
	long Number { get; set; }
	long StartIndex { get; set; }
	long EndIndex { get; set; }
	long Count { get; set; }
	long Size { get; set; }
	bool Dirty { get; set; }
	PageState State { get; set; }

	IEnumerable<TItem> Read(long index, long count);

	bool Write(long index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow);

	void EraseFromEnd(long count);
}
