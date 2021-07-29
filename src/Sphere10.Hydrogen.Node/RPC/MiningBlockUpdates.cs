using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Node.RPC {

	[Serializable]
	public class MiningBlockUpdates {
		public uint MicroBlockNumber { get; set; }
		public uint TimeStamp { get; set; }
		[JsonConverter(typeof(ByteArrayHexConverter))]
		public byte[] TargetPOW { get; set; }
	}
}
