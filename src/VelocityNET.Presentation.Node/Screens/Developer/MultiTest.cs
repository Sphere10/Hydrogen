using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using Sphere10.Framework;
using Terminal.Gui;
using Tools;
using VelocityNET.Core.Configuration;
using VelocityNET.Core.Consensus.Serializers;
using VelocityNET.Core.Maths;
using VelocityNET.Core.Mining;

namespace VelocityNET.Presentation.Node {

	[Title("Multi Part Simulator")]
	[Lifetime(ScreenLifetime.WhenVisible)]
	[MenuLocation(AppMenu.Development, "Multi Part", 2)]
	public class MultiTest : MultiPartScreen<Dictionary<string, string>> {

		public MultiTest()
		 : base(new Dictionary<string, string>(), new [] {  new MultiTest.LogScreen(), new MultiTest.LogScreen() }) {
		}


		[Title("Log")]
		public class LogScreen : Screen<Dictionary<string, string>> {
			private LogView _outputView;
			private Timer _timer;

			public LogScreen() {
			
			}
			protected override void BuildUI() {
				// _outputView
				_outputView = new LogView("Output") {
					X = 0,
					Y = 1,
					Width = Dim.Fill(),
					Height = Dim.Fill(),
					Text = "ctor() " + Guid.NewGuid()
				};
				_outputView.AppendLog("ctor() " + Guid.NewGuid());

				this.Add(new TextView() { X=0, Y=0, Width = Dim.Fill(), Height = 1, Text = Guid.NewGuid().ToString()});
				this.Add(_outputView);
				
			}

			public override void OnAppearing() {
				base.OnAppearing();
				_outputView.AppendLog("OnAppearing " +Guid.NewGuid());
				_timer = new Timer(state => {
					_outputView?.AppendLog("OnTimer " + Guid.NewGuid());
				}, null, TimeSpan.Zero, TimeSpan.FromSeconds(0.5));

			}

			public override void OnAppeared() {
				base.OnAppeared();
				
			}

			public override void OnDisappeared() {
				base.OnDisappeared();
				_timer?.Dispose();
			}
		}
	}

}
