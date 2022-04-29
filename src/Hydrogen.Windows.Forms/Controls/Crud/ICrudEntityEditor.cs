//-----------------------------------------------------------------------
// <copyright file="ICrudEntityEditor.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {
	public interface ICrudEntityEditor<TEntity> {
		Control AsControl();
		void SetEntity(DataSourceCapabilities capabilities, TEntity entity, bool isNewEntity);
		TEntity GetEntityWithChanges();
		bool HasChanges { get; }
		void UndoChanges();
		void AcceptChanges();
	}
}
