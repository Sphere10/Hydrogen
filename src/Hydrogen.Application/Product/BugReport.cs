//-----------------------------------------------------------------------
// <copyright file="BugReport.cs" company="Sphere 10 Software">
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
using System.Reflection;


namespace Sphere10.Framework.Application {

	[Obfuscation(Exclude = true)]
    public class BugReport : ClientRequest {
        public BugReport()
            : base() {
        }

        public BugReport(UserType userType, string sender, ProductInformation senderProductInformation, string bugReportText, Exception exception = null, IEnumerable<string> logEntries = null)
			: base(userType, sender, senderProductInformation, bugReportText) {
				Exception = exception.ToDiagnosticString();
				LogEntries = logEntries != null ? logEntries.ToArray() : new string[0];
        }

		public string Exception { get; set; }

        public string[] LogEntries { get; set; }

    }
}
