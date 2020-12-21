using System;
using System.Collections.Generic;
using System.Text;

namespace Sphere10.Framework.Consensus {
	
	public abstract class ConsensusEventHeader {

		public UInt32 Version { get; }
		
		public abstract ConsensusEventType Type { get; }

		public byte[] EventHistoryRoot { get; }

		public byte[] StateHistoryRoot { get; }

		public byte[] LeaderSignature { get; }

	}

	public class GenesisEventHeader : ConsensusEventHeader {
		
		public override ConsensusEventType Type => ConsensusEventType.Genesis;

		public byte[] StartPolicy { get; }

	}

	public class LeaderSelectionEventHeader : ConsensusEventHeader {
		public override ConsensusEventType Type => ConsensusEventType.LeaderSelection;

		public ulong PreviousLeaderAccount { get; }
		
		public byte[] LeaderKey { get; }
		
	}

	public class DataEventHeader : ConsensusEventHeader {
		
		public override ConsensusEventType Type => ConsensusEventType.Data;
		
		public ulong AccruedFees { get; }

		public byte[] TransactionRoot { get; }
	}

	public enum ConsensusEventType : byte {
		Genesis,
		LeaderSelection,
		Data,
	}
}
