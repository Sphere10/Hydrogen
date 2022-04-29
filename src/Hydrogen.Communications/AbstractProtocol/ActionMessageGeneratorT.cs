using System;

namespace Sphere10.Framework.Communications {
    public class ActionMessageGeneratorT<TMessage> : MessageGeneratorBase<TMessage> {
		private readonly Func<ProtocolOrchestrator, TMessage> _initiator;

		public ActionMessageGeneratorT(Func<ProtocolOrchestrator, TMessage> initiator) {
			Guard.ArgumentNotNull(initiator, nameof(initiator));
			_initiator = initiator;
		}

		protected override TMessage ExecuteInternal(ProtocolOrchestrator orchestrator)
			=> _initiator.Invoke(orchestrator);
    }

}
