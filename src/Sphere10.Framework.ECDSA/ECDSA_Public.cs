using System;
using System.Buffers.Binary;
using System.IO;

namespace Sphere10.Framework
{
    public struct ECDSA_Public
    {
        public ushort EC_OpenSSL_NID;
        public byte[] X, Y;

        internal void Clear()
        {
            EC_OpenSSL_NID = 0;
            X = null;
            Y = null;
        }

        internal int GetSerializedLength() =>
            2 + ByteArrayUtils.GetSerializedLength(X) + ByteArrayUtils.GetSerializedLength(Y);

        public byte[] ToSerialized()
        {
            var result = new byte[2 + 2 + X.Length + 2 + Y.Length];
            BinaryPrimitives.WriteUInt16LittleEndian(result, EC_OpenSSL_NID);
            var pos = 2;
            ByteArrayUtils.SaveInsideBytes(X, ref result, ref pos);
            ByteArrayUtils.SaveInsideBytes(Y, ref result, ref pos);
            return result;
        }

        internal void ToSerialized(Stream stream)
        {
            Span<byte> tmp = stackalloc byte[2];
            BinaryPrimitives.WriteUInt16LittleEndian(tmp, EC_OpenSSL_NID);
            stream.Write(tmp);
            ByteArrayUtils.ToSerialized(ref X, stream);
            ByteArrayUtils.ToSerialized(ref Y, stream);
        }

        public bool FromSerialized(byte[] serialized)
        {
            var i = 0;
            return LoadFromBytes(serialized, ref i);
        }

        internal bool FromSerialized(Stream stream)
        {
            Span<byte> tmp = stackalloc byte[2];
            if (stream.Read(tmp.Slice(0, 2)) != 2) return false;
            EC_OpenSSL_NID = BinaryPrimitives.ReadUInt16LittleEndian(tmp);
            return ByteArrayUtils.FromSerialized(ref X, stream) >= 0 &&
                   ByteArrayUtils.FromSerialized(ref Y, stream) >= 0;
        }

        internal bool LoadFromBytes(byte[] bytes, ref int startIndex)
        {
            Clear();
            if (startIndex + 2 + 2 + 2 > bytes.Length) return false;
            EC_OpenSSL_NID = BinaryPrimitives.ReadUInt16LittleEndian(bytes.AsSpan(startIndex));
            startIndex += 2;
            return ByteArrayUtils.LoadFromBytes(bytes, ref startIndex, ref X) &&
                   ByteArrayUtils.LoadFromBytes(bytes, ref startIndex, ref Y);
        }

        public bool IsEqualTo(ECDSA_Public compareTo) =>
            EC_OpenSSL_NID == compareTo.EC_OpenSSL_NID
            && X.IsEqualTo(compareTo.X)
            && Y.IsEqualTo(compareTo.Y);
    }
}