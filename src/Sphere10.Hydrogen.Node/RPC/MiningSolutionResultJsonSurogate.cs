using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sphere10.Framework;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Consensus;
using Sphere10.Hydrogen.Core.Maths;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Node.RPC {

	[Serializable]
	public class MiningSolutionResultJsonSurogate {
		[JsonConverter(typeof(StringEnumConverter))]
		public MiningSolutionResult SolutionResult { get; set; }
		public uint TimeStamp { get; set; }
		public uint BlockNumber { get; set; }
	}
}
