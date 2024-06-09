// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen;

public class ItemChecksummerDecorator<TItem, TInner> : IItemChecksummer<TItem> where TInner : IItemChecksummer<TItem> {
	internal readonly TInner InnerChecksummer;

	public ItemChecksummerDecorator(TInner innerChecksummer) {
		InnerChecksummer = innerChecksummer;
	}

	public virtual int CalculateChecksum(TItem item) => InnerChecksummer.CalculateChecksum(item);
}

public class ItemChecksummerDecorator<TItem> : ItemChecksummerDecorator<TItem, IItemChecksummer<TItem>> {
	public ItemChecksummerDecorator(IItemChecksummer<TItem> innerChecksummer) 
		: base(innerChecksummer) {
	}
}