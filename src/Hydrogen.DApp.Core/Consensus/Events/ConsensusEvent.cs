// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Consensus;

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
