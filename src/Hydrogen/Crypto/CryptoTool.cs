//-----------------------------------------------------------------------
// <copyright file="CryptoTool.cs" company="Sphere 10 Software">
//
// Copyright (c) Sphere 10 Software. All rights reserved. (http://www.sphere10.com)
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// <author>Herman Schoenfeld</author>
// <date>2018</date>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Hydrogen;

// ReSharper disable CheckNamespace
namespace Tools {

	public class Crypto {

		/// <summary>
		/// Coverup <see cref="bytes"/> with cryptographically random bytes. This stops any forensic analysis of the processes memory from retrieving sensitive <see cref="bytes"/>.
		/// </summary>
		/// <param name="bytes">The sensitive byte array to overwrite</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SecureErase(byte[] bytes) =>	GenerateCryptographicallyRandomBytes(bytes.Length).CopyTo(bytes.AsSpan());
	
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Guid NewCryptographicGuid() => CryptographicallySecureGUIDGenerator.NewCryptographicGuid();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte[] GenerateCryptographicallyRandomBytes(int count) => new SystemCRNG().NextBytes(count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string PasswordHash(string password) 
			=> PasswordHasher.CreateHash(password);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool ValidatePassword(string password, string hash) 
			=> PasswordHasher.ValidatePassword(password, hash);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EncryptStringAES(string plainText, string sharedSecret, string salt) 
			=> Encryptor.EncryptStringAES(plainText, sharedSecret, salt);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string EncryptStringAES(string plainText, string sharedSecret, byte[] salt) 
			=> Encryptor.EncryptStringAES(plainText, sharedSecret, salt);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string DecryptStringAES(string cipherText, string sharedSecret, string salt) 
			=> Encryptor.DecryptStringAES(cipherText, sharedSecret, salt);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string DecryptStringAES(string cipherText, string sharedSecret, byte[] salt) 
			=> Encryptor.DecryptStringAES(cipherText, sharedSecret, salt);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EncryptStream(Stream input, Stream output, SymmetricAlgorithm symmetricAlgorithm)
			=> Encryptor.EncryptStream(input, output, symmetricAlgorithm);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DecryptStream(Stream input, Stream output, SymmetricAlgorithm symmetricAlgorithm) 
			=> Encryptor.DecryptStream(input, output, symmetricAlgorithm);
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SymmetricAlgorithm PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC) where TSymmetricAlgorithm : SymmetricAlgorithm, new()
			=> Encryptor.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode);

	}
}