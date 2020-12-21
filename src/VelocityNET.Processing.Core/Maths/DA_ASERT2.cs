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

    public class DA_ASERT2 : IDAAlgorithm {

		public DA_ASERT2(ITargetAlgorithm targetAlgorithm, IConfiguration configuration) {
            PoWAlgorithm = targetAlgorithm;
            Config = configuration;
		}

        protected ITargetAlgorithm PoWAlgorithm { get; }

		protected IConfiguration Config { get; }

        public bool RealTime => false;

		public uint CalculateNextBlockTarget(IEnumerable<DateTime> previousBlockTimestamps, uint previousCompactTarget, uint blockNumber) {
            if (!previousBlockTimestamps.Any())
                return PoWAlgorithm.MinCompactTarget; // start at minimum

            var lastBlockTime = previousBlockTimestamps.First();
            return CalculateNextBlockTarget(
               previousCompactTarget,
               (int)DateTime.UtcNow.Subtract(lastBlockTime).TotalSeconds,
               (int)Config.NewMinerBlockTime.TotalSeconds,
               (int)Config.DAAsertRelaxationTime.TotalSeconds
            );
        }

        public uint CalculateNextBlockTarget (uint previousCompactTarget, int secondsSinceLastBlock, int blockTimeSec, int relaxationTime) {
            var prevBlockTarget = PoWAlgorithm.ToTarget(previousCompactTarget);
            var exp = FixedPoint.Exp((secondsSinceLastBlock - blockTimeSec) / (FixedPoint)relaxationTime);
            var expNumerator = new BigInteger(exp * FixedPoint.Pow(10, 5));
            var expDenominator = new BigInteger(Math.Pow(10, 5));
            var nextTarget = prevBlockTarget * expNumerator / expDenominator;
            var nextCompactTarget = PoWAlgorithm.FromTarget(nextTarget);
            return nextCompactTarget;
        }


	}


}
