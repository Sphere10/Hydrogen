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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Sphere10.Framework{

	public class TimestampLogger : DecoratedLogger {

		public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss";

		public TimestampLogger(ILogger decoratedLogger, string dateFormat = null) : base(decoratedLogger) {
			TimestampFormat = dateFormat ?? DefaultDateFormat;
		}

		public string TimestampFormat { get; set; }

		public override void Debug(string message) {
			base.Debug(GetTimestamp() + message);
		}

		public override void Info(string message) {
			base.Info(GetTimestamp() + message);
		}

		public override void Warning(string message) {
			base.Warning(GetTimestamp() + message);
		}

		public override void Error(string message) {
			base.Error(GetTimestamp() + message);
		}

		private string GetTimestamp() {
			return string.Format("{0:"+ TimestampFormat + "}: ", DateTime.Now);
		}


	}	
}
