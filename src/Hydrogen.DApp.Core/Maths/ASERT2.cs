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

namespace Hydrogen.DApp.Core.Maths;

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
