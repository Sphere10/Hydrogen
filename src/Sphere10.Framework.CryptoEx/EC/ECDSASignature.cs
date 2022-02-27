using System;
using System.ComponentModel;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Math;

namespace Sphere10.Framework.CryptoEx.EC;

internal class ECDSASignature {
	private const string InvalidDerSignature = "Invalid DER Signature";
	private BigInteger R { get; }
	private BigInteger S { get; }
	private ECDSAKeyType KeyType { get; }
	private BigInteger CurveOrder => KeyType.GetAttribute<KeyTypeOrderAttribute>().Value;
	private BigInteger HalfCurveOrder => CurveOrder.ShiftRight(1);

	private byte[] ToDer() {
		var outStream = new MemoryStream();
		var generator = new DerSequenceGenerator(outStream);
		generator.AddObject(new DerInteger(R));
		generator.AddObject(new DerInteger(S));
		generator.Close();
		return outStream.ToArray();
	}

	private bool IsLowR => R.ToByteArrayUnsigned()[0] < 0x80;

	private bool IsLowS => S.CompareTo(HalfCurveOrder) <= 0;

	/// <summary>
	/// Enforce LowS on the signature
	/// </summary>
	private ECDSASignature MakeCanonical() {
		return IsLowS ? this : new ECDSASignature(KeyType, R, S.Negate().Mod(CurveOrder));
		//return IsLowS ? this : new ECDSASignature(KeyType, R, CurveOrder.Subtract(S));
	}

	private ECDSASignature(ECDSAKeyType keyType, BigInteger r, BigInteger s) {
		R = r ?? throw new ArgumentNullException(nameof(r));
		S = s ?? throw new ArgumentNullException(nameof(s));
		KeyType = keyType;
	}

	internal ECDSASignature(ECDSAKeyType keyType, BigInteger[] rs) {
		if (rs == null) {
			throw new ArgumentNullException(nameof(rs));
		}
		KeyType = keyType;
		R = rs[0];
		S = rs[1];
	}

	internal ECDSASignature(ECDSAKeyType keyType, byte[] derSig) {

		if (!Enum.IsDefined(typeof(ECDSAKeyType), keyType)) {
			throw new InvalidEnumArgumentException(nameof(keyType), (int)keyType, typeof(ECDSAKeyType));
		}
		if (derSig == null) {
			throw new ArgumentNullException(nameof(derSig));
		}
		KeyType = keyType;
		try {
			var decoder = new Asn1InputStream(derSig);
			var derSequence = decoder.ReadObject() as DerSequence;
			if (derSequence is not { Count: 2 }) {
				throw new FormatException(InvalidDerSignature);
			}
			R = (derSequence[0] as DerInteger)?.Value;
			S = (derSequence[1] as DerInteger)?.Value;
		} catch (Exception ex) {
			throw new FormatException(InvalidDerSignature, ex);
		}
	}

	internal byte[] ToBytes(bool forceLowS = true) {
		return forceLowS ? MakeCanonical().ToDer() : ToDer();
	}

}
