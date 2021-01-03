using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Sphere10.Framework;
using Terminal.Gui;
using Tools;
using VelocityNET.Core.Configuration;
using VelocityNET.Core.Consensus.Serializers;
using VelocityNET.Core.Maths;
using VelocityNET.Core.Mining;
using VelocityNET.Presentation.Node.UI.Components;
using Key = Sphere10.Framework.Key;

namespace VelocityNET.Presentation.Node.UI {

	[Title("Mining Simulator")]
	[Lifetime(ScreenLifetime.WhenVisible)]
	[MenuLocation(AppMenu.Development, "Mining Sim")]
	public class MiningSimScreen : TabbedScreen<MiningSimScreen.MiningSimModel> {

		public MiningSimScreen() 
			: base(MiningSimModel.Default, new ConfigScreen(), new OutputScreen()) {
		}

		protected override IEnumerable<StatusItem> BuildStatusItemsInternal() {
			yield return new StatusItem(Terminal.Gui.Key.F1, "~[F1]~ Start/Stop",
				() => {
					if (Model.Started) {
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
				labelFieldLayout.AddField("Miner Count", _minerCountField, Dim.Sized(10) );
				labelFieldLayout.AddEnum("Difficulty Algorithm", "Select the difficulty algorithm which the simulation will use", () => Model.DAA, x => { Model.DAA = x; });
				labelFieldLayout.AddEnum("Hash Algorithm", "Select the hash algorithm which the simulation will use", () => Model.Hash, x => { Model.Hash = x; });
				labelFieldLayout.AddField("Block Time", new IntegerField(this.Model.BlockTime, 1, 10 * 60, x => { Model.BlockTime = (int)x; }), Dim.Sized(10));
				labelFieldLayout.AddField("Relaxation Time", new IntegerField(this.Model.RelaxationTime, 1, 99999, x => { Model.RelaxationTime = (int)x; }), Dim.Sized(10));
				labelFieldLayout.AddField("Real-Time Interval", new IntegerField(this.Model.RTTInterval, 1, 99999, x => { Model.RTTInterval = (int)x; }), Dim.Sized(10));
				this.Add(labelFieldLayout);
			}

			public override void OnModelChanged() {
				base.OnModelChanged();
				this.Model.OnStarted += x => this.Enabled = false;
				this.Model.OnStopped += x => this.Enabled = true;
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
					Height = Dim.Fill(10),
				};
				_outputLogger = new TimestampLogger(new ActionLogger(s => _log.AppendLog(s)));
				this.Add(_log);

				var _statsFrame = new FrameView("Statistics") {
					X = 0,
					Y = Pos.Bottom(_log),
					Width = Dim.Fill(),
					Height = Dim.Fill()
				};
				this.Add(_statsFrame);
			}

			public override void OnModelChanged() {
				base.OnModelChanged();
				this.Model.OnStarted += manager => {
					manager.SolutionSubmited += (o, puzzle, result) => _outputLogger.Info($"Miner: {puzzle.Block.MinerTag}, Block: {puzzle.ComputeWork().ToHexString(true)}, Result: {result}");
				};
			}

			public override IEnumerable<StatusItem> BuildStatusItems() {
				yield return new StatusItem(Terminal.Gui.Key.F2, "~[F2]~ Clear Log",
					() => {
						_log.ClearLog();
					});

				yield return new StatusItem(Terminal.Gui.Key.F3, "~[F3]~ Copy to Clipboard",
					() => {
						throw new NotImplementedException("Waiting for Gui.cs impl");
						Clipboard.Contents = _log.Contents;
					});

				yield return new StatusItem(Terminal.Gui.Key.F10, "~[F10]~ Add Miner",
					() => {
						if (Model.MinerCount < MiningSimModel.MaxMiners)
							Model.MinerCount++;
					});

				yield return new StatusItem(Terminal.Gui.Key.F11, "~[F11]~ Remove Miner",
					() => {
						if (Model.MinerCount > MiningSimModel.MinMiners)
							Model.MinerCount--;

					});

			}
		}

		public class MiningSimModel {
			public const int MinMiners = 1;
			public const int MaxMiners = 20;

			private List<SingleThreadedMiner> _miners;
			private IMiningManager _miningManager;

			public event EventHandlerEx<IMiningManager> OnStarted;
			public event EventHandlerEx<IMiningManager> OnStopped; 

			private MiningSimModel() {
				MinerCount = 1;
				TA = TargetAlgo.Molina;
				DAA = DiffAlgo.RTT_ASERT;
				Hash = HashAlgo.SHA2_256;
				BlockTime = 5;
				RelaxationTime = 50;
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
					if (Started) {
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

			public bool Started { get; private set; }

			public void Start() {
				Guard.Ensure(!Started, "Already Started");

				var chf = Hash switch {
					HashAlgo.RH2 => throw new NotImplementedException(),
					HashAlgo.SHA2_256 => CHF.SHA2_256,
					_ => throw new ArgumentOutOfRangeException()
				};

				var targetAlgo = TA switch {
					TargetAlgo.Molina => new MolinaTargetAlgorithm(),
					TargetAlgo.Satoshi => throw new NotImplementedException(),
					_ => throw new ArgumentOutOfRangeException()
				};

				
				var algo = DAA switch {
					DiffAlgo.ASERT2 => new ASERT2(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime)}),
					DiffAlgo.RTT_ASERT => new ASERT_RTT(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) }),
					_ => throw new ArgumentOutOfRangeException()
				};

				_miningManager = new TestMiningManager(chf, targetAlgo, algo, new NewMinerBlockSerializer(), TimeSpan.FromSeconds(RTTInterval));
				for (var i = 0; i < MinerCount; i++) {
					var miner = new SingleThreadedMiner($"Miner {i + 1}", _miningManager);
					_miners.Add(miner);
					miner.Start();
				}
				Started = true;
				OnStarted?.Invoke(_miningManager);
			}

			public void Stop() {
				Guard.Ensure(Started, "Not started");
				foreach (var miner in _miners) {
					miner.Stop();
					miner.Dispose();
				}
				_miners.Clear();
				_miningManager = null;
				Started = false;
				OnStopped?.Invoke(_miningManager);
			}

		}


		public enum DiffAlgo {
			[Description("Real-time ASERT")]
			RTT_ASERT,

			[Description("Last two block ASERT")]
			ASERT2
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






}
