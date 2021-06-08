using System;
using System.Threading;
using NUnit.Framework;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Text;
using Sphere10.Framework;
using Sphere10.Framework.Communications.RPC;
using Sphere10.Hydrogen.Core.Mining;
using Sphere10.Hydrogen.Core.Maths;


namespace Sphere10.Framework.Tests
{

	[TestFixture]
	[Category("RPC")]
	public class Mining {

		[Test]
		public void RTT_ASERT_RPC_MiningTest() {
			try
			{
				//start server
				JsonRpcServer server = null;
				server = new JsonRpcServer(new TcpEndPointListener(true, 25000, 5));
				server.Start();
				Thread.Sleep(250);

				//tesst miner server and client
				var BlockTime = 5;
				var RelaxationTime = 100;
				var RTTInterval = 2;
				var targetAlgo = new MolinaTargetAlgorithm();
				var rtt_asert = new ASERT_RTT(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) });
				//var asert2 = new ASERT2(targetAlgo, new ASERTConfiguration { BlockTime = TimeSpan.FromSeconds(BlockTime), RelaxationTime = TimeSpan.FromSeconds(RelaxationTime) });

				var miningManager = new TestMiningManager(CHF.SHA2_256, targetAlgo, rtt_asert, TimeSpan.FromSeconds(RTTInterval));
				var miner = new SingleThreadedMiner("miner1", "127.0.0.1", 25000);
				miner.Start();

				//wait for miner task
				//const int Runtime = 5000000;
				const int Runtime = 10*1000;
				Thread.Sleep(Runtime);
				miner.Stop();
				float hr = (float)miner.Statistics["shares"] / (float)((uint)DateTime.Now.Second - (uint)miner.Statistics["runtime"]);
				Assert.Greater(hr, 10);
				Assert.Greater(miner.Statistics["runtime"], (Runtime/1000.0f) * 0.85);
				Assert.Greater(miner.Statistics["accepted"], 1);
				Assert.Greater(miner.Statistics["shares"], 10);
			}
			catch (Exception e)
			{
				Assert.Fail(e.ToString());
			}
		}
	}
}
