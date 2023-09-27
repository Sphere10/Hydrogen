using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Hydrogen;

public class ObjectSpaceDefinition {

	public IndexDefinition[] SchemaIndexes { get; set; }

	public ContainerDefinition[] Containers { get; set; }

	public class ContainerDefinition {

		public Type ObjectType { get; set; }

		public IndexDefinition[] Indexes { get; set; }

	}

	public class IndexDefinition {
		public int ReservedStreamIndex { get; set; }

		public IndexType Type { get; set; }

		public PropertyInfo Property { get; set; }

		public Func<object, object> KeyProjection { get; set; }

	}

	public enum IndexType {
		KeyStore,
		UniqueKey,
		Index,
		FreeIndexStore,
		MerkleTree
	}

}
