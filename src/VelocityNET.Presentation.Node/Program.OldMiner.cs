//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Numerics;
//using System.Threading;
//using Sphere10.Framework;
//using VelocityNET.Core;
//using VelocityNET.Core.Encoding;
//using VelocityNET.Core.Maths;


//namespace VelocityNET.Presentation.Node {

//	internal class Program {
//		const int BlockTimeSec = 10;
//		const int RelaxationTime = 10;


//        private static void ChangeNonce(byte[] bytes, uint value) {
//			bytes[^4] = (byte)value;
//			bytes[^3] = (byte)((value >> 8) & 255);
//			bytes[^2] = (byte)((value >> 16) & 255);
//			bytes[^1] = (byte)((value >> 24) & 255);
//		}

//		public class Block {
//            public int Number;
//            public DateTime MiningStart;
//            public DateTime TimeFound;
//            public TimeSpan Duration => TimeFound - MiningStart;
//            public TimeSpan ScheduleError;
//            public double HashRate;
//            public byte[] Header;
//            public uint RequiredWork;
//            public uint Work;
//		}

//		public class BlockStats {
//            public Statistics BlockTime;
//            public Statistics ScheduleError;
//		}


//		public class BlockChain {
//            private readonly List<Block> _blocks;
//            private DateTime? _lastStartDateTime;
//            private int _hashCount;

//			public BlockChain() {
//				_blocks = new List<Block>();
//				_lastStartDateTime = null;
//                _hashCount = 0;
//            }

//            public Block Genesis => _blocks[0];

//            public void Add(Block block) {
//				_blocks.Add(block);
//			}

//			public Block CreateGenesisBlock() {
//                return new Block()
//                {
//                    Header = new byte[] { 0 },
//                    Number = 0,
//                    MiningStart = DateTime.Now,
//                    TimeFound = DateTime.Now,
//                    RequiredWork = 0,
//                    Work = 0
//                };
//            }

//			public Block NewBlockTemplate() {
//				return new Block {
//					Number = _blocks.Count,
//					MiningStart = DateTime.Now,
//					Header = Guid.NewGuid().ToByteArray(),
//					RequiredWork = CalculateNextTarget()
//				};
//            }

//			public UInt32 CalculateNextTarget() {
//                if (_blocks.Count <= 1)
//                    return Target.MinCompactTarget;
//                var tip = _blocks[^1];
//                var preTip = _blocks[^2];
//                //return DA_ASERT.CalculateNextBlockTarget(tip.RequiredWork, (int)Math.Round((tip.TimeFound - preTip.TimeFound).TotalSeconds), BlockTimeSec, RelaxationTime);
//                var lastTimeRef = tip.TimeFound;
//                return DA_ASERT.CalculateNextBlockTarget(tip.RequiredWork, (int)Math.Round((DateTime.Now - lastTimeRef).TotalSeconds), BlockTimeSec, RelaxationTime);
//            }

//            public IEnumerable<Block> Iterator => _blocks.AsQueryable().Reverse();
		
//		}

//		private static void Main(string[] args) {
//			var writer = new MulticastTextWriter(new ConsoleTextWriter(), new FileAppendTextWriter($"c:\\temp\\DA-ASERT-T{BlockTimeSec}-TAU{RelaxationTime}.csv", true));

//			var blockchain = new BlockChain();
//            var genesis = blockchain.CreateGenesisBlock();
//			blockchain.Add(genesis);
			
//			//writer.WriteLine($"DA-ASERT DAA Simulation - Start: {startTime:yyyy-MM-dd HH:mm:ss.fff}, BlockTime = {blockTimeSec} sec, Relaxation = {relaxationTime}");
//			writer.WriteLine($"Time,Block,HashRate,Schedule Error,PrevBlockTimeDelta,Required Target,Actual Target,Next Target");
//			writer.WriteLine($"{0},{0},{0},{0},{0},{0},{0}");
//			while (true) {
//                var block = blockchain.NewBlockTemplate();
//				for (var nonce = uint.MinValue; nonce < uint.MaxValue; nonce++) {
//					if (nonce % 1000 == 0)
//						block = blockchain.NewBlockTemplate();

//                    if (block.Number > 10)
//                    	Thread.Sleep(1);
//                    ChangeNonce(block.Header, nonce);
//					block.Work = Target.GetCompactTarget(Hashers.SHA2_256(block.Header));
//					if (block.Work >= block.RequiredWork) {
//                        block.TimeFound = DateTime.Now;
//						block.ScheduleError = TimeSpan.FromSeconds((block.Number * BlockTimeSec) - (block.TimeFound - blockchain.Genesis.TimeFound).TotalSeconds);
//                        block.HashRate = (nonce + 1) / block.Duration.TotalSeconds;
//                        blockchain.Add(block);
//						writer.WriteLine(
//							$"{(block.TimeFound - blockchain.Genesis.TimeFound).TotalSeconds:0},{block.Number},{block.HashRate:0.0},{block.ScheduleError.TotalSeconds:0},{block.Duration.TotalSeconds:0},{block.RequiredWork},{block.Work},{blockchain.CalculateNextTarget()}");
//						break;
//					}
//				}

//			}
//		}
//	}
//}


////var bn2 = BigInteger.Parse("57896044618658097711785492504343953926634992332820282019728792003956564819968"); // First bit 1 followed by 0
////Console.WriteLine($"{bn2.ToString()}");
////bn2 >>= 1;
////Console.WriteLine($"{bn2.ToString()}");
////bn2 >>= 1;
////Console.WriteLine($"{bn2.ToString()}");

////for (var i = 0; i < 65; i++)
////{
////    var byteString = "0x0000000000000000000000000000000000000000000000000000000000000000".ToCharArray();
////    for (var j = 0; j < i; j++)
////        byteString[byteString.Length - j - 1] = 'f';

////    var bytes = HexEncoding.Decode(new string(byteString));
////    var compact = Target.GetCompactTarget(bytes);
////    var target = Target.GetTargetAsBytes(compact);

////    Console.WriteLine($"Input: {HexEncoding.Encode(bytes, true)}  - Target: {HexEncoding.Encode(target, true)}, Compact: {compact}");
////}
