using Sphere10.Framework;
using VelocityNet.Core.Blocks;

namespace VelocityNET.ProtocolObjects {
	public class AccountSegment : ReadWriteSafeObject {
		public BlockHeader Header { get; set; }
		public MerkleList<Account> Accounts { get; set; }
	}
}