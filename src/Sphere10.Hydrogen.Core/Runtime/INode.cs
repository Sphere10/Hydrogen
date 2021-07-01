using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sphere10.Framework;
using Sphere10.Framework.Communications;
using Sphere10.Hydrogen.Core.Mining;

namespace Sphere10.Hydrogen.Core.Runtime {
	public interface INode {
		event EventHandlerEx GuiStarted;
		event EventHandlerEx GuiEnded;

		NodeTraits Traits { get; }

		NodeExitCode Run(CancellationToken cancellationToken);

		Task RequestShutdown();
	}
}
