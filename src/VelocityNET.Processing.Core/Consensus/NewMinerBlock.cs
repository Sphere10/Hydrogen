using System;
using System.Collections.Generic;
using System.Text;

namespace VelocityNET.Core.Consensus {

	public class NewMinerBlock : BlockBase {

		public string MinerTag { get; set; }
	
		public uint Nonce { get; set; }

	}

}
