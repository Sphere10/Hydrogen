//-----------------------------------------------------------------------
// <copyright file="CachedItem.cs" company="Sphere 10 Software">
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

    public class CachedItem<T> : IDisposable {

        public bool Invalidated { get; set; }

        public DateTime FetchedOn { get; internal set; }

        public DateTime LastAccessedOn { get; internal set; }

        public uint AccessedCount { get; internal set; }

        public uint Size { get; internal set; }

        public T Value { get; internal set; }

		public void Dispose() {
			if (Value is IDisposable disposable) {
				disposable.Dispose();
			}
		}

	
    }
}
