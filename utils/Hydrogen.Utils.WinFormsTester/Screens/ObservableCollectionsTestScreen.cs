//-----------------------------------------------------------------------
// <copyright file="ObservableCollectionsTest.cs" company="Sphere 10 Software">
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
using Hydrogen.Windows.Forms;

namespace Hydrogen.Utils.WinFormsTester {
    public partial class ObservableCollectionsTestScreen : ApplicationScreen {
        private TextWriter _outputWriter;
        public ObservableCollectionsTestScreen() {
            InitializeComponent();
            _outputWriter = new TextBoxWriter(this.textBoxEx1);
        }

        private void _dictionaryTestButton_Click(object sender, EventArgs _) {

            var dictionary = new ObservableDictionary<string, int>();

            dictionary.Added += (dict, e) => 
                _outputWriter.WriteLine("Single event: Added {0} at {1} ", e.CallArgs.Items.First().Value, e.CallArgs.Items.First().Key);

            dictionary.RemovedKeys += (dict, e) => 
                _outputWriter.WriteLine("Single event: Removed at {0} ", e.CallArgs.Items.First());

            dictionary.Accessed += (dict, traits) => _outputWriter.WriteLine($"Write: {traits}");

            dictionary.Add("string 1", 1);
            dictionary.Remove("string 1");
            dictionary.Add("string 2", 2);
            dictionary.Add("string 3", 3);
            dictionary.Add("string 4", 4);
            dictionary.Add("string 5", 5);
            dictionary.Add("string 6", 6);
            dictionary["string 6"] = -6;
            dictionary.Remove("string 1");
            dictionary.Remove(dictionary.ElementAt(0));
            dictionary.Clear();

        }

        private void _listTestButton_Click(object sender, EventArgs _) {
            var list = new ObservableExtendedList<string>();

            list.Added += (l, e) => _outputWriter.WriteLine($"Added {e.CallArgs.Items.Count()}: {e.CallArgs.Items.ToDelimittedString(",")}");

            list.RemovedRange += (l, e) => _outputWriter.WriteLine($"Removed {e.CallArgs.Count} at {e.CallArgs.Index}");

			list.Accessed += (l, traits) => _outputWriter.WriteLine($"Write: {traits}");

            list.Add("string 1");
            list.Add("string 2");
            list.RemoveAt(1);
            list.Remove("string 1");
            list.AddRange(new [] {"bulk 1", "bulk 2" });
            list.AddRange(new[] { "bulk 3", "bulk 4" });
            list.RemoveRange(2, 2);
            list.RemoveAt(1);
            list.RemoveAt(0);
            list.AddRange(new[] { "bulk 5", "bulk 6" });
            list.Clear();

            list.Add("padding 1");
            list.Add("padding 2");
            list.AddRange(new [] {"padding 6", "padding 7" });
            list.Insert(2, "single insert 3");
            list.InsertRange(3, new [] { "bulk insert 4", "bulk insert 5" });

            _outputWriter.WriteLine("Should be 1,2,3,4,5,6,7: {0}", list.ToDelimittedString(", "));

            list.RemoveRange(1, 5);

            _outputWriter.WriteLine("Should be 1, 7: {0}", list.ToDelimittedString(", "));

            list.Clear();
        }


    }
}
