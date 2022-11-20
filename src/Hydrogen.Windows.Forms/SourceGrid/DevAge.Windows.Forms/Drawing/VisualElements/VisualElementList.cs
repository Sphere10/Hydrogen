//-----------------------------------------------------------------------
// <copyright file="VisualElementList.cs" company="Sphere 10 Software">
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
    [Serializable]
    public class VisualElementList : List<IVisualElement>, ICloneable
    {
        #region ICloneable Members
        public object Clone()
        {
            VisualElementList elements = new VisualElementList();
            foreach (IVisualElement element in this)
            {
                elements.Add((IVisualElement)element.Clone());
            }

            return elements;
        }
        #endregion
    }
}
