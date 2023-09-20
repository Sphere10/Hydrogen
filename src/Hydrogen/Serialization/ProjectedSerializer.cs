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

public class ProjectedSerializer<TFrom, TTo> : IItemSerializer<TTo> {
	private readonly IItemSerializer<TFrom> _sourceSerializer;
	private readonly Func<TFrom, TTo> _projection;
	private readonly Func<TTo, TFrom> _inverseProjection;

	public ProjectedSerializer(IItemSerializer<TFrom> sourceSerializer, Func<TFrom, TTo> projection, Func<TTo, TFrom> inverseProjection) {
		_sourceSerializer = sourceSerializer;
		_projection = projection;
		_inverseProjection = inverseProjection;
	}

	
	
	public bool SupportsNull => _sourceSerializer.SupportsNull;

	public bool IsConstantLength => _sourceSerializer.IsConstantLength;

	public long ConstantLength => _sourceSerializer.ConstantLength;

	public long CalculateTotalSize(IEnumerable<TTo> items, bool calculateIndividualItems, out long[] itemSizes) 
		=> _sourceSerializer.CalculateTotalSize(items.Select(_inverseProjection), calculateIndividualItems, out itemSizes);

	public long CalculateSize(TTo item) 
		=> _sourceSerializer.CalculateSize(_inverseProjection(item));

	public void SerializeInternal(TTo item, EndianBinaryWriter writer) 
		=> _sourceSerializer.SerializeInternal(_inverseProjection(item), writer);

	public TTo DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> _projection(_sourceSerializer.DeserializeInternal(byteSize, reader));


}
