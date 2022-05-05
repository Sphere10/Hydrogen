using System.Security.Cryptography;

namespace Hydrogen {

	public static class PBKDF2 {
		public static byte[] DeriveKey(string secret, byte[] salt, int iterations, int keyLength) {
			var pbkdf2 = new Rfc2898DeriveBytes(secret, salt) {
				IterationCount = iterations
			};
			return pbkdf2.GetBytes(keyLength);
		}

	}

}