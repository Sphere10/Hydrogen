//-----------------------------------------------------------------------
// <copyright file="AsyncLogger.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Hydrogen {


	public class AsyncLogger : LoggerDecorator {

		private readonly BackgroundProcessor _backgroundProcessor;

		public AsyncLogger(ILogger decoratedLogger, Action<Exception> errorHandler = null) : base(decoratedLogger) {
			_backgroundProcessor = new BackgroundProcessor(new List<Action>(), errorHandler);	
		}

		public override void Debug(string message) {
			_backgroundProcessor.QueueForExecution(() => base.Debug(message));
		}

		public override void Info(string message) {
			_backgroundProcessor.QueueForExecution(() => base.Info(message));
		}

		public override void Warning(string message) {
			_backgroundProcessor.QueueForExecution(() => base.Warning(message));
		}

		public override void Error(string message) {
			_backgroundProcessor.QueueForExecution(() => base.Error(message));
		}

	}
}
