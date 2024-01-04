// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using Hydrogen.Mapping;

namespace Hydrogen.ObjectSpaces;


public class ObjectSpaceDefinition {

	public IndexDefinition[] SchemaIndexes { get; set; }

	public ContainerDefinition[] Containers { get; set; }

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
		UniqueKey,
		Index,
		FreeIndexStore,
		MerkleTree
	}

}
