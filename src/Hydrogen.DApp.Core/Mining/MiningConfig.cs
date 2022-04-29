using System;
using System.ComponentModel;
using Hydrogen;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Core.Mining {

	public class MiningConfig {
		public IMiningHasher Hasher { get; set; }
		public ICompactTargetAlgorithm TargetAlgorithm { get; set; }
		public IDAAlgorithm DAAlgorithm { get; set; }
	}

}
