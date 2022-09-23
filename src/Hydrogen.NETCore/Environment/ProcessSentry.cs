using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Hydrogen;

public class ProcessSentry {
	
	private readonly SynchronizedObject _lock;
	private Process _runningProcess;
	public ProcessSentry(string fileName) {
		FileName = fileName;
	}


	public string FileName { get; init; }

	protected Process RunningProcess => _runningProcess;

	public virtual void BreakRunningProcess()  {
			DefaultBreakProcess(_runningProcess);
	}

	public virtual async Task<bool> CanRun(CancellationToken cancellationToken = default)  {
		try {
			await RunProcess(FileName, null, null, cancellationToken);
		} catch {
			return false;
		}
		return true;
	}

	public virtual async Task<int> Run(string arguments = null, TextWriter output = null, CancellationToken cancellationToken = default)  {
		using (_lock.EnterWriteScope() ) {
			_runningProcess = new Process();
			var _ = new ActionDisposable(() => _runningProcess = null);
			return await  RunProcess(_runningProcess, FileName, arguments, output, cancellationToken);
		}
	}

	public static Task<int> RunProcess(string fileName, string arguments = null,  TextWriter output = null, CancellationToken cancellationToken = default) 
		=> RunProcess(new Process(), fileName, arguments, output, cancellationToken);
	
	private static async Task<int> RunProcess(Process process, string fileName, string arguments = null,  TextWriter output = null, CancellationToken cancellationToken = default) {
		Guard.ArgumentNotNull(process, nameof(process));
		process.StartInfo.FileName = fileName;
		process.StartInfo.Arguments = arguments;
		process.StartInfo.ErrorDialog = false;
		process.StartInfo.UseShellExecute = false; 
		process.StartInfo.CreateNoWindow = true;
		process.StartInfo.RedirectStandardInput = true;

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

	public static void DefaultBreakProcess(Process process) {
		process.StandardInput.Close(); // sends ctrl-c (but doesn't work 99% of time)
	}
}
