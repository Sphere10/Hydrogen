using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hydrogen.Tests;

public class TestObject {

	public TestObject(Random random) {
		A = random.NextString(random.Next(0, 101));
		B = random.Next(0, 1000);
		C = random.NextBool();
	}

	public TestObject(string a, int b, bool c) {
		A = a;
		B = b;
		C = c;
	}

	public string A { get; set; }

	public int B { get; set; }

	public bool C { get; set; }

	public override string ToString() => $"[TestObject] A: '{A}', B: {B}, C: {C}";

}


public class TestObjectSerializer : ItemSerializer<TestObject> {
	private readonly AutoSizedSerializer<string> _stringSerializer = new(new StringSerializer(Encoding.UTF8).AsNullable(), SizeDescriptorStrategy.UseVarInt);

	public override long CalculateSize(TestObject item)
		=> _stringSerializer.CalculateSize(item.A) + sizeof(int) + sizeof(bool);


	public override void SerializeInternal(TestObject item, EndianBinaryWriter writer) {
		_stringSerializer.SerializeInternal(item.A, writer);
		writer.Write(item.B);
		writer.Write(item.C);
	}

	public override TestObject DeserializeInternal(long byteSize, EndianBinaryReader reader) 
		=> new(_stringSerializer.Deserialize(reader), reader.ReadInt32(), reader.ReadBoolean());

}

public class TestObjectComparer : IEqualityComparer<TestObject> {
	public bool Equals(TestObject x, TestObject y) {
		if (ReferenceEquals(x, y))
			return true;
		if (ReferenceEquals(x, null))
			return false;
		if (ReferenceEquals(y, null))
			return false;
		if (x.GetType() != y.GetType())
			return false;
		return x.A == y.A && x.B == y.B && x.C == y.C;
	}
	public int GetHashCode(TestObject obj) {
		return HashCode.Combine(obj.A, obj.B, obj.C);
	}
}