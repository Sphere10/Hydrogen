using Hydrogen;
using Hydrogen.Communications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Core.Runtime {
	public class Node : INode {
		public event EventHandlerEx GuiStarted;
		public event EventHandlerEx GuiEnded;

		public Node(IApplicationPaths applicationPaths, int hostReadPort, int hostWritePort) {
		}

		public NodeTraits Traits => throw new NotImplementedException();

		public async Task Run(CancellationToken cancellationToken) {
			throw new NotImplementedException();
			
		}

		public async Task RequestShutdown() {
			throw new NotImplementedException();
		}

		protected virtual void OnGuiStarted() {
			throw new NotImplementedException();
		}

		protected virtual void OnGuiEnded() {
			throw new NotImplementedException();
		}

		private void NotifyGuiStarted(AnonymousPipe anonymousPipe) {
			OnGuiStarted();
			GuiStarted?.Invoke();
		}

		private void NotifyGuiEnded() {
			OnGuiEnded();
			GuiEnded?.Invoke();
		}

		NodeExitCode INode.Run(CancellationToken cancellationToken) {
			throw new NotImplementedException();
		}
	}
}