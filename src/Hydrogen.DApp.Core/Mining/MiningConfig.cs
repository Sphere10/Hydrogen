using System;
using System.ComponentModel;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Maths;

namespace Sphere10.Hydrogen.Core.Mining {

	public class MiningConfig {
		public IMiningHasher Hasher { get; set; }
		public ICompactTargetAlgorithm TargetAlgorithm { get; set; }
		public IDAAlgorithm DAAlgorithm { get; set; }
	}

}
