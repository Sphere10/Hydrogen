// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen;

/// <summary>
/// Password hashing based on PBKDF2-SHA1,
/// </summary>
public class PasswordHasher {
	// The following constants may be changed without breaking existing hashes.
	public const int SALT_BYTE_SIZE = 24;
	public const int HASH_BYTE_SIZE = 24;
	public const int PBKDF2_ITERATIONS = 1000;

	public const int ITERATION_INDEX = 0;
	public const int SALT_INDEX = 1;
	public const int PBKDF2_INDEX = 2;

	private PasswordHasher() {
	}

	/// <summary>
	/// Creates a salted PBKDF2 hash of the username/password pair.
	/// </summary>
	/// <param name="username">The username to hash</param>
	/// <param name="password">The password to hash</param>
	/// <returns>The hash of the password</returns>
	public static string CreateHash(string username, string password)
		=> CreateHash($"{username}:{password}");

	/// <summary>
	/// Creates a salted PBKDF2 hash of the secret.
	/// </summary>
	/// <param name="secret">The password to hash.</param>
	/// <returns>The hash of the password.</returns>
	public static string CreateHash(string secret) {
		Guard.ArgumentNotNull(secret, nameof(secret));
		var salt = Tools.Crypto.GenerateCryptographicallyRandomBytes(SALT_BYTE_SIZE);
		var hash = PBKDF2.DeriveKey(secret, salt, PBKDF2_ITERATIONS, HASH_BYTE_SIZE);
		return PBKDF2_ITERATIONS + ":" + Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
	}


	/// <summary>
	/// Validates a secret given a hash of the correct one.
	/// </summary>
	/// <param name="username">The username to check</param>
	/// <param name="password">The password to check</param>
	/// <param name="correctHash">A hash of the correct secret</param>
	/// <returns>True if the secret is correct. False otherwise.</returns>
	public static bool ValidatePassword(string username, string password, string correctHash)
		=> ValidatePassword($"{username}:{password}", correctHash);


	/// <summary>
	/// Validates a secret given a hash of the correct one.
	/// </summary>
	/// <param name="secret">The secret to check.</param>
	/// <param name="correctHash">A hash of the correct secret.</param>
	/// <returns>True if the secret is correct. False otherwise.</returns>
	public static bool ValidatePassword(string secret, string correctHash) {
		// Extract the parameters from the hash
		char[] delimiter = { ':' };
		var split = correctHash.Split(delimiter);
		var iterations = Int32.Parse(split[ITERATION_INDEX]);
		var salt = Convert.FromBase64String(split[SALT_INDEX]);
		var hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
		var testHash = PBKDF2.DeriveKey(secret, salt, iterations, hash.Length);
		return SlowEquals(hash, testHash);
	}

	/// <summary>
	/// Compares two byte arrays in length-constant time. This comparison
	/// method is used so that password hashes cannot be extracted from
	/// on-line systems using a timing attack and then attacked off-line.
	/// </summary>
	/// <param name="a">The first byte array.</param>
	/// <param name="b">The second byte array.</param>
	/// <returns>True if both byte arrays are equal. False otherwise.</returns>
	private static bool SlowEquals(byte[] a, byte[] b) {
		uint diff = (uint)a.Length ^ (uint)b.Length;
		for (int i = 0; i < a.Length && i < b.Length; i++)
			diff |= (uint)(a[i] ^ b[i]);
		return diff == 0;
	}


}
