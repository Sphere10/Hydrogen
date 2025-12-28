// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading;

namespace Hydrogen;

/// <summary>
/// Adds the current managed thread identifier as a prefix to log messages.
/// </summary>
public class ThreadIdLogger : PrefixLoggerBase {

	public const string DefaultThreadIdFormat = "(TID: {0})";

	/// <summary>
	/// Creates a thread-aware logger.
	/// </summary>
	/// <param name="decoratedLogger">Logger to wrap.</param>
	/// <param name="threadIdFormat">Optional composite format string for the thread id.</param>
	public ThreadIdLogger(ILogger decoratedLogger, string threadIdFormat = null) : base(decoratedLogger) {
		Format = threadIdFormat ?? DefaultThreadIdFormat;
	}

	/// <summary>
	/// Format string used to render the managed thread id.
	/// </summary>
	public string Format { get; set; }

	protected override string GetPrefix()
		=> string.Format(Format, Thread.CurrentThread.ManagedThreadId) + " ";

}
