// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class ActionLogger : LoggerBase {

	private readonly Action<string> _debugAction;
	private readonly Action<string> _infoAction;
	private readonly Action<string> _warningAction;
	private readonly Action<string> _errorAction;

	public ActionLogger(Action<string> action)
		: this(action, action, action, action) {
	}

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
