// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace SourceGrid.Extensions.PingGrids;

public class ListPingSource<T> : List<T>, IPingData {
	public bool AllowSort {
		get { return true; }
		set { }
	}

	public void ApplySort(string propertyName, bool @ascending) {
		this.Sort();
	}

	public object GetItemValue(int index, string propertyName) {
		throw new NotImplementedException();
	}

}


public class EmptyPingSource : IPingData {
	public object GetItemValue(int index, string propertyName) {
		throw new NotImplementedException();
	}

	public int Count {
		get { return 0; }
		set { }
	}

	public bool AllowSort {
		get { return false; }
		set { }
	}

	public void ApplySort(string propertyName, bool @ascending) {
		throw new NotImplementedException();
	}
}
