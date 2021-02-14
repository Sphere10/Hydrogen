using System;
using System.Buffers.Binary;
using System.IO;

namespace Sphere10.Framework.CryptoEx
{
    internal static class ByteArrayUtils
    {
        internal static byte[] FromString(string input)
        {
            var result = Array.Empty<byte>();
            Array.Resize(ref result, input.Length);
            for (var i = 0; i < input.Length; i++)
                result[i] = (byte) input[i];
            return result;
        }

        internal static string ToString(byte[] value) =>
            value.Length > 0 ? System.Text.Encoding.ASCII.GetString(value) : "";

        internal static bool LoadFromBytes(byte[] source, ref int startPosition, ref byte[] result)
        {
            if (startPosition + 2 > source.Length) return false;
            var size = BinaryPrimitives.ReadUInt16LittleEndian(source.AsSpan(startPosition));
            if (startPosition + 2 + size > source.Length) return false;
            Array.Resize(ref result, size);
            Buffer.BlockCopy(source, startPosition + 2, result, 0, size);
            startPosition += 2 + size;
            return true;
        }

        internal static void SaveInsideBytes(byte[] value, ref byte[] destToWrite, ref int startPosition)
        {
            if (startPosition + value.Length + 2 > destToWrite.Length)
                Array.Resize(ref destToWrite, startPosition + value.Length + 2);
            var size = (ushort) value.Length;
            BinaryPrimitives.WriteUInt16LittleEndian(destToWrite.AsSpan(startPosition), size);
            Buffer.BlockCopy(value, 0, destToWrite, startPosition + 2, size);
            startPosition += 2 + size;
        }

        internal static bool FromSerialized(byte[] serialized, ref byte[] result)
        {
            if (serialized.Length < 2) return false;
            var size = BinaryPrimitives.ReadUInt16LittleEndian(serialized);
            if (2 + size > serialized.Length) return false;
            Array.Resize(ref result, size);
            Buffer.BlockCopy(serialized, 2, result, 0, size);
            return true;
        }

        internal static byte[] ToSerialized(byte[] value)
        {
            var size = value.Length;
            if (size > 65536) throw new Exception($"Cannot serialize byte[] due high length {value.Length}");
            var result = new byte[size + 2];
            var b = new byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(b, (ushort) size);
            Buffer.BlockCopy(b, 0, result, 0, 2);
            Buffer.BlockCopy(value, 0, result, 2, size);
            return result;
        }

        internal static int FromSerialized(ref byte[] result, Stream stream, int checkLength = 0) =>
            StreamOp.ReadBytes(stream, ref result, checkLength);

        internal static void ToSerialized(ref byte[] result, Stream stream) => StreamOp.WriteBytes(stream, ref result);

        internal static byte[] FromStream(Stream stream, int startPos, int length)
        {
            var result = Array.Empty<byte>();
            Array.Resize(ref result, length);
            stream.Position = startPos;
            stream.Read(result, 0, length);
            return result;
        }

        internal static byte[] FromStream(Stream stream) =>
            FromStream(stream, 0, (int) stream.Length);

        internal static int GetSerializedLength(byte[] value) => 2 + value.Length;
    }
}