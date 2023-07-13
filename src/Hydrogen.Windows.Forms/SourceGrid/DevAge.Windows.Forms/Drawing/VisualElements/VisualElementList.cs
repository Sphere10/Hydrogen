// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace DevAge.Drawing.VisualElements;

[Serializable]
public class VisualElementList : List<IVisualElement>, ICloneable {

	#region ICloneable Members

	public object Clone() {
		VisualElementList elements = new VisualElementList();
		foreach (IVisualElement element in this) {
			elements.Add((IVisualElement)element.Clone());
		}

		return elements;
	}

	#endregion

}
