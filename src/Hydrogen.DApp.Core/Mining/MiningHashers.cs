using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {

	public class CHFhasher : IMiningHasher {
		public CHF Algo { get; set; }

		public string GetDescription() { 
			return Algo.ToString(); 
		}
		public byte[] Hash(byte[] input) {
			return Hashers.Hash(Algo, input);
		}
	}

	public class RandomHash2Hasher : IMiningHasher {
		public string GetDescription() {
			return "RH2"; 
		}
		public byte[] Hash(byte[] input) {
			return RandomHash2.Compute(input);
		}
	} 
}
