using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Hydrogen.CryptoEx {

	public class KeyTypeOrderAttribute : Attribute {
		public KeyTypeOrderAttribute(string bigIntHexValue) {
			Value = new BigInteger(1, Hex.DecodeStrict(bigIntHexValue));
		}

		public BigInteger Value { get; }

	}

}
