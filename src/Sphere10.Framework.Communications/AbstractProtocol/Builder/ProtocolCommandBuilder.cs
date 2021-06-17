using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {

    public sealed class ProtocolCommandBuilder<TChannel> : ProtocolCommandBuilderBase<TChannel, ProtocolCommandBuilder<TChannel>> where TChannel : ProtocolChannel {

		public IDictionary<Type, ICommandHandler> Build() => CommandHandlers;

	}
}
