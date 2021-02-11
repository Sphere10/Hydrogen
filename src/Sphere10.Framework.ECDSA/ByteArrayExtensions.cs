using System;
using System.Linq;

namespace Sphere10.Framework
{
    internal static class ByteArrayExtensions
    {
        internal static byte[] DeepCopy(this byte[] value)
        {
            var result = new byte[value.Length];
            Array.Copy(value, result, result.Length);
            return result;
        }

        internal static bool IsEqualTo(this byte[] value, byte[] compareTo) => value.SequenceEqual(compareTo);

        internal static byte[] Add(this byte[] value, byte[] rawValue) => value.Concat(rawValue).ToArray();

        internal static string ToPrintable(this byte[] value)
        {
            var printableOrdTable = Enumerable.Range(32, 95).ToArray(); // [32 .. 126]
            return value.Aggregate("",
                (current, t) => current + (printableOrdTable.Contains(t) ? (char) t : (char) 126));
        }

        internal static bool IsEmpty(this byte[] value) => !value.Any();

        internal static string ToHexaString(this byte[] value) => HexEncoding.Encode(value);
    }
}