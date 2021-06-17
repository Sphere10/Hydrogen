using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public sealed class ProtocolResponseBuilder<TChannel> : ProtocolResponseBuilderBase<TChannel, ProtocolResponseBuilder<TChannel>> where TChannel : ProtocolChannel {
		public MultiKeyDictionary<Type, Type, IResponseHandler> Build() => ResponseHandlers;
	}


}
