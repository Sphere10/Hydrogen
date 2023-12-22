﻿using System;
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