using System;
using System.Collections.Generic;
using System.Text;
using Sphere10.Framework;

namespace VelocityNET.Core.Mining {

	public interface IMiningManager {

		public event EventHandlerEx<object, MiningPuzzle, MiningSolutionResult> SolutionSubmited; 

		MiningManagerStatus Status { get; }

		uint MiningTarget { get; }

		MiningPuzzle RequestPuzzle(string minerTag);

		MiningSolutionResult SubmitSolution(MiningPuzzle puzzle);

	}

	public enum MiningManagerStatus {
		Stopped,
		Available,
	}

}
