// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Microsoft.AspNetCore.Components.Rendering;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public interface IGridComponent<TItem> {
		public void Render(TItem item, RenderTreeBuilder builder);
	}
}
