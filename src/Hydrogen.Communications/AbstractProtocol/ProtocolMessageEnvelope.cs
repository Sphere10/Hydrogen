namespace Hydrogen.Communications {
    public class ProtocolMessageEnvelope {
        public ProtocolDispatchType DispatchType { get; init; }
        public int RequestID { get; init; }
        public object Message { get; init; }
        public override string ToString() => $"[Protocol Message Envelope] DispatchType: {DispatchType}, RequestID: {RequestID}, Message: {Message ?? "NULL"}";
    }
}
