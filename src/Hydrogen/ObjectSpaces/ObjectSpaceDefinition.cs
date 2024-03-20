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
		for(var ix = 0; ix < Dimensions.Length; ix++) {
			var dimension = Dimensions[ix];

			var dimensionErrorPrefix = $"Dimension {ix} ({dimension.ObjectType.ToStringCS()})";

			if (dimension.Indexes is { Length: > 0 }) {
				// Ensure has 1 recyclable free-index store
				var freeIndexStores = dimension.Indexes.Count(x => x.Type == IndexType.FreeIndexStore);
				if (freeIndexStores == 0)
					result.AddError($"{dimensionErrorPrefix} requires a {nameof(IndexType.FreeIndexStore)}");

				if (freeIndexStores > 1)
					result.AddError($"{dimensionErrorPrefix} has more than one {nameof(IndexType.FreeIndexStore)} defined");

				var dimensionMerkleTrees = dimension.Indexes.Count(x => x.Type == IndexType.MerkleTree);

				// Ensure has not more than 1 merkle-tree
				if (dimensionMerkleTrees > 1)
					result.AddError($"{dimensionErrorPrefix} has more than one {nameof(IndexType.MerkleTree)} defined");

				// Ensure has merkle-tree if object space is merkelized
				if (Merkleized && dimensionMerkleTrees < 1)
					result.AddError($"{dimensionErrorPrefix} requires a {nameof(IndexType.MerkleTree)} as the parent {nameof(ObjectSpace)} is merkleized");

				// Ensure indexes which require a property member that one is specified
				var indexAtLocation = dimension.Indexes.Select( (index, loc) => (Index: index, Loc: loc));
				foreach(var indexLoc in indexAtLocation.Where(x => RequiresMember(x.Index.Type) && x.Index.KeyMember is null))
					result.AddError($"{dimensionErrorPrefix} contains an {nameof(indexLoc.Index.Type)} (at {indexLoc.Loc}) which requires a member");

				// Ensure not over-indexing properties
				foreach(var duplicateProperty in indexAtLocation.Where(x => x.Index.KeyMember is not null).GroupBy(x => x.Index.KeyMember.MemberInfo).Where(x => x.Count() > 1))
					result.AddError($"{dimensionErrorPrefix} contains {duplicateProperty.Count()} index's on property {duplicateProperty.Key.Name}");

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
				IndexType.FreeIndexStore => false,
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

		public Member KeyMember { get; set; }

	}

	public enum IndexType {
		Identifier,
		UniqueKey,
		Index,
		FreeIndexStore,
		MerkleTree
	}

}
