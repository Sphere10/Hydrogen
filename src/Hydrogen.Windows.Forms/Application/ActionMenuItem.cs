//-----------------------------------------------------------------------
// <copyright file="ActionMenuItem.cs" company="Sphere 10 Software">
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

namespace Hydrogen.Windows.Forms {
    public class ActionMenuItem : MenuItem, ILinkMenuItem {
	    private Action _selectAction;

		public ActionMenuItem(Action onClick)
			: this(string.Empty, onClick) {
        }

		public ActionMenuItem(string text, Action select) {
			Guard.ArgumentNotNull(select, nameof(select));
			Text = text;
			_selectAction = select;
		}

		public ActionMenuItem(string text, Image image16x16, Action OnClick)
            : this(text, image16x16, true, true, false) {
        }

		public ActionMenuItem(
			string text,
			Image image16x16,
			bool showOnExplorerBar = true,
			bool showOnToolBar = true,
			bool executeOnLoad = false
		) : base(image16x16, showOnExplorerBar, showOnToolBar, executeOnLoad)  {
			Text = text;
        }

		

        public virtual string Text { get; set; }

        public virtual void OnSelect() {
	        _selectAction();
        }

        public override void Dispose() {
            base.Dispose();
        }

    }
}
