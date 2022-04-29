//-----------------------------------------------------------------------
// <copyright file="ToolTipText.cs" company="Sphere 10 Software">
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

namespace SourceGrid.Cells.Controllers
{
	/// <summary>
	/// Allow to customize the tooltiptext of a cell. This class read the tooltiptext from the ICellToolTipText.GetToolTipText.  This behavior can be shared between multiple cells.
	/// </summary>
	public class ToolTipText : ControllerBase
	{
		/// <summary>
		/// Default tooltiptext
		/// </summary>
		public readonly static ToolTipText Default = new ToolTipText();

		#region IBehaviorModel Members
		public override void OnMouseEnter(CellContext sender, EventArgs e)
		{
			base.OnMouseEnter(sender, e);

			ApplyToolTipText(sender, e);
		}

		public override void OnMouseLeave(CellContext sender, EventArgs e)
		{
			base.OnMouseLeave(sender, e);

			ResetToolTipText(sender, e);
		}
		#endregion

        private string mToolTipTitle = string.Empty;
        public string ToolTipTitle
        {
            get { return mToolTipTitle; }
            set { mToolTipTitle = value; }
        }

        private System.Windows.Forms.ToolTipIcon mToolTipIcon = System.Windows.Forms.ToolTipIcon.None;
        public System.Windows.Forms.ToolTipIcon ToolTipIcon
        {
            get { return mToolTipIcon; }
            set { mToolTipIcon = value; }
        }

        private bool mIsBalloon = false;
        public bool IsBalloon
        {
            get { return mIsBalloon; }
            set { mIsBalloon = value; }
        }

        private System.Drawing.Color mBackColor = System.Drawing.Color.Empty;
        public System.Drawing.Color BackColor
        {
            get { return mBackColor; }
            set { mBackColor = value; }
        }
        private System.Drawing.Color mForeColor = System.Drawing.Color.Empty;
        public System.Drawing.Color ForeColor
        {
            get { return mForeColor; }
            set { mForeColor = value; }
        }


		/// <summary>
		/// Change the cursor with the cursor of the cell
		/// </summary>
		protected virtual void ApplyToolTipText(CellContext sender, EventArgs e)
		{
			Models.IToolTipText toolTip;
			if ( (toolTip = (Models.IToolTipText)sender.Cell.Model.FindModel(typeof(Models.IToolTipText))) != null)
			{
                string text = toolTip.GetToolTipText(sender);
                if (text != null && text.Length > 0)
                {
                    sender.Grid.ToolTipText = text;
                    sender.Grid.ToolTip.ToolTipTitle = ToolTipTitle;
                    sender.Grid.ToolTip.ToolTipIcon = ToolTipIcon;
                    sender.Grid.ToolTip.IsBalloon = IsBalloon;
                    if (BackColor.IsEmpty == false)
                        sender.Grid.ToolTip.BackColor = BackColor;
                    if (ForeColor.IsEmpty == false)
                        sender.Grid.ToolTip.ForeColor = ForeColor;
                }
			}
		}

		/// <summary>
		/// Reset the original cursor
		/// </summary>
		protected virtual void ResetToolTipText(CellContext sender, EventArgs e)
		{
			if ( sender.Cell.Model.FindModel(typeof(Models.IToolTipText)) != null)
			{
                sender.Grid.ToolTipText = null;
			}
		}
	}
}
