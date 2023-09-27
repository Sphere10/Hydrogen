// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public class ProcessSentry {
	private readonly SynchronizedObject _lock;
	private Process _runningProcess;

	public ProcessSentry(string executableFileName) {
		ExecutableFileName = executableFileName;
		_lock = new SynchronizedObject();
	}

	public string ExecutableFileName { get; }

	public TextWriter Output { get; init; } = new NoOpTextWriter();

	protected Process RunningProcess => _runningProcess;

	public virtual void BreakRunningProcess() {
		DefaultBreakProcess(_runningProcess);
	}

	public virtual async Task RunWithErrorCodeCheckAsync(string arguments = null, int expectedErrorCode = 0, CancellationToken cancellationToken = default) {
		using (_lock.EnterWriteScope()) {
			var stringBuilderTextWriter = new StringBuilderTextWriter();

			var errorCode = await RunAsync(arguments, cancellationToken);
			if (errorCode != expectedErrorCode)
				throw new ProcessSentryException(ExecutableFileName, errorCode, stringBuilderTextWriter.Builder.ToString());
		}
	}

	public virtual async Task<int> RunAsync(string arguments = null, CancellationToken cancellationToken = default) {
		using (_lock.EnterWriteScope()) {
			_runningProcess = new Process();
			var _ = new ActionDisposable(() => _runningProcess = null);
			return await RunProcess(_runningProcess, ExecutableFileName, arguments, Output, cancellationToken);
		}
	}

	public static async Task<bool> CanRunAsync(string executableFileName, CancellationToken cancellationToken = default) {
		try {
			var sentry = new ProcessSentry(executableFileName);
			await sentry.RunAsync(cancellationToken: cancellationToken);
		} catch {
			return false;
		}
		return true;
	}

	public static Task<int> RunAsync(string fileName, string arguments = null, TextWriter output = null, CancellationToken cancellationToken = default)
		=> RunProcess(new Process(), fileName, arguments, output, cancellationToken);

	private static async Task<int> RunProcess(Process process, string fileName, string arguments = null, TextWriter output = null, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(process, nameof(process));
		process.StartInfo.FileName = fileName;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.ErrorDialog = false;
		process.StartInfo.UseShellExecute = false;
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.Verb = "runas";

		if (output != null) {
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			var syncObj = new SynchronizedObject();
			process.OutputDataReceived += async (_, data) => {
				using (syncObj.EnterWriteScope())
					output.WriteLine(data.Data);
			};
		}

		if (!process.Start())
			throw new InvalidOperationException($"Unable to start process: {fileName}");

		if (!process.HasExited) {
			Tools.Exceptions.ExecuteIgnoringException(process.BeginOutputReadLine);
			Tools.Exceptions.ExecuteIgnoringException(process.BeginErrorReadLine);
		}

		await process.WaitForExitAsync();
		return process.ExitCode;

	}

	private static void DefaultBreakProcess(Process process) {
		process.StandardInput.Close(); // sends ctrl-c (but doesn't work 99% of time)
	}
}
