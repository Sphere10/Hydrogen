// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms;

public static class LabelExtensions {
	public const int MaxLabelText = 50000;
	public static bool CanTextFit(this Label label, string text) {
		using (var g = label.CreateGraphics()) {
			if (text.Length > MaxLabelText)
				text = text.Substring(0, MaxLabelText);

			var size = g.MeasureString(text, label.Font, label.Width);
			return size.Width <= label.Size.Width && size.Height <= label.Size.Height;
		}
	}

	public static SizeF FitSize(this Label label, string text, int width) {
		using (var g = label.CreateGraphics()) {
			if (text.Length > MaxLabelText)
				text = text.Substring(0, MaxLabelText);

			return g.MeasureString(text, label.Font, width);
		}
	}

}
