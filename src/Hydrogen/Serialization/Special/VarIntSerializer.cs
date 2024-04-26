namespace Hydrogen;

public sealed class VarIntSerializer : ItemSerializerBase<VarInt> {

	public static VarIntSerializer Instance { get; } = new();

	public override long CalculateSize(SerializationContext context, VarInt item) 
		=> VarInt.SizeOf(item);

	public override void Serialize(VarInt item, EndianBinaryWriter writer, SerializationContext context) 
		=> VarInt.Write(item, writer.BaseStream);

	public override VarInt Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> VarInt.Read(reader.BaseStream);
}
