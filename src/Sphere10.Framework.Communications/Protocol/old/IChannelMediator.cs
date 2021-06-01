namespace Sphere10.Framework.Communications {
	public interface IChannelMediator {
		public void ReportBadData(BadDataType badDataType, AnonymousPipe channel, string additionData = null);

	}
}
