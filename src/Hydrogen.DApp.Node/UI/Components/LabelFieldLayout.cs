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

public class LabelFieldLayout : View {
	private readonly int _leftPadding;
	private readonly int _topPadding;
	private readonly int _vspacing;
	private readonly int _labelLength;
	private int _fieldCount;

	public LabelFieldLayout(int leftPadding, int topPadding, int vspacing, int labelLength) {
		X = 0;
		Y = 0;
		Width = Dim.Fill();
		Height = Dim.Fill();
		_leftPadding = leftPadding;
		_topPadding = topPadding;
		_vspacing = vspacing;
		_labelLength = labelLength;
		_fieldCount = 0;
	}

	public void AddField(string label, View field, Dim fieldWidth) {
		var lbl = new Label(label) {
			X = _leftPadding,
			Y = _topPadding + _fieldCount++ * (_vspacing + 1),
			Width = _labelLength,
			Height = 1,
			TextAlignment = TextAlignment.Right
		};
		this.Add(lbl);

		field.X = Pos.Right(lbl) + 2;
		field.Y = Pos.Y(lbl);
		if (fieldWidth != null)
			field.Width = fieldWidth;
		this.Add(field);
	}

	public void AddTextBox(string label, string preText, Func<string, bool> validate = null, Action<string> changed = null, Dim textWidth = null) {
		var txt = new TextField(preText) { Width = Dim.Fill() };
		if (validate != null)
			txt.TextChanging += args => { args.Cancel = !validate.Invoke(args.NewText.ToString()); };

		if (changed != null)
			txt.TextChanged += x => changed.Invoke(x.ToString());

		AddField(label, txt, textWidth ?? Dim.Fill());
	}

	public void AddButton(string label, string buttonText, Action pressed) {
		AddButton(label, buttonText, out _, pressed);
	}

	public void AddButton(string label, string buttonText, out Button button, Action pressed) {
		button = new Button(buttonText) { AutoSize = true, HotKeySpecifier = '\xffff' };
		button.Clicked += pressed;
		AddField(label, button, null);
	}

	public void AddEnum<T>(string label, string dialogDesc, Func<T> fetchCurrValue, Action<T> changed) where T : struct, Enum
		=> AddEnum<T>(label,
			dialogDesc,
			fetchCurrValue,
			x => {
				changed(x);
				return true;
			});

	public void AddEnum<T>(string label, string dialogDesc, Func<T> fetchCurrValue, Func<T, bool> changed) where T : struct, Enum {
		Button btn = null;
		AddButton(
			label,
			fetchCurrValue().ToString(),
			out btn,
			() => {
				var val = fetchCurrValue();
				if (Dialogs.SelectEnum(label, dialogDesc, val, out var selection)) {
					if (!selection.Equals(val)) {
						if (changed(selection)) {
							btn.Text = selection.ToString();
						}
					}
				}
			});

	}

}
