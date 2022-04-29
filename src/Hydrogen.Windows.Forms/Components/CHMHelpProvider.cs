//-----------------------------------------------------------------------
// <copyright file="CHMHelpProvider.cs" company="Sphere 10 Software">
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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using Hydrogen;
using Hydrogen.Application;

namespace Hydrogen.Windows.Forms {

    public class CHMHelpProvider : IHelpServices {
        private static readonly object SyncObject;
        private string _chmFile;

        static CHMHelpProvider() {
            SyncObject = new object();
        }

        public CHMHelpProvider(IUserInterfaceServices userInterfaceServices, IProductInformationServices productInformationServices) {

            UserInterfaceServices = userInterfaceServices;
            ProductInformationServices = productInformationServices;
            CHMFile = null;


        }

        public IUserInterfaceServices UserInterfaceServices { get; private set; }

        public IProductInformationServices ProductInformationServices { get; private set; }

        public string CHMFile {
            get {
                if (_chmFile == null) {
                    lock (SyncObject) {
                        if (_chmFile == null) {
                            var chmQuery = ProductInformationServices.ProductInformation.HelpResources.Where(hr => hr.Item1 == HelpType.CHM);
                            if (!chmQuery.Any()) {
                                throw new SoftwareException("No default CHM help file is defined");
                            }
                            CHMFile = chmQuery.First().Item2;
                        }
                    }
                }
                return _chmFile;
            }
            private set {
                if (value == null) {
                    _chmFile = null;
                } else {
                    _chmFile = ProductInformationServices.ProductInformation.ProcessTokensInString(value);
                    if (!File.Exists(_chmFile)) {
                        throw new SoftwareException("File does not exist '{0}'", _chmFile);
                    }
                }
            }
        }

        public void ShowContextHelp(IHelpableObject helpableObject) {
            System.Windows.Forms.Help.ShowHelp(
                UserInterfaceServices.PrimaryUIController as Control,
                File.Exists(helpableObject.FileName) ? helpableObject.FileName : CHMFile,
                System.Windows.Forms.HelpNavigator.TopicId,
                helpableObject.HelpTopicID.Value.ToString()
            );
            
            // Use this to adjust the screen size of the help window
            // UserInterfaceServices.ExecuteInUIFriendlyContext( () => MoveHelpWindow(new Rectangle(0, 0, 300, 200)));
        }

        public void ShowHelp() {
            System.Windows.Forms.Help.ShowHelp(
                UserInterfaceServices.PrimaryUIController as Control,
                CHMFile
            );
        }


        // Use this to adjust the size of the help window 
        // Borrowed from http://stackoverflow.com/questions/4819570/how-can-i-control-the-size-of-the-help-window-using-system-windows-forms-help-sh
        //private static void MoveHelpWindow(Rectangle rc) {
        //    EnumThreadWndProc callback = (hWnd, lp) => {
        //        // Check if this is the help window
        //        StringBuilder sb = new StringBuilder(260);
        //        GetClassName(hWnd, sb, sb.Capacity);
        //        if (sb.ToString() != "HH Parent") return true;
        //        MoveWindow(hWnd, rc.Left, rc.Top, rc.Width, rc.Height, false);
        //        return false;
        //    };
        //    foreach (ProcessThread pth in Process.GetCurrentProcess().Threads) {
        //        EnumThreadWindows(pth.Id, callback, IntPtr.Zero);
        //    }
        //}

        //// P/Invoke declarations
        //private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
        //[DllImport("user32.dll")]
        //private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
        //[DllImport("user32.dll")]
        //private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
        //[DllImport("user32.dll")]
        //private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);

    }
}


