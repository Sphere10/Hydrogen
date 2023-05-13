// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Collections.Generic;

namespace Hydrogen {
    public interface IBijectiveDictionary<U, V> : IDictionary<U, V> {
        IBijectiveDictionary<V, U> Bijection { get; }

        bool ContainsValue(V value);

        bool TryGetKey(V value, out U key);

    }

}
