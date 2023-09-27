// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

//using System;
//using System.Collections.Generic;
//using Hydrogen;
//using Terminal.Gui;
//using Hydrogen.DApp.Core.Configuration;
//using Hydrogen.DApp.Core.Consensus.Serializers;
//using Hydrogen.DApp.Core.Maths;
//using Hydrogen.DApp.Core.Mining;

//namespace Hydrogen.DApp.Node.UI {

//	[Title("Old Mining Simulator")]
//	[Lifetime(ScreenLifetime.WhenVisible)]
//	[MenuLocation(AppMenu.Development, "Old Mining Sim")]
//	public class OldMiningSimScreen : Screen {
//		private LogView _outputView;
//		private TextField _minersField;
//		private ILogger _outputLogger;
//		private Button _startButton;
//		private List<SingleThreadedMiner> _miners;
//		private IMiningManager _miningManager;

//		public OldMiningSimScreen() {
//			// mining manager
//			var powAlg = new MolinaTargetAlgorithm();
//			var conf = new SimConfiguration();
//			_miners = new List<SingleThreadedMiner>();
//			_miningManager = new TestMiningManager(CHF.SHA2_256, powAlg, new DA_ASERT2(powAlg, conf), new NewMinerBlockSerializer(), conf);
//			_miningManager.SolutionSubmited += (o, puzzle, arg3) => RefreshStats();
//			_miningManager.SolutionSubmited += (o, puzzle, result) => _outputLogger.Info($"Miner: {puzzle.Block.MinerTag}, Block: {puzzle.ComputeWork().ToHexString(true)}, Result: { result }");
//			_outputLogger = new TimestampLogger(new ActionLogger(s => _outputView.AppendLog(s)));
//		}

//		protected override void BuildUI() {
//			var commandBarHeight = 3;

//			// command frame
//			var commandFrame = new FrameView("Commands") {
//				X = 0,
//				Y = 0,
//				Width = Dim.Fill(),
//				Height = commandBarHeight,
//				CanFocus = true
//			};
//			this.Add(commandFrame);

//			// label
//			var label = new Label("Miners:") {
//				X = 0,
//				Y = 0,
//			};
//			commandFrame.Add(label);

//			// min_minersFielderField
//			_minersField = new TextField("1") {
//				X = Pos.Right(label),
//				Y = Pos.Y(label),
//				Width = 5,
//			};
//			commandFrame.Add(_minersField);
//			_minersField.TextChanging += ValidateMinerField;
//			_minersField.Leave += MinerFieldUpdate;

//			// start button
//			_startButton = new Button("Start", true) {
//				X = Pos.Right(_minersField) + 5,
//				Y = Pos.Y(_minersField)
//			};
//			_startButton.Clicked += StartButtonOnClicked;
//			commandFrame.Add(_startButton);

//			// output frame
//			var outputFrame = new FrameView("Output") {
//				X = 0,
//				Y = Pos.Bottom(commandFrame),
//				Height = Dim.Fill(),
//				Width = Dim.Fill(),
//				CanFocus = false
//			};
//			this.Add(outputFrame);

//			// _outputView
//			_outputView = new LogView("Output") {
//				X = 0,
//				Y = 0,
//				Width = Dim.Fill(),
//				Height = Dim.Fill()
//			};
//			outputFrame.Add(_outputView);
//		}

//		public bool Started { get; set; }

//		public int Miners {
//			get => int.Parse(_minersField.Text.ToString());
//			set => _minersField.Text = value.ToString();
//		}

//		public void Start() {
//			_miners.ForEach(x => x.Start());
//			Started = true;
//		}

//		public void Stop() {
//			_miners.ForEach(x => x.Stop());
//			Started = false;
//		}

//		private void RefreshStats() {

//		}

//		private void StartButtonOnClicked() {
//			if (!Started) {
//				Start(); ;
//				_startButton.Text = "&Stop";
//			} else {
//				Stop();
//				_startButton.Text = "&Start";
//			}
//		}

//		private void MinerFieldUpdate(FocusEventArgs obj) {
//			if (Miners == _miners.Count)
//				return;
//			if (Miners < _miners.Count) {
//				var diff = _miners.Count - Miners;
//				for (var i = 0; i < diff; i++) {
//					var miner = _miners[^1];
//					_miners.Remove(miner);
//					if (miner.Status == MinerStatus.Mining)
//						miner.Stop(); 
//				}
//			} else {
//				var diff = Miners - _miners.Count;
//				for(var i = 0; i < diff; i++) {
//					var miner = new SingleThreadedMiner($"Miner {_miners.Count + 1}", _miningManager);
//					_miners.Add(miner);
//					if (Started)
//						miner.Start();
//				}
//			}

//		}

//		private static void ValidateMinerField(TextChangingEventArgs args) {
//			if (args.NewText == string.Empty)
//				args.NewText = "0";
//			else args.Cancel = !uint.TryParse(args.NewText.ToString(), out _);
//		}

//		internal class SimConfiguration : IConfiguration {

//			public TimeSpan RTTInterval => TimeSpan.FromSeconds(2);

//			public TimeSpan NewMinerBlockTime => TimeSpan.FromSeconds(5);

//			public TimeSpan DAAsertRelaxationTime => TimeSpan.FromSeconds(5 * 10);

//		}


//	}


//}


