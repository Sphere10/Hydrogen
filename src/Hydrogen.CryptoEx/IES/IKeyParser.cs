using System.IO;
using Org.BouncyCastle.Crypto;

namespace Hydrogen.CryptoEx.IES {
	public interface IKeyParser
    {
        AsymmetricKeyParameter ReadKey(Stream stream);
    }
}