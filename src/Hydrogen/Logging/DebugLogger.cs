// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Writes log messages to the debugger output window.
/// </summary>
public class DebugLogger : TextWriterLogger {
	/// <summary>
	/// Creates a logger that writes to <see cref="DebugTextWriter"/>.
	/// </summary>
	public DebugLogger()
		: base(new DebugTextWriter()) {
	}
}
