using System.IO.Pipes;

namespace Sphere10.Framework.Communications {
	public class NoOpChannelMediator : IChannelMediator {
		public void ReportBadData(BadDataType badDataType, AnonymousPipe channel, string additionData = null) {
			// Do nothing			
		}
	}
}
