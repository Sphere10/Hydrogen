//-----------------------------------------------------------------------
// <copyright file="RowHeaderBase.cs" company="Sphere 10 Software">
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
    public interface IRowHeader : IHeader
    {
    }

    [Serializable]
    public abstract class RowHeaderBase : HeaderBase, IRowHeader
    {
        #region Constuctor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RowHeaderBase()
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="other"></param>
        public RowHeaderBase(RowHeaderBase other)
            : base(other)
        {
        }
        #endregion
    }
}
