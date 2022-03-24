using System.Collections.Generic;

namespace Sphere10.Framework {

	public interface IMerkleCollection<TItem> : ICollection<TItem> {
		IMerkleTree MerkleTree { get; }
	}

}
