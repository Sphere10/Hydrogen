// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ProtocolMessageEnvelopeSerializer : ItemSerializerBase<ProtocolMessageEnvelope> {
	private readonly IItemSerializer<object> _payloadSerializer;
	private static readonly byte[] MessageEnvelopeMarker = { 0, 1, 2, 3 };
	private static readonly int MessageEnvelopeLength = MessageEnvelopeMarker.Length + sizeof(byte) + sizeof(int) + sizeof(int); // magicID + dispatchType + requestID + messageLength + payload 

	public ProtocolMessageEnvelopeSerializer(IItemSerializer<object> payloadSerializer) {
		_payloadSerializer = payloadSerializer;
	}

	public override long CalculateSize(SerializationContext context, ProtocolMessageEnvelope item)
		=> MessageEnvelopeMarker.Length + _payloadSerializer.CalculateSize(context, item.Message);

	public override void Serialize(ProtocolMessageEnvelope item, EndianBinaryWriter writer, SerializationContext context) {
		writer.Write(MessageEnvelopeMarker);
		writer.Write((byte)item.DispatchType);
		writer.Write(item.RequestID);
		writer.Write(_payloadSerializer.CalculateSize(item.Message));
		_payloadSerializer.Serialize(item.Message, writer, context);
	}

	public override ProtocolMessageEnvelope Deserialize(EndianBinaryReader reader, SerializationContext context) {
		if (reader.BaseStream.Length < MessageEnvelopeLength)
			throw new ArgumentException("Stream is too short to be a message envelope");

		// Read magic header for message object 
		var magicID = reader.ReadBytes(MessageEnvelopeMarker.Length);
		if (!magicID.AsSpan().SequenceEqual(MessageEnvelopeMarker))
			throw new ArgumentException("Message Magic ID not found, so not a message");

		// Read envelope
		var dispatchType = (ProtocolDispatchType)reader.ReadByte();
		var requestID = reader.ReadInt32();
		var messageLength = reader.ReadInt32();

		if (reader.BaseStream.Length < MessageEnvelopeLength + messageLength)
			throw new ArgumentException("Message payload not found");

		// Deserialize message
		var message = _payloadSerializer.Deserialize(reader, context);

		return new ProtocolMessageEnvelope {
			DispatchType = dispatchType,
			RequestID = requestID,
			Message = message
		};
	}
}
