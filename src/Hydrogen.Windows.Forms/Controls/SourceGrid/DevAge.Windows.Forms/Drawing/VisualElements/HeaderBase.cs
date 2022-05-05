//-----------------------------------------------------------------------
// <copyright file="HeaderBase.cs" company="Sphere 10 Software">
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

namespace DevAge.Drawing.VisualElements
{
    public interface IHeader : IBackground
    {
        ControlDrawStyle Style
        {
            get;
            set;
        }
    }

    [Serializable]
    public abstract class HeaderBase : BackgroundBase, IHeader
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public HeaderBase()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public HeaderBase(HeaderBase other)
            : base(other)
        {
            Style = other.Style;
        }
        #endregion

        #region Properties
        private ControlDrawStyle mControlDrawStyle = ControlDrawStyle.Normal;
        public virtual ControlDrawStyle Style
        {
            get { return mControlDrawStyle; }
            set { mControlDrawStyle = value; }
        }
        protected virtual bool ShouldSerializeStyle()
        {
            return Style != ControlDrawStyle.Normal;
        }
        #endregion
    }
}
