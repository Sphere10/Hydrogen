// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Dispatches log messages to user-supplied delegates so callers can hook logging into arbitrary sinks.
/// </summary>
public class ActionLogger : LoggerBase {

	private readonly Action<string> _debugAction;
	private readonly Action<string> _infoAction;
	private readonly Action<string> _warningAction;
	private readonly Action<string> _errorAction;

	/// <summary>
	/// Creates an <see cref="ActionLogger"/> that routes every log level to the same action.
	/// </summary>
	/// <param name="action">Delegate invoked for debug, info, warning, and error messages.</param>
	public ActionLogger(Action<string> action)
		: this(action, action, action, action) {
	}

	/// <summary>
	/// Creates an <see cref="ActionLogger"/> with per-level handlers.
	/// </summary>
	/// <param name="debugAction">Action invoked for <see cref="LogLevel.Debug"/> entries.</param>
	/// <param name="infoAction">Action invoked for <see cref="LogLevel.Info"/> entries.</param>
	/// <param name="warningAction">Action invoked for <see cref="LogLevel.Warning"/> entries.</param>
	/// <param name="errorAction">Action invoked for <see cref="LogLevel.Error"/> entries.</param>
	public ActionLogger(Action<string> debugAction, Action<string> infoAction, Action<string> warningAction, Action<string> errorAction) {
		_debugAction = debugAction;
		_infoAction = infoAction;
		_warningAction = warningAction;
		_errorAction = errorAction;
	}


	protected override void Log(LogLevel level, string message) {
		try {
			switch (level) {
				case LogLevel.None:
					break;
				case LogLevel.Debug:
					_debugAction(message);
					break;
				case LogLevel.Info:
					_infoAction(message);
					break;
				case LogLevel.Warning:
					_warningAction(message);
					break;
				case LogLevel.Error:
					//case LogLevel.ErrorDetail:
					_errorAction(message);
					break;
				default:
					throw new NotSupportedException($"{level}");
			}
		} catch {
			// errors do not propagate outside logging framework
		}
	}

}
