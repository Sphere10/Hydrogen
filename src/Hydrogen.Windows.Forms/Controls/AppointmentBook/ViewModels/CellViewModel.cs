//-----------------------------------------------------------------------
// <copyright file="CellViewModel.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Linq;
using System.Text;
using Hydrogen;

namespace Hydrogen.Windows.Forms.AppointmentBook {

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

		public static CellViewModel Empty { get { return new CellViewModel(string.Empty, CellTraits.Empty); } }

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
}
