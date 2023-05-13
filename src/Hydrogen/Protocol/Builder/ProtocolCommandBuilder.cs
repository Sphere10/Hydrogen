// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

using System;
using System.Collections.Generic;

namespace Hydrogen.Communications {
    public class ProtocolCommandBuilder : ProtocolBuilderMain {
		ProtocolMode _mode;

		public ProtocolCommandBuilder(ProtocolModeBuilder parent, ProtocolMode mode) 
			: base (parent) {
			_mode = mode;
		}

		public HandlerBuilder<TMessage> ForCommand<TMessage>() {
			return new(this);
		}

		public class HandlerBuilder<TMessage> {
			private readonly ProtocolCommandBuilder _parent;

			public HandlerBuilder(ProtocolCommandBuilder parent) {
				_parent = parent;
			}

			public ProtocolCommandBuilder Execute(Action action)
				=> Execute(_ => action());

			public ProtocolCommandBuilder Execute(Action<TMessage> action)
				=> Execute((_, message) => action(message));

			public ProtocolCommandBuilder Execute(Action<ProtocolOrchestrator, TMessage> action) 
				=> Execute(new ActionCommandHandler<TMessage>(action));

			public ProtocolCommandBuilder Execute(ICommandHandler<TMessage> handler) {
				_parent._mode.CommandHandlers.Add(typeof(TMessage), handler);
				return _parent;
			}
			
		}
	}
}
