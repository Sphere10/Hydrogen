// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Hydrogen.Application;

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
