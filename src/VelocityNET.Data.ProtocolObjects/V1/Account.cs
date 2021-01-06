using System;
using Sphere10.Framework;

namespace VelocityNET.ProtocolObjects {

	public class Account : Identity {
		public UInt16 AccountType { get; set; }
		public ulong Balance { get; set; }
		public ulong Vote { get; set; }
		public byte[] Key { get; set; }
		public UInt32 LastUpdatedBlock { get; set; }
		public AccountStatus Status { get; set; }
		public ulong TimeLock { get; set; }
		public byte[] HashLock { get; set; }
		public byte[] InternalData { get; set; }
		public ulong ReferenceAccount { get; set; }
		public ulong ReferenceQuantity { get; set; }
		public ulong ReferenceBlock { get; set; }
		public byte[] Seal { get; set; }
	}


	public class DataSlot {
		public ulong ID { get; set; }
		public byte[] Data { get; set; }
	}


	public class 

	public class Group : Identity {
		IMerkleList<IFuture<Identity>> Members { get; set; }
	}

	public class ConsensusSpace {

		IMerkleList<IFuture<Identity>> Identities { get; }

		IMerkleList<IFuture<Identity>> Accounts { get; }

		IMerkleList<IFuture<Identity>> Groups { get; }
	}
}