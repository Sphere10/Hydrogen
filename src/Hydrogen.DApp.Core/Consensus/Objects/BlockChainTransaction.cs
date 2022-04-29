using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Hydrogen;
using Hydrogen.DApp.Core.Consensus;
using Hydrogen.DApp.Core.Maths;

namespace Hydrogen.DApp.Core.Consensus {

	[Serializable]
	public class BlockChainTransaction {
		public UInt64	Sequence { get; set; }
		public long		TimeStamp { get; set; }
		public string	From { get; set; }
		public string	To { get; set; }
		public UInt64	Amount { get; set; }
		public UInt64	Fees { get; set; }
		public byte[]	Data { get; set; }
	}


}