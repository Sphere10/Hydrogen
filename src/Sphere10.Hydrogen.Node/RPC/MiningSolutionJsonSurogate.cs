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
		public uint		Nonce { get; set; }
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public UInt64	ExtraNonce { get; set; }
		[JsonConverter(typeof(HexadecimalValueConverterReader))]
		public uint		MinerNonce { get; set; }
		public uint		Time { get; set; }
		public string	MinerTag { get; set; }

		public MiningSolutionJsonSurogate FromNonSurogate(MiningSolution solution) {
			Nonce = solution.Nonce;
			ExtraNonce = solution.ExtraNonce;
			MinerNonce = solution.MinerNonce;
			Time = solution.Timestamp;
			MinerTag = solution.MinerTag;
			return this;
		}

		public MiningSolution ToNonSurrogate() {
			return new MiningSolution() {
				Nonce = this.Nonce,
				ExtraNonce = this.ExtraNonce,
				MinerNonce = this.MinerNonce,
				Timestamp = this.Time,
				MinerTag = System.Text.Encoding.ASCII.GetString(StringExtensions.ToHexByteArray(this.MinerTag))
			};
		}

	}
}
