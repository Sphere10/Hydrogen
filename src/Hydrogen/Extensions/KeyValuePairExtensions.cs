//-----------------------------------------------------------------------
// <copyright file="UniversalExtensions.cs" company="Sphere 10 Software">
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

using System.Collections.Generic;

namespace Hydrogen {
    public static class KeyValuePairExtensions {
        public static KeyValuePair<V, U> ToInverse<U, V>(this KeyValuePair<U, V> kvp) => new KeyValuePair<V, U>(kvp.Value, kvp.Key);
    }
}
