// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen {

	public class TimestampLogger : PrefixLoggerBase {

		public const string DefaultDateFormat = "yyyy-MM-dd HH:mm:ss";

		public TimestampLogger(ILogger decoratedLogger, string dateFormat = default) : base(decoratedLogger) {
			Format = dateFormat ?? DefaultDateFormat;
		}

		public string Format { get; set; }
		
		protected override string GetPrefix() {
			return string.Format("{0:"+ Format + "}: ", DateTime.Now);
		}


	}	
}
