// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

/// <summary>
/// Adds a static prefix to every message emitted by the decorated logger.
/// </summary>
public class PrefixLogger : PrefixLoggerBase {
	private readonly string _prefix;

	/// <summary>
	/// Creates a prefixing logger.
	/// </summary>
	/// <param name="decoratedLogger">Logger to wrap.</param>
	/// <param name="prefix">Prefix text prepended to every message.</param>
	public PrefixLogger(ILogger decoratedLogger, string prefix)
		: base(decoratedLogger) {
		_prefix = prefix;
	}

	protected override string GetPrefix() => _prefix;
}
