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
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;

/// <summary>
/// Defines the structure and policies for an <see cref="ObjectSpace"/>, including dimensions, indexes, and optional merkle/autosave traits.
/// </summary>
public class ObjectSpaceDefinition {


	public ObjectSpaceDefinition() : this(false, false) {
	}

	public ObjectSpaceDefinition(bool merkleized, bool autosave) {
		Traits = ObjectSpaceTraits.None;
		if (merkleized)
			Traits |= ObjectSpaceTraits.Merkleized;
		if (autosave)
			Traits |= ObjectSpaceTraits.AutoSave;
	}

	public ObjectSpaceTraits Traits { get; set ; } = ObjectSpaceTraits.None;
		
	/// <summary>
	/// Hash function used for merkle-enabled object spaces.
	/// </summary>
	public CHF HashFunction { get; set; } = HydrogenDefaults.HashFunction;

	/// <summary>
	/// Dimensions (tables) that compose the object space.
	/// </summary>
	public DimensionDefinition[] Dimensions { get; set; } = Array.Empty<DimensionDefinition>();

	public Result Validate() {
		var result = Result.Success;

		// Pre-condition checks
		if (Dimensions == null)
			return Result.Error($"Unexpected null property: {nameof(Dimensions)}");
		

		// At least 1 dimension
		if (Dimensions.Length == 0) {
			result.AddError("At least 1 Dimension must be defined.");
		}

		// All containers must have a unique type
		Dimensions
			.GroupBy(x => x.ObjectType)
			.Where(x => x.Count() > 1)
			.ForEach(x => result.AddError($"Container type '{x.Key}' is defined more than once."));

		// Verify all dimensions
		for(var i = 0; i < Dimensions.Length; i++) {
			var dimension = Dimensions[i];

			var dimensionErrorPrefix = $"Dimension {i} ({dimension.ObjectType.ToStringCS()})";

			if (dimension.Indexes is { Length: > 0 }) {
				// Ensure has 1 recyclable free-index store
				var recyclableIndexStores = dimension.Indexes.Count(x => x.Type == IndexType.RecyclableIndexStore);
				if (recyclableIndexStores == 0)
					result.AddError($"{dimensionErrorPrefix} requires a {nameof(IndexType.RecyclableIndexStore)}");

				if (recyclableIndexStores > 1)
					result.AddError($"{dimensionErrorPrefix} has more than one {nameof(IndexType.RecyclableIndexStore)} defined");

				var dimensionMerkleTrees = dimension.Indexes.Count(x => x.Type == IndexType.MerkleTree);

				// Ensure has not more than 1 merkle-tree
				if (dimensionMerkleTrees > 1)
					result.AddError($"{dimensionErrorPrefix} has more than one {nameof(IndexType.MerkleTree)} defined");

				// Ensure has merkle-tree if object space is merkelized
				if (Traits.HasFlag(ObjectSpaceTraits.Merkleized) && dimensionMerkleTrees < 1)
					result.AddError($"{dimensionErrorPrefix} requires a {nameof(IndexType.MerkleTree)} as the parent object space is merkleized");

				// Verify all indexes
				var alreadyProcessedMembers = new HashSet<Member>();
				for (var j = 0; j < dimension.Indexes.Length; j++) {
					var index = dimension.Indexes[j];

					// Ensure all indexes have a unique name within the container
					if (string.IsNullOrWhiteSpace(index.Name)) 
						result.AddError($"{dimensionErrorPrefix} contains an {nameof(index.Type)} (at {j}) which does not specify a unique name");
					else if (dimension.Indexes.Count(x => x.Name == index.Name) > 1)
						result.AddError($"{dimensionErrorPrefix} contains an {nameof(index.Type)} (at {j}) which uses a null or duplicated name '{index.Name}'");

					// Ensure indexes which require a property member that one is specified
					if (RequiresMember(index.Type) && index.Member is null)
						result.AddError($"{dimensionErrorPrefix} contains an {nameof(index.Type)} (at {j}) which requires a member");

					// Ensure not over-indexing members
					if (index.Member is not null && !alreadyProcessedMembers.Contains(index.Member) && dimension.Indexes.Count(x => x.Member == index.Member) > 1) {
						result.AddError($"{dimensionErrorPrefix} contains multiple index's on member {index.Member.Name}");
						alreadyProcessedMembers.Add(index.Member);
					}
				}

			} else {
				result.AddError("Dimension requires index definition");
			}
		}

		// Verify PI
		if (Traits.HasFlag(ObjectSpaceTraits.AutoSave)) {
			Dimensions
				.Where(x => !x.ChangeTracker.SupportsChangeTracking)
				.ForEach(x => result.AddError($"Dimension {x.ObjectType.ToStringCS()} does not support change tracking"));
		}

		return result;

		bool RequiresMember(IndexType type) 
			=> type switch {
				IndexType.Identifier => true,
				IndexType.UniqueKey => true,
				IndexType.Index => true,
				IndexType.RecyclableIndexStore => false,
				IndexType.MerkleTree => false,
				_ => throw new NotSupportedException(type.ToString())
			};

	}

	public class DimensionDefinition {

		internal ObjectChangeTracker ChangeTracker { get; set; } = ObjectChangeTracker.Default;

		/// <summary>
		/// CLR type of the objects stored in the dimension.
		/// </summary>
		public Type ObjectType { get; set; }

		/// <summary>
		/// Index definitions for this dimension.
		/// </summary>
		public IndexDefinition[] Indexes { get; set; } = Array.Empty<IndexDefinition>();

		/// <summary>
		/// Optional average serialized size hint used for allocation strategies.
		/// </summary>
		public int? AverageObjectSizeBytes { get; set; } = null;

	}

	public class IndexDefinition {
		
		/// <summary>
		/// Index storage type.
		/// </summary>
		public IndexType Type { get; set; }

		/// <summary>
		/// Unique name for the index within the dimension.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Member to index where required.
		/// </summary>
		public Member Member { get; set; }

		/// <summary>
		/// Defines how nulls are handled when indexing.
		/// </summary>
		public IndexNullPolicy NullPolicy { get; set; }

	}

	public enum IndexType {
		Identifier,
		UniqueKey,
		Index,
		RecyclableIndexStore,
		MerkleTree
	}

}
