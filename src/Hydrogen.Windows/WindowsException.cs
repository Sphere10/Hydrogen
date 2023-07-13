// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;


namespace Hydrogen.Windows;

public class WindowsException : SoftwareException {

	public WindowsException(string message, params object[] formatArgs)
		: base(message, formatArgs) {
	}


	public WindowsException(int winApiResult, string message, params object[] formatArgs)
		: base(
			string.Format(
				"{0}.{1}",
				message != null ? " " + string.Format(message, formatArgs) : string.Empty,
				ErrorCodeToDescription(winApiResult)
			)
		) {
	}

	public static string ErrorCodeToDescription(int win32ApiResult) {
		Exception winError = new System.ComponentModel.Win32Exception(win32ApiResult);
		return winError.Message;
	}

}
