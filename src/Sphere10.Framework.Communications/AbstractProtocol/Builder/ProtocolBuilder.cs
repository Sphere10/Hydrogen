using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Communications {

    public class ProtocolBuilder<TChannel> where TChannel : ProtocolChannel {
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
            Messages = new MessageBuilder(this);
        }

        public CommandBuilder Commands { get; }

        public RequestBuilder Requests { get; }

        public ResponseBuilder Responses { get; }

        public MessageBuilder Messages { get; }

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

        public class CommandBuilder : ProtocolCommandBuilderBase<TChannel, CommandBuilder> {
            private ProtocolBuilder<TChannel> _parent;
            public CommandBuilder(ProtocolBuilder<TChannel> parent) 
                : base(parent._commands) {
                _parent = parent;
            }

            public RequestBuilder Requests => _parent.Requests;

            public ResponseBuilder Responses => _parent.Responses;

            public MessageBuilder Messages => _parent.Messages;

            public Protocol Build() => _parent.Build();

        }

        public class RequestBuilder : ProtocolRequestBuilderBase<TChannel, RequestBuilder> {
            private ProtocolBuilder<TChannel> _parent;
            public RequestBuilder(ProtocolBuilder<TChannel> parent) 
                : base(parent._requests) {
                _parent = parent;
            }

            public CommandBuilder Commands => _parent.Commands;

            public ResponseBuilder Responses => _parent.Responses;

            public MessageBuilder Messages => _parent.Messages;

            public Protocol Build() => _parent.Build();
        }

        public class ResponseBuilder : ProtocolResponseBuilderBase<TChannel, ResponseBuilder> {
            private ProtocolBuilder<TChannel> _parent;
            public ResponseBuilder(ProtocolBuilder<TChannel> parent)
                : base(parent._responses) {
                _parent = parent;
            }

            public CommandBuilder Commands => _parent.Commands;

            public RequestBuilder Requests => _parent.Requests;

            public MessageBuilder Messages => _parent.Messages;

            public Protocol Build() => _parent.Build();

        }

        public sealed class MessageBuilder : FactorySerializerBuilderBase<object, MessageBuilder> {
            private ProtocolBuilder<TChannel> _parent;
            public MessageBuilder(ProtocolBuilder<TChannel> parent) : base(parent._serializer) {
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
