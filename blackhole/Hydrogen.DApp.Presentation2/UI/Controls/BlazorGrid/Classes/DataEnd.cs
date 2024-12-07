// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: David Price
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Microsoft.AspNetCore.Components.Rendering;

namespace Hydrogen.DApp.Presentation2.UI.Controls.BlazorGrid.Classes {
	public class DataEnd<TInItem, TOutItem> {
		public string Name { get; set; }
		public Func<object, object> DataExtractor { get; set; }

		public DataEnd() {
		}

		public DataEnd(string name, Func<object, object> dataExtractor) {
			Name = name;
			DataExtractor = dataExtractor;
		}

		public void Render(object item, RenderTreeBuilder builder) {
			var extractedData = (IColumnDefinition)DataExtractor(item);
			var componentType = extractedData.GetComponentType();
			dynamic component = Activator.CreateInstance(componentType);
			component.Render((TOutItem)extractedData, builder);
		}
	}
}
