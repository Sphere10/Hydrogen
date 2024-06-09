// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools;

public static class Runtime {
	private static readonly object _threadLock = new object();
	private static bool _hasDeterminedDesignMode = false;
	private static bool _isDesignMode = false;
	private static Assembly _entryAssembly = null;
	private static bool? _isWebApp = null;
	private static readonly Func<IntPtr> GetExceptionPointers = BuildGetExceptionPointersFunc();

	public static bool IsWasmExecutable() => (int)Environment.OSVersion.Platform == 7;

	// https://stackoverflow.com/questions/64581054/how-do-i-get-the-name-of-the-current-executable-in-c-net-5-edition
	public static string GetExecutablePath() => Process.GetCurrentProcess().MainModule.FileName;


	public static bool IsInExceptionContext() 
		=> GetExceptionPointers() != IntPtr.Zero;

	public static bool IsDebugBuild {
		get {
#if DEBUG
			return true;
#else
                return false;
#endif
		}
	}

	public static bool IsReleaseBuild {
		get {
#if RELEASE
                return true;
#else
			return false;
#endif
		}
	}


	public static bool IsWebApp {
		get {
#warning IsWebApp needs testing for NET_CORE and NET STANDARD apps
			return false;

			//#if __MOBILE__
			//            return false;
			//#else
			//                if (_isWebApp == null) {
			//                    lock (_threadLock) {
			//                        if (_isWebApp == null) {
			//                            _isWebApp = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile).EndsWith("web.config", true, CultureInfo.InvariantCulture);
			//                        }
			//                    }
			//                }
			//#endif
			//                return _isWebApp.Value;
		}
	}

	public static bool IsDesignMode {
		get {
			if (!_hasDeterminedDesignMode) {
				lock (_threadLock) {
					if (!_hasDeterminedDesignMode) {
						var processName = Process.GetCurrentProcess().ProcessName.ToUpperInvariant();

						//System.IO.File.AppendAllText("c:/temp/a.txt", processName);
						_isDesignMode = processName.IsIn("DEVENV", "SHARPDEVELOP", "DESIGNTOOLSSERVER");
						_hasDeterminedDesignMode = true;
					}
				}
			}
			return _isDesignMode;
		}
	}

	public static Assembly GetEntryAssembly() {
		if (_entryAssembly == null) {
			lock (_threadLock) {
				if (_entryAssembly == null) {
					_entryAssembly = IsWebApp ? GetWebEntryAssembly() : Assembly.GetEntryAssembly();
					if (_entryAssembly == null)
						throw new SoftwareException("Unable to determine entry assembly");
				}
			}
		}
		return _entryAssembly;
	}

	private static Assembly GetWebEntryAssembly() {
		throw new NotImplementedException();
		////return Assembly.GetExecutingAssembly();
		//var httpContextType = TypeResolver.Resolve("System.Web.HttpContext");
		//var httpContextCurrent = httpContextType.GetProperty("Current").FastGetValue(null);
		////var httpContextCurrentHandler = httpContextCurrent.GetType().GetProperty("Handler").FastGetValue(httpContextCurrent);
		//var httpContextCurrentApplicationInstance = httpContextCurrent.GetType().GetProperty("ApplicationInstance").FastGetValue(httpContextCurrent);
		//return httpContextCurrentApplicationInstance.GetType().BaseType.Assembly;
		////if ((System.Web.HttpContext.Current == null) || (System.Web.HttpContext.Current.Handler == null))
		////    return Tools.Runtime.GetEntryAssembly(); // Not a web application
		////return System.Web.HttpContext.Current.Handler.GetType().BaseType.Assembly;
	}


	public static string EncodeCommandLineArgumentWin(string argument, bool force = false) {
		// NOTE: only for Windows
		if (argument == null) throw new ArgumentNullException(nameof(argument));

		// Unless we're told otherwise, don't quote unless we actually
		// need to do so --- hopefully avoid problems if programs won't
		// parse quotes properly
		if (force == false
		    && argument.Length > 0
		    && argument.IndexOfAny(" \t\n\v\"".ToCharArray()) == -1) {
			return argument;
		}

		var quoted = new StringBuilder();
		quoted.Append('"');

		var numberBackslashes = 0;

		foreach (var chr in argument) {
			switch (chr) {
				case '\\':
					numberBackslashes++;
					continue;
				case '"':
					// Escape all backslashes and the following
					// double quotation mark.
					quoted.Append('\\', numberBackslashes * 2 + 1);
					quoted.Append(chr);
					break;
				default:
					// Backslashes aren't special here.
					quoted.Append('\\', numberBackslashes);
					quoted.Append(chr);
					break;
			}
			numberBackslashes = 0;
		}

		// Escape all backslashes, but let the terminating
		// double quotation mark we add below be interpreted
		// as a metacharacter.
		quoted.Append('\\', numberBackslashes * 2);
		quoted.Append('"');

		return quoted.ToString();
	}

	public static (string executable, string arguments) ParseCommandLine(string commandLine) {
		if (string.IsNullOrEmpty(commandLine))
			return (null, null);

		commandLine = commandLine.Trim();

		if (commandLine.StartsWith("\"")) {
			// Find the closing quote for the executable
			var endQuoteIndex = commandLine.IndexOf('\"', 1);
			if (endQuoteIndex == -1)
				throw new ArgumentException("Invalid command line: Missing closing quote for the executable path.");

			// Extract the executable part
			var executable = commandLine.Substring(1, endQuoteIndex - 1);

			// Remaining part of the command line as arguments
			var arguments = commandLine[(endQuoteIndex + 1)..].TrimStart();

			return (executable, arguments);
		} else {
			// Find the first space to separate the executable from the arguments
			var firstSpaceIndex = commandLine.IndexOf(' ');
			if (firstSpaceIndex == -1) {
				// No arguments provided, only executable
				return (commandLine, "");
			}

			// Extract the executable and arguments
			var executable = commandLine[..firstSpaceIndex];
			var arguments = commandLine[(firstSpaceIndex + 1)..];

			return (executable, arguments);
		}
	}

	public static void TouchAssembly(Type type) {
		// used to introduce assembly into domain so it can be reflected
		var assembly = type.Assembly;
	}


	private static Func<IntPtr> BuildGetExceptionPointersFunc() {
		var memberAccess = Expression.Call(typeof(Marshal).GetMethod("GetExceptionPointers"));
		var lambda = Expression.Lambda<Func<IntPtr>>(memberAccess);
		return lambda.Compile();
	}
}
