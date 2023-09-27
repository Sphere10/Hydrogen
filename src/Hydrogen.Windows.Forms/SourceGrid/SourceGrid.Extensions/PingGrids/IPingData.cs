// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Extensions.PingGrids;

public interface IPingData {
	int Count { get; }
	bool AllowSort { get; set; }

	void ApplySort(string propertyName, bool ascending);

	object GetItemValue(int index, string propertyName);
}
