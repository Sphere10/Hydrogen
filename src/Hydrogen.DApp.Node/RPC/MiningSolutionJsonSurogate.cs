using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Hydrogen;
using Hydrogen.Communications;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Node.RPC {

	[Serializable]
	public class MiningSolutionJsonSurogate {
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint WorkID { get; set; }
		public string MinerTag { get; set; }
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint		Nonce { get; set; }
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint		Time { get; set; }
	}
}
