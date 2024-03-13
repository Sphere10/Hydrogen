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

			if (dimension.Indexes is { Length: > 0 }) {
				// Ensure has 1 free-index store
				var freeIndexStores = dimension.Indexes.Count(x => x.Type == IndexType.FreeIndexStore);
				if (freeIndexStores != 1)
					result.AddError($"Dimension {ix} ({dimension.ObjectType.ToStringCS()}) had {freeIndexStores} FreeIndexStore's defined, required {1}.");

				// Ensure has 0 or 1 merkle-trees
				var dimensionMerkleTrees = dimension.Indexes.Count(x => x.Type == IndexType.MerkleTree);
				if (Merkleized && dimensionMerkleTrees != 1)
					result.AddError($"Dimension is required to have 1 merkle-tree index but there are {dimensionMerkleTrees}");

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
	}

	public class DimensionDefinition {

		public Type ObjectType { get; set; }

		public IndexDefinition[] Indexes { get; set; } = Array.Empty<IndexDefinition>();

		public int AverageObjectSizeBytes { get; set; } = 0;

	}

	public class IndexDefinition {
		
		//public int ReservedStreamIndex { get; set; }

		public IndexType Type { get; set; }

		public Member KeyMember { get; set; }

		public int MaxLength { get; set; }

	}

	public enum IndexType {
		Identifier,
		UniqueKey,
		Index,
		FreeIndexStore,
		MerkleTree
	}

}
