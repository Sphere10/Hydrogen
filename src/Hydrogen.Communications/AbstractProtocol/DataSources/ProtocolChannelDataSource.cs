using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hydrogen.Communications {
	public abstract class ProtocolChannelDataSource<TItem> : IDataSource<TItem> {

		protected ProtocolChannelDataSource(ProtocolChannel protocolChannel) {
			ProtocolChannel = protocolChannel;
			ProtocolChannel.Open();
		}

		event EventHandlerEx<DataSourceMutatedItems<TItem>> IDataSource<TItem>.MutatedItems {
			add {
				throw new System.NotImplementedException();
			}

			remove {
				throw new System.NotImplementedException();
			}
		}

		protected ProtocolChannel ProtocolChannel { get; set; }

		protected void SendBytes(System.ReadOnlyMemory<byte> bytes) {
			ProtocolChannel.TrySendBytes(bytes.ToArray());
		}

		void IDataSource<TItem>.NewDelayed(int count) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.RefreshDelayed(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.UpdateDelayed(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.DeleteDelayed(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.ValidateDelayed(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.CountDelayed() {
			throw new System.NotImplementedException();
		}

		string IDataSource<TItem>.UpdateItem(TItem item) {
			throw new System.NotImplementedException();
		}

		string IDataSource<TItem>.IdItem(TItem item) {
			throw new System.NotImplementedException();
		}

		string IDataSource<TItem>.InitializeItem(TItem item) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.Close() {
			throw new System.NotImplementedException();
		}

		Task<IEnumerable<TItem>> IDataSource<TItem>.New(int count) {
			throw new System.NotImplementedException();
		}

		Task<DataSourceItems<TItem>> IDataSource<TItem>.Read(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection, out int totalItems) {
			throw new System.NotImplementedException();
		}

		Task IDataSource<TItem>.Update(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		Task IDataSource<TItem>.Delete(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		Task<Result> IDataSource<TItem>.Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new System.NotImplementedException();
		}

		void IDataSource<TItem>.ReadDelayed(string searchTerm, int pageLength, int page, string sortProperty, SortDirection sortDirection) {
			throw new System.NotImplementedException();
		}

		Task IDataSource<TItem>.Create(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		Task IDataSource<TItem>.Refresh(TItem[] entities) {
			throw new System.NotImplementedException();
		}

		Task<DataSourceCapabilities> IDataSource<TItem>.Capabilities => throw new System.NotImplementedException();

		Task<int> IDataSource<TItem>.Count => throw new System.NotImplementedException();
	}
}