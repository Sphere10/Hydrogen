// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Terminal.Gui;

namespace Hydrogen.DApp.Node.UI;

[Title("Tabbed Screen Test")]
[Lifetime(ScreenLifetime.WhenVisible)]
[MenuLocation(AppMenu.Development, "Multi Part", int.MaxValue)]
public class TabbedScreenTest : TabbedScreen<TabbedScreenTest.Model> {

	public TabbedScreenTest()
		: base(new Model(), new LogScreen(), new OptionsScreen(), new LogScreen(), new OptionsScreen() { Enabled = false }) {
	}


	[Title("Log")]
	public class LogScreen : Screen<Model> {
		private LogView _outputView;
		private Timer _timer;
		private int _count = 0;

		protected override void LoadInternal() {

			// _outputView
			_outputView = new LogView("Output") {
				X = 0,
				Y = 1,
				Width = Dim.Fill(),
				Height = Dim.Fill(),
				Text = "ctor() " + Guid.NewGuid()
			};
			_outputView.AppendLog($"ctor() {++_count} {Guid.NewGuid()}");
			this.Add(new TextView() { X = 0, Y = 0, Width = Dim.Fill(), Height = 1, Text = Guid.NewGuid().ToString() });
			this.Add(_outputView);
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			_outputView.AppendLog($"OnAppearing {++_count} {Guid.NewGuid()}");
			_timer ??= new Timer(
				state => { _outputView?.AppendLog($"OnTimer {++_count} {Guid.NewGuid()}"); },
				null,
				TimeSpan.Zero,
				TimeSpan.FromSeconds(0.5)
			);
		}

		protected override void OnDisappeared() {
			base.OnDisappeared();
			_timer?.Dispose();
			_timer = null;
		}
	}


	[Title("Options")]
	public class OptionsScreen : FramedScreen<Model> {

		protected override void LoadInternal() {
			base.LoadInternal();
			var labelFieldLayout = new LabelFieldLayout(2, 2, 1, 25);
			labelFieldLayout.AddField("Label 1", new TextField("test text"), Dim.Fill());
			labelFieldLayout.AddTextBox("Longer Label 2", "UPPER ONLY", s => s.All(c => char.IsUpper(c) || char.IsWhiteSpace(c)));
			labelFieldLayout.AddButton("Some other large label", "Button Label", () => MessageBox.Query("Title", "Message", "Button1", "Button2"));
			labelFieldLayout.AddButton("Enum Selection",
				"Button Label",
				() => {
					if (Dialogs.SelectEnum<EnumOption>("Select EnumOption", "Please select the value from this enum", EnumOption.Option2, out var selection))
						MessageBox.Query("Title", $"Selected '{selection.ToString()}'");
				});
			this.Add(labelFieldLayout);
		}
	}


	public class Model {
		public bool EnableLog { get; set; }

		public EnumOption Option { get; set; }
	}


	public enum EnumOption {
		[Description("Option 1 text")] Option1,

		[Description("Option 2 text")] Option2,

		[Description("Option 3 text")] Option3,

	}
}
