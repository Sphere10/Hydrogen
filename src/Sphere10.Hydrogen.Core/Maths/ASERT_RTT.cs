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

    public class ASERT_RTT : IDAAlgorithm {

		public ASERT_RTT(ICompactTargetAlgorithm targetAlgorithm, ASERTConfiguration configuration) {
            TargetAlgorithm = targetAlgorithm;
            Config = configuration;
		}

        protected ICompactTargetAlgorithm TargetAlgorithm { get; }

		public ASERTConfiguration Config { get; }

        public bool RealTime => true;

		public virtual uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber) {
            if (!previousBlockTimestamps.Any())
                return TargetAlgorithm.MinCompactTarget; // start at minimum

            var lastBlockTime = previousBlockTimestamps.First();
            return CalculateNextBlockTarget(
               previousCompactTarget,
               (int)DateTime.UtcNow.Subtract(lastBlockTime).TotalSeconds,
               (int)Config.BlockTime.TotalSeconds,
               (int)Config.RelaxationTime.TotalSeconds
            );
        }

        public uint CalculateNextBlockTarget (uint previousCompactTarget, int timestampDelta, int blockTimeSec, int relaxationTime) {
            const int FloatingPointResolution = 6;
            var prevBlockTarget = TargetAlgorithm.ToTarget(previousCompactTarget);
            var exp = FixedPoint.Exp((timestampDelta - blockTimeSec) / (FixedPoint)relaxationTime);
            var expNumerator = new BigInteger(exp * FixedPoint.Pow(10, FloatingPointResolution));
            var expDenominator = new BigInteger(Math.Pow(10.0D, FloatingPointResolution));
            var nextTarget = prevBlockTarget * expNumerator / expDenominator;
            var nextCompactTarget = TargetAlgorithm.FromTarget(nextTarget);
            return nextCompactTarget;
        }

	}


}
