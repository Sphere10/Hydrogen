// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Hydrogen;
using Hydrogen.Maths;

// ReSharper disable CheckNamespace
namespace Tools;

public class Crypto {

	/// <summary>
	/// Coverup <see cref="bytes"/> with cryptographically random bytes. This stops any forensic analysis of the processes memory from retrieving sensitive <see cref="bytes"/>.
	/// </summary>
	/// <param name="bytes">The sensitive byte array to overwrite</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void SecureErase(byte[] bytes) => GenerateCryptographicallyRandomBytes(bytes.Length).CopyTo(bytes.AsSpan());


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
	public static SymmetricAlgorithm PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(string password, byte[] salt = null, PaddingMode paddingMode = PaddingMode.PKCS7, CipherMode cipherMode = CipherMode.CBC)
		where TSymmetricAlgorithm : SymmetricAlgorithm, new()
		=> Encryptor.PrepareSymmetricAlgorithm<TSymmetricAlgorithm>(password, salt, paddingMode, cipherMode);

}
