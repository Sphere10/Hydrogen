using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sphere10.Framework.Communications.AbstractProtocol.DataSources {
	public class ClientWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {

		public ClientWebSocketsDataSource(string uri, bool secure)
			: base(new ClientWebSocketsChannel(uri, secure)) {
		}

		public override Task Delete(IEnumerable<TItem> entities) {
			throw new NotImplementedException();
		}

		public override IEnumerable<TItem> New(int count) {
			return New(count);
		}

		public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new NotImplementedException();
		}
	}
}
