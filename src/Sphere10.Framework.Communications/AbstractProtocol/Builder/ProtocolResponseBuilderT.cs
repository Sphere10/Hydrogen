using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public sealed class ProtocolResponseBuilder : ProtocolResponseBuilderBase<ProtocolResponseBuilder> {
		public MultiKeyDictionary<Type, Type, IResponseHandler> Build() => ResponseHandlers;
	}
}
