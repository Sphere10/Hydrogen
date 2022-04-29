using Hydrogen;
using Hydrogen.Application;
using Hydrogen.DApp.Core.Keys;

namespace Hydrogen.DApp.Core.Wallets {
	public interface IWalletKey {
		public string Name { get; set; }
		public KeyType KeyType { get; }
		public SecureBytes PrivateKey { get; }
		public byte[] PublicKey { get; }
		public WalletKeyCapability Capability { get; }
	}
	
}
