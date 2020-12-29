using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Sphere10.Framework;
using VelocityNET.Core.Configuration;

namespace VelocityNET.Core.Maths {

    public class ASERT2 : ASERT_RTT {

		public ASERT2(ITargetAlgorithm targetAlgorithm, ASERTConfiguration configuration) 
            : base(targetAlgorithm, configuration) {
		}

		public override uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber) {
            if (previousBlockTimestamps.Count() < 2)
                return PoWAlgorithm.MinCompactTarget; // start at minimum

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
