using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
	public class ProtocolCommandBuilder<TChannel, TMessageBase> where TChannel : ProtocolChannel {
		private readonly IDictionary<Type, ICommandHandler<TChannel, TMessageBase>> _commands;

		public ProtocolCommandBuilder() {
			_commands = new Dictionary<Type, ICommandHandler<TChannel, TMessageBase>>();
		}

		public ProtocolCommandBuilder<TChannel, TMessageBase> For<TMessage>(Action<TChannel, TMessage> action) where TMessage : TMessageBase {
			_commands.Add(typeof(TMessage), new ActionCommandHandler<TChannel, TMessageBase, TMessage>(action));
			return this;
		}

		public IDictionary<Type, ICommandHandler<TChannel, TMessageBase>> Build() => _commands;
	}
}
