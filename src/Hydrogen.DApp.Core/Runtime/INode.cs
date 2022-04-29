using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hydrogen;
using Hydrogen.Communications;
using Hydrogen.DApp.Core.Mining;

namespace Hydrogen.DApp.Core.Runtime {
	public interface INode {
		event EventHandlerEx GuiStarted;
		event EventHandlerEx GuiEnded;

		NodeTraits Traits { get; }

		NodeExitCode Run(CancellationToken cancellationToken);

		Task RequestShutdown();
	}
}
