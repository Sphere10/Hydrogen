using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hydrogen.Communications;
using Hydrogen.DApp.Core;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Node.RPC {

	[Serializable]
	public class MiningBlockUpdates {
		public uint MicroBlockNumber { get; set; }
		public uint TimeStamp { get; set; }
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] TargetPOW { get; set; }
	}
}
