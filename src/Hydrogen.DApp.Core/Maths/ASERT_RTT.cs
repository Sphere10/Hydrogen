// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Hydrogen.DApp.Core.Maths;

public class ASERT_RTT : IDAAlgorithm {

	public ASERT_RTT(ICompactTargetAlgorithm targetAlgorithm, ASERTConfiguration configuration) {
		TargetAlgorithm = targetAlgorithm;
		Config = configuration;
	}

	protected ICompactTargetAlgorithm TargetAlgorithm { get; }

	public ASERTConfiguration Config { get; }

	public bool RealTime => true;

	//ASERT Head time to current time
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

	public uint CalculateNextBlockTarget(uint previousCompactTarget, int timestampDelta, int blockTimeSec, int relaxationTime) {

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
