// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

public abstract class CachedItem : IDisposable {

	protected CachedItem() {
		Traits = CachedItemTraits.Default;
	}

	public object Value { get; internal set; }

	public CachedItemTraits Traits { get; internal set; }

	public DateTime FetchedOn { get; internal set; }

	public DateTime LastAccessedOn { get; internal set; }

	public uint AccessedCount { get; internal set; }

	public long Size { get; internal set; }

	public bool CanPurge {
		get => Traits.HasFlag(CachedItemTraits.CanPurge);
		set => Traits = Traits.CopyAndSetFlags(CachedItemTraits.CanPurge, value);
	}

	public virtual void Dispose() {
		if (Value is IDisposable disposable) {
			disposable.Dispose();
		}
	}
}


public class CachedItem<T> : CachedItem {
	public new T Value {
		get => (T)base.Value;
		internal set => base.Value = value;
	}
}
