using System;

namespace Sphere10.Framework {

	public abstract class OTSConfig : ICloneable {
		public readonly AMSOTS AMSID;
		public readonly CHF HashFunction;
		public readonly bool UsePublicKeyHashOptimization;
		public readonly int DigestSize;
		public readonly OTSKeySize KeySize;
		public readonly OTSKeySize PublicKeySize;
		public readonly OTSKeySize SignatureSize;

		protected OTSConfig(AMSOTS id, CHF chf, int digestSize, bool publicKeyIsHashed, OTSKeySize keySize, OTSKeySize publicKeySize, OTSKeySize signatureSize) {
			AMSID = id;
			HashFunction = chf;
			DigestSize = digestSize;
			UsePublicKeyHashOptimization = publicKeyIsHashed;
			KeySize = keySize;
			PublicKeySize = publicKeySize;
			SignatureSize = signatureSize;
		}

		public abstract object Clone();
	}

}
