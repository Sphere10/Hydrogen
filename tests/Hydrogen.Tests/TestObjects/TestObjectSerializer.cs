using System.Text;

namespace Hydrogen.Tests;

public class TestObjectSerializer : ItemSerializer<TestObject> {
	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.UTF8, SizeDescriptorStrategy.UseVarInt).AsNullable();



	public override long CalculateSize(TestObject item)
		=> _stringSerializer.CalculateSize(item.A) + sizeof(int) + sizeof(bool);


	public override void Serialize(TestObject item, EndianBinaryWriter writer) {
		_stringSerializer.Serialize(item.A, writer);
		writer.Write(item.B);
		writer.Write(item.C);
	}

	public override TestObject Deserialize(EndianBinaryReader reader) 
		=> new(_stringSerializer.Deserialize(reader), reader.ReadInt32(), reader.ReadBoolean());

}
