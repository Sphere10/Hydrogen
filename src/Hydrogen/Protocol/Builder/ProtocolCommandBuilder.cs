// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT NON-AI software license, see the accompanying file
// LICENSE or visit https://sphere10.com/legal/NON-AI-MIT.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;

namespace Hydrogen.Communications;

public class ProtocolCommandBuilder<TMessage>  {

	private ICommandHandler _commandHandler;

	public ProtocolCommandBuilder<TMessage> HandleWith(Action action)
		=> HandleWith(_ => action());

	public ProtocolCommandBuilder<TMessage> HandleWith(Action<TMessage> action)
		=> HandleWith((_, message) => action(message));

	public ProtocolCommandBuilder<TMessage> HandleWith(Action<ProtocolOrchestrator, TMessage> action)
		=> HandleWith(new ActionCommandHandler<TMessage>(action));

	public ProtocolCommandBuilder<TMessage> HandleWith(ICommandHandler<TMessage> handler) 
		=> HandleWith((ICommandHandler)handler);

	public ProtocolCommandBuilder<TMessage> HandleWith(ICommandHandler handler) {
		Guard.ArgumentNotNull(handler, nameof(handler));
		Guard.Argument(handler.MessageType == typeof(TMessage), nameof(handler), "Message type mismatch");
		_commandHandler = handler;
		return this;
	}

	public ICommandHandler Build() {
		Guard.Ensure(_commandHandler is not null, "Command handler not set");
		return _commandHandler;
	}

}
