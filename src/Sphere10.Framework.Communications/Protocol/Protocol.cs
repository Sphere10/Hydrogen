using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
	public class Protocol<TChannel, TMessageBase> where TChannel : ProtocolChannel {
		public IItemSerializer<TMessageBase> MessageSerializer { get; init; }
		public IDictionary<Type, ICommandHandler<TChannel, TMessageBase>> CommandHandlers { get; init; }
		public IDictionary<Type, IRequestHandler<TChannel, TMessageBase>> RequestHandlers { get; init; }
	}
}
