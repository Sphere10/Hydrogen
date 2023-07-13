// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading;

namespace Hydrogen;

public class ThreadIdLogger : PrefixLoggerBase {

	public const string DefaultThreadIdFormat = "(TID: {0})";

	public ThreadIdLogger(ILogger decoratedLogger, string threadIdFormat = null) : base(decoratedLogger) {
		Format = threadIdFormat ?? DefaultThreadIdFormat;
	}

	public string Format { get; set; }

	protected override string GetPrefix()
		=> string.Format(Format, Thread.CurrentThread.ManagedThreadId) + " ";

}
