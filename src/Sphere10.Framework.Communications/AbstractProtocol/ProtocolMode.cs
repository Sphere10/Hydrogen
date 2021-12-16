using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere10.Framework.Communications {
	public class ProtocolMode {

		public ProtocolMode() {
			Number = 0;
			CommandHandlers = new Dictionary<Type, ICommandHandler>();
			RequestHandlers = new Dictionary<Type, IRequestHandler>();
			ResponseHandlers = new MultiKeyDictionary<Type, Type, IResponseHandler>();
			MessageGenerators = new Dictionary<Type, IMessageGenerator>();
			MessageSerializer = new FactorySerializer<object>();
		}

		public int Number { get; init; }

		public IDictionary<Type, ICommandHandler> CommandHandlers { get; init; }

		public IDictionary<Type, IRequestHandler> RequestHandlers { get; init; }

		public MultiKeyDictionary<Type, Type, IResponseHandler> ResponseHandlers { get; init; }

		public IDictionary<Type, IMessageGenerator> MessageGenerators { get; init; }

		public IFactorySerializer<object> MessageSerializer { get; init; }

		public Result Validate() {
			var result = Result.Default;

			if (MessageGenerators is null)
				result.AddError($"Mode {Number}: MessageGenerators is null");

			if (CommandHandlers is null)
				result.AddError($"Mode {Number}: CommandHandlers is null");

			if (RequestHandlers is null)
				result.AddError($"Mode {Number}: RequestHandlers is null");

			if (ResponseHandlers is null)
				result.AddError($"Mode {Number}: ResponseHandlers is null");

			if (MessageSerializer is null)
				result.AddError("Serializer is null");

			// Validate message generators generate correct type
			if (MessageGenerators is not null) {
				foreach (var (key, value) in MessageGenerators) {
					if (value.GetType().IsConstructedGenericTypeOf(typeof(IMessageGenerator<>))) {
						var handlerCreatedType = value.GetType().GetGenericArguments()[0];
						if (handlerCreatedType != key)
							result.AddError($"Message generator for type '{key.Name}' activates an object of type '{handlerCreatedType.Namespace}'");
					}
				}
			}

			// Check missing serializers
			if (MessageSerializer is not null) {
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

