using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Communications {

    public class Protocol {

        public Protocol() {
            HandshakeType = ProtocolHandshakeType.None;
            MessageSerializer = new FactorySerializer<object>();
            CommandHandlers = new Dictionary<Type, ICommandHandler>();
            RequestHandlers = new Dictionary<Type, IRequestHandler>();
            ResponseHandlers = new MultiKeyDictionary<Type, Type, IResponseHandler>();
        }

        public ProtocolHandshakeType HandshakeType { get; init; } 

		public IFactorySerializer<object> MessageSerializer { get; init; }

		public IDictionary<Type, ICommandHandler> CommandHandlers { get; init; }

		public IDictionary<Type, IRequestHandler> RequestHandlers { get; init; }

		public MultiKeyDictionary<Type, Type, IResponseHandler> ResponseHandlers { get; init; }

        public Result Validate() {
            var result = Result.Default;

            if (MessageSerializer is null)
                result.AddError("MessageSerializer is null");

            if (CommandHandlers is null)
                result.AddError("CommandHandlers is null");

            if (RequestHandlers is null)
                result.AddError("RequestHandlers is null");

            if (ResponseHandlers is null)
                result.AddError("ResponseHandlers is null");

            if (MessageSerializer != null) {
                var supportedTypes = MessageSerializer.RegisteredTypes.ToHashSet();
                var missingSerializers = Enumerable.Empty<Type>();
                if (CommandHandlers is not null)
                    missingSerializers = missingSerializers.Union(CommandHandlers.Keys.Where(t => !supportedTypes.Contains(t)));
                if (RequestHandlers is not null)
                    missingSerializers = missingSerializers.Union(RequestHandlers.Keys.Where(t => !supportedTypes.Contains(t)));
                if (ResponseHandlers is not null)
                    missingSerializers = missingSerializers.Union(ResponseHandlers.Keys.Where(t => !supportedTypes.Contains(t)));

                if (missingSerializers.Any())
                    foreach (var type in missingSerializers)
                        result.AddError($"Missing serializer for type '{type.Name}'");
            }
            return result;
        }
	}
}
