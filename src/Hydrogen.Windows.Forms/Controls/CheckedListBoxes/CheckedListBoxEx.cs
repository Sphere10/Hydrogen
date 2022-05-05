//-----------------------------------------------------------------------
// <copyright file="CheckedListBoxEx.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hydrogen;

namespace Hydrogen.Windows.Forms {
	public class CheckedListBoxEx : CheckedListBox {

		[Description("Occurs AFTER the checked state of an item changes.")]
		public event ItemCheckEventHandler ItemChecked;

		public CheckedListBoxEx() {
			CheckOnClick = true;
			this.ItemCheck += (o, e) => this.BeginInvokeEx(() => RaiseItemCheckedEvent(o, e));
		}

		[DefaultValue(false)]
		public bool DrawFocusRect { get; set; }


		public void CheckItems(IEnumerable<object> items) {
			foreach(var item in items) {
				for (int i = 0; i < base.Items.Count; i++) {
					if (items.Contains(base.Items[i])) {
						SetItemChecked(i, true);
					}
				}
			}
		}

		public void CheckItems(Func<object, bool> predicate) {
			CheckItems<object>(predicate);
		}

		public void CheckItems<TEntity>(Func<TEntity, bool> predicate) {
			for (int i = 0; i < base.Items.Count; i++) {
				SetItemChecked(i, predicate((TEntity)base.Items[i]));
			}
		}

		protected virtual void OnItemChecked(ItemCheckEventArgs e) {
		}

		/// <summary>Overrides the OnDrawItem for the CheckedListBox so that we can customize how the items are drawn.</summary>
		/// <param name="e">The System.Windows.Forms.DrawItemEventArgs object with the details</param>
		/// <remarks>A CheckedListBox can only have one item selected at a time and it's always the item in focus.
		/// So, don't draw an item as selected since the selection colors are hideous.  
		/// Just use the focus rect to indicate the selected item.</remarks>
		protected override void OnDrawItem(DrawItemEventArgs e) {
			Color foreColor = this.ForeColor;
			Color backColor = this.BackColor;

			DrawItemState s2 = e.State;

			//If the item is in focus, then it should always have the focus rect.
			//Sometimes it comes in with Focus and NoFocusRect.
			//This is annoying and the user can't tell where their focus is, so give it the rect.
			if ((s2 & DrawItemState.Focus) == DrawItemState.Focus) {
				if (DrawFocusRect)
					s2 &= ~DrawItemState.NoFocusRect;
				else
					s2 &= DrawItemState.NoFocusRect;
			}

			//Turn off the selected state.  Note that the color has to be overridden as well, but I just did that for all drawing since I want them to match.
			if ((s2 & DrawItemState.Selected) == DrawItemState.Selected) {
				s2 &= ~DrawItemState.Selected;

			}

			//Console.WriteLine("Draw " + e.Bounds + e.State + " --> " + s2);

			//Compile the new drawing args and let the base draw the item.
			DrawItemEventArgs e2 = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, s2, foreColor, backColor);
			base.OnDrawItem(e2);
		}

		private void RaiseItemCheckedEvent(object sender, ItemCheckEventArgs e) {
			OnItemChecked(e);
			if (ItemChecked != null)
				ItemChecked(this,e);
		}
	}
}

