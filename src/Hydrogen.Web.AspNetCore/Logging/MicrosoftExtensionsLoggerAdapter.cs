// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


public class MicrosoftExtensionsLoggerAdapter : Hydrogen.LoggerDecorator, Microsoft.Extensions.Logging.ILogger {
	public MicrosoftExtensionsLoggerAdapter(Hydrogen.ILogger decoratedLogger)
		: base(decoratedLogger) {
	}

	public IDisposable BeginScope<TState>(TState state) => Hydrogen.Disposables.None; // 

	public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel)
		=> logLevel switch {
			Microsoft.Extensions.Logging.LogLevel.Trace => Options.HasFlag(Hydrogen.LogOptions.DebugEnabled),
			Microsoft.Extensions.Logging.LogLevel.Debug => Options.HasFlag(Hydrogen.LogOptions.DebugEnabled),
			Microsoft.Extensions.Logging.LogLevel.Information => Options.HasFlag(Hydrogen.LogOptions.InfoEnabled),
			Microsoft.Extensions.Logging.LogLevel.Warning => Options.HasFlag(Hydrogen.LogOptions.WarningEnabled),
			Microsoft.Extensions.Logging.LogLevel.Error => Options.HasFlag(Hydrogen.LogOptions.ErrorEnabled),
			Microsoft.Extensions.Logging.LogLevel.Critical => Options.HasFlag(Hydrogen.LogOptions.ErrorEnabled),
			Microsoft.Extensions.Logging.LogLevel.None => false,
			_ => throw new NotSupportedException($"{logLevel}")
		};

	public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, Microsoft.Extensions.Logging.EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {
		switch (logLevel) {
			case Microsoft.Extensions.Logging.LogLevel.None:
				break;
			case Microsoft.Extensions.Logging.LogLevel.Trace:
			case Microsoft.Extensions.Logging.LogLevel.Debug:
				base.Debug(formatter(state, exception));
				break;
			case Microsoft.Extensions.Logging.LogLevel.Information:
				base.Info(formatter(state, exception));
				break;
			case Microsoft.Extensions.Logging.LogLevel.Warning:
				base.Warning(formatter(state, exception));
				break;
			case Microsoft.Extensions.Logging.LogLevel.Error:
			case Microsoft.Extensions.Logging.LogLevel.Critical:
				base.Error(formatter(state, exception));
				break;
			default:
				throw new NotSupportedException($"{logLevel}");
		}
		;

	}


}
