//-----------------------------------------------------------------------
// <copyright file="TimestampLogger.cs" company="Sphere 10 Software">
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
using System.Threading;

namespace Hydrogen {

	public class ThreadIdLogger : LoggerDecorator {

		public const string DefaultThreadIdFormat = "(TID: {0})";

		public ThreadIdLogger(ILogger decoratedLogger, string threadIdFormat = null) : base(decoratedLogger) {
			ThreadIdFormat = threadIdFormat ?? DefaultThreadIdFormat;
		}

		public string ThreadIdFormat { get; set; }

		public override void Debug(string message) {
			base.Debug(GetThreadId() + message);
		}

		public override void Info(string message) {
			base.Info(GetThreadId() + message);
		}

		public override void Warning(string message) {
			base.Warning(GetThreadId() + message);
		}

		public override void Error(string message) {
			base.Error(GetThreadId() + message);
		}

		private string GetThreadId() {
			return string.Format(ThreadIdFormat, Thread.CurrentThread.ManagedThreadId) + " ";
		}

	}	
}
