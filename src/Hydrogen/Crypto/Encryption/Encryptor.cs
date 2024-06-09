// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

// ReSharper disable CheckNamespace
namespace Hydrogen;

// TODO: Requires major refactoring and improvements
// - unify string/byte[]/stream encryption
// - unify IV/salt prepending
// - not backwards compatible
public static class Encryptor {

	public static string EncryptStringAES(string plainText, string sharedSecret, string salt) {
		return EncryptStringAES(plainText, sharedSecret, Encoding.UTF8.GetBytes(salt));
	}

	/// <summary>
	/// Encrypt the given string using AES.  The string can be decrypted using 
	/// DecryptStringAES().  The sharedSecret parameters must match.
	/// </summary>
	/// <param name="plainText">The text to encrypt.</param>
	/// <param name="sharedSecret">A password used to generate a key for encryption.</param>
	/// <param name="salt"></param>
	public static string EncryptStringAES(string plainText, string sharedSecret, byte[] salt) {
		if (String.IsNullOrEmpty(plainText))
			throw new ArgumentNullException(nameof(plainText));
		if (String.IsNullOrEmpty(sharedSecret))
			throw new ArgumentNullException(nameof(sharedSecret));

		string outStr = null; // Encrypted string to return
		AesManaged aesAlg = null; // RijndaelManaged object used to encrypt the data.

		try {
			// generate the key from the shared secret and the salt
			Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt);

			// Create a RijndaelManaged object
			aesAlg = new AesManaged();
			aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

			// Create a decryptor to perform the stream transform.
			var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using (var memStream = new MemoryStream()) {
				// prepend the IV (length + bytes)
				memStream.Write(EndianBitConverter.Little.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
				memStream.Write(aesAlg.IV, 0, aesAlg.IV.Length);

				using (CryptoStream encryptedStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write)) {
					using (StreamWriter encryptedWriter = new StreamWriter(encryptedStream)) {
						//Write all data to the stream.
						encryptedWriter.Write(plainText);
					}
				}
				outStr = Convert.ToBase64String(memStream.ToArray());
			}
		} finally {
			// Clear the RijndaelManaged object.
			aesAlg?.Clear();
		}

		// Return the encrypted bytes from the memory stream.
		return outStr;
	}

	public static string DecryptStringAES(string cipherText, string sharedSecret, string salt) {
		return DecryptStringAES(cipherText, sharedSecret, Encoding.UTF8.GetBytes(salt));
	}

	/// <summary>
	/// Decrypt the given string.  Assumes the string was encrypted using 
	/// EncryptStringAES(), using an identical sharedSecret.
	/// </summary>
	/// <param name="cipherText">The text to decrypt.</param>
	/// <param name="sharedSecret">A password used to generate a key for decryption.</param>
	public static string DecryptStringAES(string cipherText, string sharedSecret, byte[] salt) {
		if (String.IsNullOrEmpty(cipherText))
			throw new ArgumentNullException("cipherText");
		if (String.IsNullOrEmpty(sharedSecret))
			throw new ArgumentNullException("sharedSecret");

		// Declare the RijndaelManaged object
		// used to decrypt the data.
		AesManaged aesAlg = null;

		// Declare the string used to hold
		// the decrypted text.
		string plaintext = null;

		try {
			// generate the key from the shared secret and the salt
			Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, salt);

			// Create the streams used for decryption.                
			byte[] bytes = Convert.FromBase64String(cipherText);
			using (MemoryStream msDecrypt = new MemoryStream(bytes)) {
				// Create a RijndaelManaged object
				// with the specified key and IV.
				aesAlg = new AesManaged();
				aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
				// Get the initialization vector from the encrypted stream
				aesAlg.IV = ReadByteArray(msDecrypt);
				// Create a decrytor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
					using (StreamReader srDecrypt = new StreamReader(csDecrypt))

						// Read the decrypted bytes from the decrypting stream
						// and place them in a string.
						plaintext = srDecrypt.ReadToEnd();
				}
			}
		} finally {
			// Clear the RijndaelManaged object.
			aesAlg?.Clear();
		}

		return plaintext;
	}

	private static byte[] ReadByteArray(Stream s) {
		byte[] rawLength = new byte[sizeof(int)];
		if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length) {
			throw new SystemException("Stream did not contain properly formatted byte array");
		}

		byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
		if (s.Read(buffer, 0, buffer.Length) != buffer.Length) {
			throw new SystemException("Did not read byte array properly");
		}

		return buffer;
	}

	public static void EncryptStream(Stream input, Stream output, SymmetricAlgorithm symmetricAlgorithm) {
		// prepend IV
		output.Write(symmetricAlgorithm.IV, 0, symmetricAlgorithm.IV.Length);
		using (var transform = symmetricAlgorithm.CreateEncryptor())
		using (var encryptor = new CryptoStream(output, transform, CryptoStreamMode.Write)) {
			Tools.Streams.RouteStream(input, encryptor);
		}
	}

	public static void DecryptStream(Stream input, Stream output, SymmetricAlgorithm symmetricAlgorithm) {
		// read IV
		var iv = new byte[symmetricAlgorithm.BlockSize / 8];
		var numRead = input.Read(iv, 0, iv.Length);
		if (numRead != iv.Length)
			throw new Exception("Unable to contiguously read intialization vector from input stream");
		symmetricAlgorithm.IV = iv;
		using (var transform = symmetricAlgorithm.CreateDecryptor())
		using (var decryptor = new CryptoStream(input, transform, CryptoStreamMode.Read))
			Tools.Streams.RouteStream(decryptor, output);
	}

	public static SymmetricAlgorithm PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new() {
		salt ??= new byte[0];
		var algorithm = new TSymmetricAlgorithm {
			Padding = paddingMode,
			Mode = cipherMode,
		};
		algorithm.Key = PBKDF2.DeriveKey(password, salt, 100, algorithm.KeySize / 8);
		algorithm.IV = Tools.Crypto.GenerateCryptographicallyRandomBytes(algorithm.BlockSize / 8);
		return algorithm;
	}
}
