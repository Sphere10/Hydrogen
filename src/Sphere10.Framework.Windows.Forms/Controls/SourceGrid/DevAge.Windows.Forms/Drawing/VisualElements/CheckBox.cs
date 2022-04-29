//-----------------------------------------------------------------------
// <copyright file="CheckBox.cs" company="Sphere 10 Software">
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

namespace DevAge.Drawing.VisualElements
{
    [Serializable]
    public class CheckBox : CheckBoxBase
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public CheckBox()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public CheckBox(CheckBox other)
            : base(other)
        {
        }
        #endregion
        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new CheckBox(this);
        }

        protected override void OnDraw(GraphicsCache graphics, RectangleF area)
        {
            ButtonState state;
            if (Style == ControlDrawStyle.Disabled)
                state = ButtonState.Inactive;
            else if (Style == ControlDrawStyle.Pressed)
                state = ButtonState.Pushed;
            else if (Style == ControlDrawStyle.Hot)
                state = ButtonState.Normal;
            else
                state = ButtonState.Normal;

            if (CheckBoxState == CheckBoxState.Checked)
                state |= ButtonState.Checked;

            ControlPaint.DrawCheckBox(graphics.Graphics, Rectangle.Round(area), state);
        }

        protected override SizeF OnMeasureContent(MeasureHelper measure, SizeF maxSize)
        {
            //TODO Check to see if it is possible to get the real default size...
            return new SizeF(16, 16);
        }
    }
}
