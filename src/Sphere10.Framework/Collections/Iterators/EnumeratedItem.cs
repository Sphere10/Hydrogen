//-----------------------------------------------------------------------
// <copyright file="EnumeratedItem.cs" company="Sphere 10 Software">
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
using System.Linq;
using System.Text;

namespace Sphere10.Framework {

    public class EnumeratedItem<T> {
        public int Index { get; private set; }
        public T Item { get; private set; }
        public EnumeratedItemDescription Description { get; private set; }

        public EnumeratedItem(int index, T item, EnumeratedItemDescription description) {
            Index = index;
            Item = item;
            Description = description;
        }

        public bool Is(EnumeratedItemDescription description) {
            return (Description & description) == description;
        }

    }
}
