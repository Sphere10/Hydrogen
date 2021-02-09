using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Hydrogen.Core.Consensus {

	public class NewMinerBlock : BlockBase {

		public string MinerTag { get; set; }
	
		public uint Nonce { get; set; }

	}

}
