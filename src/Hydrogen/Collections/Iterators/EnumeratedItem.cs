// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen {

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
