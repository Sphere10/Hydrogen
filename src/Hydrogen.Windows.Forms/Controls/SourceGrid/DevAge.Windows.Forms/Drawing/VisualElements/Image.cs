//-----------------------------------------------------------------------
// <copyright file="Image.cs" company="Sphere 10 Software">
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
using System.ComponentModel;
using System.Drawing;

namespace DevAge.Drawing.VisualElements
{
    public interface IImage : IVisualElement
    {
        System.Drawing.Image Value
        {
            get;
            set;
        }
    }

    [Serializable]
    public class Image : VisualElementBase, IImage
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Image()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public Image(System.Drawing.Image value)
        {
            Value = value;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Image(Image other)
            : base(other)
        {
            if (other.Value != null)
                Value = (System.Drawing.Image)other.Value.Clone();
            else
                Value = null;

            Enabled = other.Enabled;
        }
        #endregion


        #region Properties
        private System.Drawing.Image mValue = null;
        /// <summary>
        /// Gets or sets the Image to draw. Default is null.
        /// </summary>
        [DefaultValue(null)]
        public System.Drawing.Image Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        private bool mEnabled = true;
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }
        #endregion

        protected override void OnDraw(GraphicsCache graphics, System.Drawing.RectangleF area)
        {
            if (Value != null)
            {
                if (Enabled)
                {
                    graphics.Graphics.DrawImage(Value, Rectangle.Round(area)); //Note: If I don't make the Rectangle.Round sometimes the image is drawed with a strange stretch (not clear). Probably this problem is caused by some rounding in the drawing code that use the float overload
                }
                else
                {
                    using (System.Drawing.Image imgDisabled = Utilities.CreateDisabledImage(Value, Color.White))
                    {
                        graphics.Graphics.DrawImage(imgDisabled, area);
                    }
                }
            }
        }

        protected override System.Drawing.SizeF OnMeasureContent(MeasureHelper measure, System.Drawing.SizeF maxSize)
        {
            if (Value != null)
                return Value.Size;
            else
                return SizeF.Empty;
        }

        public override object Clone()
        {
            return new Image(this);
        }
    }
}
