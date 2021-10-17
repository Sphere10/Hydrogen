using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public sealed class ProtocolRequestBuilder : ProtocolRequestBuilderBase<ProtocolRequestBuilder> {
		public IDictionary<Type, IRequestHandler> Build() => RequestHandlers;
	}
}
