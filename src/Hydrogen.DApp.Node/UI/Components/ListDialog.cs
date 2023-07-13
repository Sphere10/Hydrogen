// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public class ListDialog<T> : Dialog {
	private const int LeftPadding = 2;
	private const int TopPadding = 1;
	private const int RightPadding = 2;
	private const int BottomPadding = 2;
	private const int DescriptionHeight = 2;

	public ListDialog(string title, string description, ListDataSource<T> datasource, int selected = 0)
		: base(title) {


		var txt = new TextView {
			X = LeftPadding,
			Y = TopPadding,
			Width = Dim.Fill(),
			Height = DescriptionHeight,
			TextAlignment = TextAlignment.Centered,
			Text = description,
			CanFocus = false
		};
		this.Add(txt);

		var list = new ListView() {
			X = LeftPadding,
			Y = Pos.Bottom(txt),
			Width = Dim.Fill(),
			Height = Dim.Fill()
		};
		list.SetSource(datasource.Labels);
		list.SelectedItemChanged += args => {
			SelectedIndex = args.Item;
			SelectedValue = datasource.Items[args.Item];
		};

		list.KeyPress += args => {
			if (args.KeyEvent.Key.HasFlag(Terminal.Gui.Key.Esc))
				Cancelled = true;

			if (args.KeyEvent.Key.HasFlag(Terminal.Gui.Key.Enter))
				Terminal.Gui.Application.RequestStop();
		};
		list.SelectedItem = selected;
		list.SetFocus();
		this.Add(list);

		this.Width = Math.Max(datasource.MaxLen, description.Length) + LeftPadding + RightPadding + 2;
		this.Height = TopPadding + txt.Height + 1 + datasource.Count + BottomPadding;
	}

	public int SelectedIndex { get; private set; }

	public T SelectedValue { get; private set; }

	public bool Cancelled { get; private set; }

}
