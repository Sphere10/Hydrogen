using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Sphere10.Framework.CryptoEx.EC;

public class Math {
	internal static bool IsEven(ECPoint publicKey) {
		return IsEven(publicKey.AffineYCoord.ToBigInteger());
	}
	internal static bool IsEven(BigInteger publicKey) {
		//return BigInteger.Jacobi(publicKey, P) == 1);
		return publicKey.Mod(BigInteger.Two).Equals(BigInteger.Zero);
	}
}
