// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.DApp.Core.Mining;

public interface IMiningManager {

	public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited;

	uint MiningTarget { get; }

	uint BlockHeight { get; }

	MiningPuzzle RequestPuzzle(string minerTag);

	MiningSolutionResult SubmitSolution(MiningPuzzle puzzle);

}
