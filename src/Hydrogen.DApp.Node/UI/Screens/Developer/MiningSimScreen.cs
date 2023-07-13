// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Terminal.Gui;
using Hydrogen.DApp.Core.Maths;
using Hydrogen.DApp.Core.Mining;
using Hydrogen.DApp.Core.Consensus.Serializers;
using Hydrogen.DApp.Node.UI.Components;

namespace Hydrogen.DApp.Node.UI;

[Title("Mining Simulator")]
[Lifetime(ScreenLifetime.WhenVisible)]
[MenuLocation(AppMenu.Development, "Mining Sim")]
public class MiningSimScreen : TabbedScreen<MiningSimScreen.MiningSimModel> {

	public MiningSimScreen()
		: base(MiningSimModel.Default, new ConfigScreen(), new OutputScreen()) {
	}

	protected override IEnumerable<StatusItem> BuildStatusItemsInternal() {
		yield return new StatusItem(Terminal.Gui.Key.F1,
			"~[F1]~ Start/Stop",
			() => {
				if (Model.IsStarted) {
					Model.Stop();
				} else {
					Model.Start();
				}
			});
	}


	[Title("Config")]
	public class ConfigScreen : FramedScreen<MiningSimModel> {
		private IntegerField _minerCountField;
		protected override void LoadInternal() {
			base.LoadInternal();
			var labelFieldLayout = new LabelFieldLayout(2, 2, 1, 25);
			_minerCountField = new IntegerField(this.Model.MinerCount, MiningSimModel.MinMiners, MiningSimModel.MaxMiners, x => { Model.MinerCount = (int)x; });
			labelFieldLayout.AddField("Miner Count", _minerCountField, Dim.Sized(10));
			labelFieldLayout.AddEnum("Difficulty Algorithm", "Select the difficulty algorithm which the simulation will use", () => Model.DAA, x => { Model.DAA = x; });
			labelFieldLayout.AddEnum("Hash Algorithm", "Select the hash algorithm which the simulation will use", () => Model.Hash, x => { Model.Hash = x; });
			labelFieldLayout.AddField("Block Time", new IntegerField(this.Model.BlockTime, 1, 10 * 60, x => { Model.BlockTime = (int)x; }), Dim.Sized(10));
			labelFieldLayout.AddField("Relaxation Time", new IntegerField(this.Model.RelaxationTime, 1, 99999, x => { Model.RelaxationTime = (int)x; }), Dim.Sized(10));
			labelFieldLayout.AddField("Real-Time Interval", new IntegerField(this.Model.RTTInterval, 1, 99999, x => { Model.RTTInterval = (int)x; }), Dim.Sized(10));
			this.Add(labelFieldLayout);
		}

		public override void OnModelChanged() {
			base.OnModelChanged();
			this.Model.Started += x => this.Enabled = false;
			this.Model.Stopped += x => this.Enabled = true;
		}

		protected override void OnAppearing() {
			base.OnAppearing();
			_minerCountField.Text = Model.MinerCount.ToString();
		}
	}


	[Title("Output")]
	public class OutputScreen : Screen<MiningSimModel> {
		private ILogger _outputLogger;
		private LogView _log;

