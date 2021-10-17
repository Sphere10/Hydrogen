using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {

    public sealed class ProtocolCommandBuilder : ProtocolCommandBuilderBase<ProtocolCommandBuilder> {

		public IDictionary<Type, ICommandHandler> Build() => CommandHandlers;

	}
}
