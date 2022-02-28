using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications {
	public abstract class ProtocolChannelDataSource<TItem> : DataSourceBase<TItem> {

		protected ProtocolChannelDataSource(ProtocolChannel protocolChannel) {
			ProtocolChannel = protocolChannel;
			//ProtocolChannel.ReceivedBytes += ProtocolChannel_ReceivedBytes;
			ProtocolChannel.Open();
		}

		protected ProtocolChannel ProtocolChannel { get; set; }

		protected void SendBytes(System.ReadOnlyMemory<byte> bytes) {
			ProtocolChannel.TrySendBytes(bytes.ToArray());
		}

		//private void ProtocolChannel_ReceivedBytes(System.ReadOnlyMemory<byte> bytes) {
		//}

		public override Task Create(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		//public override Task<IEnumerable<TItem>> Read(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
		//	throw new System.NotImplementedException();
		//}
		public override IEnumerable<TItem> ReadSync(string searchTerm, int pageLength, ref int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			throw new System.NotImplementedException();
		}

		public override Task Refresh(TItem[] entities) {
			throw new System.NotImplementedException();
		}

		public override Task Update(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		//public override IEnumerable<TItem> New(int count) {
		//	throw new System.NotImplementedException();
		//}

		//public override Task Delete(IEnumerable<TItem> entities) {
		//	throw new System.NotImplementedException();
		//}

		//public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
		//	throw new System.NotImplementedException();
		//}

		public override Task<int> Count { get; }
	}
}