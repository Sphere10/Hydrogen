using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Configuration;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {
	public class RpcSingleThreadedMiner : IDisposable {
		protected JsonRpcClient _rpcClient;
		private Task _miningTask;
		private CancellationTokenSource _cancelSource;
		//Temporary placeholder for proper stats
		private Dictionary<string, uint> _stats;

		public RpcSingleThreadedMiner(string minerTag, string serverIP, int serverPort) {
			_miningTask = null;
			_cancelSource = null;
			_rpcClient = new JsonRpcClient(new TcpEndPoint(serverIP, serverPort));
			_stats = new Dictionary<string, uint>();
			MinerTag = minerTag;
			Status = MinerStatus.Idle;
		}

		public string MinerTag { get; }

		public Statistics LastRate { get; set; }

		public IConfiguration Configuration { get; }

		public MinerStatus Status { get; private set; }
		public Dictionary<string, uint> Statistics { get => _stats;  }

		public void Start() {
			Guard.Ensure(Status == MinerStatus.Idle, "Already Started");
			_cancelSource?.Cancel(false);
			_cancelSource = new CancellationTokenSource();
			Status = MinerStatus.Mining;
			_stats["runtime"] = (uint)DateTime.Now.Second;
			_stats["accepted"] = 0;
			_stats["rejected"] = 0;
			_stats["shares"] = 0;
			_miningTask = Task.Run(Mine, _cancelSource.Token);
		}

		public void Stop() {
			_stats["runtime"] = (uint)DateTime.Now.Second - _stats["runtime"];
			Guard.Ensure(Status == MinerStatus.Mining, "Not Started");
			Status = MinerStatus.Idle;
		}

		protected virtual void Mine() {
			try {
				while (Status != MinerStatus.Idle) {
					NewMinerBlock work = _rpcClient.RemoteCall<NewMinerBlock>("getwork", MinerTag);
					DateTimeOffset maxTime = (DateTime)work.Config["maxtime"];
					Debug.WriteLine($"Miner: New work packagewith target {work.CompactTarget}");

					//for R&D purpose, we send hash-algo and pow-algo in Block.Config
					CHF HashAlgorithm = StringExtensions.ParseEnum<CHF>((string)work.Config["hashalgo"]);
					//ignore ["powalgo"], it's alwys monilaAlgo for now
					var PoWAlgorithm = new MolinaTargetAlgorithm();
					//ICompactTargetAlgorithm DAAlgorithm;
					//if ((string)work.Config["daaalgo"] == "ASERT2")
					//	DAAlgorithm = (ICompactTargetAlgorithm)new ASERT2(PoWAlgorithm, new ASERTConfiguration { BlockTime = TimeSpan.Parse((string)work.Config["daaalgo.blocktime"]), RelaxationTime = TimeSpan.Parse((string)work.Config["daaalgo.relaxtime"]) });
					//else
					//	DAAlgorithm = (ICompactTargetAlgorithm)new ASERT_RTT(PoWAlgorithm, new ASERTConfiguration { BlockTime = TimeSpan.Parse((string)work.Config["daaalgo.blocktime"]), RelaxationTime = TimeSpan.Parse((string)work.Config["daaalgo.relaxtime"]) });

					//Ignore suggested nonce and extra-nonce
					work.NoncePlane = (uint)Tools.Maths.RNG.Next();
					work.Nonce = (uint)Tools.Maths.RNG.Next();

					uint shareCount = 0;
					while (Status == MinerStatus.Mining && (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= maxTime.ToUnixTimeSeconds()) {
						unchecked { work.Nonce++; }
						var proofOfWork = Hashers.Hash(HashAlgorithm, work.GetWorkHeader());
						var pow = PoWAlgorithm.FromDigest(proofOfWork);
						if (pow > work.CompactTarget) {
							MiningSolutionResult res = _rpcClient.RemoteCall<MiningSolutionResult>("submit", MinerTag, work.Timestamp, work.NonceSpace, work.NoncePlane, work.Nonce);
							if (res == MiningSolutionResult.Accepted)
								_stats["accepted"]++;
							else
								_stats["rejected"]++;
							// Dont update DAAlgorithm here, let node sen new upadated data on next getwork.
							break;
						}
						shareCount++;
					}
					_stats["shares"] += shareCount;
				}
			}
			catch(Exception e) {
				throw;
			}
		}

		public void Dispose() {
			if (Status == MinerStatus.Mining)
				Stop();
			_miningTask = null;
			_cancelSource?.Dispose();
		}
	}
}
