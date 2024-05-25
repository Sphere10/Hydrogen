namespace Hydrogen;

public sealed class CVarIntSerializer : ItemSerializerBase<CVarInt> {

	public static CVarIntSerializer Instance { get; } = new();

	public override long CalculateSize(SerializationContext context, CVarInt item) 
		=> CVarInt.SizeOf(item);

	public override void Serialize(CVarInt item, EndianBinaryWriter writer, SerializationContext context) 
		=> CVarInt.Write(item, writer.BaseStream);

	public override CVarInt Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> CVarInt.Read(reader.BaseStream);

}
