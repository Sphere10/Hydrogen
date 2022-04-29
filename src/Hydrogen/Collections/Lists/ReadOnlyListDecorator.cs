//-----------------------------------------------------------------------
// <copyright file="ListDecorator.cs" company="Sphere 10 Software">
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

using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Framework {

	public class ReadOnlyListDecorator<TFrom, TTo> : IReadOnlyList<TTo>  where TTo : TFrom {

        public ReadOnlyListDecorator(IReadOnlyList<TFrom> internalList) {
            InternalList = internalList;
        }

        protected IReadOnlyList<TFrom> InternalList;

        public TTo this[int index] => (TTo)InternalList[index];

        public int Count => InternalList.Count;

        public IEnumerator<TTo> GetEnumerator() => new EnumeratorDecorator<TFrom, TTo>(InternalList.GetEnumerator());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
