using System;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Sphere10.Framework.CryptoEx; 

	public class KeyTypePrimeFieldAttribute : Attribute {
		public KeyTypePrimeFieldAttribute(string bigIntHexValue) {
			Value = new BigInteger(1, Hex.DecodeStrict(bigIntHexValue));
		}

		public BigInteger Value { get; }

	}
