namespace Sphere10.Framework.Communications {
	public interface IChannelMediator {
		public void ReportBadData(BadDataType badDataType, AnonymousPipeChannel channel, string additionData = null);

	}
}
