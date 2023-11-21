using System.Text;

namespace Hydrogen.Tests;

public class TestObjectSerializer : ItemSerializerBase<TestObject> {
	private readonly IItemSerializer<string> _stringSerializer = new StringSerializer(Encoding.UTF8, SizeDescriptorStrategy.UseVarInt).AsReferenceSerializer();



	public override long CalculateSize(SerializationContext context, TestObject item)
		=> _stringSerializer.CalculateSize(context, item.A) + sizeof(int) + sizeof(bool);


	public override void Serialize(TestObject item, EndianBinaryWriter writer, SerializationContext context) {
		_stringSerializer.Serialize(item.A, writer, context);
		writer.Write(item.B);
		writer.Write(item.C);
	}

	public override TestObject Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> new(_stringSerializer.Deserialize(reader, context), reader.ReadInt32(), reader.ReadBoolean());

}