		protected override void LoadInternal() {
			this.X = 0;
			this.Y = 0;
			this.Width = Dim.Fill();
			this.Height = Dim.Fill();
			_log = new LogView("Mining Events") {
				X = 0,
				Y = 0,
				Width = Dim.Fill(),
				Height = Dim.Fill(7),
			};
			_outputLogger = new TimestampLogger(new ActionLogger(s => _log.AppendLog(s)));
			this.Add(_log);

			var statsFrame = new FrameView("Statistics") {
				X = 0,
				Y = Pos.Bottom(_log),
				Width = Dim.Fill(),
				Height = Dim.Fill()
			};
			this.Add(statsFrame);

			const int labelWidth = 15;
			const int valueWidth = 6;
			var targetLabel = new Label("Target: ") { X = 0, Y = 0, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var targetValue = new Label("N/A") { X = Pos.Right(targetLabel), Y = 0, AutoSize = false, Width = valueWidth };
			var blockCountLabel = new Label("Blocks: ") { X = Pos.Right(targetValue), Y = 0, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var blockCountValue = new Label("0") { X = Pos.Right(blockCountLabel), Y = 0, Width = valueWidth, TextAlignment = TextAlignment.Left };
			statsFrame.Add(targetLabel, targetValue, blockCountLabel, blockCountValue);

			var avgBlockTimeLabel = new Label("Avg: ") { X = 0, Y = 1, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var avgBlockTimeValue = new Label("N/A") { X = Pos.Right(avgBlockTimeLabel), Y = 1, AutoSize = false, Width = valueWidth };
			var stdBlockTimeLabel = new Label("Std: ") { X = Pos.Right(avgBlockTimeValue), Y = 1, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var stdBlockTimeValue = new Label("N/A") { X = Pos.Right(stdBlockTimeLabel), Y = 1, AutoSize = false, Width = valueWidth };
			var minMaxBlockTimeLabel = new Label("Min/Max: ") { X = Pos.Right(stdBlockTimeValue), Y = 1, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var minMaxBlockTimeValue = new Label("N/A") { X = Pos.Right(minMaxBlockTimeLabel), Y = 1, AutoSize = false, Width = Dim.Fill() };
			statsFrame.Add(avgBlockTimeLabel, avgBlockTimeValue, stdBlockTimeLabel, stdBlockTimeValue, minMaxBlockTimeLabel, minMaxBlockTimeValue);

			var last5AvgBlockTimeLabel = new Label("Last 5 Avg: ") { X = 0, Y = 2, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last5AvgBlockTimeValue = new Label("N/A") { X = Pos.Right(last5AvgBlockTimeLabel), Y = 2, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last5StdBlockTimeLabel = new Label("Std: ") { X = Pos.Right(last5AvgBlockTimeValue), Y = 2, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last5StdBlockTimeValue = new Label("N/A") { X = Pos.Right(last5StdBlockTimeLabel), Y = 2, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last5MinMaxBlockTimeLabel = new Label("Min/Max: ") { X = Pos.Right(last5StdBlockTimeValue), Y = 2, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last5MinMaxBlockTimeValue = new Label("N/A") { X = Pos.Right(last5MinMaxBlockTimeLabel), Y = 2, Width = Dim.Fill(), TextAlignment = TextAlignment.Left };
			statsFrame.Add(last5AvgBlockTimeLabel, last5AvgBlockTimeValue, last5StdBlockTimeLabel, last5StdBlockTimeValue, last5MinMaxBlockTimeLabel, last5MinMaxBlockTimeValue);

			var last10AvgBlockTimeLabel = new Label("Last 10 Avg: ") { X = 0, Y = 3, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last10AvgBlockTimeValue = new Label("N/A") { X = Pos.Right(last10AvgBlockTimeLabel), Y = 3, AutoSize = false, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last10StdBlockTimeLabel = new Label("Std: ") { X = Pos.Right(last10AvgBlockTimeValue), Y = 3, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last10StdBlockTimeValue = new Label("N/A") { X = Pos.Right(last10StdBlockTimeLabel), Y = 3, AutoSize = false, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last10MinMaxBlockTimeLabel = new Label("Min/Max: ") { X = Pos.Right(last10StdBlockTimeValue), Y = 3, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last10MinMaxBlockTimeValue = new Label("N/A") { X = Pos.Right(last10MinMaxBlockTimeLabel), Y = 3, Width = Dim.Fill(), TextAlignment = TextAlignment.Left };
			statsFrame.Add(last10AvgBlockTimeLabel, last10AvgBlockTimeValue, last10StdBlockTimeLabel, last10StdBlockTimeValue, last10MinMaxBlockTimeLabel, last10MinMaxBlockTimeValue);

			var last100AvgBlockTimeLabel = new Label("Last 100 Avg: ") { X = 0, Y = 4, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last100AvgBlockTimeValue = new Label("N/A") { X = Pos.Right(last100AvgBlockTimeLabel), Y = 4, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last100StdBlockTimeLabel = new Label("Std: ") { X = Pos.Right(last100AvgBlockTimeValue), Y = 4, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last100StdBlockTimeValue = new Label("N/A") { X = Pos.Right(last100StdBlockTimeLabel), Y = 4, AutoSize = false, Width = valueWidth, TextAlignment = TextAlignment.Left };
			var last100MinMaxBlockTimeLabel = new Label("Min/Max: ") { X = Pos.Right(last100StdBlockTimeValue), Y = 4, Width = labelWidth, TextAlignment = TextAlignment.Right };
			var last100MinMaxBlockTimeValue = new Label("N/A") { X = Pos.Right(last100MinMaxBlockTimeLabel), Y = 4, Width = Dim.Fill(), TextAlignment = TextAlignment.Left };
			statsFrame.Add(last100AvgBlockTimeLabel, last100AvgBlockTimeValue, last100StdBlockTimeLabel, last100StdBlockTimeValue, last100MinMaxBlockTimeLabel, last100MinMaxBlockTimeValue);

			// Handlers for stats update
			this.Model.Started += manager => {
				manager.SolutionSubmited += (o, puzzle, arg3) => {
					var testManager = (TestMiningManager)manager;
					targetValue.Text = EndianBitConverter.Little.GetBytes(testManager.MiningTarget).ToHexString(true);
					blockCountValue.Text = $"{testManager.BlockHeight}";
					avgBlockTimeValue.Text = $"{testManager.AllStats.Mean:0.##}";
					stdBlockTimeValue.Text = $"{testManager.AllStats.SampleStandardDeviation:0.##}";
					minMaxBlockTimeValue.Text = $"{testManager.AllStats.Minimum:0.##} / {testManager.AllStats.Maximum:0.##}";

					last5AvgBlockTimeValue.Text = $"{testManager.Last5Stats.Mean:0.##}";
					last5StdBlockTimeValue.Text = $"{testManager.Last5Stats.SampleStandardDeviation:0.##}";
					last5MinMaxBlockTimeValue.Text = $"{testManager.Last5Stats.Minimum:0.##} / {testManager.Last5Stats.Maximum:0.##}";

					last10AvgBlockTimeValue.Text = $"{testManager.Last10Stats.Mean:0.##}";
					last10StdBlockTimeValue.Text = $"{testManager.Last10Stats.SampleStandardDeviation:0.##}";
					last10MinMaxBlockTimeValue.Text = $"{testManager.Last10Stats.Minimum:0.##} / {testManager.Last10Stats.Maximum:0.##}";

					last100AvgBlockTimeValue.Text = $"{testManager.Last100Stats.Mean:0.##}";
					last100StdBlockTimeValue.Text = $"{testManager.Last100Stats.SampleStandardDeviation:0.##}";
					last100MinMaxBlockTimeValue.Text = $"{testManager.Last100Stats.Minimum:0.##} / {testManager.Last100Stats.Maximum:0.##}";
				};
			};
		}

		public override void OnModelChanged() {
			base.OnModelChanged();
			this.Model.Started += manager => {
				_outputLogger.Info("Mining Started");
				manager.SolutionSubmited += (o, puzzle, result) => _outputLogger.Info($"Miner: {puzzle.Block.MinerTag}, Block: {puzzle.ComputeWork().ToHexString(true)}, Result: {result}");
			};
			this.Model.Stopped += manager => { _outputLogger.Info("Mining Stopped"); };
		}

		public override IEnumerable<StatusItem> BuildStatusItems() {
			yield return new StatusItem(Terminal.Gui.Key.F2,
				"~[F2]~ Clear Log",
				() => { _log.ClearLog(); });

			yield return new StatusItem(Terminal.Gui.Key.F3,
				"~[F3]~ Copy to Clipboard",
				() => {
					throw new NotImplementedException("Waiting for Gui.cs impl");
					Clipboard.Contents = _log.Contents;
				});

			yield return new StatusItem(Terminal.Gui.Key.F10,
				"~[F10]~ Add Miner",
				() => {
					if (Model.MinerCount < MiningSimModel.MaxMiners) {
						Model.MinerCount++;
						_outputLogger.Info($"Miner added (total: {Model.MinerCount})");
					}
				});

			yield return new StatusItem(Terminal.Gui.Key.F11,
				"~[F11]~ Remove Miner",
				() => {
					if (Model.MinerCount > MiningSimModel.MinMiners) {
						Model.MinerCount--;
						_outputLogger.Info($"Miner removed (total: {Model.MinerCount})");
					}
				});

		}
	}


	public class MiningSimModel {
		public const int MinMiners = 1;
		public const int MaxMiners = 20;

		private List<SingleThreadedMiner> _miners;
		private IMiningManager _miningManager;

		public event EventHandlerEx<IMiningManager> Started;
		public event EventHandlerEx<IMiningManager> Stopped;

		private MiningSimModel() {
			MinerCount = 1;
			TA = TargetAlgo.Molina;
			DAA = DiffAlgo.RTT_ASERT;
			Hash = HashAlgo.SHA2_256;
			BlockTime = 5;
			RelaxationTime = 100;
			RTTInterval = 2;
			_miners = new List<SingleThreadedMiner>();
		}

		public static MiningSimModel Default => new MiningSimModel();

		private int _minerCount;

		public int MinerCount {
			get => _minerCount;
			set {
				Guard.ArgumentInRange(value, MinMiners, MaxMiners, nameof(value));
				_minerCount = value;
				if (IsStarted) {
					// _miners is created, need to adjust
					if (_miners.Count > _minerCount) {
						// remove
						for (var i = _miners.Count - 1; i >= _minerCount; i--) {
							_miners[i].Stop();
							_miners[i].Dispose();
							_miners.Remove(_miners[i]);
						}
						Debug.Assert(_miners.Count > 0);
						Debug.Assert(_miners[0].Status == MinerStatus.Mining);
					} else if (_miners.Count < _minerCount) {
						// add
						for (var i = _miners.Count; i < _minerCount; i++) {
							var miner = new SingleThreadedMiner($"Miner {i + 1}", _miningManager);
							_miners.Add(miner);
							miner.Start();
						}
					}
				}
			}
		}

		public TargetAlgo TA { get; set; }

		public DiffAlgo DAA { get; set; }

		public HashAlgo Hash { get; set; }

		public int BlockTime { get; set; }

		public int RelaxationTime { get; set; }

		public int RTTInterval { get; set; }

		public bool IsStarted { get; private set; }

		public void Start() {
			Guard.Ensure(!IsStarted, "Already Started");

			IMiningHasher hasher = Hash switch {
				HashAlgo.SHA2_256 => new CHFhasher { Algo = CHF.SHA2_256 },
				HashAlgo.RH2 => new RandomHash2Hasher(),
				_ => throw new ArgumentOutOfRangeException()
			};

			var targetAlgo = TA switch {
				TargetAlgo.Molina => new MolinaTargetAlgorithm(),
				TargetAlgo.Satoshi => throw new NotImplementedException(),
				_ => throw new ArgumentOutOfRangeException()
			};


			var daaAlgo = DAA switch {
				DiffAlgo.ASERT2 => new ASERT2(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) }),
				DiffAlgo.RTT_ASERT => new ASERT_RTT(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) }),
				_ => throw new ArgumentOutOfRangeException()
			};
			_miningManager = new TestMiningManager(
				new MiningConfig {
					Hasher = hasher,
					TargetAlgorithm = targetAlgo,
					DAAlgorithm = daaAlgo
				},
				new NewMinerBlockSerializer(),
				TimeSpan.FromSeconds(RTTInterval)
			);

			for (var i = 0; i < MinerCount; i++) {
				var miner = new SingleThreadedMiner($"Miner {i + 1}", _miningManager);
				_miners.Add(miner);
				miner.Start();
			}
			IsStarted = true;
			Started?.Invoke(_miningManager);
		}

		public void Stop() {
			Guard.Ensure(IsStarted, "Not started");
			foreach (var miner in _miners) {
				miner.Stop();
				miner.Dispose();
			}
			_miners.Clear();
			_miningManager = null;
			IsStarted = false;
			Stopped?.Invoke(_miningManager);
		}

	}


	public enum DiffAlgo {
		[Description("Real-time ASERT")] RTT_ASERT,

		[Description("Last two block ASERT")] ASERT2
	}


	public enum HashAlgo {
		[Description("Bitcoin-compatible SHA2-256")]
		SHA2_256,

		[Description("RandomHash2 (CPU-friendly GPU/ASIC resistant)")]
		RH2
	}


	public enum TargetAlgo {

		[Description("PascalCoin target/compact target conversion algorith")]
		Molina,

		[Description("BitcoinCoin target/compact target conversion algorith")]
		Satoshi,
	}


}
