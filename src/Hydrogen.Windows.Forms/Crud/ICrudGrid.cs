// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Windows.Forms;

public interface ICrudGrid {
	Type EntityEditorDisplay { get; }
	string GridTitle { get; }
	DataSourceCapabilities Capabilities { get; }
	IEnumerable<ICrudGridColumn> GridBindings { get; }

	void SetDataSource<TEntity>(ICrudDataSource<TEntity> dataSource);
}
