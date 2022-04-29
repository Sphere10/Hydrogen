using System;
using System.Collections.Generic;
using System.Text;
using Hydrogen;

namespace Hydrogen.DApp.Core.Mining {

	public interface IMiningManager {

		public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited;

		uint MiningTarget { get; }

		uint BlockHeight { get; }
		
		MiningPuzzle RequestPuzzle(string minerTag);

		MiningSolutionResult SubmitSolution(MiningPuzzle puzzle);

	}


}
