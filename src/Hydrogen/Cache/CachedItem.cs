// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Represents a cached entry with metadata used for expiration and reaping decisions.
/// </summary>
public abstract class CachedItem : IDisposable {

	protected CachedItem() {
		Traits = CachedItemTraits.Default;
	}

	/// <summary>
	/// Cached value.
	/// </summary>
	public object Value { get; internal set; }

	/// <summary>
	/// Trait flags describing cache state and permissions.
	/// </summary>
	public CachedItemTraits Traits { get; internal set; }

	/// <summary>
	/// Time the value was originally fetched.
	/// </summary>
	public DateTime FetchedOn { get; internal set; }

	/// <summary>
	/// Last time the value was accessed.
	/// </summary>
	public DateTime LastAccessedOn { get; internal set; }

	/// <summary>
	/// Number of times the value has been accessed.
	/// </summary>
	public uint AccessedCount { get; internal set; }

	/// <summary>
	/// Estimated size contribution used for capacity calculations.
	/// </summary>
	public long Size { get; internal set; }

	/// <summary>
	/// Indicates whether the cache is permitted to evict this item.
	/// </summary>
	public bool CanPurge {
		get => Traits.HasFlag(CachedItemTraits.CanPurge);
		set => Traits = Traits.CopyAndSetFlags(CachedItemTraits.CanPurge, value);
	}

	/// <summary>
	/// Disposes the cached value when it implements <see cref="IDisposable"/>.
	/// </summary>
	public virtual void Dispose() {
		if (Value is IDisposable disposable) {
			disposable.Dispose();
		}
	}
}


/// <summary>
/// Strongly-typed cached entry wrapper.
/// </summary>
public class CachedItem<T> : CachedItem {
	/// <summary>
	/// Cached value strongly typed to <typeparamref name="T"/>.
	/// </summary>
	public new T Value {
		get => (T)base.Value;
		internal set => base.Value = value;
	}
}
