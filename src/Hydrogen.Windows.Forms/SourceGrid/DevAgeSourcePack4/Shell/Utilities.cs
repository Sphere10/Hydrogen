//-----------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Sphere 10 Software">
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

namespace DevAge.Shell
{
	/// <summary>
	/// Shell utilities
	/// </summary>
	public class Utilities
	{
		public static void OpenFile(string p_File)
		{
			ExecCommand(p_File);
		}

		public static void ExecCommand(string p_Command)
		{
			System.Diagnostics.ProcessStartInfo p = new System.Diagnostics.ProcessStartInfo(p_Command);
			p.UseShellExecute = true;
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo = p;
			process.Start();
		}
	}
}
