using System;

namespace Sphere10.Framework.Communications {

	// common handshake delegates
	public delegate THandshake GenerateHandshakeDelegate<out THandshake>(ProtocolOrchestrator orchestrator);
	public delegate HandshakeOutcome ReceiveHandshakeDelegate<in THandshake, TAck>(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement);

	// 2-way handshake delegates
	public delegate HandshakeOutcome Verify2WayHandshakeDelegate<in THandshake, in TAck>(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement);
	
	// 3-way handshake delegates
	public delegate HandshakeOutcome Verify3WayHandshakeDelegate<in THandshake, in TAck, TVerack>(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out TVerack verifyAcknowledgement);
	public delegate bool Acknowledge3WayHandshakeDelegate<in THandshake, in TAck, in TVerack>(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, TVerack verifyAcknowledgement);

}
