// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications;

public class Action3WayHandshakeHandler<THandshake, TAck, TVerack> : HandshakeHandlerBase<THandshake, TAck, TVerack> {

	private readonly GenerateHandshakeDelegate<THandshake> _generate;
	private readonly ReceiveHandshakeDelegate<THandshake, TAck> _receive;
	private readonly Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> _verify;
	private readonly Acknowledge3WayHandshakeDelegate<THandshake, TAck, TVerack> _acknowledge;

	public Action3WayHandshakeHandler(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake)
		: this(generateHandshake, receiveHandshake, verifyHandshake, default) {
	}

	public Action3WayHandshakeHandler(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake,
	                                  Acknowledge3WayHandshakeDelegate<THandshake, TAck, TVerack> acknowledgeHandshake) {
		Guard.ArgumentNotNull(generateHandshake, nameof(generateHandshake));
		Guard.ArgumentNotNull(receiveHandshake, nameof(receiveHandshake));
		Guard.ArgumentNotNull(verifyHandshake, nameof(verifyHandshake));
		Guard.ArgumentNotNull(acknowledgeHandshake, nameof(acknowledgeHandshake));
		_generate = generateHandshake;
		_receive = receiveHandshake;
		_verify = verifyHandshake;
		_acknowledge = acknowledgeHandshake;
	}

	protected override THandshake GenerateHandshakeInternal(ProtocolOrchestrator orchestrator)
		=> _generate(orchestrator);

	public override HandshakeOutcome ReceiveHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, out TAck acknowledgement)
		=> _receive(orchestrator, handshake, out acknowledgement);

	public override HandshakeOutcome VerifyHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, out TVerack verifyAcknowledgement)
		=> _verify(orchestrator, handshake, acknowledgement, out verifyAcknowledgement);

	public override bool AcknowledgeHandshake(ProtocolOrchestrator orchestrator, THandshake handshake, TAck acknowledgement, TVerack verifyAcknowledgement)
		=> _acknowledge(orchestrator, handshake, acknowledgement, verifyAcknowledgement);

}
