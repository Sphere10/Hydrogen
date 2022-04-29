//-----------------------------------------------------------------------
// <copyright file="LinkMenuItem.cs" company="Sphere 10 Software">
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

namespace Sphere10.Framework.Windows.Forms {
    public class LinkMenuItem : MenuItem, ILinkMenuItem
    {
        private string _text;

        public LinkMenuItem()
            : this(string.Empty) {
        }

        public LinkMenuItem(string text)
            : this(text, null) {
        }

        public LinkMenuItem(string text, Image image16x16)
            : this(text, image16x16, true, true, false) {
        }

        public LinkMenuItem(string text, Image image16x16, bool showOnExplorerBar, bool showOnToolBar, bool executeOnLoad )
            : base(image16x16, showOnExplorerBar, showOnToolBar, executeOnLoad) 
        {
            
            _text = text;
        }


        public virtual string Text {
            get {
                return _text;
            }
            set {
                _text = value;
            }
        }



        public virtual void OnSelect() {
        }

        public override void Dispose() {
            base.Dispose();
        }

    }
}
