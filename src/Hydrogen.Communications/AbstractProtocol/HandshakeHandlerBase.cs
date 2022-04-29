namespace Sphere10.Framework.Communications {
	public abstract class HandshakeHandlerBase : IHandshakeHandler {
		public abstract object GenerateHandshake(ProtocolOrchestrator orchestrator);

		public abstract HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, object handshake, out object acknowledgement);

		public abstract HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, out object verifyAcknowledgement);

		public abstract bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, object handshake, object acknowledgement, object verifyAcknowledgement);
	}
}
