// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

// HS 2021-11-14: DISABLED until this piece is resumed once core is fleshed out

//using System;
//using NUnit.Framework;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Collections.Generic;
//using System.Diagnostics;
//using Hydrogen;
//using Hydrogen.Communications.RPC;
//using System.Threading;

//namespace Hydrogen.Tests
//{


//	public class StratumWork
//	{
//		public string coinbase1;
//		public string nonce1;
//		public uint nTime;
//		public int nonce;
//	}

//	//Example: StratumServer Apiservice Example
//	[RpcAPIService("Mining")]
//	public class MiningManager : IDisposable
//	{
//		public SimpleStratumServer MiningServer { get; set; }
//		public MiningManager(SimpleStratumServer miningServer)
//		{
//			MiningServer = miningServer;
//			ApiServiceManager.RegisterService(this);
//		}
//		public void Dispose()
//		{
//			ApiServiceManager.UnregisterService(this);
//		}

//		//simulate sending block to mine to all connected clients
//		public void SimulateBlockCreation(int TestRoundCount)
//		{

//			//active wait for a clients. Thelazyway
//			if (MiningServer.ActiveClientsCount == 0)
//				Thread.Sleep(50);

//			const int BlockTimeMs = 1000;
//			for (int i = 0; i < TestRoundCount; i++)
//			{
//				Debug.WriteLine($"Mining round #{i}");
//				MiningServer.NotifyNewBlock();
//				Thread.Sleep(BlockTimeMs);
//			}
//		}

//		//return accepted every other calls
//		[RpcAPIMethod]
//		public bool SubmitNonce(int nonce)
//		{
//			return ((nonce % 2) == 0);
//		}
//	}

//	public class SimpleStratumServer : JsonRpcServer
//	{
//		public SimpleStratumServer(IEndPoint ep) : base(ep, new JsonRpcConfig() { ConnectionMode = JsonRpcConfig.ConnectionModeEnum.Persistant, IgnoreEmptyReturnValue = true }) { }

//		public void NotifyNewBlock()
//		{
//			foreach (var client in ActiveClients)
//			{
//				client.RemoteCall("miner.notify", "123456789abcdef", "1999999", (uint)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
//			}
//		}
//	}

//	//Example: Client Side for Stratum:
//	[RpcAPIService("Miner")]
//	public class RhMinerMock : JsonRpcClientHandler
//	{
//		protected string Name;
//		static public uint Accepted = 0;
//		//A typical stratum server is not interested in external miner's json 2.0 responses from RPC calls.
//		public RhMinerMock(string name, string StratumServer, int port) : base(new TcpEndPoint(StratumServer, port), new JsonRpcConfig() { ConnectionMode = JsonRpcConfig.ConnectionModeEnum.Persistant, IgnoreEmptyReturnValue = true, Logger = new TimestampLogger(new ActionLogger((s) => Debug.WriteLine(s))) })
//		{
//			Name = name;
//		}

//		public override void Start()
//		{
//			ApiServiceManager.RegisterService(this);
//			base.Start();
//			Debug.WriteLine($"{Name} Started");
//		}
//		public override void Stop()
//		{
//			Debug.WriteLine($"{Name} Stopped");
//			ApiServiceManager.UnregisterService(this);
//			base.Stop();
//		}

//		//mining.notify
//		[RpcAPIMethod]
//		public void Notify(string coinbase1, string nonce1, uint nTime)
//		{
//			Debug.WriteLine($"Miner.Notify: Mining Notify {Name}");
//			ThreadPool.QueueUserWorkItem(_ => {
//				var work = new StratumWork { coinbase1 = coinbase1, nonce1 = nonce1, nTime = nTime };
//				Debug.WriteLine($"Miner.Notify: Mining 4 round : {Name}");
//				//mine for 4x100ms
//				for (int nonce = 0; nonce < 4; nonce++)
//				{
//					if (ProcessRound(work, nonce))
//					{
//						Debug.WriteLine($"{Name} submiting nonce {nonce}");
//						bool succeded = RemoteCall<bool>("mining.submitnonce", nonce);
//						if (succeded)
//						{
//							Interlocked.Increment(ref Accepted);
//							Debug.WriteLine($"Nonce {nonce} accepted !!!");
//						}
//					}
//				}
//				Debug.WriteLine($"Miner.Notify: Done  {Name}");
//			});
//		}
//		protected bool ProcessRound(StratumWork w, int nonce)
//		{
//			w.nonce = nonce;
//			//simulate round work
//			Thread.Sleep(100);
//			return ((nonce % 2) == 0);
//		}
//	}


//	[TestFixture]
//	public class StratumServerTest
//	{
//		protected SimpleStratumServer StratumServer { get; set; }
//		public MiningManager MiningManager { get; set; }

//		public void StartStratumServer()
//		{
//			StratumServer?.Stop();
//			StratumServer = new SimpleStratumServer(new TcpEndPointListener(true, 27001, 5));
//			StratumServer.Start();
//		}

//		public void StopStratumServer()
//		{
//			StratumServer?.Stop();
//			StratumServer = null;
//		}

//		public void StartMiningManager()
//		{
//			MiningManager = new MiningManager(StratumServer);
//		}
//		public void StopMiningManager()
//		{
//			MiningManager = null;
//		}

//		[Test]
//		public void SimpleStratumRun()
//		{
//			//setup : Init StratumServer and manager
//			StartStratumServer();
//			StartMiningManager();
//			//start 4 remote clients
//			var client = new RhMinerMock($"Miner abc", "127.0.0.1", 27001);
//			client.Start();

//			//test for 5 seonds
//			int TestRoundCount = 5;
//			MiningManager.SimulateBlockCreation(TestRoundCount);
//			ClassicAssert.AreEqual(RhMinerMock.Accepted, TestRoundCount * 4 / 2);

//			StopStratumServer();
//			StopMiningManager();
//		}

//		[Test]
//		//Test StratumServer with clients that closes every time
//		public void ClientsManualyCloses()
//		{
//		}

//		[Test]
//		//Test StratumServer big numbers of clients
//		public void ManyClients()
//		{
//		}
//	}

//}


