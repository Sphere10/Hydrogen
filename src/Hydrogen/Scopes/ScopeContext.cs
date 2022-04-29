//-----------------------------------------------------------------------
// <copyright file="ScopeContext.cs" company="Sphere 10 Software">
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
using System.Runtime.InteropServices;

namespace Sphere10.Framework {

	public abstract class ScopeContext<T> : Disposable where T : ScopeContext<T> {

		protected ScopeContext(string contextName, ScopeContextPolicy policy) {
			Policy = policy;

			ContextName = contextName;
			if (CallContext.LogicalGetData(ContextName) is T contextObject) {
                // Nested
                if (Policy == ScopeContextPolicy.MustBeRoot)
                    throw new SoftwareException("A {0} was already declared within the calling context", typeof (T).Name);

                IsRootScope = false;
                RootScope = contextObject;
            } else {
                // Root
                if (Policy == ScopeContextPolicy.MustBeNested)
                    throw new SoftwareException("No {0} was declared in the calling context", typeof (T).Name);
                IsRootScope = true;
                RootScope = (T) this;
                CallContext.LogicalSetData(ContextName, this);
            }
        }

        public string ContextName { get; }

        public ScopeContextPolicy Policy { get; }

		public bool IsRootScope { get; }

        public T RootScope { get; protected set; }

        protected abstract void OnScopeEnd(T rootScope, bool inException);

        protected static T GetCurrent(string contextName) {
            return CallContext.LogicalGetData(contextName) as T;
        }

        protected sealed override void FreeManagedResources() {
	        var inException = Tools.Runtime.IsInExceptionContext(); 
            // Remove from registry
            if (IsRootScope) {
                CallContext.LogicalSetData(ContextName, null);
            }

            // Notify end
            OnScopeEnd(RootScope, inException);
        }
    }

}
