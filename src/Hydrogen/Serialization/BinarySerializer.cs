using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Hydrogen;

/// <summary>
/// Replacement for BinarySerializer deprecated in .NET 5.0 using Hydrogen's serialization framework.
/// </summary>
public sealed class BinarySerializer : ItemSerializerDecorator<object, ReferenceSerializer<object>> {
	private readonly RegistrationSerializer _registrationsSerializer;
	private readonly SerializerFactory _staticFactory;

	private record struct FactoryRegistration(Type Type, long TypeCode);

	public BinarySerializer() : this(SerializerFactory.Default) {
	}

	public BinarySerializer(SerializerFactory serializerFactory) 
		: base(new ReferenceSerializer<object>(new PolymorphicSerializer<object>(serializerFactory))) {
		_registrationsSerializer = new RegistrationSerializer();
		_staticFactory = serializerFactory;
	}


	public override long CalculateSize(SerializationContext context, object item) {
		context.SetEphemeralFactory(new SerializerFactory(_staticFactory));
		try {
			// calculate item size
			var itemSize = base.CalculateSize(context, item);

			// calculate registrations size
			context.EphemeralFactory.FinishRegistrations();
			var registrations = context.GetEphemeralRegistrations().Select(x => new FactoryRegistration(x.DataType, x.TypeCode));
			var registrationsSize = _registrationsSerializer.CalculateSize(registrations); // don't use context

			return registrationsSize + itemSize;
		} finally {
			context.ClearEphemeralFactory();
		}
	}

	// Compatible with BinaryFormatter.Serialize
	public void Serialize(Stream stream, object item) { 
		using var writer = new EndianBinaryWriter(EndianBitConverter.Little, stream.AsNonClosing());
		using var context = SerializationContext.New;
		Serialize(item, writer, context); 
	}


	public override void Serialize(object item, EndianBinaryWriter writer, SerializationContext context) {
		context.SetEphemeralFactory(new SerializerFactory(_staticFactory));
		try {
			// serialize item into temp buffer
			using var memoryStream = new MemoryStream();
			using var memoryWriter = new EndianBinaryWriter(writer.BitConverter, memoryStream);
			base.Serialize(item, memoryWriter, context);
			var buffer = memoryStream.ToArray();

			// serialize registrations first
			context.EphemeralFactory.FinishRegistrations();
			var registrations = context.GetEphemeralRegistrations().Select(x => new FactoryRegistration(x.DataType, x.TypeCode));;
			_registrationsSerializer.Serialize(registrations, writer);  // don't use context

			// serialize item (buffer)
			writer.Write(buffer);
		} finally {
			context.ClearEphemeralFactory();
		}
	}


	// Compatible with BinaryFormatter.Deserialize
	public object Deserialize(Stream stream) {
		using var reader = new EndianBinaryReader(EndianBitConverter.Little, stream.AsNonClosing());
		using var context = SerializationContext.New;
		return Deserialize(reader, context); 
	}

	public override object Deserialize(EndianBinaryReader reader, SerializationContext context) {
	
		// Deserialize type registrations
		var registrations = _registrationsSerializer.Deserialize(reader); // don't use context

		// Rebuild ephemeral factory used for serialization
		context.SetEphemeralFactory(new SerializerFactory(_staticFactory));

		foreach(var registration in registrations.OrderBy(x => x.TypeCode)) {
			if (!context.EphemeralFactory.TryGetRegistration(registration.TypeCode, out var factoryRegistration)) {
				SerializerBuilder.FactoryAssemble(context.EphemeralFactory, registration.Type, true, registration.TypeCode);
			} else {
				// it's already registered (likely as a dependent of prior registration), need to check it matches what we expect
				Guard.Ensure(registration.Type == factoryRegistration.DataType, $"Deserialization type-code mismatch for type-code {registration.TypeCode}  (expected type {registration.Type.ToStringCS()} but was {factoryRegistration.DataType.ToStringCS()})");
			}
		}
		context.EphemeralFactory.FinishRegistrations();
		
		// Deserialize item
		var item = base.Deserialize(reader, context);
		
		return item;
	}

	

	private class RegistrationSerializer : ProjectedSerializer<IEnumerable<(Type, long)>, IEnumerable<FactoryRegistration>> {
		public RegistrationSerializer() 
			: base(
				  new TaggedTypeCollectionSerializer<long>(PrimitiveSerializer<long>.Instance, SizeDescriptorStrategy.UseCVarInt),
				  x => x.Select(y => new FactoryRegistration(y.Item1, y.Item2)),
				  x => x.Select(y => (y.Type, y.TypeCode))
			) {
		}
	}
}
