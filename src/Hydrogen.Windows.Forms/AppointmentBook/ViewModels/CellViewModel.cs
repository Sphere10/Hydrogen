// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Drawing;

namespace Hydrogen.Windows.Forms.AppointmentBook;

public sealed class CellViewModel {

	public CellViewModel() {
	}

	public CellViewModel(string text, CellTraits traits, FontStyle fontStyle = FontStyle.Regular) {
		Text = text;
		TextColor = Color.Black;
		BackColor = Color.White;
		FontStyle = FontStyle.Regular;
		Alignment = ContentAlignment.MiddleLeft;
		Traits = traits;
	}

	public ContentAlignment Alignment { get; set; }
	public string Text { get; set; }
	public Color TextColor { get; set; }
	public Color BackColor { get; set; }
	public FontStyle FontStyle { get; set; }
	public CellTraits Traits { get; set; }
	public object GridCellObject { get; set; }

	public static CellViewModel Empty {
		get { return new CellViewModel(string.Empty, CellTraits.Empty); }
	}

	public static CellViewModel[] CreateArray(int count) {
		try {
			var arr = new CellViewModel[count];
			for (int i = 0; i < count; i++) {
				arr[i] = CellViewModel.Empty;
			}
			return arr;
		} catch (Exception error) {
			SystemLog.Exception(error);
			ExceptionDialog.Show(error);
			return new CellViewModel[0];
		}
	}
}
