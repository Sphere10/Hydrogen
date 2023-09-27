// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Dev Age
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace DevAge.Shell;

/// <summary>
/// Shell utilities
/// </summary>
public class Utilities {
	public static void OpenFile(string p_File) {
		ExecCommand(p_File);
	}

	public static void ExecCommand(string p_Command) {
		System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(p_Command);
		p.UseShellExecute = true;
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		process.StartInfo = p;
		process.Start();
	}
}
