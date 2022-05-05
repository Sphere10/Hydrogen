using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Hydrogen;
using Hydrogen.Communications;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Node.RPC {

	[Serializable]
	public class MiningSolutionResultJsonSurogate {
		[JsonConverter(typeof(StringEnumConverter))]
		public MiningSolutionResult SolutionResult { get; set; }
		public uint TimeStamp { get; set; }
		public uint BlockNumber { get; set; }
	}
}
