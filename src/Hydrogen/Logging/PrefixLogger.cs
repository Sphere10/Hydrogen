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

namespace Hydrogen {

	public class PrefixLogger : LoggerDecorator {
		private readonly string _prefix;

		public PrefixLogger(ILogger decoratedLogger, string prefix)
			: base(decoratedLogger) {
			_prefix = prefix;
		}

		public override void Debug(string message) {
			base.Debug(_prefix + message);
		}

		public override void Info(string message) {
			base.Info(_prefix + message);
		}

		public override void Warning(string message) {
			base.Warning(_prefix + message);
		}

		public override void Error(string message) {
			base.Error(_prefix + message);
		}
	}	
}
