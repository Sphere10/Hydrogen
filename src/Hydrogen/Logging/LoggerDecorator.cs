//-----------------------------------------------------------------------
// <copyright file="DecoratedLogger.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class LoggerDecorator : ILogger {

		private readonly ILogger _decoratedLogger;

		public LoggerDecorator(ILogger decoratedLogger) {
			_decoratedLogger = decoratedLogger;
		}

        public LogOptions Options { 
			get => _decoratedLogger.Options;
			set => _decoratedLogger.Options = value;
		}

	    public virtual void Debug(string message) {
			_decoratedLogger.Debug(message);	
		}

		public virtual void Info(string message) {
			_decoratedLogger.Info(message);
		}

		public virtual void Warning(string message) {
			_decoratedLogger.Warning(message);
		}

		public virtual void Error(string message) {
			_decoratedLogger.Error(message);
		}
	}
}
