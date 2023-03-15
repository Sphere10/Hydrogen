using System.Security.Cryptography;
using Hydrogen.Maths;

namespace Hydrogen;

/// <summary>
/// A non-deterministic cryptographically secure random number generator that uses system libraries under the hood.
/// </summary>
public class SystemCRNG : IRandomNumberGenerator {

	private readonly RNGCryptoServiceProvider _rng;

	public SystemCRNG() {
		_rng = new RNGCryptoServiceProvider();
	}

	public byte[] NextBytes(int count) {
		Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));
		var bytes = new byte[count];
		_rng.GetBytes(bytes);
		return bytes;
	}

}

