using System;

namespace Hydrogen.Communications {
	public abstract class MessageGeneratorBase<TMessage> : MessageGeneratorBase, IMessageGenerator<TMessage> {

		TMessage IMessageGenerator<TMessage>.Execute(ProtocolOrchestrator orchestrator)
			=> ExecuteInternal(orchestrator);

		public sealed override object Execute(ProtocolOrchestrator orchestrator) {
			Guard.ArgumentNotNull(orchestrator, nameof(orchestrator));
			return ((IMessageGenerator<TMessage>)this).Execute(orchestrator);
		}

		protected abstract TMessage ExecuteInternal(ProtocolOrchestrator orchestrator);

	}
}
