// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public class ScopeDecorator<TScope> : IScope where TScope : IScope {
	public event EventHandlerEx ScopeEnd;

	protected internal TScope Internal;

	public ScopeDecorator(TScope internalScope) {
		Guard.ArgumentNotNull(internalScope, nameof(internalScope));
		Internal = internalScope;
	}

	public virtual void Dispose() => Internal.Dispose();

	public virtual ValueTask DisposeAsync() => Internal.DisposeAsync();

}
