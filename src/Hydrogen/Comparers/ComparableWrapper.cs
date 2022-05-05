//-----------------------------------------------------------------------
// <copyright file="ComparableWrapper.cs" company="Sphere 10 Software">
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

namespace Hydrogen {
	public class ComparableWrapper<T> : IComparable<T>, IComparable, IEquatable<T> {
		public readonly T @Object;
		private readonly Comparer<T> comparer;

		public ComparableWrapper(T internalObject) {
			@Object = internalObject;
			comparer = Comparer<T>.Default;
		}


		public int CompareTo(T other) {
			return comparer.Compare(@Object, other);
		}

		public int CompareTo(object obj) {
			return Comparer<object>.Default.Compare(@Object, obj);
		}

		public bool Equals(T other) {
			return CompareTo(other) == 0;
		}
	}
}
