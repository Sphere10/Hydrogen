using System.IO.Pipes;

namespace Sphere10.Framework.Communications {
	public class NoOpChannelMediator : IChannelMediator {
		public void ReportBadData(BadDataType badDataType, AnonymousPipeChannel channel, string additionData = null) {
			// Do nothing			
		}
	}
}
