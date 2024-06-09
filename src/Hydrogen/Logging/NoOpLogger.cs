// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// No-operation logger. Does nothing.
/// </summary>
/// <remarks></remarks>
public class NoOpLogger : LoggerBase {

	protected override void Log(LogLevel logLevel, string message) {
		// do nothing
	}
}
