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
/// Adds a timestamp prefix to every log entry.
/// </summary>
public class TimestampLogger : PrefixLoggerBase {

	public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss";

	/// <summary>
	/// Creates a timestamping logger.
	/// </summary>
	/// <param name="decoratedLogger">Logger to wrap.</param>
	/// <param name="dateFormat">Optional date/time format string.</param>
	public TimestampLogger(ILogger decoratedLogger, string dateFormat = default) : base(decoratedLogger) {
		Format = dateFormat ?? DefaultDateFormat;
	}

	/// <summary>
	/// Format string used to render the timestamp.
	/// </summary>
	public string Format { get; set; }

	protected override string GetPrefix() {
		return string.Format("{0:" + Format + "}: ", DateTime.Now);
	}


}
