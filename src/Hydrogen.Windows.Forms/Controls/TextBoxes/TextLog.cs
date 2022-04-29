//-----------------------------------------------------------------------
// <copyright file="TextLog.cs" company="Sphere 10 Software">
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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sphere10.Framework;

namespace Sphere10.Framework.Windows.Forms.TextBoxes {
    public partial class TextLog : UserControl {
        private readonly ProducerConsumerQueue<char> _appendQueue; 
        private readonly LargeCollection<char> _data;
        private readonly Throttle _throttle;
        public TextLog() {
            InitializeComponent();
            _appendQueue = new ProducerConsumerQueue<char>((c) => sizeof (char), 100000);
            _data = new LargeCollection<char>(65536, 1, (c) => sizeof(char));
            _throttle = new Throttle(2.0f);
        }

        public void AppendText() {
            
        }
        public int SelectionStart {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public int SelectionLength {
            get {
                throw new NotImplementedException();
            }
            set {
                throw new NotImplementedException();
            }
        }

        public string SelectedText {
            get {
                throw new NotImplementedException();
            }
        }

        
    }
}
