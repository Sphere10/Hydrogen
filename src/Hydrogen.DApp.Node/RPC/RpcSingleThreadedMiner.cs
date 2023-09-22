// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Polyminer
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen.Communications.RPC;
using Hydrogen.DApp.Core.Configuration;
using Hydrogen.DApp.Core.Consensus.Serializers;
using Hydrogen.DApp.Core.Maths;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Node.RPC;

public class RpcSingleThreadedMiner : IDisposable {
	private JsonRpcClient _rpcClient;
	private Task _miningTask;
	private CancellationTokenSource _cancelSource;

	//Temporary placeholder for proper stats
	private Dictionary<string, uint> _stats;

	public RpcSingleThreadedMiner(string minerTag, string serverIP, int serverPort) {
		_miningTask = null;
		_cancelSource = null;
		_rpcClient = new JsonRpcClient(new TcpEndPoint(serverIP, serverPort), JsonRpcConfig.Default);
		_stats = new Dictionary<string, uint>();
		MinerTag = minerTag;
		Status = MinerStatus.Idle;
	}

	public void Dispose() {
		if (Status == MinerStatus.Mining)
			Stop();
		_miningTask = null;
		_cancelSource?.Dispose();
	}

	public string MinerTag { get; }

	public Statistics LastRate { get; set; }

	public IConfiguration Configuration { get; }

	public MinerStatus Status { get; private set; }

	public Dictionary<string, uint> Statistics {
		get => _stats;
	}

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
			var blockSerializer = new NewMinerBlockSerializer();
			while (Status != MinerStatus.Idle) {
				//ignore ["powalgo"], it's alwys monilaAlgo for now
				var PoWAlgorithm = new MolinaTargetAlgorithm();
				var miningWork = _rpcClient.RemoteCall<NewMinerBlockSurogate>("getwork", MinerTag);
				var work = miningWork.ToNonSurrogate(PoWAlgorithm);
				var maxTime = (DateTimeOffset)miningWork.Config["maxtime"];
				var hashAlgoName = (string)miningWork.Config["hashalgo"];

				//for R&D purpose, we send hash-algo and pow-algo in Block.Config
				var HashAlgorithm = StringExtensions.ParseEnum<CHF>(hashAlgoName);
				//ICompactTargetAlgorithm DAAlgorithm;
				//if ((string)work.Config["daaalgo"] == "ASERT2")
				//	DAAlgorithm = (ICompactTargetAlgorithm)new ASERT2(PoWAlgorithm, new ASERTConfiguration { BlockTime = TimeSpan.Parse((string)work.Config["daaalgo.blocktime"]), RelaxationTime = TimeSpan.Parse((string)work.Config["daaalgo.relaxtime"]) });
				//else
				//	DAAlgorithm = (ICompactTargetAlgorithm)new ASERT_RTT(PoWAlgorithm, new ASERTConfiguration { BlockTime = TimeSpan.Parse((string)work.Config["daaalgo.blocktime"]), RelaxationTime = TimeSpan.Parse((string)work.Config["daaalgo.relaxtime"]) });

				//Ignore suggested nonce and extra-nonce
				work.Nonce = (uint)Tools.Maths.RNG.Next();

				uint shareCount = 0;
				while (Status == MinerStatus.Mining && (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= maxTime.ToUnixTimeSeconds()) {
					unchecked {
						work.Nonce++;
					}
					var proofOfWork = Hashers.Hash(HashAlgorithm, blockSerializer.SerializeBytesLE(work));
					var pow = PoWAlgorithm.FromDigest(proofOfWork);
					if (pow > work.CompactTarget) {
						MiningSolutionResult res = _rpcClient.RemoteCall<MiningSolutionResult>("submit", miningWork.WorkID, MinerTag, work.UnixTime, work.Nonce);
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
		} catch (Exception) {
			throw;
		}
	}
}
