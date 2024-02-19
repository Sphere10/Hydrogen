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

	public IndexDefinition[] SchemaIndexes { get; set; }

	public ContainerDefinition[] Containers { get; set; }


	public Result Validate() {
		var result = Result.Success;

		// Verify all containers
		if (Containers != null) {
			// At least 1 objectStream
			if (Containers.Length == 0) {
				result.AddError("At least 1 objectStream must be defined.");
			}

			// Verify objectStream
			foreach (var (container, ix) in Containers.WithIndex()) {
				if (container.Indexes != null && container.Indexes.Length > 0) {
					// Ensure has 1 free-index store
					var freeIndexStores = container.Indexes.Count(x => x.Type == IndexType.FreeIndexStore);
					if (freeIndexStores != 1)
						result.AddError($"Container {ix} ({container.ObjectType.ToStringCS()}) had {freeIndexStores} FreeIndexStore's defined, required {1}.");

					// TODO: Ensure has 0 or 1 merkle-trees

				} else {
					result.AddError("Container requires index definition");
				}
			}


			// All containers must have a unique type
			Containers
				.GroupBy(x => x.ObjectType)
				.Where(x => x.Count() > 1)
				.ForEach(x => result.AddError($"Container type '{x.Key}' is defined more than once."));

			// Each objectStream index correctly maps to an object property

		} else {
			result.AddError("No containers defined.");
		}

		return result;
	}


	public class ContainerDefinition {

		public Type ObjectType { get; set; }

		public IndexDefinition[] Indexes { get; set; }

		public int AverageObjectSizeBytes { get; set; }

	}

	public class IndexDefinition {
		public int ReservedStreamIndex { get; set; }

		public IndexType Type { get; set; }

		public Member KeyMember { get; set; }

		public int MaxLength { get; set; }

	}

	public class UniqueKey : IndexDefinition {

	}

	public enum IndexType {
		Identifier,
		UniqueKey,
		Index,
		FreeIndexStore,
		MerkleTree
	}

}
