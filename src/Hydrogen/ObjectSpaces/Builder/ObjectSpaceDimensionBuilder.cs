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

	public IEnumerable<ObjectSpaceDefinition.IndexDefinition> Indexes => _indexes;

	public ObjectSpaceDimensionBuilder<T> UsingSerializer<TSerializer>() where TSerializer : IItemSerializer<T>, new() {
		_parent.UsingSerializer<T, TSerializer>();
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

	public ObjectSpaceDimensionBuilder<T> WithIdentifier<TMember>(Expression<Func<T, TMember>> memberExpression) {
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Identifier,
			KeyMember = memberExpression.ToMember()
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithIndexOn<TMember>(Expression<Func<T, TMember>> memberExpression) {
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.Index,
			KeyMember = memberExpression.ToMember()
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithUniqueKeyOn<TMember>(Expression<Func<T, TMember>> memberExpression) {
		var index = new ObjectSpaceDefinition.IndexDefinition {
			Type = ObjectSpaceDefinition.IndexType.UniqueKey,
			KeyMember = memberExpression.ToMember()
		};
		_indexes.Add(index);
		return this;
	}

	public ObjectSpaceDimensionBuilder<T> WithRecyclableIndexes() {
		_indexes.Add(new ObjectSpaceDefinition.IndexDefinition { Type = ObjectSpaceDefinition.IndexType.RecyclableIndexStore });
		return this;

	}

	public ObjectSpaceDimensionBuilder<T> Merkleized() {
		_indexes.Add(new ObjectSpaceDefinition.IndexDefinition { Type = ObjectSpaceDefinition.IndexType.MerkleTree });
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
