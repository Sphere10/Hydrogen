using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
	public class ProtocolMessageEnvelopeSerializer : ItemSerializer<ProtocolMessageEnvelope> {
		private readonly IItemSerializer<object> _payloadSerializer;
		private static readonly byte[] MessageEnvelopeMarker = {0,1,2,3};
		private static readonly int MessageEnvelopeLength = MessageEnvelopeMarker.Length + sizeof(byte) + sizeof(int) + sizeof(int);  // magicID + dispatchType + requestID + messageLength + payload 

		public ProtocolMessageEnvelopeSerializer(IItemSerializer<object> payloadSerializer) {
			_payloadSerializer = payloadSerializer;
		}

		public override int CalculateSize(ProtocolMessageEnvelope item)
			=> MessageEnvelopeMarker.Length + _payloadSerializer.CalculateSize(item.Message);	   
		
		public override bool TrySerialize(ProtocolMessageEnvelope item, EndianBinaryWriter writer, out int bytesWritten) {
			writer.Write(MessageEnvelopeMarker);
			writer.Write((byte)item.DispatchType);
			writer.Write(item.RequestID);
			writer.Write(_payloadSerializer.CalculateSize(item.Message));
			bytesWritten = MessageEnvelopeLength;
			if (!_payloadSerializer.TrySerialize(item.Message, writer, out var itemBytesWritten)) 
				return false;
			bytesWritten += itemBytesWritten;
			return true;
		}

		public override bool TryDeserialize(int byteSize, EndianBinaryReader reader, out ProtocolMessageEnvelope envelope) {
			//using var readStream = new MemoryStream(bytes.ToArray()); // TODO: uses slow ToArray
			envelope = null;

			if (reader.BaseStream.Length < MessageEnvelopeLength)
				return false; // Not a message envelope

			// Read magic header for message object 
			var magicID = reader.ReadBytes(MessageEnvelopeMarker.Length);
			if (!magicID.AsSpan().SequenceEqual(MessageEnvelopeMarker))
				return false; // Message Magic ID not found, so not a message

			// Read envelope
			var dispatchType = (ProtocolDispatchType)reader.ReadByte();
			var requestID = reader.ReadInt32();
			var messageLength = reader.ReadInt32();

			if (reader.BaseStream.Length < MessageEnvelopeLength + messageLength)
				return false; // message not present

			// Deserialize message
			if (!_payloadSerializer.TryDeserialize((int)messageLength, reader, out var message))
				return false; //  Malformed message payload(unable to deserialize message

			envelope = new ProtocolMessageEnvelope {
				DispatchType = dispatchType,
				RequestID = requestID,
				Message = message
			};

			return true;
		}

	}
}
