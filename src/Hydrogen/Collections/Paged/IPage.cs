// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen {
	public interface IPage<TItem> : IEnumerable<TItem> {
		int Number { get; set; }
		int StartIndex { get; set; }
		int EndIndex { get; set; }
		int Count { get; set; }
		int Size { get; set; }
		bool Dirty { get; set; }
		PageState State { get; set; }
		IEnumerable<TItem> Read(int index, int count);
		bool Write(int index, IEnumerable<TItem> items, out IEnumerable<TItem> overflow);
		void EraseFromEnd(int count);
	}
}
