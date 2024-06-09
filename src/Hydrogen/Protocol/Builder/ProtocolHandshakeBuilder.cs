// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ProtocolHandshakeBuilder {
	ProtocolBuilder _parent;
	object _specificBuilder;

	public ProtocolHandshakeBuilder(ProtocolBuilder parent) {
		_parent = parent;
	}


	public TwoWayHandshakeBuilder UseTwoWay() {
		_specificBuilder = new TwoWayHandshakeBuilder(this);
		return (TwoWayHandshakeBuilder)_specificBuilder;
	}

	public ThreeWayHandshakeBuilder UseThreeWay() {
		_specificBuilder = new ThreeWayHandshakeBuilder(this);
		return (ThreeWayHandshakeBuilder)_specificBuilder;
		return new(this);
	}

	public ProtocolHandshake Build() {
		return _specificBuilder switch {
			TwoWayHandshakeBuilder twoWayBuilder => twoWayBuilder.Build(),
			ThreeWayHandshakeBuilder threeWayBuilder => threeWayBuilder.Build(),
			_ => throw new InternalErrorException("0924E3CD-C144-4D4D-AFF2-E037C82D8CD2")
		};
	}


	public class TwoWayHandshakeBuilder {
		private readonly ProtocolHandshakeBuilder _parent;
		private CommunicationRole? _initiator = null;
		private IHandshakeHandler _handler = null;
		private Type[] _messageTypes = null;

		public TwoWayHandshakeBuilder(ProtocolHandshakeBuilder parent) {
			_parent = parent;
		}

		public TwoWayHandshakeBuilder InitiatedBy(CommunicationRole initiatingParty) {
			_initiator = initiatingParty;
			return this;
		}

		public TwoWayHandshakeBuilder HandleWith<THandshake, TAck>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake) where THandshake : new()
			=> HandleWith((_) => new THandshake(), receiveHandshake, verifyHandshake);

		public TwoWayHandshakeBuilder HandleWith<THandshake, TAck>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake)
			=> HandleWith(new Action2WayHandshakeHandler<THandshake, TAck>(generateHandshake, receiveHandshake, verifyHandshake), typeof(THandshake), typeof(TAck));


		public TwoWayHandshakeBuilder HandleWith(IHandshakeHandler handler, Type handshakeType, Type ackType) {
			Guard.ArgumentNotNull(handler, nameof(handler));
			Guard.ArgumentNotNull(handshakeType, nameof(handshakeType));
			Guard.ArgumentNotNull(ackType, nameof(ackType));
			_handler = handler;
			_messageTypes = [handshakeType, ackType];
			return this;
		}

		internal ProtocolHandshake Build()
			=> new() {
				Type = ProtocolHandshakeType.TwoWay,
				Initiator = _initiator ?? throw new InvalidOperationException("No initiator was specified"),
				Handler = _handler ?? throw new InvalidOperationException("No handler was configured"),
				MessageTypes = _messageTypes ?? Array.Empty<Type>()
			};
	}


	public class ThreeWayHandshakeBuilder {
		private readonly ProtocolHandshakeBuilder _parent;
		private CommunicationRole? _initiator = null;
		private IHandshakeHandler _handler = null;
		private Type[] _messageTypes = null;

		public ThreeWayHandshakeBuilder(ProtocolHandshakeBuilder parent) {
			_parent = parent;
		}

		public ThreeWayHandshakeBuilder InitiatedBy(CommunicationRole initiatingParty) {
			_initiator = initiatingParty;
			return this;
		}

		//// allow use of Verify2Way handshake delegate assuming TVerack: new()
		//public ProtocolHandshakeBuilder HandledBy<THandshake, TAck, TVerack>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake) where THandshake : new() where TVerack : new()
		//	=> HandledBy((_) => new THandshake(), receiveHandshake, verifyHandshake);

		public ThreeWayHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake) where THandshake : new()
			=> HandleWith((_) => new THandshake(), receiveHandshake, verifyHandshake);

		public ThreeWayHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake,
																			  Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake)
			=> HandleWith(generateHandshake, receiveHandshake, verifyHandshake, (_, _, _, _) => true);

		public ThreeWayHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake,
																			  Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake, Acknowledge3WayHandshakeDelegate<THandshake, TAck, TVerack> acknowledgeHandshake)
			=> HandleWith(new Action3WayHandshakeHandler<THandshake, TAck, TVerack>(generateHandshake, receiveHandshake, verifyHandshake, acknowledgeHandshake), typeof(THandshake), typeof(TAck), typeof(TVerack));

		public ThreeWayHandshakeBuilder HandleWith(IHandshakeHandler handler, Type handshakeType, Type ackType, Type verackType) {
			Guard.ArgumentNotNull(handler, nameof(handler));
			Guard.ArgumentNotNull(handshakeType, nameof(handshakeType));
			Guard.ArgumentNotNull(ackType, nameof(ackType));
			Guard.ArgumentNotNull(verackType, nameof(verackType));
			_messageTypes = [handshakeType, ackType, verackType];
			return this;
		}


		internal ProtocolHandshake Build()
			=> new() {
				Type = ProtocolHandshakeType.ThreeWay,
				Initiator = _initiator ?? throw new InvalidOperationException("No initiator was specified"),
				Handler = _handler ?? throw new InvalidOperationException("No handler was configured"),
				MessageTypes = _messageTypes ?? Array.Empty<Type>()
			};

	}

}
