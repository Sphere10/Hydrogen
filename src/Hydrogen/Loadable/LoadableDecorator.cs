// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System.Threading.Tasks;

namespace Hydrogen;

public class LoadableDecorator<TLoadableImpl> : ILoadable where TLoadableImpl : ILoadable {
	public event EventHandlerEx<object> Loading {
		add => Internal.Loading += value;
		remove => Internal.Loading -= value;
	}

	public event EventHandlerEx<object> Loaded {
		add => Internal.Loaded += value;
		remove => Internal.Loaded -= value;
	}

	protected readonly TLoadableImpl Internal;

	public LoadableDecorator(TLoadableImpl @internal) {
		Guard.ArgumentNotNull(@internal, nameof(@internal));
		Internal = @internal;
	}

	public virtual bool RequiresLoad => Internal.RequiresLoad;

	public virtual void Load() => Internal.Load();

	public virtual Task LoadAsync() => Internal.LoadAsync();
}


public class LoadableDecorator : LoadableDecorator<ILoadable> {
	public LoadableDecorator(ILoadable @internal) : base(@internal) {
	}
}
