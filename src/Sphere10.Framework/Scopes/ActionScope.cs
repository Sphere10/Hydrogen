//-----------------------------------------------------------------------
// <copyright file="ActionScope.cs" company="Sphere 10 Software">
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

	public class ActionScope : Scope {
		private readonly Action _endAction;

        public ActionScope(Action endAction) {
            _endAction = endAction;
		}

        protected override void OnScopeEnd() {
            _endAction?.Invoke();
        }
	}

    public class ActionScope<T> : ActionScope, IScope<T> {

		public ActionScope(T item, Action<T> endAction) 
			: base(() => endAction(item)) {
			Item = item;
		}

		public T Item { get; }
	}
}
