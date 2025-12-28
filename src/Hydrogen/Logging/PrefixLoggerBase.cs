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
/// Decorator that adds a prefix to every log message produced by the wrapped logger.
/// </summary>
public abstract class PrefixLoggerBase : LoggerDecorator {
	protected PrefixLoggerBase(ILogger decoratedLogger)
		: base(decoratedLogger) {
	}

	public override void Debug(string message) {
		base.Debug($"{GetPrefix()}" + message);
	}

	public override void Info(string message) {
		base.Info($"{GetPrefix()}" + message);
	}

	public override void Warning(string message) {
		base.Warning($"{GetPrefix()}" + message);
	}

	public override void Error(string message) {
		base.Error($"{GetPrefix()}" + message);
	}

	public override void Exception(Exception exception, string message = null) {
		if (LoggerHelper.TryHydrateErrorMessage(exception, Options, out var exceptionMessage))
			Error(!string.IsNullOrWhiteSpace(message) ? $"{message}. {exceptionMessage}" : exceptionMessage);
	}

	/// <summary>
	/// Returns the prefix to prepend to each message.
	/// </summary>
	protected abstract string GetPrefix();

}
