using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Communications {

    public class ProtocolBuilder {
        private readonly IDictionary<Type, ICommandHandler> _commands;
        private readonly IDictionary<Type, IRequestHandler> _requests;
        private readonly MultiKeyDictionary<Type, Type, IResponseHandler> _responses;
        private readonly IFactorySerializer<object> _serializer;

        public ProtocolBuilder() {
            _commands = new Dictionary<Type, ICommandHandler>();
            _requests = new Dictionary<Type, IRequestHandler>();
            _responses = new MultiKeyDictionary<Type, Type, IResponseHandler>();
            _serializer = new FactorySerializer<object>();
            Commands = new CommandBuilder(this);
            Requests = new RequestBuilder(this);
            Responses = new ResponseBuilder(this);
            MessageDefinitions = new MessageBuilder(this);
        }

        //public HandshakeBuilder Handshake { get; }

        public CommandBuilder Commands { get; }

        public RequestBuilder Requests { get; }

        public ResponseBuilder Responses { get; }

        public MessageBuilder MessageDefinitions { get; }

        public Protocol Build() {
            var protocol = new Protocol() {
                CommandHandlers = _commands,
                RequestHandlers = _requests,
                ResponseHandlers = _responses,
                MessageSerializer = _serializer
            };
            var result = protocol.Validate();
            if (result.Failure)
                throw new ProtocolBuilderException(result.ErrorMessages.ToDelimittedString(Environment.NewLine));
            return protocol;
        }

        public class CommandBuilder : ProtocolCommandBuilderBase<CommandBuilder> {
            private ProtocolBuilder _parent;
            public CommandBuilder(ProtocolBuilder parent) 
                : base(parent._commands) {
                _parent = parent;
            }

            public RequestBuilder Requests => _parent.Requests;

            public ResponseBuilder Responses => _parent.Responses;

            public MessageBuilder MessageDefinitions => _parent.MessageDefinitions;

            public Protocol Build() => _parent.Build();

        }

        public class RequestBuilder : ProtocolRequestBuilderBase<RequestBuilder> {
            private ProtocolBuilder _parent;
            public RequestBuilder(ProtocolBuilder parent) 
                : base(parent._requests) {
                _parent = parent;
            }

            public CommandBuilder Commands => _parent.Commands;

            public ResponseBuilder Responses => _parent.Responses;

            public MessageBuilder MessageDefinitions => _parent.MessageDefinitions;

            public Protocol Build() => _parent.Build();
        }

        public class ResponseBuilder : ProtocolResponseBuilderBase<ResponseBuilder> {
            private ProtocolBuilder _parent;
            public ResponseBuilder(ProtocolBuilder parent)
                : base(parent._responses) {
                _parent = parent;
            }

            public CommandBuilder Commands => _parent.Commands;

            public RequestBuilder Requests => _parent.Requests;

            public MessageBuilder MessageDefinitions => _parent.MessageDefinitions;

            public Protocol Build() => _parent.Build();

        }

        public sealed class MessageBuilder : FactorySerializerBuilderBase<object, MessageBuilder> {
            private ProtocolBuilder _parent;
            public MessageBuilder(ProtocolBuilder parent) : base(parent._serializer) {
                _parent = parent;
            }

            public CommandBuilder Commands { get; }

            public RequestBuilder Requests { get; }

            public ResponseBuilder Responses { get; }

            public Protocol Build() => _parent.Build();

            public MessageBuilder Use(IFactorySerializer<object> serializer) {
                Guard.ArgumentNotNull(serializer, nameof(serializer));
                base.Serializer = serializer;
                return this;
            }


		}
    }
}
