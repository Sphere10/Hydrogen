// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid;

public interface IRows {
	bool IsRowVisible(int row);


	void HideRow(int row);

	void ShowRow(int row);

	/// <summary>
	/// Use this method to show or hide row
	/// </summary>
	/// <param name="row"></param>
	/// <param name="isVisible"></param>
	void ShowRow(int row, bool isVisible);
}
