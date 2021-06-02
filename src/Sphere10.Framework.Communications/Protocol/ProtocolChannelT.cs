using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolChannel<TEndpoint, TMessage> : ProtocolChannel {
        private const int DefaultMinMessageLength = 0;
        private const int DefaultMaxMessageLength = 65536;

        // Events
        public event EventHandlerEx<object, TMessage> ReceivedMessage;
        public event EventHandlerEx<object, TMessage> SentMessage;
        private readonly byte[] _messageMagicID;

        protected ProtocolChannel(byte[] messageMagicID) {
            Guard.ArgumentNotNull(messageMagicID, nameof(messageMagicID));
	        _messageMagicID = messageMagicID;
        } 

        // Properties
        public bool MessageSerializationEnabled { get; set; }

        public virtual IItemSerializer<TMessage> MessageSerializer { get; init; }

        public virtual int MinMessageLength { get; init; } = DefaultMinMessageLength;

        public virtual int MaxMessageLength { get; init; } = DefaultMaxMessageLength;

        public virtual TEndpoint Endpoint { get; protected set; }

        // Methods
        public virtual async Task SendMessage(TMessage message) {
            Guard.Ensure(MessageSerializationEnabled, "Message Serialization is not enabled");
            using var memStream = new MemoryStream();
            using var writer = new EndianBinaryWriter(EndianBitConverter.Little, memStream);
            writer.Write(_messageMagicID);
            writer.Write((uint)MessageSerializer.CalculateSize(message));
            MessageSerializer.Serialize(message, writer);
            await SendBytes(memStream.ToArray());
            NotifySentMessage(message);
        }

        // Template-pattern OnEvent methods

        protected override void OnReceivedBytes(byte[] bytes) {
            base.OnReceivedBytes(bytes);
            
            if (!MessageSerializationEnabled)
                return;

            using var readStream = new MemoryStream(bytes);
            using var reader = new EndianBinaryReader(EndianBitConverter.Little, readStream);

            if (readStream.Length < _messageMagicID.Length)
	            return; // Not an object message

            // Read magic header for message object 
            var magicID = reader.ReadBytes(_messageMagicID.Length);
            if (!magicID.AsSpan().SequenceEqual(_messageMagicID))
	            return; // Message Magic ID not found, so not a message

            // Read message object length
            if (readStream.Length < _messageMagicID.Length + 4)
                throw new ProtocolException(BadDataType.MissingMessageLength, this);
            var size = reader.ReadUInt32();
            if (size < MinMessageLength || size > MaxMessageLength)
                throw new ProtocolException(BadDataType.InvalidMessageLength, this);

            // Deserialize message
            TMessage message;
            try {
                message = MessageSerializer.Deserialize((int)size, reader);
            } catch (Exception error) {
                throw new ProtocolException(BadDataType.InvalidMessage, this, error);
            }
            NotifyReceivedMessage(message);
        }

        protected virtual void OnReceivedMessage(TMessage message) {
        }

        protected virtual void OnSentMessage(TMessage message) {
        }

        // Notify methods

        protected virtual void NotifyReceivedMessage(TMessage message) {
            OnReceivedMessage(message);
            ReceivedMessage?.Invoke(this, message);
        }

        protected virtual void NotifySentMessage(TMessage message) {
            OnSentMessage(message);
            SentMessage?.Invoke(this, message);
        }

    }
   
}
