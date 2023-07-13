// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using Hydrogen.Communications;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen.DApp.Core.Runtime;

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
