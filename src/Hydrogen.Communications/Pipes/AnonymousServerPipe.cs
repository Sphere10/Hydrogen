// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.IO.Pipes;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Hydrogen.Communications;

public sealed class AnonymousServerPipe : AnonymousPipe {
	private readonly string _processPath;
	private readonly string _arguments;
	private readonly Func<string, string, string, string> _argInjectorFunc;
	private Process _childProcess;

	/// <summary>
	/// Constructor.
	/// </summary>
	/// <param name="processPath">Path to the child process</param>
	/// <param name="arguments">Arguments to pass into the child process</param>
	/// <param name="argInjectorFunc">A callback which will inject the read and write pipe handle into the <paramref name="arguments"/>. The first argument of <paramref name="argInjectorFunc"/> is the entire arguments string (empty if none), the second argument is the server read pipe handle, the third argument is the server write pipe handle, the return value is the final argument string to pass into the process with read/write pipe handles injected.</param>
	/// <param name="mediator">Handles bad messages.</param>
	/// <returns>A channel used to send messages backwards and forwards</returns>
	/// <remarks>If <paramref name="argInjectorFunc"/> is null the read pipe handle and write pipe handle are passed consequtively.</remarks>
	public AnonymousServerPipe(string processPath, string arguments = "", Func<string, string, string, string> argInjectorFunc = null) {
		Guard.ArgumentNotNullOrEmpty(processPath, nameof(processPath));
		Guard.FileExists(processPath);
		_processPath = processPath;
		_arguments = arguments;
		_argInjectorFunc = argInjectorFunc ?? ((args, readHandle, writeHandle) => $"{args} {readHandle} {writeHandle}");
	}

	public override CommunicationRole LocalRole => CommunicationRole.Server;

	protected override async Task<(AnonymousPipeEndpoint endpoint, PipeStream readStream, PipeStream writeStream)> OpenPipeInternal() {
		var readPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
		var writePipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
		var endpoint = new AnonymousPipeEndpoint {
			ReaderHandle = readPipe.GetClientHandleAsString(),
			WriterHandle = writePipe.GetClientHandleAsString()
		};

		// Start child process
		_childProcess = new Process {
			StartInfo = {
				FileName = _processPath,
				Arguments = _argInjectorFunc(_arguments, endpoint.ReaderHandle, endpoint.WriterHandle),
				UseShellExecute = false // note: MUST be false or won't work (Win64)
			}
		};
		await Task.Run(() => _childProcess.Start());

		// Dispose pipe handles (owned by child process now)
		readPipe.DisposeLocalCopyOfClientHandle();
		writePipe.DisposeLocalCopyOfClientHandle();

		return (endpoint, readPipe, writePipe);
	}


	protected override async Task CloseInternal() {
		await base.CloseInternal();
		await Task.Run(() => _childProcess.WaitForExit(DefaultTimeout));
		if (!_childProcess.HasExited)
			_childProcess.Kill();
	}

}
