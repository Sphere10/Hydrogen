using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC;

public class CustomEcDsaSigner : ECDsaSigner {
	private BigInteger HalfCurveOrder => Order.ShiftRight(1);
	private bool ForceLowR { get; }

	public CustomEcDsaSigner(bool forceLowR = true) : this(new RandomDsaKCalculator(), forceLowR) {
	}

	public CustomEcDsaSigner(IDsaKCalculator kCalculator, bool forceLowR = true) : base(kCalculator) {
		ForceLowR = forceLowR;
	}

	public override BigInteger[] GenerateSignature(byte[] message) {
		var ec = key.Parameters;
		var n = ec.N;
		var e = CalculateE(n, message);
		var d = (key as ECPrivateKeyParameters)?.D;

		if (kCalculator.IsDeterministic) {
			kCalculator.Init(n, d, message);
		} else {
			kCalculator.Init(n, random);
		}

		BigInteger r, s;

		var basePointMultiplier = CreateBasePointMultiplier();

		// 5.3.2
		do // Generate s
		{
			BigInteger k;
			do // Generate r
			{
				do {
					k = kCalculator.NextK();

					var p = basePointMultiplier.Multiply(ec.G, k).Normalize();

					// 5.3.3
					r = p.AffineXCoord.ToBigInteger().Mod(n);
				} while (r.SignValue == 0);
			} while (ForceLowR && r.ToByteArrayUnsigned()[0] >= 0x80);
			s = BigIntegers.ModOddInverse(n, k).Multiply(e.Add(d?.Multiply(r))).Mod(n);
		} while (s.SignValue == 0);

		return new[] { r, s };
	}

	public override bool VerifySignature(byte[] message, BigInteger r, BigInteger s) {
		return s.CompareTo(HalfCurveOrder) <= 0 && base.VerifySignature(message, r, s);
	}


	/// <summary>
	/// Deterministic Signer
	/// https://www.rfc-editor.org/rfc/rfc6979
	/// </summary>SHA-256
	/// <returns></returns>
	public static ISigner GetRfc6979DeterministicSigner() {
		return new DsaDigestSigner(new CustomEcDsaSigner(new HMacDsaKCalculator(new Sha256Digest())),
			new NullDigest(),
			new CustomDsaEncoding());
	}

}
