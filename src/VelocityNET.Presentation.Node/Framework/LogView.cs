using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Collections.Lists;
using Terminal.Gui;

namespace VelocityNET.Presentation.Node {
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

		public string Output => FastStringBuilder.From(_lines, true);

		public void AppendLog(string text) {
			
			while (_lines.Count > _maxLines) 
				_lines.RemoveAt(0);
			_lines.Add(text);
			_textView.Text = Output;
			if (_textView.IsInitialized)
				_textView.MoveEnd();
			//if (_realtimeUpdate)
				Application.MainLoop.Invoke(() => _textView.SetNeedsDisplay());
			//else _textView.SetNeedsDisplay();
		}

		
	}
}
