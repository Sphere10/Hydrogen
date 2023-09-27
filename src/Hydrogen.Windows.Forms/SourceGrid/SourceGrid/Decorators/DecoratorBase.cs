// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace SourceGrid.Decorators;

public abstract class DecoratorBase {
	public abstract bool IntersectWith(CellRange range);

	public abstract void Draw(RangePaintEventArgs e);
}
