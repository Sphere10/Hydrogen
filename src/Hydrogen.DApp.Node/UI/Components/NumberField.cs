// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI.Components;

public class IntegerField : TextField {
	private long _min;
	private long _max;

	public IntegerField(long value, long min, long max, Action<long> changed = null) {
		_min = min;
		_max = max;
		Value = value;
		AutoClip = true;
		this.Text = $"{Value}";
		base.TextChanging += args => {
			args.Cancel = !TryParse(args.NewText.ToString(), out var newText, out _);
			if (!args.Cancel)
				args.NewText = newText;
		};
		base.TextChanged += x => {
			Value = Parse(x.ToString());
			changed?.Invoke(Value);
		};
	}

	public bool AutoClip { get; set; }

	public long Value { get; private set; }

	private bool TryParse(string text, out string valueText, out long value) {
		if (long.TryParse(text, out var parsedVal)) {
			if (_min <= parsedVal && parsedVal <= _max) {
				value = parsedVal;
				valueText = text;
				return true;
			}
			if (AutoClip) {
				value = parsedVal.ClipTo(_min, _max);
				valueText = value.ToString();
				return true;
			}
		}
		value = default;
		valueText = default;
		return false;
	}

	private long Parse(string text) {
		if (!TryParse(text, out _, out var val))
			throw new InternalErrorException("8A01B4CE-081F-41D3-AFFF-45F281C8D4B2");
		return val;
	}


}
