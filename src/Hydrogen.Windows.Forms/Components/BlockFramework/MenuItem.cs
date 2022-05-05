//-----------------------------------------------------------------------
// <copyright file="MenuItem.cs" company="Sphere 10 Software">
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
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Hydrogen.Windows.Forms {

    public class MenuItem : IMenuItem 
    {
        private IMenu _parent;
        private Image _image16x16;
        private bool _showOnExplorerBar;
        private bool _showOnToolBar;
        private bool _executeOnLoad;



        public MenuItem()
            : this(null)        {
        }

        public MenuItem(Image image16x16)
            : this(image16x16, true, true, false) {
        }

        public MenuItem(Image image16x16, bool showOnExplorerBar, bool showOnToolBar, bool executeOnLoad) {
            _parent = null;
            _image16x16 = image16x16;
            _showOnExplorerBar = showOnExplorerBar;
            _showOnToolBar = showOnToolBar;
            _executeOnLoad = executeOnLoad;
        }

        public IMenu Parent {
            get {
                return _parent;
            }
            set {
                _parent = value;
            }
        }

        public Image Image16x16 {
            get {
                return _image16x16;
            }
            set {
                _image16x16 = value;
            }
        }

        public bool ShowOnExplorerBar {
            get {
                return _showOnExplorerBar;
            }
            set {
                _showOnExplorerBar = value;
            }
        }

        public bool ShowOnToolStrip {
            get {
                return _showOnToolBar;
            }
            set {
                _showOnToolBar = value;
            }
        }

        public bool ExecuteOnLoad {
            get { return _executeOnLoad; }
            set { _executeOnLoad = value; }
        }

        public virtual void Dispose() {
            _parent = null;
            if (_image16x16 != null) {
                _image16x16.Dispose();
            }
        }
    }
}
