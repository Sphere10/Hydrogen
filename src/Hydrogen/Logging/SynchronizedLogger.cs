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
/// Wraps a logger with read/write synchronization so callers can safely log from multiple threads.
/// </summary>
public class SynchronizedLogger : SynchronizedObject, ILogger {
	private readonly ILogger _internalLogger;

	/// <summary>
	/// Creates a synchronized wrapper around the provided logger.
	/// </summary>
	public SynchronizedLogger(ILogger internalLogger) {
		Guard.ArgumentNotNull(internalLogger, nameof(internalLogger));
		_internalLogger = internalLogger;
	}

	/// <inheritdoc />
	public LogOptions Options {
		get {
			using (EnterReadScope())
				return _internalLogger.Options;
		}

		set {
			using (EnterWriteScope())
				_internalLogger.Options = value;
		}
	}

	public void Debug(string message) {
		using (EnterWriteScope())
			_internalLogger.Debug(message);
	}

	public void Info(string message) {
		using (EnterWriteScope())
			_internalLogger.Info(message);
	}

	public void Warning(string message) {
		using (EnterWriteScope())
			_internalLogger.Warning(message);
	}

	public void Error(string message) {
		using (EnterWriteScope())
			_internalLogger.Error(message);
	}

	public void Exception(Exception exception, string message = null) {
		using (EnterWriteScope())
			_internalLogger.Exception(exception, message);
	}
}
