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
using System.Xml.Linq;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;


public class ObjectSpaceDefinition {

	public bool Merkleized { get; set ; } = false;
		
	public CHF HashFunction { get; set; } = HydrogenDefaults.HashFunction;

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
				if (Merkleized && dimensionMerkleTrees < 1)
					result.AddError($"{dimensionErrorPrefix} requires a {nameof(IndexType.MerkleTree)} as the parent {nameof(ObjectSpace)} is merkleized");

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

		// All containers must have a unique type
		Dimensions
			.GroupBy(x => x.ObjectType)
			.Where(x => x.Count() > 1)
			.ForEach(x => result.AddError($"Container type '{x.Key}' is defined more than once."));

		// Each objectStream index correctly maps to an object property

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

		public Type ObjectType { get; set; }

		public IndexDefinition[] Indexes { get; set; } = Array.Empty<IndexDefinition>();

		public int? AverageObjectSizeBytes { get; set; } = null;

	}

	public class IndexDefinition {
		
		public IndexType Type { get; set; }

		public string Name { get; set; }

		public Member Member { get; set; }

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
