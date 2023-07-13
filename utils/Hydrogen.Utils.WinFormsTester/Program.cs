// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Windows.Forms;
using Hydrogen.Application;


namespace Hydrogen.Utils.WinFormsTester;

/// <summary>
/// Test Licenses
///	S394-TNQV-CC95-MENK			30 Days then Disable
///	2BJR-7APX-YC47-WEWK			1 Load Then Disable
///	TJVM-WVEX-Q7QY-LJEJ			Disable on 1 May 
///	PNE2-NVNQ-7SDT-C62K			Limit to 1 user (or disable)
///	ATPE-JHRQ-S4XB-QWE5			All of the above cripples software
///	C6M8-XYQC-SS5R-NNPR			Permanent trial license for version 1 only
///	7SV5-5PVD-75SY-PJVY			Full Version
///	UCHX-KMD4-33EQ-ZWVF			Cripple on 30 Apr
///	DKZ8-P47S-8XBS-DYN5			Feature Set Permanently crippled
/// </summary>
static class Program {


	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args) {
		System.Windows.Forms.Application.EnableVisualStyles();
		System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
		AppDomain.CurrentDomain.UnhandledException += (s, e) => Tools.Lambda.ActionIgnoringExceptions(() => ExceptionDialog.Show("Error", (Exception)e.ExceptionObject)).Invoke();
		System.Windows.Forms.Application.ThreadException += (xs, xe) => Tools.Lambda.ActionIgnoringExceptions(() => ExceptionDialog.Show("Error", xe.Exception)).Invoke();
		SystemLog.RegisterLogger(new ConsoleLogger());
		HydrogenFramework.Instance.StartWinFormsApplication<BlockMainForm>();
	}
}
