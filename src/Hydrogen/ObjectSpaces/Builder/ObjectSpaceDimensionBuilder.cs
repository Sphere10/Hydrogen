// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
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
	private ObjectChangeTracker _changeTracker;

	public ObjectSpaceDimensionBuilder(ObjectSpaceBuilder parent) {
		_parent = parent;
		_averageItemSize = null;
		_indexes = new List<ObjectSpaceDefinition.IndexDefinition>();
		_changeTracker = ObjectChangeTracker.Default;

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

	public ObjectSpaceDimensionBuilder<T> MakeSerializer() {
		_parent.MakeSerializer<T>();
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

	public ObjectSpaceDimensionBuilder<T> UsingComparer(object comparer) 
		=> (ObjectSpaceDimensionBuilder<T>)((IObjectSpaceDimensionBuilder)this).UsingComparer(comparer);

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.UsingComparer(object comparer) {
		_parent.UsingComparer(typeof(T), comparer);
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

	public ObjectSpaceDimensionBuilder<T> UsingEqualityComparer(object comparer) 
		=> (ObjectSpaceDimensionBuilder<T>)((IObjectSpaceDimensionBuilder)this).UsingEqualityComparer(comparer);

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.UsingEqualityComparer(object comparer) {
		_parent.UsingEqualityComparer(typeof(T), comparer);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithIdentifier<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null) 
		=> WithIdentifier(memberExpression.ToMember(), indexName);

	public new ObjectSpaceDimensionBuilder<T> WithIdentifier(Member member, string indexName = null) 
		=> (ObjectSpaceDimensionBuilder<T>)((IObjectSpaceDimensionBuilder)this).WithIdentifier(member, indexName);

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.WithIdentifier(Member member, string indexName) {
		Guard.ArgumentNotNull(member, nameof(member));
		Guard.Argument(member.DeclaringType == typeof(T), nameof(member), $"Not a member of {typeof(T).ToStringCS()}");
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Identifier,
			Name = indexName ?? member.Name,
			Member = member,
			NullPolicy = IndexNullPolicy.ThrowOnNull
		};
		_indexes.Add(index);
		return this;
	}
	
	public ObjectSpaceDimensionBuilder<T> WithIndexOn<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) 
		=> WithIndexOn(memberExpression.ToMember(), indexName, nullPolicy);

	public ObjectSpaceDimensionBuilder<T> WithIndexOn(Member member, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) 
		=> (ObjectSpaceDimensionBuilder<T>)((IObjectSpaceDimensionBuilder)this).WithIndexOn(member, indexName, nullPolicy);

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.WithIndexOn(Member member, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
		Guard.ArgumentNotNull(member, nameof(member));
		Guard.Argument(member.DeclaringType == typeof(T), nameof(member), $"Not a member of {typeof(T).ToStringCS()}");
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Index,
			Name = indexName ?? member.Name,
			Member = member,
			NullPolicy = nullPolicy
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithUniqueIndexOn<TMember>(Expression<Func<T, TMember>> memberExpression, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull)
		=> WithUniqueIndexOn(memberExpression.ToMember(), indexName, nullPolicy);

	public ObjectSpaceDimensionBuilder<T> WithUniqueIndexOn(Member member, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) 
		=> (ObjectSpaceDimensionBuilder<T>)((IObjectSpaceDimensionBuilder)this).WithUniqueIndexOn(member, indexName, nullPolicy);


	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.WithUniqueIndexOn(Member member, string indexName = null, IndexNullPolicy nullPolicy = IndexNullPolicy.IgnoreNull) {
		Guard.ArgumentNotNull(member, nameof(member));
		Guard.Argument(member.DeclaringType == typeof(T), nameof(member), $"Not a member of {typeof(T).ToStringCS()}");
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

	public ObjectSpaceDimensionBuilder<T> WithChangeTrackingVia<TMember>(Expression<Func<T, TMember>> memberExpression) 
		=> (ObjectSpaceDimensionBuilder<T>)WithChangeTrackingVia(memberExpression.ToMember());

	public IObjectSpaceDimensionBuilder WithChangeTrackingVia(Member member) {
		_changeTracker = new ObjectChangeTracker(member);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> OptimizeAssumingAverageItemSize(int bytes) {
		_averageItemSize = bytes;
		return this;
	}

	public ObjectSpaceBuilder Done() {
		if (!_parent.ContainsSerializer(typeof(T))) 
			MakeSerializer();
		return _parent;
	}

	public ObjectSpaceDefinition.DimensionDefinition BuildDefinition() {
		return new ObjectSpaceDefinition.DimensionDefinition {
			ObjectType = typeof(T),
			AverageObjectSizeBytes = _averageItemSize,
			Indexes = _indexes.ToArray(),
			ChangeTracker = _changeTracker
		};
	}

	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.WithRecyclableIndexes() 
		=> WithRecyclableIndexes();


	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.Merkleized()
		=> Merkleized();
	
	IObjectSpaceDimensionBuilder IObjectSpaceDimensionBuilder.OptimizeAssumingAverageItemSize(int bytes) 
		=> OptimizeAssumingAverageItemSize(bytes);

}
