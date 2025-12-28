// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Writes log messages to the console using <see cref="ConsoleTextWriter"/>.
/// </summary>
public class ConsoleLogger : TextWriterLogger {
	/// <summary>
	/// Creates a console logger that emits every log level by default.
	/// </summary>
	public ConsoleLogger()
		: base(new ConsoleTextWriter()) {
	}
}
