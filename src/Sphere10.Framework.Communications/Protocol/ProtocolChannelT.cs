using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolChannel<TEndpoint, TMessage> : ProtocolChannel {
	    // Events
        public event EventHandlerEx<object, TMessage> ReceivedMessage;
        public event EventHandlerEx<object, TMessage> SentMessage;

        // Properties

        public bool MessageSerializationEnabled { get; set; }

        public virtual IItemSerializer<TMessage> MessageSerializer { get; init; }
        
        public new virtual TEndpoint Endpoint {
	        get => (TEndpoint)base.Endpoint;
	        protected set => base.Endpoint = value;
        }

        // Methods
        public virtual async Task SendMessage(TMessage message) {
            Guard.Ensure(MessageSerializationEnabled, "Message Serialization is not enabled");
            await SendBytes(MessageSerializer.SerializeLE(message));
            NotifySentMessage(message);
        }

        // Template-pattern OnEvent methods

        protected override void OnReceivedBytes(byte[] bytes) {
            base.OnReceivedBytes(bytes);
            
            if (!MessageSerializationEnabled)
                return;

            using var readStream = new MemoryStream(bytes);
            using var reader = new EndianBinaryReader(EndianBitConverter.Little, readStream);

            if (readStream.Length < 4)
                throw new ProtocolException(BadDataType.MissingMessageLength, this);

            var size = reader.ReadUInt32();
            if (size < MinMessageLength || size > MaxMessageLength)
                throw new ProtocolException(BadDataType.InvalidMessageLength, this);

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
