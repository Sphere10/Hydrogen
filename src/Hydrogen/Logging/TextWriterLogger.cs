// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;

namespace Hydrogen;

/// <summary>
/// Logger implementation that writes formatted entries to a <see cref="TextWriter"/>.
/// </summary>
public class TextWriterLogger : LoggerBase {

	private readonly TextWriter _writer;

	/// <summary>
	/// Initializes a new logger that writes to the debug output window.
	/// </summary>
	public TextWriterLogger()
		: this(new DebugTextWriter()) {
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TextWriterLogger"/> class.
	/// </summary>
	/// <param name="writer">The writer.</param>
	/// <remarks></remarks>
	public TextWriterLogger(TextWriter writer) {
		_writer = writer;
		Options = LogOptions.VerboseProfile;
	}


	/// <summary>
	/// Logs the message.
	/// </summary>
	/// <param name="message">The message.</param>
	protected override void Log(LogLevel level, string message) {
		try {
			_writer.Write($"[{level}] ");
			_writer.Write(message + Environment.NewLine);
		} catch {
			// errors do not propagate outside logging framework
		}
	}

}
