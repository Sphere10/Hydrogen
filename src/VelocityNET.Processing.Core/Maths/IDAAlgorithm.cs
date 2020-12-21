using System;
using System.Collections.Generic;
using System.Text;

namespace VelocityNET.Core.Maths {
	public interface IDAAlgorithm {
		
		bool RealTime { get; }
		
		uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber);
	}
}
