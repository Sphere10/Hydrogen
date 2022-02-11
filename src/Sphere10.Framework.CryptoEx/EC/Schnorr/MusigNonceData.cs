using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;

namespace Sphere10.Framework.CryptoEx.EC;

internal class MuSigNonceData
{
	internal byte[] PrivateNonce { get; set; }
	internal byte[] PublicNonce { get; set; }
}

internal class MuSigPrivateNonce
{
	internal byte[] K1 { get; set; }
	internal byte[] K2 { get; set; }

	internal byte[] GetFullNonce()
	{
		return Arrays.Concatenate(K1, K2);
	}
}

internal class MuSigPublicNonce
{
	internal byte[] R1 { get; set; }
	internal byte[] R2 { get; set; }

	internal byte[] GetFullNonce()
	{
		return Arrays.Concatenate(R1, R2);
	}
}

public class MuSigSessionNonce
{
	public byte[] AggregatedNonce { get; internal set; }
	public byte[] FinalNonce { get; internal set; }
	public BigInteger NonceCoefficient { get; internal set; }
	public bool FinalNonceParity { get; internal set; }
}
