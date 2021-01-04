
namespace VelocityNET.ProtocolObjects {

	public abstract class PrivateKey : CryptographicKey {
		public abstract PublicKey CalculatePublicKey(ulong signerNonce);
		public abstract byte[] Sign(byte[] digest, ulong signerNonce);
	}

}