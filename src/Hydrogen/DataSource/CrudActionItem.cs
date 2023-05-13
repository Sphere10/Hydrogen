// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen {
	public class CrudActionItem<TItem> {
		public CrudAction CrudAction { get; init; }
		public TItem Item { get; set; }

		public CrudActionItem(CrudAction crudAction, TItem item) {	
			CrudAction = crudAction;
			Item = item;
		}
	}
}