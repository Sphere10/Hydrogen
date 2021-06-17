using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public sealed class ProtocolRequestBuilder<TChannel> : ProtocolRequestBuilderBase<TChannel, ProtocolRequestBuilder<TChannel>> where TChannel : ProtocolChannel {
		public IDictionary<Type, IRequestHandler> Build() => RequestHandlers;
	}
}
