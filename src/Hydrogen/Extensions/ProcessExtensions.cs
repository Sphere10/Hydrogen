// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Hydrogen;

// https://raw.githubusercontent.com/dotnet/extensions/ffb7c20fb22a31ac31d3a836a8455655867e8e16/shared/Microsoft.Extensions.Process.Sources/ProcessHelper.cs
public static class ProcessExtensions {
	private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
	private static readonly TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

	public static void KillTree(this Process process) {
		process.KillTree(_defaultTimeout);
	}

	public static void KillTree(this Process process, TimeSpan timeout) {
		string stdout;
		if (_isWindows) {
			RunProcessAndWaitForExit(
				"taskkill",
				$"/T /F /PID {process.Id}",
				timeout,
				out stdout);
		} else {
			var children = new HashSet<int>();
			GetAllChildIdsUnix(process.Id, children, timeout);
			foreach (var childId in children) {
				KillProcessUnix(childId, timeout);
			}
			KillProcessUnix(process.Id, timeout);
		}
	}

	public static void WaitForExit(this Process process, TimeSpan timeSpan)
		=> process.WaitForExit((int)timeSpan.TotalMilliseconds);


	private static void GetAllChildIdsUnix(int parentId, ISet<int> children, TimeSpan timeout) {
		string stdout;
		var exitCode = RunProcessAndWaitForExit(
			"pgrep",
			$"-P {parentId}",
			timeout,
			out stdout);

		if (exitCode == 0 && !string.IsNullOrEmpty(stdout)) {
			using (var reader = new StringReader(stdout)) {
				while (true) {
					var text = reader.ReadLine();
					if (text == null) {
						return;
					}

					int id;
					if (int.TryParse(text, out id)) {
						children.Add(id);
						// Recursively get the children
						GetAllChildIdsUnix(id, children, timeout);
					}
				}
			}
		}
	}

	private static void KillProcessUnix(int processId, TimeSpan timeout) {
		string stdout;
		RunProcessAndWaitForExit(
			"kill",
			$"-TERM {processId}",
			timeout,
			out stdout);
	}

	private static int RunProcessAndWaitForExit(string fileName, string arguments, TimeSpan timeout, out string stdout) {
		var startInfo = new ProcessStartInfo {
			FileName = fileName,
			Arguments = arguments,
			RedirectStandardOutput = true,
			UseShellExecute = false
		};

		var process = Process.Start(startInfo);

		stdout = null;
		if (process.WaitForExit((int)timeout.TotalMilliseconds)) {
			stdout = process.StandardOutput.ReadToEnd();
		} else {
			process.Kill();
		}

		return process.ExitCode;
	}
}
