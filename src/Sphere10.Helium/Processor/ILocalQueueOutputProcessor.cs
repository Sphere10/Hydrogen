using System.Collections.Generic;
using Sphere10.Helium.Framework;
using Sphere10.Helium.Message;

namespace Sphere10.Helium.Processor {
	public interface ILocalQueueOutputProcessor {

		public void OnCommittedLocalQueue(object sender); //TODO Jake: how to deal with this sender object//

		public void ProcessAllMessagesSynchronously();

		public void ExtractHandler();

		public void InvokeHandler(List<PluginAssemblyHandlerDto> handlerTypeList, IMessage message);
	}
}