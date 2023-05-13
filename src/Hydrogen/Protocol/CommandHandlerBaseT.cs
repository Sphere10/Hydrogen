// Copyright (c) Sphere 10 Software. All rights reserved. (https://sphere10.com)
// Author: Herman Schoenfeld
//
// Distributed under the MIT software license, see the accompanying file
// LICENSE or visit http://www.opensource.org/licenses/mit-license.php.
//
// This notice must not be removed when duplicating this file or its contents, in whole or in part.

namespace Hydrogen.Communications {

    public abstract class CommandHandlerBase<TMessage> : CommandHandlerBase, ICommandHandler<TMessage> {

        public override void Execute(ProtocolOrchestrator orchestrator, object command) {
            Guard.ArgumentCast<TMessage>(command, out var commandT, nameof(command));
            Execute(orchestrator, commandT);
        }

        public abstract void Execute(ProtocolOrchestrator orchestrator, TMessage command);

    }
}
