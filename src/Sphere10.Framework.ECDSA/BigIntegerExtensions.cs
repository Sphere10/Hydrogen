using Org.BouncyCastle.Math;

namespace Sphere10.Framework
{
    internal static class BigIntegerExtensions
    {
        internal static BigInteger Clone(this BigInteger value) => new(value.SignValue, value.ToByteArray());
    }
}