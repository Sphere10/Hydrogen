// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public class LoggerDecorator : ILogger {

	private readonly ILogger _decoratedLogger;

	public LoggerDecorator(ILogger decoratedLogger) {
		_decoratedLogger = decoratedLogger;
	}

	public LogOptions Options {
		get => _decoratedLogger.Options;
		set => _decoratedLogger.Options = value;
	}

	public virtual void Debug(string message) {
		_decoratedLogger.Debug(message);
	}

	public virtual void Info(string message) {
		_decoratedLogger.Info(message);
	}

	public virtual void Warning(string message) {
		_decoratedLogger.Warning(message);
	}

	public virtual void Error(string message) {
		_decoratedLogger.Error(message);
	}

	public virtual void Exception(Exception exception) {
		_decoratedLogger.Exception(exception);
	}
}
