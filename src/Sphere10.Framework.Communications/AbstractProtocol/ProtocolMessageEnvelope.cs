namespace Sphere10.Framework.Communications {
    public class ProtocolMessageEnvelope {
        public ProtocolMessageType MessageType { get; init; }
        public int RequestID { get; init; }
        public object Message { get; init; }
        public override string ToString() => $"[Protocol Message Envelope] Type: {MessageType}, RequestID: {RequestID}, Message: {Message ?? "NULL"}";
    }
}
