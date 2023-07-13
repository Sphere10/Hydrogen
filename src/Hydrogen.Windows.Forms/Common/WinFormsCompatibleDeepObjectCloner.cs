// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;
using System.Drawing;

namespace Hydrogen.Windows.Forms;

public class WinFormsCompatibleDeepObjectCloner : DeepObjectCloner {
	public WinFormsCompatibleDeepObjectCloner() {
		base.DontCloneTypes.AddRange(new[] { typeof(Font), typeof(Color) });
	}

	protected override object DeepClone(object source, IDictionary<Reference<object>, object> clones) {
		if (source is Bitmap) {
			return new Bitmap((Bitmap)source);
		}
		return base.DeepClone(source, clones);
	}

}
