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

namespace Hydrogen {

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
}
