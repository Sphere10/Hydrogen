// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hydrogen;

public sealed class ProjectedSerializer<TFrom, TTo> : IItemSerializer<TTo> {
	private readonly IItemSerializer<TFrom> _sourceSerializer;
	private readonly Func<TFrom, TTo> _projection;
	private readonly Func<TTo, TFrom> _inverseProjection;

	public ProjectedSerializer(IItemSerializer<TFrom> sourceSerializer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) {
		_sourceSerializer = sourceSerializer;
		_projection = projection;
		_inverseProjection = inverseProjection;
	}

	public bool IsStaticSize => _sourceSerializer.IsStaticSize;

	public long StaticSize => _sourceSerializer.StaticSize;

	public long CalculateTotalSize(IEnumerable<TTo> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _sourceSerializer.CalculateTotalSize(items.Select(_inverseProjection), calculateIndividualItems, out itemSizes);

	public long CalculateSize(TTo item) 
		=> _sourceSerializer.CalculateSize(_inverseProjection(item));

	public void SerializeInternal(TTo item, EndianBinaryWriter writer) 
		=> _sourceSerializer.SerializeInternal(_inverseProjection(item), writer);

	public TTo DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _projection(_sourceSerializer.DeserializeInternal(byteSize, reader));
}


/*
public class PackedComparer : IComparer<object> {
	private readonly object _comparer;
	private readonly IComparer<object> _projectedComparer;
	
	private PackedComparer(object comparer, IComparer<object> projectedComparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		Guard.ArgumentNotNull(projectedComparer, nameof(projectedComparer));
		Guard.Argument(comparer.GetType().IsSubtypeOfGenericType(typeof(IComparer<>)), nameof(projectedComparer), "Must be an IComparer<>");	
		Guard.Argument(projectedComparer.GetType().IsSubtypeOfGenericType(typeof(ProjectedComparer<,>)), nameof(projectedComparer), "Must be an ProjectedComparer<>");	
		_comparer = comparer;
		_projectedComparer = projectedComparer;
	}

	public int Compare(object x, object y) => _projectedComparer.Compare(x, y);

	public static PackedComparer Pack<TItem>(IComparer<TItem> comparer) {
		Guard.ArgumentNotNull(comparer, nameof(comparer));
		return new PackedComparer(comparer, comparer.AsProjection(x => (object)x, x => (TItem)x));
	}

	public IComparer<TItem> Unpack<TItem>() {
		var unpacked = _comparer as IComparer<TItem>;
		Guard.Ensure(unpacked != null, $"Cannot unpack {_comparer.GetType().Name} as is not an IComparer<{typeof(TItem).Name}>");
		return unpacked;
	}
	
} 

 */