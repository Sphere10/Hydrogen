using System;
using System.Collections.Generic;

namespace Sphere10.Framework.Communications {
    public abstract class ProtocolCommandBuilderBase<TProtocolCommandBuilder> 
		where TProtocolCommandBuilder : ProtocolCommandBuilderBase<TProtocolCommandBuilder> {
		protected readonly IDictionary<Type, ICommandHandler> CommandHandlers;

		protected ProtocolCommandBuilderBase() 
			: this(new Dictionary<Type, ICommandHandler>()) {
		}

		protected ProtocolCommandBuilderBase(IDictionary<Type, ICommandHandler> commands) {
			CommandHandlers = commands;
		}

		public HandlerBuilder<TMessage> ForCommand<TMessage>() {
			return new HandlerBuilder<TMessage>(this as TProtocolCommandBuilder);
		}

		protected void Add(Type type, ICommandHandler handler) {
			CommandHandlers.Add(type, handler);
        }

		public class HandlerBuilder<TMessage> {
			private readonly TProtocolCommandBuilder _parent;

			public HandlerBuilder(TProtocolCommandBuilder parent) {
				_parent = parent;
			}

			public TProtocolCommandBuilder Execute(Action action)
				=> Execute(_ => action());

			public TProtocolCommandBuilder Execute(Action<TMessage> action)
				=> Execute((_, message) => action(message));

			public TProtocolCommandBuilder Execute(Action<ProtocolOrchestrator, TMessage> action) 
				=> Execute(new ActionCommandHandler<TMessage>(action));

			public TProtocolCommandBuilder Execute(ICommandHandler<TMessage> handler) {
				_parent.Add(typeof(TMessage), handler);
				return _parent;
			}
			
		}
	}
}
