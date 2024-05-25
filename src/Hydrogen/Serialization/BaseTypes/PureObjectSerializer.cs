namespace Hydrogen;

public sealed class PureObjectSerializer : ItemSerializerBase<object> {

	public override long CalculateSize(SerializationContext context, object item) => 0L;

	public override void Serialize(object item, EndianBinaryWriter writer, SerializationContext context) {
		// No state to serialize
	}

	public override object Deserialize(EndianBinaryReader reader, SerializationContext context) 
		=> new (); // No state to deserialize
		
}
