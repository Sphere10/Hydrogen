//-----------------------------------------------------------------------
// <copyright file="SettingsTest.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrogen;
using Hydrogen.Application;
using Hydrogen.Data;
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
    public partial class SettingsTest : ApplicationScreen {
        private readonly TextWriter _outputWriter;
        public SettingsTest() {
            InitializeComponent();
            _outputWriter = new TextBoxWriter(_outputTextBox);
        }

        private void _test1Button_Click(object sender, EventArgs e) {
            var settingsObject = UserSettings.Get<Test1Settings>();

            var x = 1;
        }
    }


    public class Test1Settings : SettingsObject {

        [DefaultValue(DBMSType.Sqlite)]
        public DBMSType DBMSType { get; set; }


        [DefaultValue(null)]
        public string ConnectionString { get; set; }


        [DefaultValue("en")]
        public string ActiveLanguage { get; set; }

        [DefaultValue(true)]
        public bool FirstTimeRun { get; set; }

        [DefaultValue(0)]
        public int RunCount { get; set; }

        [DefaultDate(true, 0, 0, 0, 0, 0, 0, 0)]
        public DateTime FirstRunDate { get; set; }

    }
}
