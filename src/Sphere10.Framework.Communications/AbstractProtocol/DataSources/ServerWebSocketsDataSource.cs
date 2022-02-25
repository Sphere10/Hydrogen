using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sphere10.Framework.Communications {
	public class ServerWebSocketsDataSource<TItem> : ProtocolChannelDataSource<TItem> {

		List<TItem> Items { get; set; } = new List<TItem>();
		public ServerWebSocketsDataSource(IPEndPoint localEndpoint, IPEndPoint remoteEndpoint, bool secure)
			: base(new ServerWebSocketsChannel(localEndpoint, remoteEndpoint, secure)) {
		}

		public override Task Delete(IEnumerable<TItem> entities) {
			throw new System.NotImplementedException();
		}

		public override IEnumerable<TItem> New(int count) {

			var type = typeof(TItem);

			var list = new List<TItem>();

			for (int i = 0; i < count; i++) {
				var newInstance = (TItem)Activator.CreateInstance(type, new object[Items.Count + 1]);
				list.Add(newInstance);
				Items.Add(newInstance);
			}

SystemLog.Info($"Created {count} objects");
foreach (var item in list) {
	var jsonItem = JsonConvert.SerializeObject(item);
	SystemLog.Info(jsonItem);
}
			return list;
		}

		public override Task<Result> Validate(IEnumerable<(TItem entity, CrudAction action)> actions) {
			throw new System.NotImplementedException();
		}
	}
}