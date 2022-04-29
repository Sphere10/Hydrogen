//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Sphere 10 Software">
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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using Sphere10.Framework.Windows.Forms;
using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Framework.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using Sphere10.Framework.Communications;
using Sphere10.FrameworkTester.WinForms;

namespace Sphere10.FrameworkTester.WinForms {

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

        public static CommandLineParameters Parameters = new() {
            Commands = new CommandLineCommand[] {
                new("anonymouspipeclient", "AnonymousPipeClient child process tester") {
                    Parameters = new CommandLineParameter[] {
                        new("read", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory),
                        new("write", "AnonymousPipeClient child process tester", CommandLineParameterOptions.Mandatory),
                    }
                }
            },
        };


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Result<CommandLineResults> arguments = Parameters.TryParseArguments(args);
            if (arguments.Failure) {
                // Todo: should only fail if wrong, not empty
                Parameters.PrintHeader();
                foreach (string errorMessage in arguments.ErrorMessages) {
                    Console.WriteLine(errorMessage);
                }
            } 
            if (arguments.Value.HelpRequested) {
                Parameters.PrintHelp();
            }

                        
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Tools.Lambda.ActionIgnoringExceptions(() => ExceptionDialog.Show("Error", (Exception)e.ExceptionObject)).Invoke();
            System.Windows.Forms.Application.ThreadException += (xs, xe) => Tools.Lambda.ActionIgnoringExceptions(() => ExceptionDialog.Show("Error", xe.Exception)).Invoke();
            SystemLog.RegisterLogger(new ConsoleLogger());

            ComponentRegistry.Instance.RegisterComponentInstance<CommandLineResults>(arguments.Value);
            ComponentRegistry.Instance.RegisterApplicationBlock<TestBlock>(1);
            ComponentRegistry.Instance.RegisterApplicationBlock<TestBlock2>(2);
            Sphere10Framework.Instance.StartWinFormsApplication<BlockMainForm>();
        }
    }
}
