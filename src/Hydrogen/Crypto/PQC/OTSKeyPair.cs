namespace Sphere10.Framework {

	public class OTSKeyPair {
		public OTSKeyPair(byte[,] privateKey, byte[,] publicKey, IFuture<byte[]> publicKeyHash) {
			PrivateKey = privateKey;
			PublicKey = publicKey;
			PublicKeyHash = publicKeyHash;
		}
		public readonly byte[,] PrivateKey;
		public readonly byte[,] PublicKey;
		public readonly IFuture<byte[]> PublicKeyHash;
	}

}
