using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sphere10.Framework;
using Sphere10.Hydrogen.Core.Configuration;

namespace Sphere10.Hydrogen.Core.Maths {

    public class ASERT2 : ASERT_RTT {

		public ASERT2(ICompactTargetAlgorithm targetAlgorithm, ASERTConfiguration configuration) 
            : base(targetAlgorithm, configuration) {
		}

        //ASERT Head-1 time to Head time
		public override uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber) {
            if (previousBlockTimestamps.Count() < 2)
                return TargetAlgorithm.MinCompactTarget; // start at minimum

            var lastBlockTimes = previousBlockTimestamps.Take(2).ToArray();
            return CalculateNextBlockTarget(
               previousCompactTarget,
               (int)lastBlockTimes[0].Subtract(lastBlockTimes[1]).TotalSeconds,
               (int)Config.BlockTime.TotalSeconds,
               (int)Config.RelaxationTime.TotalSeconds
            );
        }
	}


}
