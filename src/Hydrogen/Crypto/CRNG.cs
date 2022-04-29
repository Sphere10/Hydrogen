using System.Security.Cryptography;

namespace Hydrogen {

	public class CRNG {
		private readonly RNGCryptoServiceProvider _rng;

		public CRNG() {
			_rng = new RNGCryptoServiceProvider();
		}

		public byte[] NextBytes(int count) {
			Guard.ArgumentInRange(count, 0, int.MaxValue, nameof(count));
			var bytes = new byte[count];
			_rng.GetBytes(bytes);
			return bytes;
		}


	}

}