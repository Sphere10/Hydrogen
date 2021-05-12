using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
	public class AnonymousPipeProtocol  {
		FactorySerializer<IAnonymousPipeMessage> MessageSerializer { get; init; }
		IDictionary<Type, IAnonymousPipeCommandHandler> CommandHandlers { get; init; }
		IDictionary<Type, IAnonymousPipeRequestHandler> RequestHandlers { get; init; }
	}
}
