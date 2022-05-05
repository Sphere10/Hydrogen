using System;
using System.Collections.Generic;
using System.Text;

namespace Hydrogen.DApp.Core.Maths {
	public interface IDAAlgorithm {
		
		bool RealTime { get; }
		
		uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber);
	}
}
