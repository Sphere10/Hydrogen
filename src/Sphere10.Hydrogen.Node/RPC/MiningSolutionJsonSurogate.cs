using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sphere10.Framework;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node.RPC {

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
