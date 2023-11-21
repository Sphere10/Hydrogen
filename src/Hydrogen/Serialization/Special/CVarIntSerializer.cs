namespace Hydrogen;

public sealed class CVarIntSerializer : ItemSerializerBase<long> {

	public static CVarIntSerializer Instance { get; } = new();

	public override long CalculateSize(SerializationContext context, long item) 
		=> CVarInt.SizeOf(unchecked((ulong)item));

	public override void Serialize(long item, EndianBinaryWriter writer, SerializationContext context) 
		=> CVarInt.Write(unchecked((ulong)item), writer.BaseStream);

	public override long Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> unchecked((long)CVarInt.Read(reader.BaseStream));
}