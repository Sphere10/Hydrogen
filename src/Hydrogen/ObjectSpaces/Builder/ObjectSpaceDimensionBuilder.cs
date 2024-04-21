﻿// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;

public class ObjectSpaceDimensionBuilder<T> : IObjectSpaceDimensionBuilder {
	protected readonly ObjectSpaceBuilder _parent;
	private readonly IList<ObjectSpaceDefinition.IndexDefinition> _indexes;
	private int? _averageItemSize;

	public ObjectSpaceDimensionBuilder(ObjectSpaceBuilder parent) {
		_parent = parent;
		_averageItemSize = null;
		_indexes = new List<ObjectSpaceDefinition.IndexDefinition>();

		// Add recyclable free index store by default
		WithRecyclableIndexes();
	}

	public Type ItemType => typeof(T);

	public IEnumerable<ObjectSpaceDefinition.IndexDefinition> Indexes => _indexes;

	public ObjectSpaceDimensionBuilder<T> UsingSerializer<TSerializer>() where TSerializer : IItemSerializer<T>, new() {
		_parent.UsingSerializer<T, TSerializer>();
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> UsingSerializer(IItemSerializer<T> serializer) {
		_parent.UsingSerializer(serializer);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> UsingComparer<TComparer>() where TComparer : IComparer<T>, new() {
		_parent.UsingComparer<T, TComparer>();
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> UsingComparer(IComparer<T> comparer) {
		_parent.UsingComparer(comparer);
		return this;
	}
	
	public ObjectSpaceDimensionBuilder<T> UsingEqualityComparer<TComparer>() where TComparer : IEqualityComparer<T>, new() {
		_parent.UsingEqualityComparer<T, TComparer>();
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> UsingEqualityComparer(IEqualityComparer<T> comparer) {
		_parent.UsingEqualityComparer(comparer);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithIdentifier<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null) {
		var member = memberExpression.ToMember();
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Identifier,
			Name = indexName ?? member.Name,
			Member = member,
			NullPolicy = IndexNullPolicy.ThrowOnNull
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithIndexOn<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
		var member = memberExpression.ToMember();
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Index,
			Name = indexName ?? member.Name,
			Member = member,
			NullPolicy = nullPolicy
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithUniqueIndexOn<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
		var member = memberExpression.ToMember();
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.UniqueKey,
			Name = indexName ?? member.Name,
			Member = member,
			NullPolicy = nullPolicy
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithRecyclableIndexes(string indexName = null) {
		_indexes.Add(
			new ObjectSpaceDefinition.IndexDefinition { 
				Type = ObjectSpaceDefinition.IndexType.RecyclableIndexStore,
				Name = indexName ?? HydrogenDefaults.DefaultReyclableIndexIndexName
			}
		);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> Merkleized(string indexName = null) {
		_indexes.Add(
			new ObjectSpaceDefinition.IndexDefinition { 
				Type = ObjectSpaceDefinition.IndexType.MerkleTree,
				Name = indexName ?? HydrogenDefaults.DefaultMerkleTreeIndexName
			}
		);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> OptimizeAssumingAverageItemSize(int bytes) {
		_averageItemSize = bytes;
		return this;
	}

	public ObjectSpaceBuilder Done() {
		return _parent;
	}

	public ObjectSpaceDefinition.DimensionDefinition BuildDefinition() {
		return new ObjectSpaceDefinition.DimensionDefinition {
			ObjectType = typeof(T),
			AverageObjectSizeBytes = _averageItemSize,
			Indexes = _indexes.ToArray()
		};
	}

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.WithRecyclableIndexes() 
		=> WithRecyclableIndexes();


	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.Merkleized()
		=> Merkleized();
	
	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.OptimizeAssumingAverageItemSize(int bytes) 
		=> OptimizeAssumingAverageItemSize(bytes);

}
