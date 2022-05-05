//-----------------------------------------------------------------------
// <copyright file="Icon.cs" company="Sphere 10 Software">
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
    public interface IIcon : IVisualElement
    {
        System.Drawing.Icon Value
        {
            get;
            set;
        }
    }

    [Serializable]
    public class Icon : VisualElementBase, IIcon
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Icon()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value"></param>
        public Icon(System.Drawing.Icon value)
        {
            Value = value;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public Icon(Icon other)
            : base(other)
        {
            if (other.Value != null)
                Value = (System.Drawing.Icon)other.Value.Clone();
            else
                Value = null;
        }
        #endregion

        #region Properties
        private System.Drawing.Icon mValue = null;
        /// <summary>
        /// Gets or sets the Icon to draw. Default is null.
        /// </summary>
        [DefaultValue(null)]
        public System.Drawing.Icon Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        #endregion

        protected override void OnDraw(GraphicsCache graphics, System.Drawing.RectangleF area)
        {
            if (Value != null)
            {
                graphics.Graphics.DrawIcon(Value, Rectangle.Round(area));
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
            return new Icon(this);
        }
    }
}
