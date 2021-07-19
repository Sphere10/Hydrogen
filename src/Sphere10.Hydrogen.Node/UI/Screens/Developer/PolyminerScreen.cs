using Sphere10.Hydrogen.Node.UI;
using Terminal.Gui;
using Sphere10.Hydrogen.Node.UI.Components;
using System.ComponentModel;
using Sphere10.Framework;
using Sphere10.Framework.CryptoEx;
using Sphere10.Hydrogen.Node.RPC;
using Sphere10.Hydrogen.Core.Mining;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Consensus.Serializers;
using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Hydrogen.Core.Consensus;

namespace Sphere10.Hydrogen.Node.UI {

	[Title("Polyminer's Screen")]
	[Lifetime(ScreenLifetime.WhenVisible)]
	[MenuLocation(AppMenu.Development, "Polyminer", 1)]
	public class MiningServerScreen : TabbedScreen<MiningServerScreen.MiningServerModel> {

		public MiningServerScreen()
			: base(MiningServerModel.Default, new ConfigScreen(), new OutputScreen()) {
		}

		protected override IEnumerable<StatusItem> BuildStatusItemsInternal() {
			yield return new StatusItem(Terminal.Gui.Key.F1, "~[F1]~ Start/Stop",
				() => {
					if (Model.IsStarted) {
						Model.Stop();
					} else {
						Model.Start();
					}
				});
		}


		[Title("Config")]
		public class ConfigScreen : FramedScreen<MiningServerModel> {
			private IntegerField _RpcServerPort;
			protected override void LoadInternal() {
				base.LoadInternal();

				var labelFieldLayout = new LabelFieldLayout(2, 2, 1, 25);
				_RpcServerPort = new IntegerField(Model.RpcServerPort, 255, 32000, x => { Model.RpcServerPort = (int)x; });
				labelFieldLayout.AddField("Rpc server port", _RpcServerPort, Dim.Sized(10));
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
			}
		}

		[Title("Output")]
		public class OutputScreen : Screen<MiningServerModel> {
			private ILogger _outputLogger;
			private LogView _log;

			protected override void LoadInternal() {
				this.X = 0;
				this.Y = 0;
				this.Width = Dim.Fill();
				this.Height = Dim.Fill();
				_log = new LogView("Log") {
					X = 0,
					Y = 0,
					Width = Dim.Fill(),
					Height = Dim.Fill(7),
				};

				_outputLogger = new TimestampLogger(new MainThreadActionLogger(s => {
					StringBuilder sb = new StringBuilder(s);
					//prevent json text to break the ui thread (that apply formats later on).
					sb.Replace("{", "{{");
					sb.Replace("}", "}}");
					s = sb.ToString();
					_log.AppendLog(s);
				}));
				_outputLogger.Options = LogOptions.DebugBuildDefaults;
				this.Add(_log);

				var statsFrame = new FrameView("Statistics") {
					X = 0,
					Y = Pos.Bottom(_log),
					Width = Dim.Fill(),
					Height = Dim.Fill()
				};
				this.Add(statsFrame);
			}

			public override void OnModelChanged() {
				base.OnModelChanged();
				this.Model.Started += manager => {
					_outputLogger.Info($"RPC mining server started on port {Model.RpcServerPort}");
					manager.SolutionSubmited += (o, puzzle, result) => _outputLogger.Info($"Miner: '{puzzle.Block.MinerTag}', Block: {puzzle.ComputeWork().ToHexString(true)}, Result: {result}");
					(manager as RpcMiningManager).Logger = _outputLogger;
				};

				this.Model.Stopped += manager => {
					_outputLogger.Info("RPC mining server stopped");
					(manager as RpcMiningManager).Logger = null;
				};
			}
		}

		//TODO: maybe bring that logger to Sphere10.Framework.Logging
		public class MainThreadActionLogger : ActionLogger {
			public MainThreadActionLogger(Action<string> action)
				: base(action) {
			}
			protected void LogMessage(Action<string> action, string message) {
				try {
#if DEBUG
					System.Diagnostics.Debug.WriteLine(message);
#endif
					Application.MainLoop.Invoke(() => action(message));
				} catch {
				}
			}
		}

		//management code
		public class MiningServerModel {
			//private const int DefaultBlockTime = 120;
			private const int DefaultBlockTime = 20;
			private IMiningManager _miningManager;

			public event EventHandlerEx<IMiningManager> Started;
			public event EventHandlerEx<IMiningManager> Stopped;

			public int RpcServerPort { get; set; } = 27001;
			public HashAlgo Hash { get; set; } = HashAlgo.RH2;
			public int BlockTime { get; set; } = DefaultBlockTime;

			public int RelaxationTime { get; set; } = 20 * DefaultBlockTime;

			public int RTTInterval { get; set; } = 5;

			public bool IsStarted { get; private set; }

			private MiningServerModel() { }
			public static MiningServerModel Default => new MiningServerModel();

			public void Start() {
				Guard.Ensure(!IsStarted, "Already Started");

				IMiningHasher hasher = Hash switch {
					HashAlgo.SHA2_256 => new CHFhasher { Algo = CHF.SHA2_256 },
					HashAlgo.RH2 => new RandomHash2Hasher(),
					_ => throw new ArgumentOutOfRangeException()
				};

				var targetAlgo = new MolinaTargetAlgorithm();

				var miningManager = new RpcMiningManager(
					new MiningConfig {
						Hasher = hasher,
						TargetAlgorithm = targetAlgo,
						DAAlgorithm = new ASERT_RTT(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) })
					},
					new RpcServerConfig { IsLocal = true, Port = RpcServerPort, MaxListeners = 12 },
					new NewMinerBlockSerializer(),
					BlockTime,
					TimeSpan.FromSeconds(RTTInterval)
				);

				miningManager.StartMiningServer("PolyminerTAG");

				_miningManager = miningManager;
				IsStarted = true;
				Started?.Invoke(_miningManager);
			}

			public void Stop() {
				Guard.Ensure(IsStarted, "Not started");
				(_miningManager as RpcMiningManager).StopMiningServer();
				IsStarted = false;
				Stopped?.Invoke(_miningManager);
				(_miningManager as RpcMiningManager).Dispose();
				_miningManager = null;
			}


			public enum HashAlgo {
				[Description("Bitcoin-compatible SHA2-256")]
				SHA2_256,

				[Description("RandomHash2 (CPU-friendly GPU/ASIC resistant)")]
				RH2
			}

		}
	}

}