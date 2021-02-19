using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.IO;

namespace Sphere10.Framework.CryptoEx.IES {

	public class EciesPublicKeyParser : IKeyParser {
		private readonly ECDomainParameters _ecParams;

		public EciesPublicKeyParser(ECDomainParameters ecParams) {
			_ecParams = ecParams;
		}

		public AsymmetricKeyParameter ReadKey(Stream stream) {
			byte[] v;
			int first = stream.ReadByte();

			// Decode the public ephemeral key
			switch (first) {
				case 0x00: // infinity
					throw new IOException("Sender's public key invalid.");

				case 0x02: // compressed
				case 0x03: // Byte length calculated as in ECPoint.getEncoded();
					v = new byte[1 + (_ecParams.Curve.FieldSize + 7) / 8];
					break;

				case 0x04: // uncompressed or
				case 0x06: // hybrid
				case 0x07: // Byte length calculated as in ECPoint.getEncoded();
					v = new byte[1 + 2 * ((_ecParams.Curve.FieldSize + 7) / 8)];
					break;

				default:
					throw new IOException("Sender's public key has invalid point encoding 0x" + first.ToString("X2"));
			}

			v[0] = (byte)first;
			Streams.ReadFully(stream, v, 1, v.Length - 1);

			return new ECPublicKeyParameters(_ecParams.Curve.DecodePoint(v), _ecParams);
		}
	}
}