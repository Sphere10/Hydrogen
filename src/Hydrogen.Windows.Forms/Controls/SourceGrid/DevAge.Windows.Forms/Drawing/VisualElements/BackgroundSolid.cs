//-----------------------------------------------------------------------
// <copyright file="BackgroundSolid.cs" company="Sphere 10 Software">
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

namespace DevAge.Drawing.VisualElements
{
    [Serializable]
    public class BackgroundSolid : VisualElementBase
    {
        #region Constructor
        public BackgroundSolid()
        {
        }

        public BackgroundSolid(Color backcolor)
        {
            BackColor = backcolor;
        }

        public BackgroundSolid(BackgroundSolid other)
            : base(other)
        {
            BackColor = other.BackColor;
        }
        #endregion

        #region Properties
        private Color mBackColor = Color.Empty;
        /// <summary>
        /// Gets or sets the back color of the content.
        /// </summary>
        public virtual Color BackColor
        {
            get { return mBackColor; }
            set { mBackColor = value; }
        }
        protected virtual bool ShouldSerializeBackColor()
        {
            return BackColor != Color.Empty;
        }
        #endregion

        protected override void OnDraw(GraphicsCache graphics, System.Drawing.RectangleF area)
        {
            if (BackColor != Color.Empty)
            {
                SolidBrush brush = graphics.BrushsCache.GetBrush(BackColor);
                graphics.Graphics.FillRectangle(brush, area);
            }
        }

        protected override SizeF OnMeasureContent(MeasureHelper measure, SizeF maxSize)
        {
            return SizeF.Empty;
        }

        /// <summary>
        /// Clone
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new BackgroundSolid(this);
        }
    }
}
