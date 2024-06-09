// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

/// <summary>
/// Base class to handle scopes which are aware of other scope instances activated within the call context. These can be
/// used share state and behaviour within the disparate call contexts (e.g. to implement database style commit/abort transactional scopes).
/// </summary>
public abstract class ContextScope : ScopeBase, IContextScope {

	protected ContextScope(ContextScopePolicy policy, string contextID) {
		Guard.ArgumentNotNullOrWhitespace(contextID, nameof(contextID));
		Policy = policy;
		ContextID = contextID;
		if (CallContext.LogicalGetData(ContextID) is IContextScope contextObject) {
			// Nested
			if (Policy == ContextScopePolicy.MustBeRoot)
				throw new SoftwareException($"A scope context '{contextID}' has already been declared within the calling context");
			RootScope = contextObject;
			OnContextResume();
		} else {
			// Root
			if (Policy == ContextScopePolicy.MustBeNested)
				throw new SoftwareException($"No scope context '{contextID}' exists within the calling context");
			RootScope = this;
			CallContext.LogicalSetData(ContextID, this);
			OnContextStart();
		}
	}

	/// <summary>
	/// An identifier used to distinguish scopes types within a call context. This generally 1-1 to the repository
	/// instance name that the scope context is being used on.
	/// </summary>
	public string ContextID { get; }

	public ContextScopePolicy Policy { get; }

	public IContextScope RootScope { get; }

	public bool IsRootScope => ReferenceEquals(this, RootScope);


	protected sealed override void OnScopeEnd() {
		OnScopeEndInternal();
		if (IsRootScope) {
			CallContext.LogicalSetData(ContextID, null);
			OnContextEnd();
		}
	}

	protected sealed override async ValueTask OnScopeEndAsync() {
		await OnScopeEndInternalAsync();
		if (IsRootScope) {
			CallContext.LogicalSetData(ContextID, null);
			await OnContextEndAsync();
		}
	}

	protected virtual void OnScopeEndInternal() {
	}

	protected virtual async ValueTask OnScopeEndInternalAsync() {
	}

	protected virtual void OnContextStart() {
	}

	protected virtual void OnContextResume() {
	}

	protected abstract void OnContextEnd();

	protected abstract ValueTask OnContextEndAsync();

	protected static IContextScope GetCurrent(string contextID) {
		return CallContext.LogicalGetData(contextID) as IContextScope;
	}
}
