// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class IndexCountCallArgs : CallArgs {

	public IndexCountCallArgs(long index, long count) : base(index, count) {
	}

	public long Index {
		get => (int)base[0];
		set => base[0] = value;
	}

	public long Count {
		get => (int)base[1];
		set => base[1] = value;
	}
}
