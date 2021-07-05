using System;
using System.IO;
using AutoFixture;
using NUnit.Framework;

namespace Sphere10.Framework.Tests
{
    public class GenericItemSerializerTests
    {
        private readonly IItemSerializer<TestObject> _serializer = GenericItemSerializer<TestObject>.Default;

        private IFixture _fixture = new Fixture();

        [Test]
        public void SimpleObjectSerializeDeserialize()
        {
            GenericItemSerializer<TestObject>.Register<TestObject>();
            var item = _fixture.Create<TestObject>();

            using var memoryStream = new MemoryStream();
            var writer = new EndianBinaryWriter(EndianBitConverter.Little, memoryStream);
            var byteCount = _serializer.Serialize(item, writer);

            Assert.AreEqual(memoryStream.Length, byteCount);

            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new EndianBinaryReader(EndianBitConverter.Little, memoryStream);
            var deserializedItem = _serializer.Deserialize(byteCount, reader);

            Assert.AreEqual(item, deserializedItem);
        }
    }

    internal class TestObject
    {
        protected bool Equals(TestObject other) {
            return A == other.A && B == other.B && C == other.C && D == other.D && E == other.E && F == other.F && G == other.G && H == other.H && I.Equals(other.I) && J == other.J && K == other.K && L == other.L && M.Equals(other.M);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((TestObject)obj);
        }

        public override int GetHashCode() {
            var hashCode = new HashCode();
            hashCode.Add(A);
            hashCode.Add(B);
            hashCode.Add(C);
            hashCode.Add(D);
            hashCode.Add(E);
            hashCode.Add(F);
            hashCode.Add(G);
            hashCode.Add(H);
            hashCode.Add(I);
            hashCode.Add(J);
            hashCode.Add(K);
            hashCode.Add(L);
            hashCode.Add(M);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(TestObject left, TestObject right) {
            return Equals(left, right);
        }

        public static bool operator !=(TestObject left, TestObject right) {
            return !Equals(left, right);
        }

        public string A { get; set; }
        public bool B { get; set; }
        public int C { get; set; }
        public uint D { get; set; }
        public short E { get; set; }
        public ushort F { get; set; }
        public long G { get; set; }
        public ulong H { get; set; }
        public double I { get; set; }
        public decimal J { get; set; }
        public sbyte K { get; set; }
        public byte L { get; set; }
        public float M { get; set; }
    }
}