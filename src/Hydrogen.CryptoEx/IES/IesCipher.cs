// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Ugochukwu Mmaduekwe
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Data;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Hydrogen.CryptoEx.EC.IES;

namespace Hydrogen.CryptoEx.IES;

public class IesCipher {
	private readonly CustomIesEngine _customIesEngine;
	private readonly int _ivLength;
	private bool _forEncryption;
	private readonly MemoryStream _buffer;
	private IesParameterSpec _engineSpec;
	private AsymmetricKeyParameter _key;
	private SecureRandom _random;

	public IesCipher(CustomIesEngine customIesEngine, int ivLength = 0) {
		_customIesEngine = customIesEngine;
		_ivLength = ivLength;
		_buffer = new MemoryStream();
	}

	public void Init(bool forEncryption, ICipherParameters key, IAlgorithmParameterSpec engineSpec, SecureRandom random) {
		_forEncryption = forEncryption;

		if (engineSpec == null) {
			throw new ArgumentNullException(string.Format("{0}", "Engine Spec Cannot Be Nil"));
		}

		if (engineSpec is IesParameterSpec) {
			_engineSpec = (IesParameterSpec)engineSpec;
		} else {
			throw new InvalidParameterException("Must be Passed IES Parameter Spec");
		}

		byte[] nonce = _engineSpec.GetNonce();

		if ((_ivLength != 0) && (nonce == null || nonce.Length != _ivLength)) {
			throw new InvalidParameterException("NONCE in IES Parameters needs to be " + _ivLength + " bytes long");
		}

		// Parse the recipient's key
		if (forEncryption) {
			if (!(key is AsymmetricKeyParameter) || ((AsymmetricKeyParameter)key).IsPrivate) {
				throw new InvalidKeyException("Must be Passed Recipient's Public EC Key for Encryption");
			}

			_key = (AsymmetricKeyParameter)key;
		} else {
			if (key is ParametersWithRandom) {
				key = ((ParametersWithRandom)key).Parameters;
			}

			if (!(key is AsymmetricKeyParameter) || !((AsymmetricKeyParameter)key).IsPrivate) {
				throw new InvalidKeyException("Must be Passed Recipient's Private EC Key for Decryption");
			}

			_key = (AsymmetricKeyParameter)key;
		}

		_random = random;
		_buffer.Flush();
		_buffer.SetLength(0);
	}

	public void ProcessBytes(byte[] input) {
		ProcessBytes(input, 0, input.Length);
	}

	public void ProcessBytes(byte[] input, int inOff, int inLen) {
		_buffer.Write(input, inOff, inLen);
	}

	public byte[] DoFinal(byte[] input) {
		return DoFinal(input, 0, input.Length);
	}

	public byte[] DoFinal(byte[] input, int inOff, int inLen) {
		if (inLen != 0) {
			_buffer.Write(input, inOff, inLen);
		}

		byte[] @in = _buffer.ToArray();
		_buffer.Flush();
		_buffer.SetLength(0);

		// Convert parameters for use in IESEngine
		ICipherParameters @params = new IesWithCipherParameters(_engineSpec.GetDerivationV(),
			_engineSpec.GetEncodingV(),
			_engineSpec.GetMacKeySize(),
			_engineSpec.GetCipherKeySize());

		if (_engineSpec.GetNonce() != null) {
			@params = new ParametersWithIV(@params, _engineSpec.GetNonce());
		}

		ECDomainParameters ecParams = ((ECKeyParameters)_key).Parameters;

		if (_forEncryption) {
			// Generate the ephemeral key pair
			ECKeyPairGenerator gen = new ECKeyPairGenerator();
			gen.Init(new ECKeyGenerationParameters(ecParams, _random));

			bool usePointCompression = _engineSpec.GetPointCompression();
			EphemeralKeyPairGenerator
				kGen = new EphemeralKeyPairGenerator(gen, new KeyEncoder(usePointCompression));

			// Encrypt the buffer
			try {
				_customIesEngine.Init(_key, @params, kGen);

				return _customIesEngine.ProcessBlock(@in, 0, @in.Length);
			} catch (Exception e) {
				throw new DataException("unable to process block", e);
			}
		} else {
			// Decrypt the buffer
			try {
				_customIesEngine.Init(_key, @params, new ECIESPublicKeyParser(ecParams));

				return _customIesEngine.ProcessBlock(@in, 0, @in.Length);
			} catch (InvalidCipherTextException e) {
				throw new DataException("unable to process block", e);
			}
		}
	}

	public int DoFinal(byte[] input, int inOff, int inLen, byte[] output, int outOff) {
		byte[] buf = DoFinal(input, inOff, inLen);
		Array.Copy(buf, 0, output, outOff, buf.Length);
		return buf.Length;
	}
}
