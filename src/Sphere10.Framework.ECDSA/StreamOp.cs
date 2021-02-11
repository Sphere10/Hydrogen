using System;
using System.Buffers.Binary;
using System.IO;

namespace Sphere10.Framework
{
    internal static class StreamOp
    {
        internal static byte[] SaveStreamToRaw(Stream stream)
        {
            var result = new byte[stream.Length];
            stream.Position = 0;
            stream.Read(result, 0, result.Length);
            return result;
        }

        internal static void LoadStreamFromRaw(Stream stream, byte[] raw) => stream.Write(raw, 0, raw.Length);

        internal static int ReadBytes(Stream stream, ref byte[] value, int checkLength = 0)
        {
            if (stream.Length - stream.Position < 2)
            {
                value = Array.Empty<byte>();
                return -1;
            }

            Span<byte> b = stackalloc byte[2];
            stream.Read(b.Slice(0, 2));
            var w = BinaryPrimitives.ReadUInt16LittleEndian(b);

            if (stream.Length - stream.Position < w || (checkLength > 0) && (w != checkLength))
            {
                stream.Position -= 2; // Go back!
                value = Array.Empty<byte>();
                return -1;
            }

            ;
            Array.Resize(ref value, w);
            if (w > 0)
                stream.Read(value, 0, w);
            return w + 2;
        }

        internal static int ReadString(Stream stream, out string value)
        {
            var raw = Array.Empty<byte>();
            var result = ReadBytes(stream, ref raw);
            value = ByteArrayUtils.ToString(raw);
            return result;
        }

        internal static int WriteBytes(Stream stream, ref byte[] value)
        {
            if (value.Length > 65536) throw new Exception($"Invalid Stream Size! {value.Length}");
            var w = (ushort) value.Length;
            Span<byte> tmp = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(tmp, w);
            stream.Write(tmp);
            if (w > 0)
                stream.Write(value, 0, w);
            return w + 2;
        }
    }
}