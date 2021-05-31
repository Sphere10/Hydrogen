using Sphere10.Framework;
using Sphere10.Framework.Application;
using Sphere10.Hydrogen.Core.Keys;

namespace Sphere10.Hydrogen.Core.Wallets {
	public interface IWalletKey {
		public string Name { get; set; }
		public KeyType KeyType { get; }
		public SecureBytes PrivateKey { get; }
		public byte[] PublicKey { get; }
		public WalletKeyCapability Capability { get; }
	}
	
}
