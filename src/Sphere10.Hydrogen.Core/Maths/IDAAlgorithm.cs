using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Hydrogen.Core.Maths {
	public interface IDAAlgorithm {
		
		bool RealTime { get; }
		
		uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber);
	}
}
