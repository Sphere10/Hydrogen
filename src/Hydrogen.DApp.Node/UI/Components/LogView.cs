// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

public class LogView : FrameView {
	private const int DefaultMaxLines = 250;
	private readonly IList<string> _lines;
	private readonly TextView _textView;
	private Throttle throttle;

	private int _maxLines;

	public LogView(string title) : base(title) {
		MaxLines = 250;
		_lines = new List<string>(MaxLines);
		_textView = new TextView {
			X = 0,
			Y = 0,
			Width = Dim.Fill(),
			Height = Dim.Fill(),
			CanFocus = false,
			ReadOnly = true
		};
		throttle = new Throttle(TimeSpan.FromMilliseconds(250));
		this.Add(_textView);

	}

	public int MaxLines {
		get => _maxLines;
		set {
			Guard.ArgumentInRange(value, 1, int.MaxValue, nameof(value));
			_maxLines = value;
		}
	}

	public string Contents => FastStringBuilder.From(_lines, true);

	public void AppendLog(string text) {

		while (_lines.Count > _maxLines)
			_lines.RemoveAt(0);
		_lines.Add(text);
		_textView.Text = Contents;
		if (_textView.IsInitialized)
			_textView.MoveEnd();

		//TODO: avoid invoke if on main thread
		Terminal.Gui.Application.MainLoop.Invoke(() => _textView.SetNeedsDisplay());
	}

	public void ClearLog() {
		_lines.Clear();
		_textView.Text = string.Empty;
		Terminal.Gui.Application.MainLoop.Invoke(() => _textView.SetNeedsDisplay());
	}

}
