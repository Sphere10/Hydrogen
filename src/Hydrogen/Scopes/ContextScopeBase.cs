//-----------------------------------------------------------------------
// <copyright file="ContextScopeBase.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2022</date>
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Base class to handle scopes which are aware of other scope instances activated within the call context. These can be
/// used share state and behaviour within the disparate call contexts (e.g. to implement database style commit/abort transactional scopes).
/// </summary>
/// <typeparam name="TScope">The Subclass type (needed to properly type the <see cref="RootScope"/>)</typeparam>
public abstract class ContextScopeBase<TScope> : Scope where TScope : ContextScopeBase<TScope> {

	protected ContextScopeBase(ContextScopePolicy policy, string contextID) {
		Guard.ArgumentNotNullOrWhitespace(contextID, nameof(contextID));
		Policy = policy;
		ContextID = contextID;
		if (CallContext.LogicalGetData(ContextID) is TScope contextObject) {
			// Nested
			if (Policy == ContextScopePolicy.MustBeRoot)
				throw new SoftwareException("A {0} was already declared within the calling context", typeof(TScope).Name);

			IsRootScope = false;
			RootScope = contextObject;
		} else {
			// Root
			if (Policy == ContextScopePolicy.MustBeNested)
				throw new SoftwareException("No {0} was declared in the calling context", typeof(TScope).Name);
			IsRootScope = true;
			RootScope = (TScope)this;
			CallContext.LogicalSetData(ContextID, this);
		}
	}

	/// <summary>
	/// An identifier used to distinguish scopes types within a call context. This generally 1-1 to the repository
	/// instance name that the scope context is being used on.
	/// </summary>
	public string ContextID { get; }

	public ContextScopePolicy Policy { get; }

	public bool IsRootScope { get; }

	public TScope RootScope { get; protected set; }

	protected abstract void OnScopeEnd(TScope rootScope, bool inException);

	protected abstract ValueTask OnScopeEndAsync(TScope rootScope, bool inException);

	protected static TScope GetCurrent(string contextID) {
		return CallContext.LogicalGetData(contextID) as TScope;
	}

	protected sealed override void FreeManagedResources() {
		var inException = Tools.Runtime.IsInExceptionContext();
		// Remove from registry
		if (IsRootScope) 
			CallContext.LogicalSetData(ContextID, null);

		// Notify end
		OnScopeEnd(RootScope, inException);
	}

	protected sealed override ValueTask FreeManagedResourcesAsync() {
		var inException = Tools.Runtime.IsInExceptionContext();
		// Remove from registry
		if (IsRootScope)
			CallContext.LogicalSetData(ContextID, null);

		// Notify end
		return OnScopeEndAsync(RootScope, inException);
	}
}