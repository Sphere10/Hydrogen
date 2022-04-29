using System.Collections;
using System.Collections.Generic;

namespace Sphere10.Hydrogen.Core.Consensus {

	/// <summary>
	/// A Hydrogen Operation references a rule (method), arguments (method arguments), entities (list of entities it
	/// </summary>
	public class Operation {

		public int Rule { get; set; }

	}


	public class Transaction {
		public byte[][] Arguments { get; set; }

		public byte[][] Entities { get; set; }

		public IEnumerable<byte[]> Signatures { get; set; }

	}

	public class Entity {
		public byte[] Object { get; }
	}
}
