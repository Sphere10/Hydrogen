using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IMerkleizedCollection<TItem> : ICollection<TItem> {
		IMerkleTree MerkleTree { get; }
	}

}
