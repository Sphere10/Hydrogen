// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen;


public class AsyncLogger : LoggerDecorator {

	private readonly SerialThreadPool _serialThreadPool;

	public AsyncLogger(ILogger decoratedLogger, Action<Exception> errorHandler = null) : base(decoratedLogger) {
		_serialThreadPool = new SerialThreadPool(errorHandler);
	}

	public override void Debug(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Debug(message));
	}

	public override void Info(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Info(message));
	}

	public override void Warning(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Warning(message));
	}

	public override void Error(string message) {
		_serialThreadPool.QueueUserWorkItem(() => base.Error(message));
	}

	public override void Exception(Exception exception) {
		_serialThreadPool.QueueUserWorkItem(() => base.Exception(exception));
	}
};

