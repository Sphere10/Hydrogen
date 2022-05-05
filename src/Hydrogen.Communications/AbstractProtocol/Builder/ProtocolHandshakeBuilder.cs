using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Hydrogen.Communications {
	public class ProtocolHandshakeBuilder : ProtocolBuilderMain {
		ProtocolHandshake _handshake;

		public ProtocolHandshakeBuilder(ProtocolBuilder protocolBuilder, ProtocolHandshake definition) 
			: base(protocolBuilder) {
			_handshake = definition;
		}


		public TwoWayHandshakeBuilder TwoWay {
			get {
				_handshake.Type = ProtocolHandshakeType.TwoWay;
				return new(this);
			}
		}

		public ThreeWayHandshakeBuilder ThreeWay {
			get {
				_handshake.Type = ProtocolHandshakeType.ThreeWay;
				return new(this);
			}
		}

		public class TwoWayHandshakeBuilder {
			private readonly ProtocolHandshakeBuilder _parent;

			public TwoWayHandshakeBuilder(ProtocolHandshakeBuilder parent) {
				_parent = parent;
			}

			public TwoWayHandshakeBuilder InitiatedBy(CommunicationRole initiatingParty) {
				_parent._handshake.Initiator = initiatingParty;
				return this;
			}

			public ProtocolHandshakeBuilder HandleWith<THandshake, TAck>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake) where THandshake : new()
				=> HandleWith((_) => new THandshake(), receiveHandshake, verifyHandshake);

			public ProtocolHandshakeBuilder HandleWith<THandshake, TAck>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake)
				=> HandleWith(new Action2WayHandshakeHandler<THandshake, TAck>(generateHandshake, receiveHandshake, verifyHandshake), typeof(THandshake), typeof(TAck));


			public ProtocolHandshakeBuilder HandleWith(IHandshakeHandler handler, Type handshakeType, Type ackType) {
				Guard.ArgumentNotNull(handler, nameof(handler));
				Guard.ArgumentNotNull(handshakeType, nameof(handshakeType));
				Guard.ArgumentNotNull(ackType, nameof(ackType));
				_parent._handshake.MessageTypes.Clear();
				_parent._handshake.MessageTypes.Add(handshakeType);
				_parent._handshake.MessageTypes.Add(ackType);
				_parent._handshake.Handler = handler;
				return _parent;
			}

		}

		public class ThreeWayHandshakeBuilder {
			private readonly ProtocolHandshakeBuilder _parent;

			public ThreeWayHandshakeBuilder(ProtocolHandshakeBuilder parent) {
				_parent = parent;
			}

			public ThreeWayHandshakeBuilder InitiatedBy(CommunicationRole initiatingParty) {
				_parent._handshake.Initiator = initiatingParty;
				return this;
			}

			//// allow use of Verify2Way handshake delegate assuming TVerack: new()
			//public ProtocolHandshakeBuilder HandledBy<THandshake, TAck, TVerack>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify2WayHandshakeDelegate<THandshake, TAck> verifyHandshake) where THandshake : new() where TVerack : new()
			//	=> HandledBy((_) => new THandshake(), receiveHandshake, verifyHandshake);

			public ProtocolHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake) where THandshake : new()
				=> HandleWith((_) => new THandshake(), receiveHandshake, verifyHandshake);

			public ProtocolHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake)
				=> HandleWith(generateHandshake, receiveHandshake, verifyHandshake, (_, _, _, _) => true);

			public ProtocolHandshakeBuilder HandleWith<THandshake, TAck, TVerack>(GenerateHandshakeDelegate<THandshake> generateHandshake, ReceiveHandshakeDelegate<THandshake, TAck> receiveHandshake, Verify3WayHandshakeDelegate<THandshake, TAck, TVerack> verifyHandshake, Acknowledge3WayHandshakeDelegate<THandshake, TAck, TVerack> acknowledgeHandshake)
				=> HandleWith(new Action3WayHandshakeHandler<THandshake, TAck, TVerack>(generateHandshake, receiveHandshake, verifyHandshake, acknowledgeHandshake), typeof(THandshake), typeof(TAck), typeof(TVerack));

			public ProtocolHandshakeBuilder HandleWith(IHandshakeHandler handler, Type handshakeType, Type ackType, Type verackType) {
				Guard.ArgumentNotNull(handler, nameof(handler));
				Guard.ArgumentNotNull(handshakeType, nameof(handshakeType));
				Guard.ArgumentNotNull(ackType, nameof(ackType));
				Guard.ArgumentNotNull(verackType, nameof(verackType));
				_parent._handshake.MessageTypes.Clear();
				_parent._handshake.MessageTypes.Add(handshakeType);
				_parent._handshake.MessageTypes.Add(ackType);
				_parent._handshake.MessageTypes.Add(verackType);
				_parent._handshake.Handler = handler;
				return _parent;
			}

		}

	}
}
