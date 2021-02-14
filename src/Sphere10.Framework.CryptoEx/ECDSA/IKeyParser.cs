using System.IO;
using Org.BouncyCastle.Crypto;

namespace Sphere10.Framework.CryptoEx
{
    public interface IKeyParser
    {
        AsymmetricKeyParameter ReadKey(Stream stream);
    }
}